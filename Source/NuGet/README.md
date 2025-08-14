# Dan's ProjectTask Library

A flexible .NET project management function library with no dependencies on any operating system or UI.

ProjectTask is a powerful, stand-alone .NET Standard 2.0 class library built to support flexible, event-driven project management, across any methodology, whether it's MSF (Microsoft Solutions Framework), XP (Extreme Programming / Extreme Productivity), Agile, SCRUM, Kanban, or good old-fashioned Waterfall.

With full support for JSON serialization, dependency handling, and hierarchical task structures, this library is designed to be embedded into a wide variety of planning and scheduling applications. Whether you're managing a sprawling enterprise program or organizing a weekend hackathon, ProjectTask can adapt to your needs.

Following are some basic examples.

## Create a New Project, Task, and Dependency in a Few Lines of Code
In the following example, you can see how simple it is to create a project, a normal task, and a dependent task.

```cs
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

```

For more elaborate examples, please see the ProjectTaskExample application code on GitHub at [https://github.com/danielanywhere/ProjectTask/blob/main/Source/ProjectTaskExample/Program.cs](https://github.com/danielanywhere/ProjectTask/blob/main/Source/ProjectTaskExample/Program.cs).


## Contributions Welcome
If you run into bugs or have feature requests, please create an issue on [this project's GitHub page](https://github.com/danielanywhere/ProjectTask).


## Updates

| Version | Description |
|---------|-------------|
| 25.2814.3647 | Initial public release. |


## More Information

For more information, please see the GitHub project:
[danielanywhere/ProjectTask](https://github.com/danielanywhere/ProjectTask)

Full API documentation is available at this library's [GitHub User Page](https://danielanywhere.github.io/ProjectTask).




