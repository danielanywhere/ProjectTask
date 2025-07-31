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
	//*	TaskTypeCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of TaskTypeItem Items.
	/// </summary>
	public class TaskTypeCollection : ChangeObjectCollection<TaskTypeItem>
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
		public TaskTypeItem this[string displayName]
		{
			get
			{
				TaskTypeItem result = null;

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
		/// Add one or more task types using a simple tuple approach.
		/// </summary>
		/// <param name="types">
		/// One or more displayName, taskType tuples.
		/// </param>
		/// <returns>
		/// Reference to a list of items that have been added to the collection.
		/// </returns>
		/// <remarks>
		/// To use this method, call it similarly to:
		/// <code>project.TaskTypes.Add(
		/// (&quot;Project&quot;, ProjectTaskTypeEnum.Project),
		/// (&quot;Task&quot;, ProjectTaskTypeEnum.Task));
		/// </code>
		/// </remarks>
		public List<TaskTypeItem> Add(
			params (string displayName, ProjectTaskTypeEnum taskType)[] types)
		{
			List<TaskTypeItem> result = new List<TaskTypeItem>();
			TaskTypeItem taskType = null;

			if(types?.Length > 0)
			{
				foreach((string displayName, ProjectTaskTypeEnum taskType) item in
					types)
				{
					if(item.displayName?.Length > 0)
					{
						taskType = new TaskTypeItem()
						{
							DisplayName = item.displayName,
							TaskType = item.taskType
						};
						this.Add(taskType);
						result.Add(taskType);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*



	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	TaskTypeItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual type definition for identifying a kind of project or task.
	/// </summary>
	public class TaskTypeItem : BaseItem
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
		/// Get/Set the user-readable name of the type.
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
		//*	ShouldSerializeTaskType																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the TaskType property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeTaskType()
		{
			return mTaskType != ProjectTaskTypeEnum.None;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TaskType																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TaskType">TaskType</see>.
		/// </summary>
		private ProjectTaskTypeEnum mTaskType = ProjectTaskTypeEnum.None;
		/// <summary>
		/// Get/Set a value indicating the base type of the item.
		/// </summary>
		[JsonProperty(Order = 3)]
		[JsonConverter(typeof(StringEnumConverter))]
		public ProjectTaskTypeEnum TaskType
		{
			get { return mTaskType; }
			set
			{
				bool bChanged = (mTaskType != value);
				mTaskType = value;
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
