/*
 * Copyright (c). 2025 Daniel Patterson, MCSD (danielanywhere).
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

using ProjectTask;
using static ProjectTask.ProjectTaskUtil;

namespace ProjectTaskExample
{
	//*-------------------------------------------------------------------------*
	//*	Program																																	*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Main instance of the ProjectTaskExample application.
	/// </summary>
	public class Program
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* RandomFloat																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a random single point floating point value within the specified
		/// range.
		/// </summary>
		/// <param name="minimum">
		/// The minimum allowable value in the range.
		/// </param>
		/// <param name="maximum">
		/// The maximum allowable value in the range.
		/// </param>
		/// <returns>
		/// A random floating point value in the requested range.
		/// </returns>
		private static float RandomFloat(float minimum, float maximum)
		{
			Random random = new Random((int)DateTime.Now.Ticks);
			return ConvertRange(0f, 1f, minimum, maximum, random.NextSingle());
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Main																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Configure and run the application.
		/// </summary>
		public static void Main(string[] args)
		{
			Program app = new Program();
			app.Run();
			Console.WriteLine("Press [Enter] to exit...");
			Console.ReadLine();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Run																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run the configured application.
		/// </summary>
		public void Run()
		{
			int count = 0;
			List<FreeBusyItem> freeBusyItems = null;
			int index = 0;
			TaskItem project = null;
			ProjectContext projectContext = ActiveProjectContext;
			ScheduleEngine scheduler = null;
			TaskItem task = null;
			List<TaskItem> tasks = null;
			TimeBlockItem timeBlockWeekday = null;

			Console.WriteLine("Testing Dan's ProjectTask Library...");

			//	Configure the base data.
			//	Task states.
			projectContext.TaskStates.Add(
				("TODO", ProjectTaskStateEnum.Queued),
				("InProgress", ProjectTaskStateEnum.Active),
				("Completed", ProjectTaskStateEnum.Closed));

			//	Task types.
			projectContext.TaskTypes.Add(
				("Project", ProjectTaskTypeEnum.Project),
				("Task", ProjectTaskTypeEnum.Task));

			//	Set the default type and status.
			ActiveProjectContext.DefaultItemType =
				ActiveProjectContext.TaskTypes["Task"];
			ActiveProjectContext.DefaultItemStatus =
				ActiveProjectContext.TaskStates["TODO"];

			//	Configure the users.
			projectContext.Contacts.Add(
				("Pickle Featherstone", "pickle.featherstone@quirkymail.com"),
				("Waffle McSnort", "waffle.mcsnort@laughnet.com"),
				("Sassy Bumbleshoe", "sassy.bumbleshoe@buzzmail.io"),
				("Tater Gigglesworth", "tater.gigglesworth@spudmail.biz"),
				("Barkley Von Snickle", "barkley.snickle@howlzone.org"));

			//	When assigning supervisor, you can remove spaces or used the unique
			//	portion of the display name. The report is first, the supervisor is
			//	second.
			projectContext.Contacts.AssignSupervisor("WaffleMcSnort", "Tater");
			projectContext.Contacts.AssignSupervisor("Barkley", "Tater");

			//	Create a project.
			tasks = projectContext.Tasks.Add(
				("Maiden Project", "Create a new project-level task.",
					"Project", "InProgress"),
				("Kick-Off Task", "Create a new task.",
					"Task", "TODO"));
			//	One way to assign all of the tasks is to get the project into an
			//	object first, then assign all of the non-project items to it.
			count = tasks.Count;
			if(count > 0)
			{
				//	Associate tasks with their project.
				task = tasks[0];
				task.StartDate = DateTime.Today;
				for(index = 1; index < count; index++)
				{
					task.Tasks.Add(tasks[index]);
				}
			}
			//	Create another project.
			tasks = projectContext.Tasks.Add(
				("Echoes of Tomorrow", "Develop a **biodegradable, " +
				"plant-based sensor network** designed to monitor subtle " +
				"environmental shifts in vulnerable ecosystems. " +
				"These tiny, self-sustaining nodes, powered by microbial fuel " +
				"cells, will wirelessly transmit data on temperature, " +
				"humidity, and atmospheric composition, dissolving harmlessly into " +
				"the soil after their operational lifespan. " +
				"The project seeks to provide early warning systems for ecological " +
				"distress, enabling proactive conservation efforts and offering a " +
				"sustainable alternative to conventional electronic monitoring.",
				"Project", "TODO"),
				("Microbial Fuel Cell Optimization", "Focus on optimizing the power " +
				"output and longevity of the microbial fuel cells that will power " +
				"the sensor nodes. This involves experimenting with different " +
				"organic substrates, electrode materials, and microbial strains to " +
				"achieve a stable and sufficient energy supply for extended periods " +
				"in various environmental conditions. " +
				"Success will be measured by the consistent generation of voltage " +
				"needed to power the sensors for a target duration of at least six " +
				"months.", "Task", "TODO"),
				("Biodegradable Polymer Casing Development", "Develop and test " +
				"biodegradable polymer casings for the sensor nodes. " +
				"The primary goal is to create a material that can protect the " +
				"internal components from the elements while the sensor is " +
				"operational, but then breaks down completely and harmlessly in " +
				"the natural environment within a specified timeframe after " +
				"deployment. This will involve researching novel bioplastics, " +
				"conducting degradation rate studies in simulated soil and water " +
				"conditions, and ensuring the material doesn't leach harmful " +
				"byproducts.", "Task", "TODO"),
				("Wireless Data Transmission Protocol Design", "Design and " +
				"implement a low-power, short-range wireless data transmission " +
				"protocol for the sensor network. Given the biodegradable nature " +
				"of the nodes and their remote locations, the protocol must be" +
				"incredibly energy-efficient to maximize battery life " +
				"(powered by the microbial fuel cells) and robust enough to " +
				"transmit data reliably through varying terrain and foliage. " +
				"This includes selecting appropriate radio frequencies, " +
				"developing error correction mechanisms, and designing a secure " +
				"communication pathway to a central data collection hub.",
				"Task", "TODO"));

			//	Tasks can also be bulk-assigned programmatically, using the
			//	AssociateTasksByName function.
			Console.WriteLine("Associate 3 tasks to Echoes of Tomorrow...");
			count = projectContext.Tasks.AssociateTasksByName("Echoes of Tomorrow",
				"Microbial Fuel Cell Optimization",
				"Biodegradable Polymer Casing Development",
				"Wireless Data Transmission Protocol Design");
			if(count != 3)
			{
				Console.WriteLine(" Error: 3 associations were expected, and " +
					$"{count} {(count == 1 ? "was" : "were")} found.");
			}
			else
			{
				Console.WriteLine(" Great: 3 tasks have been associated.");
			}

			task = projectContext.Tasks["EchoesOfTomorrow"];
			if(task != null)
			{
				//	Project found using compact format.
				//	Set this project to complete after the maiden project.
				task.Dependencies.Add(new DependencyItem()
				{
					DependencyType = DependencyTypeEnum.StartOnCompletion,
					RemoteDependency = projectContext.Tasks["MaidenProject"]
				});
			}

			//	Create time estimates for all of the tasks.
			tasks = projectContext.Tasks.FindAll(x =>
				CompactValue(x.ItemType?.DisplayName) == "task");
			foreach(TaskItem taskItem in tasks)
			{
				taskItem.EstimatedManHours = RandomFloat(1f, 25f);
			}

			//	Create the general weekday schedule. M-F 8:00-12:00, 1:00-5:00
			//	If we don't assign any individuals to projects or tasks, the
			//	scheduler will interpret that each of the contacts in the
			//	file is on the team, and those individuals will be servicing
			//	tasks on a first-come-first-serve basis.
			timeBlockWeekday = new TimeBlockItem
			{
				DisplayName = "Weekdays"
			};
			timeBlockWeekday.Entries.Add(
				(ScheduleRepetitionRate.Weekday,
					new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0)),
				(ScheduleRepetitionRate.Weekday,
					new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0))
			);
			timeBlockWeekday.ExtendedProperties.SetValue("Default", "1");
			projectContext.TimeBlocks.Add(timeBlockWeekday);

			projectContext.FreeBusyConnectors.Clear();
			projectContext.Tasks.ClearCalculatedFlag();

			scheduler = new ScheduleEngine(projectContext);
			//	Estimate the first project first.
			freeBusyItems =
				scheduler.CalculateTask(projectContext.Tasks["MaidenProject"]);
			Console.WriteLine("Schedule Entries:");
			foreach(FreeBusyItem freeBusyItem in freeBusyItems)
			{
				Console.WriteLine($" {freeBusyItem}");
			}

			Console.WriteLine("Clearing Project Context...");
			ActiveProjectContext.Clear();
			project = new TaskItem("Main Project", "Project");
			task = project.Tasks.Add("Design Module");

			task.Dependencies.Add(
				"Requirements Gathering",
				DependencyTypeEnum.StartOnCompletion);

			Console.WriteLine("Tasks Created:");
			foreach(TaskItem taskItem in ActiveProjectContext.Tasks)
			{
				Console.WriteLine($" {taskItem.DisplayName}");
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
