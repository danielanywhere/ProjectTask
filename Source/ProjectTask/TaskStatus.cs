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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	TaskStatusCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of TaskStatusItem Items.
	/// </summary>
	public class TaskStatusCollection : ChangeObjectCollection<TaskStatusItem>
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
		/// Return an item from the collection by its display name.
		/// </summary>
		public TaskStatusItem this[string displayName]
		{
			get
			{
				TaskStatusItem result = null;

				if(displayName?.Length > 0)
				{
					result = this.FirstOrDefault(x => x.DisplayName.ToLower() ==
						displayName.ToLower());
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add one or more task states using a simple tuple approach.
		/// </summary>
		/// <param name="states">
		/// One or more displayName, taskStatus tuples.
		/// </param>
		/// <returns>
		/// Reference to a list of items that have been added to the collection.
		/// </returns>
		/// <remarks>
		/// To use this method, call it similarly to:
		/// <code>project.TaskStates.Add(
		/// (&quot;TODO&quot;, ProjectTaskStateEnum.Queued),
		/// (&quot;InProgress&quot;, ProjectTaskStateEnum.Active));
		/// </code>
		/// </remarks>
		public List<TaskStatusItem> Add(
			params (string displayName, ProjectTaskStateEnum taskStatus)[] states)
		{
			List<TaskStatusItem> result = new List<TaskStatusItem>();
			TaskStatusItem taskStatus = null;

			if(states?.Length > 0)
			{
				foreach((string displayName, ProjectTaskStateEnum taskStatus) item in
					states)
				{
					if(item.displayName?.Length > 0)
					{
						taskStatus = new TaskStatusItem()
						{
							DisplayName = item.displayName,
							TaskState = item.taskStatus
						};
						this.Add(taskStatus);
						result.Add(taskStatus);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	TaskStatusItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a status state that can be assigned to a project or
	/// task.
	/// </summary>
	public class TaskStatusItem : BaseItem
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
		//*	DisplayName																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="DisplayName">DisplayName</see>.
		/// </summary>
		private string mDisplayName = "";
		/// <summary>
		/// Get/Set the user-readable name of the state.
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
		//*	ShouldSerializeTaskState																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the TaskState property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeTaskState()
		{
			return mTaskState != ProjectTaskStateEnum.None;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TaskState																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TaskState">TaskState</see>.
		/// </summary>
		private ProjectTaskStateEnum mTaskState = ProjectTaskStateEnum.None;
		/// <summary>
		/// Get/Set a value indicating the base status of the item.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty(Order = 3)]
		public ProjectTaskStateEnum TaskState
		{
			get { return mTaskState; }
			set
			{
				bool bChanged = (mTaskState != value);
				mTaskState = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*



	}
	//*-------------------------------------------------------------------------*

}
