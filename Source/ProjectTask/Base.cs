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
using System.Runtime.CompilerServices;
using System.Text;

using Newtonsoft.Json;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	BaseCollection																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of BaseItem Items.
	/// </summary>
	public class BaseCollection : ChangeObjectCollection<BaseItem>
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


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	BaseItem																																*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Base information found in every ProjectTask object.
	/// </summary>
	public class BaseItem : ChangeObjectItem, IItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* mComments_CollectionChanged																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The content of the Comments collection has changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Collection change event arguments.
		/// </param>
		private void mComments_CollectionChanged(object sender,
			CollectionChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Comments");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mComments_ItemPropertyChanged																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property on an item in the Comments collection has changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mComments_ItemPropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Comments");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mExtendedProperties_CollectionChanged																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the ExtendedProperties collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Collection change event arguments.
		/// </param>
		private void mExtendedProperties_CollectionChanged(object sender,
			CollectionChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("ExtendedProperties");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mExtendedProperties_ItemPropertyChanged																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The value of an item in the extended properties collection has changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mExtendedProperties_ItemPropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("ExtendedProperties");
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* OnCollectionChanged																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the CollectionChanged event when the contents of a member
		/// collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Collection change event arguments.
		/// </param>
		protected override void OnCollectionChanged(object sender,
			CollectionChangeEventArgs e)
		{
			base.OnCollectionChanged(sender, e);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OnPropertyChanged																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the PropertyChanged event when the value of a member property
		/// has changed.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property whose value has changed.
		/// </param>
		protected override void OnPropertyChanged(
			[CallerMemberName] string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the BaseItem item.
		/// </summary>
		public BaseItem()
		{
			Trace.WriteLine("BaseItem constructor...");
			mItemId = mNextItemId++;
			mComments = new CommentCollection();
			mComments.CollectionChanged += mComments_CollectionChanged;
			mComments.ItemPropertyChanged += mComments_ItemPropertyChanged;
			mExtendedProperties = new PropertyCollection();
			mExtendedProperties.CollectionChanged +=
				mExtendedProperties_CollectionChanged;
			mExtendedProperties.ItemPropertyChanged +=
				mExtendedProperties_ItemPropertyChanged;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Comments																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Comments">Comments</see>.
		/// </summary>
		private CommentCollection mComments = new CommentCollection();
		/// <summary>
		/// Get a reference to the comments that have been created on this item.
		/// </summary>
		[JsonConverter(typeof(TicketCollectionConverter<CommentItem>))]
		[JsonProperty(Order = 101)]
		public CommentCollection Comments
		{
			get { return mComments; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ExtendedProperties																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="ExtendedProperties">ExtendedProperties</see>.
		/// </summary>
		private PropertyCollection mExtendedProperties = null;
		/// <summary>
		/// Get a reference to a collection of properties defined for this contact.
		/// </summary>
		[JsonProperty(Order = 100)]
		public PropertyCollection ExtendedProperties
		{
			get { return mExtendedProperties; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ItemId																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ItemId">ItemId</see>.
		/// </summary>
		private int mItemId = 0;
		/// <summary>
		/// Get/Set the locally unique identification of this record.
		/// </summary>
		[JsonIgnore]
		public int ItemId
		{
			get { return mItemId; }
			set
			{
				bool bChanged = (mItemId != value);
				mItemId = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ItemTicket																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="ItemTicket">ItemTicket</see>.
		/// </summary>
		private string mItemTicket = Guid.NewGuid().ToString("D");
		/// <summary>
		/// Get/Set the globally unique identification of this record.
		/// </summary>
		[JsonProperty(Order = 1)]
		public string ItemTicket
		{
			get { return mItemTicket; }
			set
			{
				bool bChanged = (mItemTicket != value);
				mItemTicket = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	NextItemId																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="NextItemId">NextItemId</see>.
		/// </summary>
		private static int mNextItemId = 1;
		/// <summary>
		/// Get/Set the next local item ID for this record.
		/// </summary>
		public static int NextItemId
		{
			get { return mNextItemId; }
			set { mNextItemId = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeExtendedProperties																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ExtendedProperties property
		/// should be serialized.
		/// </summary>
		/// <returns>
		/// True if the property will be serialized. Otherwise, false.
		/// </returns>
		public bool ShouldSerializeExtendedProperties()
		{
			return (mExtendedProperties?.Count > 0);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeItemId																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ItemId property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeItemId()
		{
			return false;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeItemTicket																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ItemTicket property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeItemTicket()
		{
			return true;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
