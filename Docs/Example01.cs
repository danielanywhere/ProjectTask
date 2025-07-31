var root = new ProjectTask("Main Project");
var subTask = root.Tasks.Add("Design Module");

subTask.Dependencies.Add(
	"Requirements Gathering",
	DependencyTypeEnum.StartAfter);
