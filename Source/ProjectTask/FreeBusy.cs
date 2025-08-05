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

using static ProjectTask.ProjectTaskUtil;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	FreeBusyCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of FreeBusyItem Items.
	/// </summary>
	public class FreeBusyCollection : ChangeObjectCollection<FreeBusyItem>
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
		//* Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add the specified date range into the target collection as well as
		/// possible, returning a new free/busy item containing the part of the
		/// range that could be used.
		/// </summary>
		/// <param name="dateRange">
		/// Reference to the date range to insert.
		/// </param>
		/// <returns>
		/// Reference to the new free/busy connector portion of the date range that
		/// was able to fit within the collection, if available. Otherwise, null.
		/// </returns>
		public FreeBusyItem Add(DateRangeItem dateRange)
		{
			List<FreeBusyItem> connectors = null;
			DateTime endDate = DateTime.MinValue;
			DateRangeItem range = null;
			FreeBusyItem result = null;

			if(dateRange != null)
			{
				if(!this.Exists(x =>
					DateRangeItem.Fit(x.DateRange, dateRange) == RangePartEnum.All))
				{
					//	No cases found where none of the range can be placed.
					if(!this.Exists(x =>
					DateRangeItem.Fit(x.DateRange, dateRange) == RangePartEnum.Part))
					{
						//	No cases found where only a part of the range can be placed.
						result = new FreeBusyItem()
						{
							DateRange = dateRange
						};
						this.Add(result);
					}
					else
					{
						//	Configure the date range to work around the existing
						//	free/busy items.
						range = DateRangeItem.Clone(dateRange);
						connectors = this.FindAll(x =>
							DateRangeItem.Fit(x.DateRange, dateRange) == RangePartEnum.Part);
						foreach(FreeBusyItem busyItem in connectors)
						{
							//	Shave away at the date range.
							//	The free busy date will either cover the left side or the
							//	right side. Any strip in the middle automatically covers
							//	the entire left side.
							if(DateTime.Compare(
								busyItem.DateRange.EndDate, range.EndDate) < 0)
							{
								//	The existing date covers the left side.
								endDate = range.EndDate;
								range.StartDate = busyItem.DateRange.EndDate;
								range.EndDate = endDate;
							}
							if(DateTime.Compare(
								busyItem.DateRange.StartDate, range.StartDate) > 0)
							{
								//	The existing date covers the right side.
								range.EndDate = busyItem.DateRange.StartDate;
							}
						}
						result = new FreeBusyItem()
						{
							DateRange = range
						};
						this.Add(result);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	FreeBusyItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a resource's availability, associated with a task.
	/// </summary>
	public class FreeBusyItem : ChangeObjectItem, IItem
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
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the FreeBusyItem item.
		/// </summary>
		public FreeBusyItem()
		{
			mItemId = mNextItemId++;
			ActiveProjectContext.FreeBusyConnectors.Add(this);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Busy																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Busy">Busy</see>.
		/// </summary>
		private bool mBusy = false;
		/// <summary>
		/// Get/Set a value indicating whether this connector identifies busy.
		/// </summary>
		[JsonProperty(Order = 3)]
		public bool Busy
		{
			get { return mBusy; }
			set
			{
				bool bChanged = (mBusy != value);
				mBusy = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Clone																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a deep copy clone of the specified free/busy connector.
		/// </summary>
		/// <param name="freeBusyItem">
		/// Reference to the item to copy.
		/// </param>
		/// <returns>
		/// Reference to the new clone of the caller's item.
		/// </returns>
		public static FreeBusyItem Clone(FreeBusyItem freeBusyItem)
		{
			FreeBusyItem result = null;

			if(freeBusyItem != null)
			{
				result = new FreeBusyItem()
				{
					mBusy = freeBusyItem.mBusy,
					mContact = freeBusyItem.mContact,
					mDateRange = freeBusyItem.mDateRange,
					mItemTicket = freeBusyItem.mItemTicket,
					mTask = freeBusyItem.mTask,
					mTimeNotation = TimeNotationItem.Clone(freeBusyItem.mTimeNotation)
				};
				foreach(PropertyItem propertyItem in freeBusyItem.mExtendedProperties)
				{
					result.mExtendedProperties.Add(PropertyItem.Clone(propertyItem));
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Contact																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Contact">Contact</see>.
		/// </summary>
		private ContactItem mContact = null;
		/// <summary>
		/// Get/Set a reference to the contact to which this connector is attached.
		/// </summary>
		[JsonProperty(Order = 4)]
		public ContactItem Contact
		{
			get { return mContact; }
			set
			{
				bool bChanged = (mContact != value);
				mContact = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DateRange																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="DateRange">DateRange</see>.
		/// </summary>
		private DateRangeItem mDateRange = new DateRangeItem();
		/// <summary>
		/// Get/Set a reference to the date range occupied by this connector.
		/// </summary>
		[JsonProperty(Order = 2)]
		public DateRangeItem DateRange
		{
			get { return mDateRange; }
			set
			{
				bool bChanged = (mDateRange != value);
				mDateRange = value;
				if(mDateRange == null)
				{
					mDateRange = new DateRangeItem();
				}
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
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
		//*	ShouldSerializeBusy																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Busy property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeBusy()
		{
			return true;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeContact																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Contact property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeContact()
		{
			return mContact != null;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeDateRange																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the DateRange property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeDateRange()
		{
			return mDateRange != null;
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
			return mItemTicket?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeTask																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Task property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeTask()
		{
			return mTask != null;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeTimeNotation																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the TimeNotation property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeTimeNotation()
		{
			return mTimeNotation != null;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Task																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Task">Task</see>.
		/// </summary>
		private TaskItem mTask = null;
		/// <summary>
		/// Get/Set a reference to the task to which this connector is attached.
		/// </summary>
		[JsonProperty(Order = 5)]
		public TaskItem Task
		{
			get { return mTask; }
			set
			{
				bool bChanged = (mTask != value);
				mTask = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TimeNotation																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TimeNotation">TimeNotation</see>.
		/// </summary>
		private TimeNotationItem mTimeNotation = null;
		/// <summary>
		/// Get/Set a reference to the time notation associated with the contact.
		/// </summary>
		[JsonProperty(Order = 6)]
		public TimeNotationItem TimeNotation
		{
			get { return mTimeNotation; }
			set
			{
				bool bChanged = (mTimeNotation != value);
				mTimeNotation = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this item.
		/// </summary>
		/// <returns>
		/// The user-readable string representation of this item.
		/// </returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("Free/Busy:  ");
			builder.Append(mBusy ? "Busy" : "Free");
			builder.Append(";\r\n Time:       ");
			builder.Append(mDateRange.StartDate.ToString("yyyy-MM-dd HH:mm"));
			builder.Append(" to ");
			builder.Append(mDateRange.EndDate.ToString("yyyy-MM-dd HH:mm"));
			builder.Append(";\r\n Task:       ");
			if(mTask != null)
			{
				builder.Append(mTask.DisplayName);
				builder.Append($" ({mTask.EstimatedManHours:0.0}hr)");
			}
			else
			{
				builder.Append("(none)");
			}
			builder.Append(";\r\n Contact:    ");
			if(mContact != null)
			{
				builder.Append(mContact.DisplayName);
			}
			else
			{
				builder.Append("(none)");
			}
			builder.Append($";\r\n Id:         {mItemId}\r\n");
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
