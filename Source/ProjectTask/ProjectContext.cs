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
using System.Diagnostics;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using static ProjectTask.ProjectTaskUtil;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	ProjectContext																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Complete application context information about an entire set of
	/// projects, tasks, resources, and schedules.
	/// </summary>
	public class ProjectContext
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* ClearEmptyComments																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clear all of the comments in the collection that are either null or
		/// are reference placeholders for comments not found.
		/// </summary>
		/// <param name="comments">
		/// Reference to the comments collection to clean.
		/// </param>
		private static void ClearEmptyComments(List<CommentItem> comments)
		{
			CommentItem comment = null;
			int count = 0;
			int index = 0;

			if(comments?.Count > 0)
			{
				count = comments.Count;
				for(index = 0; index < count; index++)
				{
					comment = comments[index];
					if(comment == null || IsReferenceOnly(comment))
					{
						//	Remove anything that hasn't been resolved.
						comments.RemoveAt(index);
						index--;  //	Deindex.
						count--;  //	Discount.
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ResolveCollection																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Resolve the members of a collection that might contain references.
		/// </summary>
		/// <typeparam name="T">
		/// Type of collection to enumerate.
		/// </typeparam>
		/// <param name="fileCollection">
		/// Reference to the full collection providing all of the lookup
		/// information.
		/// </param>
		/// <param name="refCollection">
		/// Reference to the collection containing reference items that may need
		/// to be resolved.
		/// </param>
		private static void ResolveCollection<T>(IList<T> fileCollection,
			IList<T> refCollection) where T : IItem, new()
		{
			int count = 0;
			int index = 0;
			T item = default(T);
			T lookup = default(T);

			if(fileCollection?.Count > 0 && refCollection?.Count > 0)
			{
				count = refCollection.Count;
				for(index = 0; index < count; index++)
				{
					item = refCollection[index];
					lookup = fileCollection.FirstOrDefault(x =>
						x.ItemTicket == item.ItemTicket);
					if(lookup != null)
					{
						refCollection[index] = lookup;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ResolveReferences																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Resolve all of the reference objects that are loaded solely through
		/// their tickets.
		/// </summary>
		/// <param name="file">
		/// Reference to the project file for which the references will be
		/// resolved.
		/// </param>
		private static void ResolveReferences(ProjectContext file)
		{
			ContactItem contact = null;
			OrganizationItem organization = null;
			TaskItem task = null;
			TaskStatusItem taskStatus = null;
			TaskTypeItem taskType = null;

			//	TODO: Check the following items after deserialization to see if constructors were bypassed.
			//	Comments.
			foreach(CommentItem commentItem in file.mComments)
			{
				if(commentItem.ItemId == 0)
				{
					commentItem.ItemId = CommentItem.NextItemId++;
				}
			}
			//	Contacts.
			foreach(ContactItem contactItem in file.mContacts)
			{
				if(contactItem.ItemId == 0)
				{
					contactItem.ItemId = ContactItem.NextItemId++;
				}
				if(contactItem.DefaultOrganization != null)
				{
					organization = file.mOrganizations.FirstOrDefault(x =>
						x.ItemTicket == contactItem.DefaultOrganization.ItemTicket);
					if(organization != null)
					{
						contactItem.DefaultOrganization = organization;
					}
				}
				if(contactItem.SupervisorContact != null)
				{
					contact = file.mContacts.FirstOrDefault(x =>
						x.ItemTicket == contactItem.SupervisorContact.ItemTicket);
					if(contact != null)
					{
						contactItem.SupervisorContact = contact;
					}
				}
				ResolveCollection(file.mComments, contactItem.Comments);
			}
			//	Dependencies.
			foreach(DependencyItem dependencyItem in file.mDependencies)
			{
				if(dependencyItem.ItemId == 0)
				{
					dependencyItem.ItemId = DependencyItem.NextItemId++;
				}
				if(dependencyItem.RemoteDependency != null)
				{
					task = file.mTasks.FirstOrDefault(x =>
						x.ItemTicket == dependencyItem.RemoteDependency.ItemTicket);
					if(task != null)
					{
						dependencyItem.RemoteDependency = task;
					}
				}
				ResolveCollection(file.mComments, dependencyItem.Comments);
			}
			//	Free/Busy Connections.
			foreach(FreeBusyItem freeBusyItem in file.mFreeBusyConnectors)
			{
				if(freeBusyItem.ItemId == 0)
				{
					freeBusyItem.ItemId = FreeBusyItem.NextItemId++;
				}
				if(freeBusyItem.Contact != null)
				{
					contact = file.mContacts.FirstOrDefault(x =>
						x.ItemTicket == freeBusyItem.Contact.ItemTicket);
					if(contact != null)
					{
						freeBusyItem.Contact = contact;
					}
				}
				if(freeBusyItem.Task != null)
				{
					task = file.mTasks.FirstOrDefault(x =>
						x.ItemTicket == freeBusyItem.Task.ItemTicket);
					if(task != null)
					{
						freeBusyItem.Task = task;
					}
				}
			}
			//	Organizations.
			foreach(OrganizationItem organizationItem in file.mOrganizations)
			{
				if(organizationItem.ItemId == 0)
				{
					organizationItem.ItemId = OrganizationItem.NextItemId++;
				}
				if(organizationItem.DefaultContact != null)
				{
					contact = file.mContacts.FirstOrDefault(x =>
						x.ItemTicket == organizationItem.ItemTicket);
					if(contact != null)
					{
						organizationItem.DefaultContact = contact;
					}
				}
				ResolveCollection(file.mComments, organizationItem.Comments);
			}
			//	Tasks.
			foreach(TaskItem taskItem in file.mTasks)
			{
				if(taskItem.ItemId == 0)
				{
					taskItem.ItemId = TaskItem.NextItemId++;
				}
				if(taskItem.ItemStatus != null)
				{
					taskStatus = file.mTaskStates.FirstOrDefault(x =>
						x.ItemTicket == taskItem.ItemStatus.ItemTicket);
					if(taskStatus != null)
					{
						taskItem.ItemStatus = taskStatus;
					}
				}
				if(taskItem.ItemType != null)
				{
					taskType = file.mTaskTypes.FirstOrDefault(x =>
						x.ItemTicket == taskItem.ItemType.ItemTicket);
					if(taskType != null)
					{
						taskItem.ItemType = taskType;
					}
				}
				if(taskItem.OwnerContact != null)
				{
					contact = file.mContacts.FirstOrDefault(x =>
						x.ItemTicket == taskItem.OwnerContact.ItemTicket);
					if(contact != null)
					{
						taskItem.OwnerContact = contact;
					}
				}
				if(taskItem.ReviewerContact != null)
				{
					contact = file.mContacts.FirstOrDefault(x =>
						x.ItemTicket == taskItem.ReviewerContact.ItemTicket);
					if(contact != null)
					{
						taskItem.ReviewerContact = contact;
					}
				}
				ResolveCollection(file.mComments, taskItem.Comments);
				ResolveCollection(file.mDependencies, taskItem.Dependencies);
				ResolveCollection(file.mTimers, taskItem.Timers);
				ResolveCollection(file.mContacts, taskItem.TeamContacts);
				ResolveCollection(file.mTasks, taskItem.Tasks);
			}
			foreach(TaskItem taskItem in file.mTasks)
			{
				foreach(TaskItem childTaskItem in taskItem.Tasks)
				{
					if(childTaskItem.ParentTask == null)
					{
						childTaskItem.ParentTask = taskItem;
						break;
					}
				}
			}
			//	Task States.
			foreach(TaskStatusItem statusItem in file.mTaskStates)
			{
				if(statusItem.ItemId == 0)
				{
					statusItem.ItemId = TaskStatusItem.NextItemId++;
				}
				ResolveCollection(file.mComments, statusItem.Comments);
			}
			//	Task Types.
			foreach(TaskTypeItem typeItem in file.mTaskTypes)
			{
				if(typeItem.ItemId == 0)
				{
					typeItem.ItemId = TaskTypeItem.NextItemId++;
				}
				ResolveCollection(file.mComments, typeItem.Comments);
			}
			//	Time Blocks.
			foreach(TimeBlockItem blockItem in file.mTimeBlocks)
			{
				if(blockItem.ItemId == 0)
				{
					blockItem.ItemId = TimeBlockItem.NextItemId++;
				}
				ResolveCollection(file.mComments, blockItem.Comments);
				ResolveCollection(file.mTimeNotations, blockItem.Entries);
			}
			//	Time Notations.
			foreach(TimeNotationItem notationItem in file.mTimeNotations)
			{
				if(notationItem.ItemId == 0)
				{
					notationItem.ItemId = TimeNotationItem.NextItemId++;
				}
				ResolveCollection(file.mComments, notationItem.Comments);
			}
			//	Timers.
			foreach(TimerItem timerItem in file.mTimers)
			{
				if(timerItem.ItemId == 0)
				{
					timerItem.ItemId = TimerItem.NextItemId++;
				}
				ResolveCollection(file.mComments, timerItem.Comments);
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
		/// Create a new instance of the ProjectContext item.
		/// </summary>
		public ProjectContext()
		{
			mComments = new CommentCollection();
			mContacts = new ContactCollection();
			mDependencies = new DependencyCollection();
			mFreeBusyConnectors = new FreeBusyCollection();
			mOrganizations = new OrganizationCollection();
			mTasks = new TaskCollection();
			mTaskStates = new TaskStatusCollection();
			mTaskTypes = new TaskTypeCollection();
			mTimeBlocks = new TimeBlockCollection();
			mTimeNotations = new TimeNotationCollection();
			mTimers = new TimerCollection();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Clear																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clear the entire contents of the project context.
		/// </summary>
		public void Clear()
		{
			mComments.Clear();
			mContacts.Clear();
			mDependencies.Clear();
			mFreeBusyConnectors.Clear();
			mOrganizations.Clear();
			mTasks.Clear();
			mTaskStates.Clear();
			mTaskTypes.Clear();
			mTimeBlocks.Clear();
			mTimeNotations.Clear();
			mTimers.Clear();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ClearEmptyReferences																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clear the empty references on objects, setting them to null or removing
		/// them from collections.
		/// </summary>
		public void ClearEmptyReferences()
		{
			ContactItem contact = null;
			int count = 0;
			DependencyItem dependency = null;
			int index = 0;
			TimeNotationItem notation = null;
			TaskItem task = null;
			TimerItem timer = null;

			//	Contacts.
			foreach(ContactItem contactItem in mContacts)
			{
				ClearEmptyComments(contactItem.Comments);
				if(contactItem.DefaultOrganization != null &&
					IsReferenceOnly(contactItem.DefaultOrganization))
				{
					contactItem.DefaultOrganization = null;
				}
				if(contactItem.SupervisorContact != null &&
					IsReferenceOnly(contactItem.SupervisorContact))
				{
					contactItem.SupervisorContact = null;
				}
			}
			//	Dependencies.
			foreach(DependencyItem dependencyItem in mDependencies)
			{
				ClearEmptyComments(dependencyItem.Comments);
				if(dependencyItem.RemoteDependency != null &&
					IsReferenceOnly(dependencyItem.RemoteDependency))
				{
					dependencyItem.RemoteDependency = null;
				}
			}
			//	Free/Busy Connectors.
			foreach(FreeBusyItem freeBusyItem in mFreeBusyConnectors)
			{
				if(freeBusyItem.Contact != null &&
					IsReferenceOnly(freeBusyItem.Contact))
				{
					freeBusyItem.Contact = null;
				}
				if(freeBusyItem.Task != null &&
					IsReferenceOnly(freeBusyItem.Task))
				{
					freeBusyItem.Task = null;
				}
			}
			//	Organizations.
			foreach(OrganizationItem organizationItem in mOrganizations)
			{
				ClearEmptyComments(organizationItem.Comments);
				if(organizationItem.DefaultContact != null &&
					IsReferenceOnly(organizationItem.DefaultContact))
				{
					organizationItem.DefaultContact = null;
				}
			}
			//	Tasks.
			foreach(TaskItem taskItem in mTasks)
			{
				ClearEmptyComments(taskItem.Comments);
				count = taskItem.Dependencies.Count;
				for(index = 0; index < count; index++)
				{
					dependency = taskItem.Dependencies[index];
					if(dependency == null || IsReferenceOnly(dependency))
					{
						taskItem.Dependencies.RemoveAt(index);
						index--;  //	Deindex.
						count--;  //	Discount.
					}
				}
				if(taskItem.OwnerContact != null &&
					IsReferenceOnly(taskItem.OwnerContact))
				{
					taskItem.OwnerContact = null;
				}
				if(taskItem.ReviewerContact != null &&
					IsReferenceOnly(taskItem.ReviewerContact))
				{
					taskItem.ReviewerContact = null;
				}
				count = taskItem.Tasks.Count;
				for(index = 0; index < count; index++)
				{
					task = taskItem.Tasks[index];
					if(task == null || IsReferenceOnly(task))
					{
						taskItem.Tasks.RemoveAt(index);
						index--;  //	Deindex.
						count--;  //	Discount.
					}
				}
				count = taskItem.TeamContacts.Count;
				for(index = 0; index < count; index++)
				{
					contact = taskItem.TeamContacts[index];
					if(contact == null || IsReferenceOnly(contact))
					{
						taskItem.TeamContacts.RemoveAt(index);
						index--;  //	Deindex.
						count--;  //	Discount.
					}
				}
				count = taskItem.Timers.Count;
				for(index = 0; index < count; index++)
				{
					timer = taskItem.Timers[index];
					if(timer == null || IsReferenceOnly(timer))
					{
						taskItem.Timers.RemoveAt(index);
						index--;  //	Deindex.
						count--;  //	Discount.
					}
				}
			}
			//	Task States.
			foreach(TaskStatusItem statusItem in mTaskStates)
			{
				ClearEmptyComments(statusItem.Comments);
			}
			//	Task Types.
			foreach(TaskTypeItem typeItem in mTaskTypes)
			{
				ClearEmptyComments(typeItem.Comments);
			}
			//	Time Blocks.
			foreach(TimeBlockItem blockItem in mTimeBlocks)
			{
				ClearEmptyComments(blockItem.Comments);
				count = blockItem.Entries.Count;
				for(index = 0; index < count; index++)
				{
					notation = blockItem.Entries[index];
					if(notation == null || IsReferenceOnly(notation))
					{
						blockItem.Entries.RemoveAt(index);
						index--;  //	Deindex.
						count--;  //	Discount.
					}
				}
			}
			//	Time Notations.
			foreach(TimeNotationItem notationItem in mTimeNotations)
			{
				ClearEmptyComments(notationItem.Comments);
			}
			//	Timers.
			foreach(TimerItem timerItem in mTimers)
			{
				ClearEmptyComments(timerItem.Comments);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Clone																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a deep copy of the provided project file.
		/// </summary>
		/// <param name="projectFile">
		/// Reference to the project file to clone.
		/// </param>
		/// <returns>
		/// Reference to a new project file containing a deep copy of all of the
		/// objects in the caller's object model, if the conversion was successful.
		/// Otherwise, null.
		/// </returns>
		public static ProjectContext Clone(ProjectContext projectFile)
		{
			string content = "";
			ProjectContext result = null;

			if(projectFile != null)
			{
				content = Pack(projectFile);
				result = Parse(content);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Comments																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Comments">Comments</see>.
		/// </summary>
		private CommentCollection mComments = null;
		/// <summary>
		/// Get a reference to the collection of comments found in this file.
		/// </summary>
		public CommentCollection Comments
		{
			get { return mComments; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Contacts																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Contacts">Contacts</see>.
		/// </summary>
		private ContactCollection mContacts = null;
		/// <summary>
		/// Get a reference to the contacts in this file.
		/// </summary>
		public ContactCollection Contacts
		{
			get { return mContacts; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DefaultItemStatus																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="DefaultItemStatus">DefaultItemStatus</see>.
		/// </summary>
		private TaskStatusItem mDefaultItemStatus = null;
		/// <summary>
		/// Get/Set a reference to the default item status for new tasks.
		/// </summary>
		public TaskStatusItem DefaultItemStatus
		{
			get { return mDefaultItemStatus; }
			set { mDefaultItemStatus = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DefaultItemType																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="DefaultItemType">DefaultItemType</see>.
		/// </summary>
		private TaskTypeItem mDefaultItemType = null;
		/// <summary>
		/// Get/Set a reference to the default item type for new tasks.
		/// </summary>
		public TaskTypeItem DefaultItemType
		{
			get { return mDefaultItemType; }
			set { mDefaultItemType = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Dependencies																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="Dependencies">Dependencies</see>.
		/// </summary>
		private DependencyCollection mDependencies = null;
		/// <summary>
		/// Get a reference to the collection of dependency tasks in this file.
		/// </summary>
		public DependencyCollection Dependencies
		{
			get { return mDependencies; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FreeBusyConnectors																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="FreeBusyConnectors">FreeBusyConnectors</see>.
		/// </summary>
		private FreeBusyCollection mFreeBusyConnectors = new FreeBusyCollection();
		/// <summary>
		/// Get a reference to the collection of free/busy connectors for this
		/// file.
		/// </summary>
		public FreeBusyCollection FreeBusyConnectors
		{
			get { return mFreeBusyConnectors; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Organizations																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Organizations">Organizations</see>.
		/// </summary>
		private OrganizationCollection mOrganizations = null;
		/// <summary>
		/// Get a reference to the all of the organizations in the file.
		/// </summary>
		public OrganizationCollection Organizations
		{
			get { return mOrganizations; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Pack																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Pack the contents of the project file object model to a string and
		/// return that string to the caller.
		/// </summary>
		/// <param name="fileObject">
		/// Reference to the object model to be written.
		/// </param>
		/// <param name="messageHandler">
		/// Optional reference to a message handler delegate appointed to receive
		/// messages from this function, in the form of 'static void
		/// CallbackName(string message);'.
		/// </param>
		/// <returns>
		/// Text version of the project file, in JSON format, if successful.
		/// Otherwise, an empty string.
		/// </returns>
		public static string Pack(ProjectContext fileObject,
			Action<string> messageHandler = null)
		{
			string content = "";
			string message = "";

			if(fileObject != null)
			{
				try
				{
					//	TODO: Insert a pre-check here to verify that all referenced
					//	objects are in the database, then provide warnings whenever
					//	that wasn't the case.
					content = JsonConvert.SerializeObject(fileObject,
						Formatting.Indented);
				}
				catch(Exception ex)
				{
					message = $"Error in WriteFile: {ex.Message}";
				}
			}
			if(message.Length > 0)
			{
				if(messageHandler != null)
				{
					messageHandler(message);
				}
				else
				{
					Trace.WriteLine(message);
				}
			}
			return content;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Parse																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Parse the JSON formatted project file, returning a fully populated
		/// project file object model.
		/// </summary>
		/// <param name="content">
		/// JSON formatted content representing the project file to use.
		/// </param>
		/// <param name="messageHandler">
		/// Optional reference to a message handler delegate appointed to receive
		/// messages from this function, in the form of 'static void
		/// CallbackName(string message);'.
		/// </param>
		/// <returns>
		/// Reference to the object model representation of the caller's project
		/// file content, if successful. Otherwise, null.
		/// </returns>
		public static ProjectContext Parse(string content,
			Action<string> messageHandler = null)
		{
			string message = "";
			ProjectContext result = null;

			if(content?.Length > 0)
			{
				try
				{
					result = JsonConvert.DeserializeObject<ProjectContext>(content);
					ResolveReferences(result);
				}
				catch(Exception ex)
				{
					message = $"Error in Parse: {ex.Message}";
				}
			}
			else
			{
				message = "Error in Parse: No content supplied.";
			}
			if(message.Length > 0)
			{
				if(messageHandler != null)
				{
					messageHandler(message);
				}
				else
				{
					Trace.WriteLine(message);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ReadFile																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Read and parse the specified JSON file, returning a resulting
		/// project file.
		/// </summary>
		/// <param name="filename">
		/// The fully qualified path and filename of the file to read.
		/// </param>
		/// <param name="messageHandler">
		/// Optional reference to a message handler delegate appointed to receive
		/// messages from this function, in the form of 'static void
		/// CallbackName(string message);'.
		/// </param>
		/// <returns>
		/// Reference to the object-based representation of the specified file,
		/// if legitimate. Otherwise, null.
		/// </returns>
		public static ProjectContext ReadFile(string filename,
			Action<string> messageHandler = null)
		{
			string content = "";
			string message = "";
			ProjectContext result = null;

			if(filename?.Length > 0)
			{
				if(File.Exists(filename))
				{
					try
					{
						content = File.ReadAllText(filename);
						result = Parse(content, messageHandler);
					}
					catch(Exception ex)
					{
						message = $"Error in ReadFile: {ex.Message}";
					}
				}
				else
				{
					message = $"Error in ReadFile: File not found. {filename}";
				}
			}
			else
			{
				message = "Error in ReadFile: File not specified.";
			}
			if(message.Length > 0)
			{
				if(messageHandler != null)
				{
					messageHandler(message);
				}
				else
				{
					Trace.WriteLine(message);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Tasks																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Tasks">Tasks</see>.
		/// </summary>
		private TaskCollection mTasks = null;
		/// <summary>
		/// Get a reference to the tasks collection.
		/// </summary>
		public TaskCollection Tasks
		{
			get { return mTasks; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TaskStates																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TaskStates">TaskStates</see>.
		/// </summary>
		private TaskStatusCollection mTaskStates = null;
		/// <summary>
		/// Get a reference to the collection of defined task states.
		/// </summary>
		public TaskStatusCollection TaskStates
		{
			get { return mTaskStates; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TaskTypes																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TaskTypes">TaskTypes</see>.
		/// </summary>
		private TaskTypeCollection mTaskTypes = null;
		/// <summary>
		/// Get a reference to the collection of task type definitions.
		/// </summary>
		public TaskTypeCollection TaskTypes
		{
			get { return mTaskTypes; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TimeBlocks																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TimeBlocks">TimeBlocks</see>.
		/// </summary>
		private TimeBlockCollection mTimeBlocks = null;
		/// <summary>
		/// Get a reference to the collection of defined schedule time blocks.
		/// </summary>
		public TimeBlockCollection TimeBlocks
		{
			get { return mTimeBlocks; }
			set { mTimeBlocks = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TimeNotations																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TimeNotations">TimeNotations</see>.
		/// </summary>
		private TimeNotationCollection mTimeNotations = null;
		/// <summary>
		/// Get a reference to the collection of time notations defined for this
		/// file.
		/// </summary>
		public TimeNotationCollection TimeNotations
		{
			get { return mTimeNotations; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Timers																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Timers">Timers</see>.
		/// </summary>
		private TimerCollection mTimers = null;
		/// <summary>
		/// Get a reference to the collection of timers.
		/// </summary>
		public TimerCollection Timers
		{
			get { return mTimers; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* WriteFile																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Write the contents of the provided file object model to the specified
		/// file.
		/// </summary>
		/// <param name="fileObject">
		/// Reference to the object model version of the file to be written.
		/// </param>
		/// <param name="filename">
		/// Fully qualified path and filename of the file to write.
		/// </param>
		/// <param name="messageHandler">
		/// Optional reference to a message handler delegate appointed to receive
		/// messages from this function, in the form of 'static void
		/// CallbackName(string message);'.
		/// </param>
		public static void WriteFile(ProjectContext fileObject, string filename,
			Action<string> messageHandler = null)
		{
			string content = "";
			string message = "";
			if(fileObject != null)
			{
				if(filename?.Length > 0)
				{
					try
					{
						content = Pack(fileObject, messageHandler);
						if(content.Length > 0)
						{

						}
						File.WriteAllText(filename, content);
					}
					catch(Exception ex)
					{
						message = $"Error in WriteFile: {ex.Message}";
					}
				}
				else
				{
					message = "Error in WriteFile: Filename not specified.";
				}
			}
			else
			{
				message = "Error in WriteFile: File object not specified.";
			}
			if(message.Length > 0)
			{
				if(messageHandler != null)
				{
					messageHandler(message);
				}
				else
				{
					Trace.WriteLine(message);
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
