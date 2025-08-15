# Dan's ProjectTask Library

A Flexible .NET Project Management Library

*Plan it. Schedule it. Track it. Transform it.*

<p>&nbsp;</p>

Current Status: In-Testing for version 25.2800.0000.

<p>&nbsp;</p>

## Introduction

ProjectTask is a powerful, stand-alone .NET Standard 2.0 class library
built to support flexible, event-driven project management, across any
methodology, whether it's MSF (Microsoft Solutions Framework), XP
(Extreme Programming / Extreme Productivity), Agile, SCRUM, Kanban, or
good old-fashioned Waterfall.

With full support for JSON serialization, dependency handling, and
hierarchical task structures, this library is designed to be embedded
into a wide variety of planning and scheduling applications. Whether
you're managing a sprawling enterprise program or organizing a weekend
hackathon, ProjectTask can adapt to your needs.

... and by the way, tasks and projects are interchangeable, meaning that
you can seamlessly convert between them without data loss just by
changing the item type.

<p>&nbsp;</p>

## Features

Following are only some of the features you can find in this library.

-   **Automatic Task and Project Estimation and Scheduling**  
    Calculate and prepare the timeline of an entire project and its
    tasks from a single call.
-   **Unified Task and Project Model**  
    Treat projects and tasks as one and the same. Any item can contain
    children and dependencies, and you can switch types freely.
-   **Event-Driven Property Change Notification**  
    You can register events on objects and their properties to get
    notified in real time when anything changes, with fine-grained
    control over object updates.
-   **Full JSON Serialization and Deserialization**  
    Save and load project structures with ease using standard,
    human-readable JSON format.
-   **Dependency and Hierarchy Handling**  
    Model complex workflows with multiple parents, children, and task
    dependencies.
-   **Versatile Methodology Support**  
    Designed to flex with your process, whether you're sprinting in
    Agile or plotting milestones in Waterfall.
-   **Rooted in Legacy and Built for the Future**  
    Born from ANSI-C 5.0 in the early 1990s and evolved through decades
    of application. This library is robust, refined, and battle-tested.

<p>&nbsp;</p>

## Some History

Before *task management* was a buzzword, I was
hand-rolling an early predecessor of this library in Microsoft QuickC
circa 1990, using structs, pointers, and address arithmetic.
Object-oriented design? You better believe it, even without C++.

From those humble beginnings to today’s .NET ecosystem, the ProjectTask
library embodies decades of iteration across multiple applications,
tools, and programming eras.

<blockquote>Trivia: The original version was written in ANSI-C on the
ancestor of Visual Studio, QuickC, long before Agile was agile!</blockquote>

<p>&nbsp;</p>

## Example Usage

Check out the [example project in the the Source / ProjectTaskExample
folder](https://github.com/danielanywhere/ProjectTask/blob/main/Source/ProjectTaskExample/Program.cs)
of this this repository to see how to accomplish any of the following.  

-   Create a project and task hierarchy.
-   Set up dependencies.
-   Serialize to and from JSON.
-   Listen for property changes.

  
```cs
using ProjectTask;

// Configure the base data.
// This only needs to be done once per session,
// or can be read from the database or data file.
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

<p>&nbsp;</p>

## Concepts and Design Philosophy

The following forethought and considerations go into the construction
and maintenance of this library.

-   **No artificial limitations**. Any task item can be a task, a
    project, or something in between.
-   **Recursive structures**. You can plan and manage complex
    multi-level task trees with ease.
-   **Minimal assumptions**. This library is stationed at the
    object-model level. You can bring your own UI, storage, or
    scheduling layers as you see fit.

<p>&nbsp;</p>

## Contributing

If you've got ideas, issues, or improvements, contributions are
welcomed. Fork the repository, start a feature branch, and send over a
pull request.

<p>&nbsp;</p>

## Acknowledgements

Thanks to decades of previous system users, inspiration, late-night
debugging, and the enduring spirit of old-school development
environments.  
  
<blockquote>"Who says you couldn’t do OOP in C? You just had more elbow room
for getting it done right."

*-Daniel Patterson*</blockquote>
