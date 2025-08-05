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

using static ProjectTask.ProjectTaskUtil;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	TimeBlockCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of TimeBlockItem Items.
	/// </summary>
	public class TimeBlockCollection : ChangeObjectCollection<TimeBlockItem>
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
		//* GetBlockWithMaxTime																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the time block with the maximum amount of available time.
		/// </summary>
		/// <param name="blocks">
		/// Reference to the collection of blocks to test.
		/// </param>
		/// <returns>
		/// Reference to the time block with the maximum amount of available time
		/// in the group, if found. Otherwise, null.
		/// </returns>
		public static TimeBlockItem GetBlockWithMaxTime(List<TimeBlockItem> blocks)
		{
			int count = 0;
			DateTime currentTime = DateTime.Now;
			float hours = 0f;
			float hoursMax = -1f;
			int index = 0;
			int indexMax = 0;
			TimeBlockItem result = null;

			if(blocks?.Count > 0)
			{
				if(blocks.Count == 1)
				{
					result = blocks[0];
				}
				else
				{
					count = blocks.Count;
					index = 0;
					indexMax = -1;
					foreach(TimeBlockItem blockItem in blocks)
					{
						hours =
							TimeNotationCollection.TotalTime(blocks[index].Entries,
							currentTime);
						if(hours > hoursMax)
						{
							hoursMax = hours;
							indexMax = index;
						}
						index++;
					}
					if(indexMax > -1)
					{
						result = blocks[indexMax];
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	TimeBlockItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a reusable block of time entries.
	/// </summary>
	public class TimeBlockItem : BaseItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* mEntries_CollectionChanged																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the Entries collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Collection change event arguments.
		/// </param>
		private void mEntries_CollectionChanged(object sender,
			CollectionChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Entries");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mEntries_ItemAdded																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// An item has been added to the Entries property.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Item event arguments.
		/// </param>
		private void mEntries_ItemAdded(object sender,
			ItemEventArgs<TimeNotationItem> e)
		{
			if(e.Data != null)
			{
				if(!ActiveProjectContext.TimeNotations.Contains(e.Data))
				{
					ActiveProjectContext.TimeNotations.Add(e.Data);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mEntries_ItemPropertyChanged																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property on an item in the Entries collection has changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mEntries_ItemPropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Entries");
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
		/// Create a new instance of the TimeBlockItem item.
		/// </summary>
		public TimeBlockItem()
		{
			ItemId = NextItemId++;
			mEntries = new TimeNotationCollection();
			mEntries.CollectionChanged += mEntries_CollectionChanged;
			mEntries.ItemAdded += mEntries_ItemAdded;
			mEntries.ItemPropertyChanged += mEntries_ItemPropertyChanged;
			ActiveProjectContext.TimeBlocks.Add(this);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Clone																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create and return a deep copy of the provided time block item.
		/// </summary>
		/// <param name="block">
		/// Reference to the block to be copied.
		/// </param>
		/// <returns>
		/// Reference to the new clone of the provided time block item.
		/// </returns>
		public static TimeBlockItem Clone(TimeBlockItem block)
		{
			TimeBlockItem result = null;

			if(block != null)
			{
				result = new TimeBlockItem()
				{
					ItemTicket = block.ItemTicket,
					mDisplayName = block.mDisplayName
				};
				foreach(CommentItem commentItem in block.Comments)
				{
					result.Comments.Add(CommentItem.Clone(commentItem));
				}
				foreach(TimeNotationItem notationItem in block.mEntries)
				{
					result.Entries.Add(TimeNotationItem.Clone(notationItem));
				}
				foreach(PropertyItem propertyItem in block.ExtendedProperties)
				{
					result.ExtendedProperties.Add(PropertyItem.Clone(propertyItem));
				}
			}
			return result;
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
		/// Get/Set the user-readable name of this schedule configuration.
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
		//*	Entries																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Entries">Entries</see>.
		/// </summary>
		private TimeNotationCollection mEntries = null;
		/// <summary>
		/// Get a reference to the collection of individual day and time slots.
		/// </summary>
		[JsonConverter(typeof(TicketCollectionConverter<TimeNotationItem>))]
		[JsonProperty(Order = 3)]
		public TimeNotationCollection Entries
		{
			get { return mEntries; }
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
		//*	ShouldSerializeEntries																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Entries property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeEntries()
		{
			return mEntries?.Count > 0;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
