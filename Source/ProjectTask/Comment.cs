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
using System.Text;

using Newtonsoft.Json;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	CommentCollection																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of CommentItem Items.
	/// </summary>
	public class CommentCollection : ChangeObjectCollection<CommentItem>
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
	//*	CommentItem																															*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a single comment post.
	/// </summary>
	public class CommentItem : ChangeObjectItem, IItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* mBody_CollectionChanged																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the Body property have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Collection change event arguments.
		/// </param>
		private void mBody_CollectionChanged(object sender,
			CollectionChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Body");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mExtendedProperties_CollectionChanged																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the extended properties collection have changed.
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
		/// Create a new instance of the CommentItem item.
		/// </summary>
		public CommentItem()
		{
			mBody = new MultilineString();
			mBody.CollectionChanged += mBody_CollectionChanged;
			mExtendedProperties = new PropertyCollection();
			mExtendedProperties.CollectionChanged +=
				mExtendedProperties_CollectionChanged;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Body																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Body">Body</see>.
		/// </summary>
		private MultilineString mBody = null;
		/// <summary>
		/// Get a reference to the multiline string content representing the body
		/// of the comment.
		/// </summary>
		[JsonProperty(Order = 4)]
		public MultilineString Body
		{
			get { return mBody; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Clone																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a deep copy of the provided comment.
		/// </summary>
		/// <param name="comment">
		/// Reference to the comment to clone.
		/// </param>
		/// <returns>
		/// Reference to the new clone of the caller's comment.
		/// </returns>
		public static CommentItem Clone(CommentItem comment)
		{
			CommentItem result = null;

			if(comment != null)
			{
				comment = new CommentItem()
				{
					mBody = MultilineString.Clone(comment.mBody),
					mItemId = comment.mItemId,
					mItemTicket = comment.mItemTicket,
					mPostDate = comment.mPostDate,
					mUsername = comment.mUsername
				};
				foreach(PropertyItem propertyItem in comment.mExtendedProperties)
				{
					result.mExtendedProperties.Add(PropertyItem.Clone(propertyItem));
				}
			}
			return result;
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
		/// Get a reference to the collection of extended properties available for
		/// this comment.
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
		//*	PostDate																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="PostDate">PostDate</see>.
		/// </summary>
		private DateTime mPostDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the date and time upon which the comment was posted.
		/// </summary>
		[JsonProperty(Order = 2)]
		public DateTime PostDate
		{
			get { return mPostDate; }
			set
			{
				bool bChanged = (mPostDate.CompareTo(value) != 0);
				mPostDate = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeBody																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Body property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeBody()
		{
			return mBody.Count > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeDatePosted																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the PostDate property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializePostDate()
		{
			return DateTime.Compare(mPostDate, DateTime.MinValue) != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeExtendedProperties																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ExtendedProperties property
		/// should be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeExtendedProperties()
		{
			return mExtendedProperties?.Count > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeItemTicket																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ItemTicket property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeItemTicket()
		{
			return true;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeUsername																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Username property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeUsername()
		{
			return mUsername?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Username																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Username">Username</see>.
		/// </summary>
		private string mUsername = "";
		/// <summary>
		/// Get/Set the name or initials of the user making the post.
		/// </summary>
		[JsonProperty(Order = 3)]
		public string Username
		{
			get { return mUsername; }
			set
			{
				bool bChanged = (mUsername != value);
				mUsername = value;
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
