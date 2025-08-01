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
using System.Linq;
using System.Text;

using static ProjectTask.ProjectTaskUtil;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	ScheduleEngine																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Automated scheduling engine.
	/// </summary>
	public class ScheduleEngine
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* AddAllocation																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add and schedule an allocation, as appropriate, for the specified
		/// contact and task combination.
		/// </summary>
		/// <param name="allocations">
		/// Reference to the collection of allocations under construction.
		/// </param>
		/// <param name="contact">
		/// Reference to the contact to be added or updated.
		/// </param>
		/// <param name="task">
		/// Reference to the task to be updated.
		/// </param>
		private void AddAllocation(List<ContactAllocationItem> allocations,
			ContactItem contact, TaskItem task)
		{
			ContactAllocationItem contactAllocation = null;
			List<FreeBusyItem> freeBusies = null;
			TimeBlockItem timeBlock = null;

			if(allocations != null && contact != null && task != null)
			{
				contactAllocation =
					allocations.FirstOrDefault(x =>
						x.Contact.ItemId == contact.ItemId);
				if(contactAllocation == null)
				{
					contactAllocation = new ContactAllocationItem()
					{
						Contact = contact
					};
					freeBusies = mProjectFile.FreeBusyConnectors.FindAll(x =>
						x.Contact.ItemId == contact.ItemId && x.Busy == true);
					foreach(FreeBusyItem busyItem in freeBusies)
					{
						contactAllocation.FreeBusyConnections.Add(
							FreeBusyItem.Clone(busyItem));
					}
					allocations.Add(contactAllocation);
				}
				if(!contactAllocation.Tasks.Exists(x => x.ItemId == task.ItemId))
				{
					contactAllocation.Tasks.Add(task);
				}
				if(contact.Schedule.Count > 0)
				{
					foreach(TimeBlockItem blockItem in contact.Schedule)
					{
						if(!contactAllocation.TimeBlocks.Exists(x =>
							x.ItemId == blockItem.ItemId))
						{
							//	All schedule-related entries are sacrificial.
							contactAllocation.TimeBlocks.Add(
								TimeBlockItem.Clone(blockItem));
						}
					}
				}
				else if(contactAllocation.TimeBlocks.Count == 0)
				{
					//	If the contact doesn't have an explicit schedule and one
					//	has not yet been assigned, then create one from the default
					//	time block or from an 'always on' model.
					timeBlock = mProjectFile.TimeBlocks.FirstOrDefault(x =>
						x.ExtendedProperties.GetStringValue("Default") == "1");
					if(timeBlock == null)
					{
						timeBlock = new TimeBlockItem()
						{
							DisplayName = "AlwaysOn",
							ItemId = 25073110,
							ItemTicket = "f313d61f-e94e-4971-9b09-6384f4389625"
						};
						timeBlock.Entries.Add(new TimeNotationItem()
						{
							RepetitionRate = ScheduleRepetitionRate.YearDay,
							ActiveDayIndex = 1,
							ActiveMonthIndex = 1,
							StartDate = DateTime.Now.Date.AddDays(-1d),
							EndDate = DateTime.Now.Date.AddYears(10)
						});
					}
					if(!contactAllocation.TimeBlocks.Exists(x =>
						x.ItemId == timeBlock.ItemId))
					{
						//	All schedule-related entries are sacrificial.
						contactAllocation.TimeBlocks.Add(
							TimeBlockItem.Clone(timeBlock));
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AddChildTasks																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add the child tasks of the current parent task to the list, if they are
		/// unique.
		/// </summary>
		/// <param name="parentTask">
		/// Reference to the task whose children will be added.
		/// </param>
		/// <param name="tasks">
		/// Reference to the collection of tasks to which any children will be
		/// appended.
		/// </param>
		private static void AddChildTasks(TaskItem parentTask,
			List<TaskItem> tasks)
		{
			if(parentTask != null && tasks != null)
			{
				foreach(TaskItem taskItem in parentTask.Tasks)
				{
					AddChildTasks(taskItem, tasks);
					if(IsOpen(taskItem.ItemStatus) &&
						!tasks.Exists(x => x.ItemTicket == taskItem.ItemTicket))
					{
						tasks.Add(taskItem);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AddDependentTasks																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add all dependent tasks to the correct positions in the list.
		/// </summary>
		/// <param name="tasks">
		/// Reference to the collection of tasks to be completed.
		/// </param>
		private void AddDependentTasks(List<TaskItem> tasks)
		{
			bool bContinue = true;
			int count = 0;
			List<DependencyItem> dependentEntries = null;
			List<TaskItem> dependentTasks = new List<TaskItem>();
			int index = 0;
			//TaskItem remote = null;
			int remoteIndex = 0;
			TimeSpan span = TimeSpan.Zero;
			TaskItem task = null;

			//	TODO: !1 - Stopped here...
			//	TODO: Currently rearranging the Local <-> Remote dependency placements.
			//	When defining this kind of record, we say This item can start on completion of 'Remote Project'.
			if(mProjectFile != null && tasks?.Count > 0)
			{
				count = tasks.Count;
				for(index = 0; index < count; index ++)
				{
					bContinue = true;
					task = tasks[index];
					dependentEntries =
						mProjectFile.Dependencies.FindAll(x =>
							x.RemoteDependency.ItemId == task.ItemId);
					dependentTasks = mProjectFile.Tasks.FindAll(x =>
						dependentEntries.Exists(y =>
							x.Dependencies.Exists(z => z.ItemId == y.ItemId)));
					foreach(TaskItem taskItem in dependentTasks)
					{
						dependentEntries = taskItem.Dependencies.FindAll(x =>
							x.RemoteDependency.ItemId == task.ItemId);
						foreach(DependencyItem dependencyItem in dependentEntries)
						{
							switch(dependencyItem.DependencyType)
							{
								case DependencyTypeEnum.StartAfter:
								case DependencyTypeEnum.StartOnCompletion:
									//	That task can start after this task begins or
									//	after it ends.
									remoteIndex = -1;
									//remote = taskItem;
									if(tasks.Exists(x => x.ItemTicket == taskItem.ItemTicket))
									{
										remoteIndex = tasks.IndexOf(taskItem);
									}
									else
									{
										tasks.Insert(index, taskItem);
										index--;  //	Deindex. Re-measure.
										index -= InsertChildTasks(index, taskItem, tasks);
										bContinue = false;
									}
									if(remoteIndex > index)
									{
										//	The dependent item occurs after this item. Move it up.
										tasks.RemoveAt(remoteIndex);
										tasks.Insert(index, taskItem);
										index--;  //	Deindex. Re-measure.
										MoveChildTasksToIndex(index, taskItem, tasks);
										bContinue = false;
									}
									break;
								case DependencyTypeEnum.TriggerRisingEdge:
									//	Trigger the remote task into action when this task
									//	is started. Used for industrial automation (rising edge).
									break;
								case DependencyTypeEnum.TriggerFallingEdge:
									//	Trigger the remote task into action upon completion of
									//	this task. Used for industrial automation (falling edge).
									break;
							}
							if(!bContinue)
							{
								break;
							}
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AllChildTasksCalculated																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether all child tasks of the specified
		/// task have been calculated.
		/// </summary>
		/// <param name="tasks">
		/// Reference to the collection of tasks being worked on.
		/// </param>
		/// <param name="task">
		/// Reference to the task whose children will be checked.
		/// </param>
		/// <returns>
		/// True if all child tasks of the main task have been calculated.
		/// Otherwise, false.
		/// </returns>
		private static bool AllChildTasksCalculated(List<TaskItem> tasks,
			TaskItem task)
		{
			TaskItem childTask = null;
			bool result = false;

			if(tasks?.Count > 0 && task != null)
			{
				result = true;
				foreach(TaskItem childTaskItem in task.Tasks)
				{
					childTask = tasks.FirstOrDefault(x =>
						x.ItemId == childTaskItem.ItemId);
					if(childTask == null ||
						childTask.CalculatedTimeElapsed <
						childTask.CalculatedTimeEstimated)
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AllDependentTasksCalculated																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether all dependent tasks of the specified
		/// task have been calculated.
		/// </summary>
		/// <param name="tasks">
		/// Reference to the collection of tasks being worked on.
		/// </param>
		/// <param name="task">
		/// Reference to the task whose dependent tasks will be checked.
		/// </param>
		/// <returns>
		/// True if all dependent tasks of the main task have been calculated.
		/// Otherwise, false.
		/// </returns>
		private static bool AllDependentTasksCalculated(List<TaskItem> tasks,
			TaskItem task)
		{
			TaskItem dependentTask = null;
			bool result = false;

			if(tasks?.Count > 0 && task != null)
			{
				result = true;
				foreach(DependencyItem dependencyItem in task.Dependencies)
				{
					dependentTask = tasks.FirstOrDefault(x =>
						x.ItemId == dependencyItem.RemoteDependency.ItemId);
					if(dependentTask == null ||
						dependentTask.CalculatedTimeElapsed <
						dependentTask.CalculatedTimeEstimated)
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AllTasksCalculated																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether all task calculation estimates have
		/// been completed.
		/// </summary>
		/// <param name="tasks">
		/// Reference to the collection of tasks to test.
		/// </param>
		/// <returns>
		/// True if the calculated hours for all tasks have been completed.
		/// Otherwise, false.
		/// </returns>
		private static bool AllTasksCalculated(List<TaskItem> tasks)
		{
			bool result = true;

			if(tasks?.Count > 0)
			{
				foreach(TaskItem taskItem in tasks)
				{
					if(taskItem.CalculatedTimeElapsed < taskItem.CalculatedTimeEstimated)
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AssignContacts																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Assign contacts to all of the tasks that don't yet have any explicit
		/// contacts.
		/// </summary>
		/// <param name="tasks">
		/// Reference to the collection of tasks for which contacts need to be
		/// assigned.
		/// </param>
		/// <returns>
		/// Reference to a collection of contact, task, time allocation
		/// helpers that have been created to allow the matching of contacts
		/// to tasks.
		/// </returns>
		private List<ContactAllocationItem> AssignContacts(List<TaskItem> tasks)
		{
			List<ContactAllocationItem> contactAllocations =
				new List<ContactAllocationItem>();
			DateTime currentTime = DateTime.Now;
			List<ContactItem> taskContacts = null;
			List<ContactItem> teamContacts = null;

			if(tasks?.Count > 0)
			{
				taskContacts = GetTaskContacts(tasks);
				foreach(TaskItem taskItem in tasks)
				{
					if(taskItem.TeamContacts.Count == 0)
					{
						teamContacts = TaskItem.GetInheritedTeamContacts(taskItem);
						if(teamContacts.Count > 0)
						{
							//	Inherited contacts.
							foreach(ContactItem contactItem in teamContacts)
							{
								AddAllocation(contactAllocations, contactItem, taskItem);
							}
						}
						else
						{
							//	There are no inherited contacts for this item. Use
							//	the full-spectrum contacts.
							foreach(ContactItem contactItem in taskContacts)
							{
								AddAllocation(contactAllocations, contactItem, taskItem);
							}
						}
					}
					else
					{
						//	Explicitly assigned contacts.
						foreach(ContactItem contactItem in taskItem.TeamContacts)
						{
							AddAllocation(contactAllocations, contactItem, taskItem);
						}
					}
				}
			}
			return contactAllocations;
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* ClearAllocations																											*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Clear all of the time allocations related to tasks in the supplied
		///// list.
		///// </summary>
		///// <param name="tasks">
		///// Reference to the collection of tasks that will be releasing all of
		///// their allocations on the free/busy board.
		///// </param>
		//private void ClearAllocations(List<TaskItem> tasks)
		//{
		//	List<FreeBusyItem> freeBusyClips = null;

		//	if(mProjectFile != null && tasks?.Count > 0)
		//	{
		//		foreach(TaskItem taskItem in tasks)
		//		{
		//			freeBusyClips =
		//				mProjectFile.FreeBusyConnectors.FindAll(x =>
		//					x.Task?.ItemId == taskItem.ItemId);
		//			foreach(FreeBusyItem busyItem in freeBusyClips)
		//			{
		//				busyItem.Task = null;
		//				busyItem.Busy = false;
		//			}
		//		}
		//	}
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FinalizeCalculations																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Finalize the calculations of each of the tasks in the collection.
		/// </summary>
		/// <param name="tasks">
		/// Reference to the collection of tasks for which to finalize calculation.
		/// </param>
		private static void FinalizeCalculations(List<TaskItem> tasks)
		{
			if(tasks?.Count > 0)
			{
				foreach(TaskItem taskItem in tasks)
				{
					taskItem.ExtendedProperties.SetValue("Calculated", "1");
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetEarliestStartDate																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the earliest work date found associated with the specified task.
		/// </summary>
		/// <param name="freeBusyItems">
		/// Reference to a collection of free/busy connectors to inspect.
		/// </param>
		/// <param name="task">
		/// Reference to the task to be identified.
		/// </param>
		/// <returns>
		/// The earliest starting work date found for the specified task in the
		/// provided connectors.
		/// </returns>
		private static DateTime GetEarliestStartDate(
			List<FreeBusyItem> freeBusyItems, TaskItem task)
		{
			int itemId = 0;
			DateTime result = DateTime.MaxValue;

			if(freeBusyItems?.Count > 0 && task != null)
			{
				itemId = task.ItemId;
				foreach(FreeBusyItem busyItem in freeBusyItems)
				{
					if(busyItem.Task.ItemId == itemId &&
						DateTime.Compare(busyItem.DateRange.StartDate, result) < 0)
					{
						result = busyItem.DateRange.StartDate;
					}
				}
			}
			if(DateTime.Compare(result, DateTime.MaxValue) == 0)
			{
				result = DateTime.MinValue;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetLatestEndDate																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the latest work date found associated with the specified task.
		/// </summary>
		/// <param name="freeBusyItems">
		/// Reference to a collection of free/busy connectors to inspect.
		/// </param>
		/// <param name="task">
		/// Reference to the task to be identified.
		/// </param>
		/// <returns>
		/// The latest ending work date found for the specified task in the
		/// provided connectors.
		/// </returns>
		private static DateTime GetLatestEndDate(
			List<FreeBusyItem> freeBusyItems, TaskItem task)
		{
			int itemId = 0;
			DateTime result = DateTime.MinValue;

			if(freeBusyItems?.Count > 0 && task != null)
			{
				itemId = task.ItemId;
				foreach(FreeBusyItem busyItem in freeBusyItems)
				{
					if(busyItem.Task.ItemId == itemId &&
						DateTime.Compare(busyItem.DateRange.EndDate, result) > 0)
					{
						result = busyItem.DateRange.EndDate;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetTaskContacts																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a reference to a collection of contacts found on all tasks in
		/// the supplied task collection.
		/// </summary>
		/// <param name="tasks">
		/// Reference to the collection of tasks to review.
		/// </param>
		/// <returns>
		/// Reference to the collection of contacts found on all tasks in the
		/// supplied task collection. If no contacts were found on tasks, the
		/// collection of all contacts is returned. If no contacts were found in
		/// the project file, an empty collection is returned.
		/// </returns>
		private List<ContactItem> GetTaskContacts(List<TaskItem> tasks)
		{
			List<ContactItem> result = new List<ContactItem>();

			if(tasks?.Count > 0 &&
				tasks.Exists(x => x.TeamContacts.Count > 0) &&
				tasks.Exists(x => x.TeamContacts.Count == 0))
			{
				//	If there are any contacts in the full list, then use those to
				//	populate the unpopulated items.
				foreach(TaskItem taskItem in tasks)
				{
					foreach(ContactItem contactItem in taskItem.TeamContacts)
					{
						if(!result.Exists(x => x.ItemId == contactItem.ItemId))
						{
							result.Add(contactItem);
						}
					}
				}
			}
			else if(tasks == null || !tasks.Exists(x => x.TeamContacts.Count > 0))
			{
				//	When no contacts have been specified, then all contacts are
				//	on the team, if there are any.
				foreach(ContactItem contactItem in mProjectFile.Contacts)
				{
					result.Add(contactItem);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* InitializeCalculations																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Initialize the calculation values on each task in the list.
		/// </summary>
		/// <param name="tasks">
		/// Reference to the collection of tasks to be calculated.
		/// </param>
		private static void InitializeCalculations(List<TaskItem> tasks)
		{
			if(tasks?.Count > 0)
			{
				foreach(TaskItem taskItem in tasks)
				{
					taskItem.ExtendedProperties.SetValue("Estimated", "0");
					taskItem.CalculatedStartDate = DateTime.MinValue;
					taskItem.CalculatedEndDate = DateTime.MinValue;
					taskItem.CalculatedTimeElapsed =
						TimerCollection.Sum(taskItem.Timers);
					taskItem.CalculatedTimeEstimated =
						taskItem.EstimatedManHours - taskItem.CalculatedTimeElapsed;
					if(taskItem.CalculatedTimeEstimated < 0f)
					{
						taskItem.CalculatedTimeEstimated = 0f;
					}
					if(taskItem.EstimatedManHours == 0f)
					{
						//	TODO: Allow the administrator to set default task time to trend or specific value.
						//	When zero man-hours were estimated, let's default to 1 if it
						//	is a task and 0 if it is a project.
						if(IsTaskType(taskItem))
						{
							taskItem.EstimatedManHours = 1f;
							taskItem.CalculatedTimeEstimated += taskItem.EstimatedManHours;
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* InitializeSchedule																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Initialize the contact free/busy schedules using the provided
		/// collection of per-contact assignments and the total time needed for
		/// all tasks.
		/// </summary>
		/// <param name="allocations">
		/// Reference to a collection of schedule allocations made per contact.
		/// </param>
		private void InitializeSchedule(List<ContactAllocationItem> allocations,
			float totalTime)
		{
			ContactItem contact = null;
			int count = 0;
			DateTime currentDate = DateTime.Now + new TimeSpan(1, 0, 0);
			float currentTime = totalTime;
			DateRangeItem dateRange = null;
			int dayIndex = 0;
			FreeBusyItem freeBusy = null;
			int index = 0;
			List<DateRangeItem> renderedDates = null;
			List<DateRangeItem> runningDates = new List<DateRangeItem>();
			TimeSpan timeSpan = new TimeSpan(24, 0, 0);

			if(mProjectFile != null && allocations?.Count > 0 && totalTime > 0f)
			{
				foreach(ContactAllocationItem allocationItem in allocations)
				{
					//	Current contact.
					contact = allocationItem.Contact;
					while(currentTime > 0f && dayIndex < 366)
					{
						foreach(TimeBlockItem blockItem in allocationItem.TimeBlocks)
						{
							foreach(TimeNotationItem notationItem in blockItem.Entries)
							{
								renderedDates =
									TimeNotationItem.RenderDay(notationItem, currentDate);
								count = renderedDates.Count;
								for(index = 0; index < count; index ++)
								{
									dateRange = renderedDates[index];
									freeBusy =
										allocationItem.FreeBusyConnections.Add(dateRange);
									if(freeBusy != null)
									{
										//	These are all free times to allocate.
										freeBusy.Busy = false;
										//	Loop member changed. No foreach allowed.
										renderedDates[index] = freeBusy.DateRange;
									}
									else
									{
										renderedDates[index].StartDate = DateTime.MinValue;
										renderedDates[index].EndDate = DateTime.MinValue;
									}
								}
								currentTime -= DateRangeCollection.TotalHours(renderedDates);
							}
						}
						dayIndex++;
						currentDate += timeSpan;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* InsertChildTasks																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Insert the child tasks of the current parent task at the specified
		/// index in the list, if they are unique.
		/// </summary>
		/// <param name="targetIndex">
		/// The target list index at which the child tasks will be inserted.
		/// </param>
		/// <param name="parentTask">
		/// Reference to the task whose children will be added.
		/// </param>
		/// <param name="tasks">
		/// Reference to the collection of tasks to which any children will be
		/// appended.
		/// </param>
		/// <returns>
		/// Count of items inserted into the list.
		/// </returns>
		private static int InsertChildTasks(int targetIndex, TaskItem parentTask,
			List<TaskItem> tasks)
		{
			int count = 0;
			int index = 0;
			int result = 0;
			TaskItem taskItem = null;

			if(parentTask != null && tasks != null)
			{
				count = parentTask.Tasks.Count;
				for(index = count - 1; index > -1; index --)
				{
					taskItem = parentTask.Tasks[index];
					result += InsertChildTasks(targetIndex, taskItem, tasks);
					if(IsOpen(taskItem.ItemStatus) &&
						!tasks.Exists(x => x.ItemTicket == taskItem.ItemTicket))
					{
						tasks.Insert(targetIndex, taskItem);
						result++;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* MatchTasks																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Match the estimated tasks to the available contact time.
		/// </summary>
		/// <param name="contactAllocations">
		/// Reference to the collection of contact time allocations,
		/// associated tasks, and free/busy connectors.
		/// </param>
		/// <param name="tasks">
		/// Reference to the collection of all tasks currently in review.
		/// </param>
		/// <returns>
		/// Reference to a collection of Free/Busy connectors created to
		/// associate with the contacts and tasks.
		/// </returns>
		private static List<FreeBusyItem> MatchTasks(
			List<ContactAllocationItem> contactAllocations, List<TaskItem> tasks)
		{
			FreeBusyItem availableTime = null;
			DateTime endDate = DateTime.MinValue;
			FreeBusyItem freeBusy = null;
			List<FreeBusyItem> freeBusyItems = new List<FreeBusyItem>();
			float hoursAvailable = 0f;
			float hoursOutstanding = 0f;
			int passCount = 10;
			int passIndex = 0;

			if(contactAllocations?.Count > 0)
			{
				for(passIndex = 0; passIndex < passCount; passIndex ++)
				{
					foreach(ContactAllocationItem allocationItem in contactAllocations)
					{
						foreach(TaskItem taskItem in allocationItem.Tasks)
						{
							hoursOutstanding =
								taskItem.CalculatedTimeEstimated -
								taskItem.CalculatedTimeElapsed;
							if(hoursOutstanding < 0f)
							{
								hoursOutstanding = 0f;
							}
							if(hoursOutstanding > 0f &&
								AllChildTasksCalculated(tasks, taskItem) &&
								AllDependentTasksCalculated(tasks, taskItem))
							{
								//	Still work to do on this task.
								availableTime = null;
								if(DateTime.Compare(taskItem.StartDate,
									DateTime.MinValue) == 0 ||
									DateTime.Compare(taskItem.StartDate, DateTime.Today) <= 0)
								{
									//	Work can start at any time.
									availableTime = allocationItem.FreeBusyConnections.
										FirstOrDefault(x =>
											x.Busy == false &&
											DateTime.Compare(
												x.DateRange.StartDate, DateTime.Today) >= 0);
								}
								else
								{
									//	Get the first available time after the start date.
									availableTime = allocationItem.FreeBusyConnections.
										FirstOrDefault(x =>
											x.Busy == false &&
											DateTime.Compare(
												x.DateRange.StartDate, taskItem.StartDate) >= 0);
								}
								if(availableTime != null)
								{
									//	Apply the available time to this task.
									freeBusy = new FreeBusyItem()
									{
										Busy = true,
										Contact = allocationItem.Contact,
										Task = taskItem,
										TimeNotation = availableTime.TimeNotation
									};
									hoursAvailable =
										DateRangeItem.TotalHours(availableTime.DateRange);
									if(hoursAvailable >= hoursOutstanding)
									{
										//	Clear out the task calculation.
										endDate = availableTime.DateRange.EndDate;
										freeBusy.DateRange.StartDate =
											availableTime.DateRange.StartDate;
										freeBusy.DateRange.Duration =
											ToTimeSpan(hoursOutstanding);
										availableTime.DateRange.StartDate =
											freeBusy.DateRange.EndDate;
										availableTime.DateRange.EndDate = endDate;
										taskItem.CalculatedTimeElapsed =
											taskItem.CalculatedTimeEstimated;
									}
									else
									{
										//	Apply all of the available hours for this entry.
										freeBusy.DateRange.StartDate =
											availableTime.DateRange.StartDate;
										freeBusy.DateRange.EndDate =
											availableTime.DateRange.EndDate;
										taskItem.CalculatedTimeElapsed += hoursAvailable;
									}
									freeBusyItems.Add(freeBusy);
								}
							}
						}
					}
					if(AllTasksCalculated(tasks))
					{
						break;
					}
				}
				foreach(TaskItem taskItem in tasks)
				{
					taskItem.CalculatedStartDate =
						GetEarliestStartDate(freeBusyItems, taskItem);
					taskItem.CalculatedEndDate =
						GetLatestEndDate(freeBusyItems, taskItem);
				}
			}
			return freeBusyItems;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* MoveChildTasksToIndex																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Move the child tasks of the current parent task to the specified
		/// index in the list.
		/// </summary>
		/// <param name="targetIndex">
		/// The target list index at which the child tasks will be inserted.
		/// </param>
		/// <param name="parentTask">
		/// Reference to the task whose children will be moved.
		/// </param>
		/// <param name="tasks">
		/// Reference to the collection of tasks to be rearranged.
		/// </param>
		private static void MoveChildTasksToIndex(int targetIndex,
			TaskItem parentTask, List<TaskItem> tasks)
		{
			int childIndex = 0;
			int count = 0;
			int index = 0;
			TaskItem taskItem = null;

			if(parentTask != null && tasks != null)
			{
				count = parentTask.Tasks.Count;
				for(index = count - 1; index > -1; index--)
				{
					taskItem = parentTask.Tasks[index];
					childIndex = tasks.IndexOf(taskItem);
					if(childIndex > targetIndex)
					{
						//	Only the move the item to a higher priority.
						MoveChildTasksToIndex(targetIndex, taskItem, tasks);
						if(IsOpen(taskItem.ItemStatus) &&
							!tasks.Exists(x => x.ItemTicket == taskItem.ItemTicket))
						{
							tasks.RemoveAt(childIndex);
							tasks.Insert(targetIndex, taskItem);
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the ScheduleEngine item.
		/// </summary>
		public ScheduleEngine()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the ScheduleEngine item.
		/// </summary>
		/// <param name="projectFile">
		/// Reference to the project file upon which schedules will be created.
		/// </param>
		public ScheduleEngine(ProjectFile projectFile)
		{
			ProjectFile = projectFile;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CalculateTask																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Calcukate the specified task and its children and dependencies.
		/// </summary>
		/// <param name="task">
		/// Reference to a project or task to calculate. If the item has child
		/// tasks, those will be calculated prior to using any estimated time in
		/// this item for the personnel directly assigned to this or any parent
		/// items.
		/// </param>
		/// <returns>
		/// Reference to a collection of free/busy clips allocated by this task and
		/// its dependencies.
		/// </returns>
		public List<FreeBusyItem> CalculateTask(TaskItem task)
		{
			List<ContactAllocationItem> contactAllocations = null;
			List<TaskItem> tasks = new List<TaskItem>();
			List<FreeBusyItem> schedule = new List<FreeBusyItem>();
			float totalTime = 0f;

			if(mProjectFile != null && task != null && IsOpen(task.ItemStatus))
			{
				AddChildTasks(task, tasks);
				tasks.Add(task);
				AddDependentTasks(tasks);
				InitializeCalculations(tasks);

				//	Tasks are ready.
				totalTime = TaskCollection.GetTotalOutstandingEstimatedTime(tasks);

				contactAllocations = AssignContacts(tasks);
				InitializeSchedule(contactAllocations, totalTime);

				//	Contacts are ready.
				//	!1 - Stopped here...
				//	TODO: Render the calculated schedule.
				MatchTasks(contactAllocations, tasks);

				FinalizeCalculations(tasks);
			}
			return schedule;
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	Contacts																															*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Private member for <see cref="Contacts">Contacts</see>.
		///// </summary>
		//private ContactCollection mContacts = null;
		///// <summary>
		///// Get/Set a reference to the contacts in this file.
		///// </summary>
		//public ContactCollection Contacts
		//{
		//	get { return mContacts; }
		//	set { mContacts = value; }
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProjectFile																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ProjectFile">ProjectFile</see>.
		/// </summary>
		private ProjectTask.ProjectFile mProjectFile = null;
		/// <summary>
		/// Get/Set a reference to the project file in use for scheduling.
		/// </summary>
		public ProjectTask.ProjectFile ProjectFile
		{
			get { return mProjectFile; }
			set
			{
				mProjectFile = value;
				//if(value != null)
				//{
				//	mProjectFile = ProjectFile.Clone(value);
				//}
				//else
				//{
				//	mProjectFile = null;
				//}
				//if(mProjectFile != null)
				//{
				//	Clean up all empty references.
				mProjectFile.ClearEmptyReferences();
				//}
			}
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	Tasks																																	*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Private member for <see cref="Tasks">Tasks</see>.
		///// </summary>
		//private TaskCollection mTasks = null;
		///// <summary>
		///// Get/Set a reference to the tasks collection.
		///// </summary>
		//public TaskCollection Tasks
		//{
		//	get { return mTasks; }
		//	set { mTasks = value; }
		//}
		////*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
