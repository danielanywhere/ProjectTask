using ProjectTask;

// Configure the base data.
// This only needs to be done once per session.
// Task states.
ActiveProjectContext.TaskStates.Add(
 ("TODO", ProjectTaskStateEnum.Queued),
 ("InProgress", ProjectTaskStateEnum.Active),
 ("Completed", ProjectTaskStateEnum.Closed));

//	Task types.
ActiveProjectContext.TaskTypes.Add(
 ("Project", ProjectTaskTypeEnum.Project),
 ("Task", ProjectTaskTypeEnum.Task));

//	Set the default type and status.
ActiveProjectContext.DefaultItemType =
 ActiveProjectContext.TaskTypes["Task"];
ActiveProjectContext.DefaultItemStatus =
 ActiveProjectContext.TaskStates["TODO"];

// General runtime example.
TaskItem root = new TaskItem("Main Project", "Project");
TaskItem subTask = root.Tasks.Add("Design Module");

// This task will start after the Requirements Gathering task is done.
// Since Requirements Gathering doesn't yet exist, it will be created.
subTask.Dependencies.Add(
 "Requirements Gathering",
 DependencyTypeEnum.StartOnCompletion);

