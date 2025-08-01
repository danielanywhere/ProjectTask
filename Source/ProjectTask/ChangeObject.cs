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
using System.Runtime.CompilerServices;
using System.Text;

using Newtonsoft.Json;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	ChangeObjectCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ChangeObjectItem Items, which itself raises events when
	/// the contents of this collection or when the property of one of its items
	/// have changed.
	/// </summary>
	/// <typeparam name="T">
	/// Any type for which change handling will be configured.
	/// </typeparam>
	public class ChangeObjectCollection<T> : List<T>, IParentCollection
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* OnAdd																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the Add event when an item has been added to the collection.
		/// </summary>
		/// <param name="item">
		/// Reference to the item that has been added.
		/// </param>
		protected virtual void OnAdd(T item)
		{
			ItemAdded?.Invoke(this, new ItemEventArgs<T>(item));
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OnCollectionChanged																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the CollectionChanged event when the contents of the collection
		/// have changed.
		/// </summary>
		/// <param name="actionName">
		/// Name of the action: Add, Remove, Collection.
		/// </param>
		protected virtual void OnCollectionChanged(string actionName)
		{
			CollectionChanged?.Invoke(this, new CollectionChangeEventArgs()
			{
				ActionName = actionName,
				PropertyName = mPropertyName
			});
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OnItemPropertyChanged																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the ItemPropertyChanged event when the value of an item property
		/// has changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Dock panel property change event arguments.
		/// </param>
		protected virtual void OnItemPropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(e != null && !e.Handled)
			{
				ItemPropertyChanged?.Invoke(sender, e);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OnRemove																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the Remove event when an item has been removed from the
		/// collection.
		/// </summary>
		/// <param name="item">
		/// Reference to the item that has been removed.
		/// </param>
		protected virtual void OnRemove(T item)
		{
			ItemRemoved?.Invoke(this, new ItemEventArgs<T>(item));
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add an item to the collection.
		/// </summary>
		/// <param name="item">
		/// Reference to the item to be added.
		/// </param>
		public new void Add(T item)
		{
			if(item != null)
			{
				if(item is IItem @itemInterface && itemInterface.ItemId == 0)
				{
					if(mProjectFile != null)
					{
						itemInterface.ItemId = mNextItemId++;
					}
					if(itemInterface.Parent == null || mProjectFile != null)
					{
						itemInterface.Parent = this;
					}
				}
				if(item is ChangeObjectItem @objectItem)
				{
					objectItem.PropertyChanged += OnItemPropertyChanged;
				}
				base.Add(item);
				OnAdd(item);
				OnCollectionChanged("Add");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AddRange																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a range of items to the collection.
		/// </summary>
		/// <param name="collection">
		/// Reference to the collection of items to be added.
		/// </param>
		public new void AddRange(IEnumerable<T> collection)
		{
			if(collection != null)
			{
				foreach(T tItem in collection)
				{
					if(tItem is IItem @itemInterface)
					{
						itemInterface.ItemId = mNextItemId++;
						if(itemInterface.Parent == null)
						{
							itemInterface.Parent = this;
						}
					}
					if(tItem is ChangeObjectItem @objectItem)
					{
						objectItem.PropertyChanged += OnItemPropertyChanged;
					}
					base.Add(tItem);
					OnAdd(tItem);
					OnCollectionChanged("Add");
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Clear																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove all of the elements from the property collection.
		/// </summary>
		public new void Clear()
		{
			base.Clear();
			OnCollectionChanged("Collection");
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CollectionChanged																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raised when the contents of the collection have changed.
		/// </summary>
		public event EventHandler<CollectionChangeEventArgs> CollectionChanged;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Insert																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Insert an item into the collection at the specified ordinal index.
		/// </summary>
		/// <param name="index">
		/// The 0-based index at which to insert the new item.
		/// </param>
		/// <param name="item">
		/// Reference to the item to be inserted.
		/// </param>
		public new void Insert(int index, T item)
		{
			if(index > -1 && item != null)
			{
				if(item is IItem @itemInterface)
				{
					itemInterface.ItemId = mNextItemId++;
				}
				if(item is ChangeObjectItem @objectItem)
				{
					objectItem.PropertyChanged += OnItemPropertyChanged;
				}
				base.Insert(index, item);
				OnAdd(item);
				OnCollectionChanged("Add");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* InsertRange																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Insert a range of items at the specified ordinal index.
		/// </summary>
		/// <param name="index">
		/// The 0-based index at which to insert the new item.
		/// </param>
		/// <param name="collection">
		/// Reference to the collection of items to insert.
		/// </param>
		public new void InsertRange(int index, IEnumerable<T> collection)
		{
			int activeIndex = index;

			if(index > -1 && collection != null)
			{
				foreach(T tItem in collection)
				{
					if(tItem is IItem @itemInterface)
					{
						itemInterface.ItemId = mNextItemId++;
					}
					if(tItem is ChangeObjectItem @objectItem)
					{
						objectItem.PropertyChanged += OnItemPropertyChanged;
					}
					base.Insert(activeIndex++, tItem);
					OnAdd(tItem);
					OnCollectionChanged("Add");
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ItemAdded																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raised when an item is added to the collection.
		/// </summary>
		public event EventHandler<ItemEventArgs<T>> ItemAdded;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ItemPropertyChanged																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raised when the value of a property on an individual item has changed.
		/// </summary>
		public event EventHandler<PropertyChangeEventArgs> ItemPropertyChanged;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ItemRemoved																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raised when an item is removed from the collection.
		/// </summary>
		public event EventHandler<ItemEventArgs<T>> ItemRemoved;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	NextItemId																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="NextItemId">NextItemId</see>.
		/// </summary>
		private int mNextItemId = 1;
		///// <summary>
		///// Get/Set the next local item ID.
		///// </summary>
		//[JsonIgnore]
		//public int NextItemId
		//{
		//	get { return mNextItemId; }
		//	set { mNextItemId = value; }
		//}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parent																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Parent">Parent</see>.
		/// </summary>
		private IParentCollection mParent = null;
		/// <summary>
		/// Get/Set a reference to the parent collection.
		/// </summary>
		public IParentCollection Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProjectFile																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ProjectFile">ProjectFile</see>.
		/// </summary>
		private ProjectTask.ProjectFile mProjectFile = null;
		/// <summary>
		/// Get/Set a reference to the project object model to which it belongs.
		/// </summary>
		[JsonIgnore]
		public ProjectTask.ProjectFile ProjectFile
		{
			get { return mProjectFile; }
			set { mProjectFile = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	PropertyName																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member value for <see cref="PropertyName">PropertyName</see>.
		/// </summary>
		private string mPropertyName = "";
		/// <summary>
		/// Get/Set a property name to be associated with this collection for
		/// bubble-up events.
		/// </summary>
		public string PropertyName
		{
			get { return mPropertyName; }
			set { mPropertyName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Remove																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove the first matching instance of the specified item from the
		/// collection.
		/// </summary>
		/// <param name="item">
		/// Reference to the item to be removed.
		/// </param>
		/// <returns>
		/// Value indicating whether the specified item was removed from the
		/// collection.
		/// </returns>
		public new bool Remove(T item)
		{
			bool result = base.Remove(item);

			if(item != null && item is ChangeObjectItem @objectItem)
			{
				objectItem.PropertyChanged -= OnItemPropertyChanged;
			}
			if(result)
			{
				OnRemove(item);
				OnCollectionChanged("Remove");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RemoveAll																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove all items matching the condition from the collection.
		/// </summary>
		/// <param name="match">
		/// Reference to the predicate condition to match.
		/// </param>
		/// <returns>
		/// Count of items removed.
		/// </returns>
		public new int RemoveAll(Predicate<T> match)
		{
			int count = 0;
			int index = 0;
			T item = default(T);
			int result = 0;

			if(match != null)
			{
				count = this.Count;
				for(index = 0; index < count; index++)
				{
					item = this[index];
					if(match(item))
					{
						//	Unregister the item.
						if(item is ChangeObjectItem @objectItem)
						{
							objectItem.PropertyChanged -= OnItemPropertyChanged;
						}
						//	Remove the item.
						base.RemoveAt(index);
						//	Discount.
						count--;
						//	Deindex.
						index--;
						result++;
						OnRemove(item);
						OnCollectionChanged("Remove");
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RemoveAt																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove the item at the specified ordinal index of the collection.
		/// </summary>
		/// <param name="index">
		/// The 0-based index at which the item will be removed.
		/// </param>
		public new void RemoveAt(int index)
		{
			T item = default(T);

			if(index > -1 && index < this.Count)
			{
				item = this[index];
				//	Unregister the item.
				if(item is ChangeObjectItem @objectItem)
				{
					objectItem.PropertyChanged -= OnItemPropertyChanged;
				}
				base.RemoveAt(index);
				OnRemove(item);
				OnCollectionChanged("Remove");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RemoveRange																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove a range of items from the collection.
		/// </summary>
		/// <param name="index">
		/// The index at which to begin removing.
		/// </param>
		/// <param name="count">
		/// The count of items to remove.
		/// </param>
		public new void RemoveRange(int index, int count)
		{
			T item = default(T);
			int remaining = 0;

			if(index > -1 && index < this.Count && count > 0)
			{
				remaining = Math.Min(count, this.Count - index);
				while(remaining > 0)
				{
					item = this[index];
					//	Unregister the item.
					if(item is ChangeObjectItem @objectItem)
					{
						objectItem.PropertyChanged -= OnItemPropertyChanged;
					}
					base.RemoveAt(index);
					OnRemove(item);
					OnCollectionChanged("Remove");
					remaining--;
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ChangeObjectItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// The object whose changes will raise an event.
	/// </summary>
	public class ChangeObjectItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* OnCollectionChanged																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the PropertyChanged event when the contents of a member
		/// collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Collection change event arguments.
		/// </param>
		protected virtual void OnCollectionChanged(object sender,
			CollectionChangeEventArgs e)
		{
			OnPropertyChanged(e.PropertyName);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OnPropertyChanged																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the PropertyChanged event when the value of a property has
		/// changed.
		/// </summary>
		/// <param name="propertyName">
		/// The name of the property whose value has changed.
		/// </param>
		protected virtual void OnPropertyChanged(
			[CallerMemberName] string propertyName = null)
		{
			if(PropertyChanged != null)
			{
				PropertyChanged.Invoke(this, new PropertyChangeEventArgs()
				{
					PropertyName = propertyName
				});
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* PropertyChanged																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raised when the value of a property has changed.
		/// </summary>
		public event EventHandler<PropertyChangeEventArgs> PropertyChanged;
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
