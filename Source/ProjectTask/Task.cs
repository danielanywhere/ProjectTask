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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using static ProjectTask.ProjectTaskUtil;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	TaskCollection																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of TaskItem Items.
	/// </summary>
	public class TaskCollection : ChangeObjectCollection<TaskItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Indexer																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a reference to the first task in the collection found with the
		/// specified display name.
		/// </summary>
		/// <param name="displayName">
		/// The display name of the task to be found.
		/// </param>
		/// <remarks>
		/// The tasks in the collection are compared by 'compact name', where
		/// all spaces are removed and a case-insensitive comparison is made.
		/// </remarks>
		public TaskItem this[string displayName]
		{
			get
			{
				TaskItem result = null;

				if(displayName?.Length > 0)
				{
					result = this.FirstOrDefault(x =>
						CompactEqual(x.DisplayName, displayName));
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add one or more tasks using a simple tuple approach.
		/// </summary>
		/// <param name="entries">
		/// One or more displayName, description, itemType, itemStatus tuples.
		/// </param>
		/// <returns>
		/// Reference to a list of items that have been added to the collection.
		/// </returns>
		/// <remarks>
		/// To use this method, call it similarly to:
		/// <code>project.Tasks.Add(
		/// (&quot;Maiden Project&quot;, 
		/// &quot;Creating a new project-level task.&quot;,
		/// &quot;Project&quot;, &quot;InProgress&quot;),
		/// (&quot;Kick-Off Task&quot;, 
		/// &quot;Creating a new task.&quot;,
		/// &quot;Task&quot;, &quot;TODO&quot;),
		/// </code>
		/// </remarks>
		public List<TaskItem> Add(
			params (string displayName, string description, string itemType,
				string itemStatus)[] entries)
		{
			List<TaskItem> result = new List<TaskItem>();
			TaskItem task = null;

			if(entries?.Length > 0)
			{
				foreach((string displayName, string description,
					string itemType, string itemStatus) item in entries)
				{
					if(item.displayName?.Length > 0)
					{
						task = new TaskItem()
						{
							DisplayName = item.displayName,
							Description = item.description
						};
						if(item.itemType?.Length > 0 && this.ProjectFile != null)
						{
							task.ItemType = this.ProjectFile.TaskTypes[item.itemType];
						}
						if(item.itemStatus?.Length > 0 && this.ProjectFile != null)
						{
							task.ItemStatus = this.ProjectFile.TaskStates[item.itemStatus];
						}
						this.Add(task);
						result.Add(task);
					}
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Add one or more tasks using a simple tuple approach.
		/// </summary>
		/// <param name="entries">
		/// One or more displayName, description, itemType, itemStatus tuples.
		/// </param>
		/// <returns>
		/// Reference to a list of items that have been added to the collection.
		/// </returns>
		/// <remarks>
		/// To use this method, call it similarly to:
		/// <code>project.Tasks.Add(
		/// (&quot;Maiden Project&quot;, 
		/// &quot;Creating a new project-level task.&quot;,
		/// &quot;Project&quot;, &quot;InProgress&quot;),
		/// (&quot;Kick-Off Task&quot;, 
		/// &quot;Creating a new task.&quot;,
		/// &quot;Task&quot;, &quot;TODO&quot;),
		/// </code>
		/// </remarks>
		public List<TaskItem> Add(
			params (string displayName, string description, TaskTypeItem itemType,
				TaskStatusItem itemStatus)[] entries)
		{
			List<TaskItem> result = new List<TaskItem>();
			TaskItem task = null;

			if(entries?.Length > 0)
			{
				foreach((string displayName, string description,
					TaskTypeItem itemType, TaskStatusItem itemStatus) item in entries)
				{
					if(item.displayName?.Length > 0)
					{
						task = new TaskItem()
						{
							DisplayName = item.displayName,
							Description = item.description
						};
						if(item.itemType != null)
						{
							task.ItemType = item.itemType;
						}
						if(item.itemStatus != null)
						{
							task.ItemStatus = item.itemStatus;
						}
						this.Add(task);
						result.Add(task);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AssociateTasksByName																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Associate all of the tasks specified in the child task list
		/// parameters with that one specified in the parent name.
		/// </summary>
		/// <param name="parentName">
		/// Name of the task serving in the parent role.
		/// </param>
		/// <param name="childNames">
		/// Names of one or more tasks serving in the child role.
		/// </param>
		/// <returns>
		/// Count of child tasks associated.
		/// </returns>
		public int AssociateTasksByName(string parentName,
			params string[] childNames)
		{
			int result = 0;
			TaskItem taskChild = null;
			TaskItem taskParent = null;

			if(parentName?.Length > 0 && childNames?.Length > 0 &&
				childNames.FirstOrDefault(x => x?.Length > 0) != null)
			{
				taskParent = this.FirstOrDefault(x =>
					CompactEqual(x.DisplayName, parentName));
				if(taskParent != null)
				{
					foreach(string childNameItem in childNames)
					{
						if(childNameItem?.Length > 0 &&
							!CompactEqual(childNameItem, parentName))
						{
							taskChild = this.FirstOrDefault(x =>
								CompactEqual(x.DisplayName, childNameItem));
							if(taskChild != null)
							{
								taskParent.Tasks.Add(taskChild);
							}
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ClearCalculatedFlag																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clear the Calculated flag on all tasks.
		/// </summary>
		public void ClearCalculatedFlag()
		{
			foreach(TaskItem taskItem in this)
			{
				taskItem.ExtendedProperties.SetValue("Calculated", "0");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetTotalEstimatedTime																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the total estimated time of all tasks in the provided
		/// collection.
		/// </summary>
		/// <param name="tasks">
		/// Reference to the collection of tasks to summarize.
		/// </param>
		/// <returns>
		/// Total estimated time in all tasks.
		/// </returns>
		public static float GetTotalEstimatedTime(List<TaskItem> tasks)
		{
			float result = 0f;

			if(tasks?.Count > 0)
			{
				result = tasks.Sum(x => x.EstimatedManHours);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetTotalOutstandingEstimatedTime																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the total outstanding estimated time of all tasks in the
		/// provided collection.
		/// </summary>
		/// <param name="tasks">
		/// Reference to the collection of tasks to summarize.
		/// </param>
		/// <returns>
		/// Total outstanding estimated time in all tasks.
		/// </returns>
		public static float GetTotalOutstandingEstimatedTime(List<TaskItem> tasks)
		{
			float result = 0f;

			if(tasks?.Count > 0)
			{
				result = tasks.Sum(x => x.CalculatedTimeEstimated);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	TaskItem																																*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual project or task.
	/// </summary>
	public class TaskItem : BaseItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* mDependencies_CollectionChanged																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the Dependencies collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Collection change event arguments.
		/// </param>
		private void mDependencies_CollectionChanged(object sender,
			CollectionChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Dependencies");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mDependencies_ItemPropertyChanged																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A value within an item of the Dependencies collection has been changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mDependencies_ItemPropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Dependencies");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mDescription_CollectionChanged																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the description multiline string have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Collection changed event arguments.
		/// </param>
		private void mDescription_CollectionChanged(object sender,
			CollectionChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Description");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mItemStatus_PropertyChanged																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property has changed on the ItemStatus object.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mItemStatus_PropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("ItemStatus");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mItemType_PropertyChanged																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property has changed on the ItemType object.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mItemType_PropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("ItemType");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mOwnerContact_PropertyChanged																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property has changed on the OwnerContact object.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mOwnerContact_PropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("OwnerContact");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mReviewerContact_PropertyChanged																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property has changed on the ReviewerContact object.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mReviewerContact_PropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("ReviewerContact");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mTasks_CollectionChanged																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the Tasks collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Collection changed event arguments.
		/// </param>
		private void mTasks_CollectionChanged(object sender,
			CollectionChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Tasks");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mTasks_ItemPropertyChanged																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property in an item of the tasks collection has changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mTasks_ItemPropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Tasks");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mTeamContacts_CollectionChanged																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the TeamContacts collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Collection changed event arguments.
		/// </param>
		private void mTeamContacts_CollectionChanged(object sender,
			CollectionChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("TeamContacts");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mTeamContacts_ItemPropertyChanged																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property on an item in the TeamContacts collection has changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mTeamContacts_ItemPropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("TeamContacts");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mTimers_CollectionChanged																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the Timers collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Collection changed event arguments.
		/// </param>
		private void mTimers_CollectionChanged(object sender,
			CollectionChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Timers");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mTimers_ItemPropertyChanged																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property of an item on the Timers collection has changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mTimers_ItemPropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Timers");
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
		/// Create a new instance of the TaskItem item.
		/// </summary>
		public TaskItem()
		{
			mDependencies = new DependencyCollection();
			mDependencies.CollectionChanged += mDependencies_CollectionChanged;
			mDependencies.ItemPropertyChanged += mDependencies_ItemPropertyChanged;
			mDescription = new MultilineString();
			mDescription.CollectionChanged += mDescription_CollectionChanged;
			mTasks = new TaskCollection();
			mTasks.CollectionChanged += mTasks_CollectionChanged;
			mTasks.ItemPropertyChanged += mTasks_ItemPropertyChanged;
			mTeamContacts = new ContactCollection();
			mTeamContacts.CollectionChanged += mTeamContacts_CollectionChanged;
			mTeamContacts.ItemPropertyChanged += mTeamContacts_ItemPropertyChanged;
			mTimers = new TimerCollection();
			mTimers.CollectionChanged += mTimers_CollectionChanged;
			mTimers.ItemPropertyChanged += mTimers_ItemPropertyChanged;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	BudgetAmount																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="BudgetAmount">BudgetAmount</see>.
		/// </summary>
		private float mBudgetAmount = 0f;
		/// <summary>
		/// Get/Set the allocated budget for this project.
		/// </summary>
		[JsonProperty(Order = 15)]
		public float BudgetAmount
		{
			get { return mBudgetAmount; }
			set
			{
				bool bChanged = (mBudgetAmount != value);
				mBudgetAmount = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	BudgetState																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="BudgetState">BudgetState</see>.
		/// </summary>
		private BudgetStatusEnum mBudgetState = BudgetStatusEnum.None;
		/// <summary>
		/// Get/Set the status of the budget request.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty(Order = 17)]
		public BudgetStatusEnum BudgetState
		{
			get { return mBudgetState; }
			set
			{
				bool bChanged = (mBudgetState != value);
				mBudgetState = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	BudgetType																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="BudgetType">BudgetType</see>.
		/// </summary>
		private BudgetTypeEnum mBudgetType = BudgetTypeEnum.None;
		/// <summary>
		/// Get/Set the type of budget being requested.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty(Order = 16)]
		public BudgetTypeEnum BudgetType
		{
			get { return mBudgetType; }
			set
			{
				bool bChanged = (mBudgetType != value);
				mBudgetType = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CalculatedEndDate																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="CalculatedEndDate">CalculatedEndDate</see>.
		/// </summary>
		private DateTime mCalculatedEndDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the date and time at which the task is calculated to be
		/// completed.
		/// </summary>
		[JsonProperty(Order = 12)]
		public DateTime CalculatedEndDate
		{
			get { return mCalculatedEndDate; }
			set
			{
				bool bChanged = (mCalculatedEndDate.CompareTo(value) != 0);
				mCalculatedEndDate = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CalculatedStartDate																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="CalculatedStartDate">CalculatedStartDate</see>.
		/// </summary>
		private DateTime mCalculatedStartDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the date and time at which the task is calculated to start.
		/// </summary>
		[JsonProperty(Order = 11)]
		public DateTime CalculatedStartDate
		{
			get { return mCalculatedStartDate; }
			set
			{
				bool bChanged = (mCalculatedStartDate.CompareTo(value) != 0);
				mCalculatedStartDate = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CalculatedTimeElapsed																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="CalculatedTimeElapsed">CalculatedTimeElapsed</see>.
		/// </summary>
		private float mCalculatedTimeElapsed = 0f;
		/// <summary>
		/// Get/Set the total calculated time elapsed.
		/// </summary>
		/// <remarks>
		/// This value is equal to the total amount of time already elapsed
		/// plus the amount of time allocated during the scheduling process.
		/// </remarks>
		[JsonProperty(Order = 13)]
		public float CalculatedTimeElapsed
		{
			get { return mCalculatedTimeElapsed; }
			set
			{
				bool bChanged = (mCalculatedTimeElapsed != value);
				mCalculatedTimeElapsed = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CalculatedTimeEstimated																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="CalculatedTimeEstimated">CalculatedTimeEstimated</see>.
		/// </summary>
		private float mCalculatedTimeEstimated = 0f;
		/// <summary>
		/// Get/Set the estimated time remaining for calculation.
		/// </summary>
		/// <remarks>
		/// As time is allocated during the scheduling process, that time is
		/// subtracted from this value.
		/// </remarks>
		[JsonProperty(Order = 14)]
		public float CalculatedTimeEstimated
		{
			get { return mCalculatedTimeEstimated; }
			set
			{
				bool bChanged = (mCalculatedTimeEstimated != value);
				mCalculatedTimeEstimated = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CompletionDate																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="CompletionDate">CompletionDate</see>.
		/// </summary>
		private DateTime mCompletionDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the date and time upon which this project was completed.
		/// </summary>
		[JsonProperty(Order = 10)]
		public DateTime CompletionDate
		{
			get { return mCompletionDate; }
			set
			{
				bool bChanged = (mCompletionDate.CompareTo(value) != 0);
				mCompletionDate = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CreateDate																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="CreateDate">CreateDate</see>.
		/// </summary>
		private DateTime mCreateDate = DateTime.Now;
		/// <summary>
		/// Get/Set the date and time upon whcih the project record was created.
		/// </summary>
		[JsonProperty(Order = 6)]
		public DateTime CreateDate
		{
			get { return mCreateDate; }
			set
			{
				bool bChanged = (mCreateDate.CompareTo(value) != 0);
				mCreateDate = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Dependencies																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Dependencies">Dependencies</see>.
		/// </summary>
		private DependencyCollection mDependencies = null;
		/// <summary>
		/// Get a reference to the collection of projects and tasks upon which this
		/// project is dependent.
		/// </summary>
		[JsonConverter(typeof(TicketCollectionConverter<DependencyItem>))]
		[JsonProperty(Order = 21)]
		public DependencyCollection Dependencies
		{
			get { return mDependencies; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Description																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Description">Description</see>.
		/// </summary>
		private MultilineString mDescription = null;
		/// <summary>
		/// Get/Set a reference to a brief description of the project, in
		/// multi-line format.
		/// </summary>
		[JsonProperty(Order = 3)]
		public MultilineString Description
		{
			get { return mDescription; }
			set
			{
				bool bChanged = (mDescription != value);
				if(bChanged && mDescription != null)
				{
					mDescription.CollectionChanged -= mDescription_CollectionChanged;
				}
				mDescription = value;
				if(bChanged)
				{
					if(mDescription != null)
					{
						mDescription.CollectionChanged += mDescription_CollectionChanged;
					}
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DisplayName																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="DisplayName">DisplayName</see>.
		/// </summary>
		private string mDisplayName = "";
		/// <summary>
		/// Get/Set the user-readable name of this item.
		/// </summary>
		[JsonProperty(Order = 2)]
		public string DisplayName
		{
			get { return mDisplayName; }
			set
			{
				bool bChanged = (mDisplayName != value);
				mDisplayName = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	EstimatedManHours																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="EstimatedManHours">EstimatedManHours</see>.
		/// </summary>
		private float mEstimatedManHours = 0f;
		/// <summary>
		/// Get/Set the man-hours estimated to complete this task.
		/// </summary>
		[JsonProperty(Order = 9)]
		public float EstimatedManHours
		{
			get { return mEstimatedManHours; }
			set
			{
				bool bChanged = (mEstimatedManHours != value);
				mEstimatedManHours = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetInheritedTeamContacts																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Retrieve a list of inherited team contacts from the ancestry of the
		/// provided task.
		/// </summary>
		/// <param name="task">
		/// Reference to the task for which ancestors will be queried for their
		/// team contacts.
		/// </param>
		/// <returns>
		/// Reference to a new collection of team contacts inherited from the
		/// ancestry chain of the provided task, if found. Otherwise, an
		/// empty collection.
		/// </returns>
		public static List<ContactItem> GetInheritedTeamContacts(TaskItem task)
		{
			List<ContactItem> result = new List<ContactItem>();

			if(task != null && task.mParentTask != null)
			{
				GetInheritedTeamContacts(task, result);
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Fill a list of inherited team contacts from the ancestry of the
		/// provided task.
		/// </summary>
		/// <param name="task">
		/// Reference to the task for which ancestors will be queried for their
		/// team contacts.
		/// </param>
		/// <param name="list">
		/// Reference to the collection of unique inherited team contacts found
		/// within the ancestry.
		/// </param>
		public static void GetInheritedTeamContacts(TaskItem task,
			List<ContactItem> list)
		{
			if(task != null && task.mParentTask != null && list != null)
			{
				if(task.mParentTask.mTeamContacts.Count > 0)
				{
					foreach(ContactItem contactItem in
						task.mParentTask.mTeamContacts)
					{
						if(!list.Exists(x => x.ItemId == contactItem.ItemId))
						{
							list.Add(contactItem);
						}
					}
				}
				GetInheritedTeamContacts(task.mParentTask, list);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ItemStatus																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ItemStatus">ItemStatus</see>.
		/// </summary>
		private TaskStatusItem mItemStatus = null;
		/// <summary>
		/// Get/Set a reference to the status of the item.
		/// </summary>
		[JsonConverter(typeof(TicketConverter<TaskStatusItem>))]
		[JsonProperty(Order = 5)]
		public TaskStatusItem ItemStatus
		{
			get { return mItemStatus; }
			set
			{
				bool bChanged = (mItemStatus != value);
				if(bChanged && mItemStatus != null)
				{
					mItemStatus.PropertyChanged -= mItemStatus_PropertyChanged;
				}
				mItemStatus = value;
				if(bChanged)
				{
					if(mItemStatus != null)
					{
						mItemStatus.PropertyChanged += mItemStatus_PropertyChanged;
					}
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ItemType																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ItemType">ItemType</see>.
		/// </summary>
		private TaskTypeItem mItemType = null;
		/// <summary>
		/// Get/Set a reference to the type of the item.
		/// </summary>
		[JsonConverter(typeof(TicketConverter<TaskTypeItem>))]
		[JsonProperty(Order = 4)]
		public TaskTypeItem ItemType
		{
			get { return mItemType; }
			set
			{
				bool bChanged = (mItemType != value);
				if(bChanged && mItemType != null)
				{
					mItemType.PropertyChanged -= mItemType_PropertyChanged;
				}
				mItemType = value;
				if(bChanged)
				{
					if(mItemType != null)
					{
						mItemType.PropertyChanged += mItemType_PropertyChanged;
					}
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OwnerContact																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="OwnerContact">OwnerContact</see>.
		/// </summary>
		private ContactItem mOwnerContact = null;
		/// <summary>
		/// Get/Set a reference to the contact representing the owner of this
		/// project.
		/// </summary>
		[JsonConverter(typeof(TicketConverter<ContactItem>))]
		[JsonProperty(Order = 18)]
		public ContactItem OwnerContact
		{
			get { return mOwnerContact; }
			set
			{
				bool bChanged = (mOwnerContact != value);
				if(bChanged && mOwnerContact != null)
				{
					mOwnerContact.PropertyChanged -= mOwnerContact_PropertyChanged;
				}
				mOwnerContact = value;
				if(bChanged)
				{
					if(mOwnerContact != null)
					{
						mOwnerContact.PropertyChanged += mOwnerContact_PropertyChanged;
					}
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ParentTask																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ParentTask">ParentTask</see>.
		/// </summary>
		private TaskItem mParentTask = null;
		/// <summary>
		/// Get/Set a reference to the project or task of which this item is a
		/// child.
		/// </summary>
		[JsonIgnore]
		public TaskItem ParentTask
		{
			get { return mParentTask; }
			set
			{
				//	To avoid circular references, the parent will not echo to child.
				bool bChanged = (mParentTask != value);
				mParentTask = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ReviewerContact																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ReviewerContact">ReviewerContact</see>.
		/// </summary>
		private ContactItem mReviewerContact = null;
		/// <summary>
		/// Get/Set a reference to the contact who will be reviewing this project
		/// or task.
		/// </summary>
		[JsonConverter(typeof(TicketConverter<ContactItem>))]
		[JsonProperty(Order = 19)]
		public ContactItem ReviewerContact
		{
			get { return mReviewerContact; }
			set
			{
				bool bChanged = (mReviewerContact != value);
				if(bChanged && mReviewerContact != null)
				{
					mReviewerContact.PropertyChanged -=
						mReviewerContact_PropertyChanged;
				}
				mReviewerContact = value;
				if(bChanged)
				{
					if(mReviewerContact != null)
					{
						mReviewerContact.PropertyChanged +=
							mReviewerContact_PropertyChanged;
					}
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeBudgetAmount																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the BudgetAmount property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeBudgetAmount()
		{
			return mBudgetAmount != 0f && (!float.IsNaN(mBudgetAmount));
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeBudgetState																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the BudgetState property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeBudgetState()
		{
			return mBudgetState != BudgetStatusEnum.None;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeBudgetType																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the BudgetType property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeBudgetType()
		{
			return mBudgetType != BudgetTypeEnum.None;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeCalculatedEndDate																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the CalculatedEndDate property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeCalculatedEndDate()
		{
			return DateTime.Compare(mCalculatedEndDate, DateTime.MinValue) != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeCalculatedStartDate																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the CalculatedStartDate property
		/// should be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeCalculatedStartDate()
		{
			return DateTime.Compare(mCalculatedStartDate, DateTime.MinValue) != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeCalculatedTimeElapsed																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the CalculatedTimeElapsed property
		/// should be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeCalculatedTimeElapsed()
		{
			return mCalculatedTimeElapsed != 0f;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeCalculatedTimeEstimated																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the CalculatedTimeEstimated property
		/// should be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeCalculatedTimeEstimated()
		{
			return mCalculatedTimeEstimated != 0f;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeCompletionDate																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the CompletionDate property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeCompletionDate()
		{
			return DateTime.Compare(mCompletionDate, DateTime.MinValue) != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeCreateDate																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the CreateDate property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeCreateDate()
		{
			return DateTime.Compare(mCreateDate, DateTime.MinValue) != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeDependencies																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Dependencies property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeDependencies()
		{
			return mDependencies?.Count > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeDescription																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Description property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeDescription()
		{
			return mDescription?.Count > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeDisplayName																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the DisplayName property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeDisplayName()
		{
			return mDisplayName?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeEstimatedManHours																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the EstimatedManHours property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeEstimatedManHours()
		{
			return true;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeItemStatus																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ItemStatus property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeItemStatus()
		{
			return mItemStatus != null;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeItemType																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ItemType property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeItemType()
		{
			return mItemType != null;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeOwnerContact																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the OwnerContact property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeOwnerContact()
		{
			return mOwnerContact != null;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeReviewerContact																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ReviewerContact property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeReviewerContact()
		{
			return mReviewerContact != null;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeStartDate																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the StartDate property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeStartDate()
		{
			return DateTime.Compare(mStartDate, DateTime.MinValue) != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeTargetDate																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the TargetDate property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeTargetDate()
		{
			return DateTime.Compare(mTargetDate, DateTime.MinValue) != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeTasks																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Tasks property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeTasks()
		{
			return mTasks?.Count > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeTeamContacts																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the TeamContacts property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeTeamContacts()
		{
			return mTeamContacts?.Count > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeTimers																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Timers property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeTimers()
		{
			return mTimers?.Count > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StartDate																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="StartDate">StartDate</see>.
		/// </summary>
		private DateTime mStartDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the date and time upon which the project is scheduled to
		/// start.
		/// </summary>
		[JsonProperty(Order = 7)]
		public DateTime StartDate
		{
			get { return mStartDate; }
			set
			{
				bool bChanged = (mStartDate.CompareTo(value) != 0);
				mStartDate = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TargetDate																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TargetDate">TargetDate</see>.
		/// </summary>
		private DateTime mTargetDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the date and time before which the project is due.
		/// </summary>
		[JsonProperty(Order = 8)]
		public DateTime TargetDate
		{
			get { return mTargetDate; }
			set
			{
				bool bChanged = (mTargetDate.CompareTo(value) != 0);
				mTargetDate = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
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
		/// Get a reference to the reference to the collection of tasks composing
		/// this project or task.
		/// </summary>
		[JsonConverter(typeof(TicketCollectionConverter<TaskItem>))]
		[JsonProperty(Order = 22)]
		public TaskCollection Tasks
		{
			get { return mTasks; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TeamContacts																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TeamContacts">TeamContacts</see>.
		/// </summary>
		private ContactCollection mTeamContacts = null;
		/// <summary>
		/// Get a reference to the collection of contacts representing members of
		/// the team.
		/// </summary>
		[JsonConverter(typeof(TicketCollectionConverter<ContactItem>))]
		[JsonProperty(Order = 20)]
		public ContactCollection TeamContacts
		{
			get { return mTeamContacts; }
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
		/// Get a reference to the collection of running and elapsed timers on
		/// this item.
		/// </summary>
		[JsonProperty(Order = 23)]
		public TimerCollection Timers
		{
			get { return mTimers; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*


}
