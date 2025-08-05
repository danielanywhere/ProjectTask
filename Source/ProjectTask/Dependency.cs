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

using static ProjectTask.ProjectTaskUtil;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	DependencyCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of DependencyItem Items.
	/// </summary>
	public class DependencyCollection : ChangeObjectCollection<DependencyItem>
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
		//*	Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add an item to the collection by member values.
		/// </summary>
		/// <param name="dependencyTaskDisplayName">
		/// Display name of the task upon which this item will be dependent.
		/// </param>
		/// <param name="dependencyType">
		/// The type of dependency to set on the specified task.
		/// </param>
		/// <returns>
		/// Reference to the newly created and added dependency, if legitimate.
		/// Otherwise, null.
		/// </returns>
		public DependencyItem Add(string dependencyTaskDisplayName,
			DependencyTypeEnum dependencyType)
		{
			DependencyItem result = null;
			TaskItem task = null;

			if(dependencyTaskDisplayName?.Length > 0)
			{
				task = ActiveProjectContext.Tasks.FirstOrDefault(x =>
					CompactEqual(x.DisplayName, dependencyTaskDisplayName));
				if(task == null)
				{
					task = new TaskItem()
					{
						DisplayName = dependencyTaskDisplayName
					};
				}
				result = this.FirstOrDefault(x =>
					x.RemoteDependency.ItemId == task.ItemId);
				if(result == null)
				{
					//	The item is not registered in this collection.
					result = new DependencyItem()
					{
						DependencyType = dependencyType,
						RemoteDependency = task
					};
					this.Add(result);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*



	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	DependencyItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information relating a single dependency to the project or task dependent
	/// upon it.
	/// </summary>
	public class DependencyItem : BaseItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* mRemoteDependency_PropertyChanged																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property has changed on the RemoteDependency object.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mRemoteDependency_PropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("RemoteDependency");
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
		/// Create a new instance of the DependencyItem item.
		/// </summary>
		public DependencyItem()
		{
			ItemId = NextItemId++;
			ActiveProjectContext.Dependencies.Add(this);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DependencyDate																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="DependencyDate">DependencyDate</see>.
		/// </summary>
		private DateTime mDependencyDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the date and time related to the ancestor project or task.
		/// </summary>
		[JsonProperty(Order = 3)]
		public DateTime DependencyDate
		{
			get { return mDependencyDate; }
			set
			{
				bool bChanged = (mDependencyDate.CompareTo(value) != 0);
				mDependencyDate = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DependencyType																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="DependencyType">DependencyType</see>.
		/// </summary>
		private DependencyTypeEnum mDependencyType = DependencyTypeEnum.None;
		/// <summary>
		/// Get/Set the type of dependency identified with this association.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty(Order = 4)]
		public DependencyTypeEnum DependencyType
		{
			get { return mDependencyType; }
			set
			{
				bool bChanged = (mDependencyType != value);
				mDependencyType = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RemoteDependency																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="RemoteDependency">RemoteDependency</see>.
		/// </summary>
		private TaskItem mRemoteDependency = null;
		/// <summary>
		/// Get/Set a reference to the project or task for which the dependency is
		/// defined.
		/// </summary>
		[JsonConverter(typeof(TicketConverter<TaskItem>))]
		[JsonProperty(Order = 2)]
		public TaskItem RemoteDependency
		{
			get { return mRemoteDependency; }
			set
			{
				bool bChanged = (mRemoteDependency != value);
				if(bChanged && mRemoteDependency != null)
				{
					mRemoteDependency.PropertyChanged -=
						mRemoteDependency_PropertyChanged;
				}
				mRemoteDependency = value;
				if(bChanged)
				{
					if(mRemoteDependency != null)
					{
						mRemoteDependency.PropertyChanged +=
							mRemoteDependency_PropertyChanged;
					}
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeDependencyDate																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the DependencyDate property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeDependencyDate()
		{
			return true;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeDependencyType																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the DependencyType property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeDependencyType()
		{
			return true;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeRemoteDependency																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the RemoteDependency property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeRemoteDependency()
		{
			return mRemoteDependency != null;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
