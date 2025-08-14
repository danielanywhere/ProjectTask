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
	//*	DateRangeCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of DateRangeItem Items.
	/// </summary>
	public class DateRangeCollection : List<DateRangeItem>
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
		/// Create a new DateRangeItem, add it to the Collection, and return it to
		/// the caller.
		/// </summary>
		/// <returns>
		/// Newly created and added DateRangeItem.
		/// </returns>
		public DateRangeItem Add()
		{
			DateRangeItem result = new DateRangeItem();

			this.Add(result);
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Add another collection of Date Ranges to this collection.
		/// </summary>
		/// <param name="value">
		/// Collection of Date Ranges containing items to add.
		/// </param>
		public void Add(DateRangeCollection value)
		{
			if(value != null)
			{
				//	If the caller presented a legal value, then continue.
				foreach(DateRangeItem dr in value)
				{
					this.Add(dr);
				}
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Add a new Date Range to the collection by member values.
		/// </summary>
		/// <param name="startDate">
		/// Start Date.
		/// </param>
		/// <param name="endDate">
		/// End Date.
		/// </param>
		/// <returns>
		/// Reference to the newly created and added DateRangeItem.
		/// </returns>
		public DateRangeItem Add(DateTime startDate, DateTime endDate)
		{
			DateRangeItem result = new DateRangeItem(startDate, endDate);

			this.Add(result);
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Add a new Date Range to the collection by Member Values.
		/// </summary>
		/// <param name="startDate">
		/// Date Time Value to serve as the Start Date.
		/// </param>
		/// <param name="duration">
		/// Duration of the event.
		/// </param>
		/// <returns>
		/// Reference to the newly created and added DateRangeItem.
		/// </returns>
		public DateRangeItem Add(DateTime startDate, TimeSpan duration)
		{
			DateRangeItem result = new DateRangeItem(startDate, duration);

			this.Add(result);
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	And																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Logical AND the intersecting Dates and Times of the members of a
		/// collection to produce a single common value.
		/// </summary>
		/// <param name="source">
		/// Collection of Date Ranges to be merged to a single common Range.
		/// </param>
		/// <returns>
		/// Date Range representing the common Date and Time Range for all members
		/// of the collection. If some members did not have common ranges, then
		/// a null value is returned.
		/// </returns>
		public static DateRangeItem And(DateRangeCollection source)
		{
			int count = 0;       //	List Count.
			int indexI = 0;      //	Inner Position.
			int indexO = 0;      //	Outer Position.
			DateRangeItem result = null;

			if(source != null)
			{
				//	If the caller provided a legal value, then continue.
				count = source.Count;
				for(indexO = 0; indexO < count; indexO++)
				{
					//	Outer list.
					if(result == null)
					{
						//	If the reference item has not yet been set, then set it now.
						result = source[indexO];
					}
					for(indexI = indexO + 1; indexI < count; indexI++)
					{
						//	Inner list.
						result = DateRangeItem.And(source[indexI], result);
						if(result == null)
						{
							break;
						}
					}
					if(result == null)
					{
						break;
					}
				}
			}

			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Logical AND the intersecting dates and times of two Date Range
		/// collections.
		/// </summary>
		/// <param name="source1">
		/// First collection of Date Ranges to be merged.
		/// </param>
		/// <param name="source2">
		/// Second collection of Date Ranges to be merged.
		/// </param>
		/// <returns>
		/// New Date Range collection containing Ranges that represent the
		/// intersecting dates and times between the caller's collections.
		/// </returns>
		/// <remarks>
		/// If Both Date Ranges include a Tag value, then the tag from left
		/// collection is placed in the resulting entry.
		/// </remarks>
		public static DateRangeCollection And(DateRangeCollection source1,
			DateRangeCollection source2)
		{
			DateRangeCollection result = new DateRangeCollection();

			foreach(DateRangeItem dr1 in source1)
			{
				foreach(DateRangeItem dr2 in source2)
				{
					if(DateRangeItem.AndFit(dr1, dr2) != RangePartEnum.None)
					{
						result.Add(DateRangeItem.And(dr1, dr2));
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Mask																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Mask off Date Ranges in the caller's Reference Collection, using
		/// intersecting Dates and Times found in the Mask Collection.
		/// </summary>
		/// <param name="reference">
		/// Reference collection containing valid Date Ranges.
		/// </param>
		/// <param name="mask">
		/// Mask containing a Date Range to mask out of the reference collection.
		/// </param>
		/// <returns>
		/// New Date Range collection containing reference Ranges with mask
		/// applied.
		/// </returns>
		/// <remarks>
		/// If Both Date Ranges include a Tag value, then the tag from reference
		/// collection is placed in the resulting entry.
		/// </remarks>
		public static DateRangeCollection Mask(DateRangeCollection reference,
			DateRangeItem mask)
		{
			int count = 0;           //	List Count.
			DateRangeItem[] dates;      //	Resulting Range of Dates.
			int index = 0;           //	List Position.
			DateRangeItem range;        //	Working Range.
			DateRangeCollection result = new DateRangeCollection();

			result.Add(reference);

			if(mask != null)
			{
				//	If mask values exist, then mask from the reference.
				count = result.Count;
				for(index = 0; index < count; index++)
				{
					range = result[index];
					if(DateRangeItem.AndFit(range, mask) != RangePartEnum.None)
					{
						dates = DateRangeItem.Mask(range, mask);
						//	Invalidate the previous reference. We have fragments
						//	to store.
						result.RemoveAt(index);
						foreach(DateRangeItem drag in dates)
						{
							//	Add a Date Range for each remnant found.
							result.Add(drag);
						}
						index = -1;
						count = result.Count;
						break;
					}
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Mask off Date Ranges in the caller's Reference Collection, using
		/// intersecting Dates and Times found in the Mask Collection.
		/// </summary>
		/// <param name="reference">
		/// Reference collection containing valid Date Ranges.
		/// </param>
		/// <param name="mask">
		/// Mask collection containing Date Ranges to mask out of the reference
		/// collection.
		/// </param>
		/// <returns>
		/// New Date Range collection containing reference Ranges with mask
		/// applied.
		/// </returns>
		/// <remarks>
		/// If Both Date Ranges include a Tag value, then the tag from reference
		/// collection is placed in the resulting entry.
		/// </remarks>
		public static DateRangeCollection Mask(DateRangeCollection reference,
			DateRangeCollection mask)
		{
			int count = 0;           //	List Count.
			DateRangeItem[] dates;      //	Resulting Range of Dates.
			int index = 0;           //	List Position.
			DateRangeItem range;        //	Working Range.
			DateRangeCollection result = new DateRangeCollection();

			result.Add(reference);

			if(mask.Count != 0)
			{
				//	If mask values exist, then mask from the reference.
				count = result.Count;
				for(index = 0; index < count; index++)
				{
					range = result[index];
					foreach(DateRangeItem drm in mask)
					{
						if(DateRangeItem.AndFit(range, drm) != RangePartEnum.None)
						{
							dates = DateRangeItem.Mask(range, drm);
							//	Invalidate the previous reference. We have fragments
							//	to store.
							result.RemoveAt(index);
							foreach(DateRangeItem drag in dates)
							{
								//	Add a Date Range for each remnant found.
								result.Add(drag);
							}
							index = -1;
							count = result.Count;
							break;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string contents of this collection.
		/// </summary>
		/// <returns>
		/// String contents of this collection.
		/// </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			foreach(DateRangeItem drg in this)
			{
				sb.Append(drg.ToString() + "\n");
			}
			return sb.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TotalHours																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the total number of hours in the provided date range collection.
		/// </summary>
		/// <param name="dateRanges">
		/// Reference to the collection of date ranges to measure.
		/// </param>
		/// <returns>
		/// The total number of decimal hours in the provided collection of date
		/// ranges, if valid. Otherwise, 0.
		/// </returns>
		public static float TotalHours(List<DateRangeItem> dateRanges)
		{
			float result = 0f;

			if(dateRanges?.Count > 0)
			{
				foreach(DateRangeItem dateRangeItem in dateRanges)
				{
					result += DateRangeItem.TotalHours(dateRangeItem);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	DateRangeItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Definition of the Time between two Dates.
	/// </summary>
	public class DateRangeItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	OnChange																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the Change Event when a value in this instance has been changed.
		/// </summary>
		/// <param name="e">
		/// Standard Event Arguments.
		/// </param>
		protected virtual void OnChange(EventArgs e)
		{
			if(Change != null)
			{
				Change(this, e);
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new Instance of the DateRangeItem Item.
		/// </summary>
		public DateRangeItem()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the DateRangeItem Item.
		/// </summary>
		/// <param name="range">
		/// Reference Range.
		/// </param>
		public DateRangeItem(DateRangeItem range)
		{
			StartDate = range.StartDate;
			EndDate = range.EndDate;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the DateRangeItem Item.
		/// </summary>
		/// <param name="startDate">
		/// Starting Date of the Range.
		/// </param>
		/// <param name="endDate">
		/// Ending Date of the Range.
		/// </param>
		public DateRangeItem(DateTime startDate, DateTime endDate)
		{
			StartDate = startDate;
			EndDate = endDate;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the DateRangeItem Item.
		/// </summary>
		/// <param name="startDate">
		/// Starting Date of the Range.
		/// </param>
		/// <param name="duration">
		/// Duration from Starting Date to Ending Date.
		/// </param>
		public DateRangeItem(DateTime startDate, TimeSpan duration)
		{
			StartDate = startDate;
			Duration = duration;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	And																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the logical AND result of the two specified Date Ranges.
		/// </summary>
		/// <param name="date1">
		/// First operand.
		/// </param>
		/// <param name="date2">
		/// Second operand.
		/// </param>
		/// <returns>
		/// If the two Date Ranges had at least some time in common, then the
		/// mutually specified time between them, specified as a new Date Range
		/// object. Otherwise, null.
		/// </returns>
		public static DateRangeItem And(DateRangeItem date1, DateRangeItem date2)
		{
			DateRangeItem result = null;

			if(DateTime.Compare(date1.StartDate, date2.StartDate) <= 0 &&
				DateTime.Compare(date1.EndDate, date2.StartDate) > 0)
			{
				//	If here, then Date 1 starts on or before Date 2, and ends
				//	after Date 2 starts. Date 2 is the starting reference, and the
				//	least of the end dates of the ending reference.
				if(DateTime.Compare(date1.EndDate, date2.EndDate) > 0)
				{
					//	If date 1 ends after date 2, then date 2 end.
					result = new DateRangeItem(date2.StartDate, date2.EndDate);
				}
				else
				{
					//	Otherwise, date 1 end.
					result = new DateRangeItem(date2.StartDate, date1.EndDate);
				}
			}
			else if(DateTime.Compare(date2.StartDate, date1.StartDate) <= 0 &&
				DateTime.Compare(date2.EndDate, date1.StartDate) > 0)
			{
				//	If here, then Date 2 starts on or before Date 1, and ends
				//	after Date 1 starts. Date 1 is the starting reference, and the
				//	least of the end dates is the ending reference.
				if(DateTime.Compare(date2.EndDate, date1.EndDate) > 0)
				{
					//	If date 2 ends after date 1, then date 1 end.
					result = new DateRangeItem(date1.StartDate, date1.EndDate);
				}
				else
				{
					//	Otherwise, date 2 end.
					result = new DateRangeItem(date1.StartDate, date2.EndDate);
				}
			}
			if(result != null)
			{
				//	If a return value exists, then attempt to associate tags.
				if(date1.Tag != null)
				{
					//	If the first date has a tag, then use it.
					result.Tag = date1.Tag;
				}
				else if(date2.Tag != null)
				{
					//	Otherwise, if the second date has a tag, then use it.
					result.Tag = date2.Tag;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	AndFit																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating the type of fit the two specified Date Ranges
		/// will yield if logically ANDed together.
		/// </summary>
		/// <param name="date1">
		/// First operand.
		/// </param>
		/// <param name="date2">
		/// Second operand.
		/// </param>
		/// <returns>
		/// If the two Date Ranges had at least some time in common, then
		/// RangePartEnum.All or RangePartEnum.Part. Otherwise, RangePartEnum.None.
		/// </returns>
		public static RangePartEnum AndFit(DateRangeItem date1, DateRangeItem date2)
		{
			RangePartEnum result = RangePartEnum.None;

			if(DateTime.Compare(date1.StartDate, date2.StartDate) == 0 &&
				DateTime.Compare(date1.EndDate, date2.EndDate) == 0)
			{
				result = RangePartEnum.All;
			}
			else
			{
				if(DateTime.Compare(date1.StartDate, date2.StartDate) <= 0 &&
					DateTime.Compare(date1.EndDate, date2.StartDate) > 0)
				{
					//	If here, then Date 1 starts on or before Date 2, and ends
					//	after Date 2 starts. Date 2 is the starting reference, and the
					//	least of the end dates of the ending reference.
					result = RangePartEnum.Part;
				}
				else if(DateTime.Compare(date2.StartDate, date1.StartDate) <= 0 &&
					DateTime.Compare(date2.EndDate, date1.StartDate) > 0)
				{
					//	If here, then Date 2 starts on or before Date 1, and ends
					//	after Date 1 starts. Date 1 is the starting reference, and the
					//	least of the end dates is the ending reference.
					result = RangePartEnum.Part;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Change																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fired when a member of this instance have been changed.
		/// </summary>
		public event EventHandler Change;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Clone																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a deep copy of the provided date range.
		/// </summary>
		/// <param name="dateRange">
		/// Reference to the date range to be copied.
		/// </param>
		/// <returns>
		/// Reference to a new clone of the caller's date range.
		/// </returns>
		public static DateRangeItem Clone(DateRangeItem dateRange)
		{
			DateRangeItem result = null;

			if(dateRange != null)
			{
				result = new DateRangeItem()
				{
					mDuration = dateRange.mDuration,
					mEndDate = dateRange.mEndDate,
					mStartDate = dateRange.mStartDate,
					mTag = dateRange.mTag
				};
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Duration																															*
		//*-----------------------------------------------------------------------*
		private TimeSpan mDuration = TimeSpan.MinValue;
		/// <summary>
		/// Get/Set the Duration between the Starting and Ending Dates.
		/// </summary>
		[JsonProperty(Order = 2)]
		public TimeSpan Duration
		{
			get { return mDuration; }
			set
			{
				//	If we are setting the Duration, then override the current End
				//	Date.
				mDuration = value;
				mEndDate = mStartDate.Add(mDuration);
				OnChange(new EventArgs());
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	EndDate																																*
		//*-----------------------------------------------------------------------*
		private DateTime mEndDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the Ending Date of the Range.
		/// </summary>
		[JsonProperty(Order = 1)]
		public DateTime EndDate
		{
			get { return mEndDate; }
			set
			{
				//	If we are setting the End Date, then override the current
				//	duration.
				mEndDate = value;
				mDuration = mEndDate.Subtract(mStartDate);
				OnChange(new EventArgs());
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Fit																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether one date range will fit within
		/// another.
		/// </summary>
		/// <param name="target">
		/// The Target Date Range that is expected to contain the item under test.
		/// </param>
		/// <param name="test">
		/// The Date Range to place into the target.
		/// </param>
		/// <returns>
		/// Date Range Part indicating whether none, part, or all of the item
		/// under test will fit within the target item.
		/// </returns>
		public static RangePartEnum Fit(DateRangeItem target, DateRangeItem test)
		{
			RangePartEnum result = RangePartEnum.None;

			if((DateTime.Compare(target.StartDate, test.StartDate) >= 0 &&
				DateTime.Compare(target.StartDate, test.EndDate) < 0) ||
				(DateTime.Compare(test.StartDate, target.StartDate) >= 0 &&
				DateTime.Compare(test.StartDate, target.EndDate) < 0))
			{
				//	If the Target Date starts on or before the Test Date, and
				//	the Target Date ends after the starting of the Test Date, then
				//	at least part of the Test Date will fit.
				result = RangePartEnum.Part;
				if(DateTime.Compare(test.StartDate, target.StartDate) >= 0 &&
					DateTime.Compare(test.EndDate, target.EndDate) <= 0)
				{
					//	If the Target End Date extends beyond the Test End Date, then
					//	the entire Test Date will fit.
					result = RangePartEnum.All;
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return a value indicating whether the specified time is within the
		/// target Date Range.
		/// </summary>
		/// <param name="target">
		/// Target Date Range.
		/// </param>
		/// <param name="test">
		/// The Date and Time to test.
		/// </param>
		/// <returns>
		/// If the Date Fits within the target range, then RangePartEnum.All.
		/// Otherwise, RangePartEnum.None.
		/// </returns>
		public static RangePartEnum Fit(DateRangeItem target, DateTime test)
		{
			RangePartEnum result = RangePartEnum.None;

			if((DateTime.Compare(target.StartDate, test) <= 0 &&
				DateTime.Compare(target.EndDate, test) >= 0))
			{
				//	If the Target Date starts on or before the Test Date, and
				//	the Target Date ends on or after the Test Date, then the Test
				//	fits.
				result = RangePartEnum.All;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Mask																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the logical result of a Date Range that has had some time masked
		/// out.
		/// </summary>
		/// <param name="date">
		/// The Date Range for which part or all of the area will be masked out.
		/// </param>
		/// <param name="mask">
		/// Date Range initialized as a working mask, or black space.
		/// </param>
		/// <returns>
		/// If the two Date Ranges had at least some time not in common, then the
		/// specified time not occurring within the mask, specified as a new Date
		/// Range array with possibly one or two sides. Otherwise, zero length
		/// array.
		/// </returns>
		public static DateRangeItem[] Mask(DateRangeItem date, DateRangeItem mask)
		{
			DateRangeItem common = And(date, mask); //	Common Area.
			DateRangeItem rangeL = null;    //	Left Range.
			DateRangeItem rangeR = null;    //	Right Range.
			int count = 0;             //	Return Item Count.
			DateRangeItem[] result = new DateRangeItem[0];

			if(common == null)
			{
				//	If there is no common area, then the entire provided date
				//	is left intact.
				result = new DateRangeItem[1];
				result[0] = date;
			}
			else
			{
				//	If there is some common area between the two items, then
				//	the common area is blacked out.
				if(DateTime.Compare(date.StartDate, mask.StartDate) < 0)
				{
					//	If the date being compared begins before the mask, then
					//	left section returns the portion up to the mask.
					count++;
					rangeL = new DateRangeItem(date.StartDate, mask.StartDate);
				}
				if(DateTime.Compare(date.EndDate, mask.EndDate) > 0)
				{
					//	If the date being compared ends after the mask, then
					//	right section returns the portion beyond the mask.
					count++;
					rangeR = new DateRangeItem(mask.EndDate, date.EndDate);
				}
				if(count != 0)
				{
					//	If we have return components, then prepare them for return.
					result = new DateRangeItem[count];
					if(rangeL != null)
					{
						result[0] = rangeL;
						if(rangeR != null)
						{
							result[1] = rangeR;
						}
					}
					else if(rangeR != null)
					{
						result[0] = rangeR;
					}
				}
			}

			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Nand																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the logical NAND result of the two specified Date Ranges.
		/// </summary>
		/// <param name="date1">
		/// First operand.
		/// </param>
		/// <param name="date2">
		/// Second operand.
		/// </param>
		/// <returns>
		/// If the two Date Ranges had at least some time not in common, then the
		/// specified time not common between them, specified as a new Date Range
		/// object. Otherwise, zero length array.
		/// </returns>
		public static DateRangeItem[] Nand(DateRangeItem date1, DateRangeItem date2)
		{
			DateRangeItem common = And(date1, date2); //	Common Area.
			DateRangeItem rangeL = null;    //	Left Range.
			DateRangeItem rangeR = null;    //	Right Range.
			int count = 0;             //	Return Item Count.
			DateRangeItem[] result = new DateRangeItem[0];

			if(common != null)
			{
				//	There is some area equal between the two dates. Mask out the
				//	equal area.
				if(DateTime.Compare(date1.StartDate, date2.StartDate) < 0)
				{
					//	If here, then Date 1 starts before Date 2
					count++;
					rangeL = new DateRangeItem(date1.StartDate, date2.StartDate);
				}
				else if(DateTime.Compare(date1.StartDate, date2.StartDate) > 0)
				{
					count++;
					rangeL = new DateRangeItem(date2.StartDate, date1.StartDate);
				}
				if(DateTime.Compare(date1.EndDate, date2.EndDate) < 0)
				{
					count++;
					rangeR = new DateRangeItem(date1.EndDate, date2.EndDate);
				}
				else if(DateTime.Compare(date1.EndDate, date2.EndDate) > 0)
				{
					count++;
					rangeR = new DateRangeItem(date2.EndDate, date1.EndDate);
				}
				if(count != 0)
				{
					result = new DateRangeItem[count];
					if(rangeL != null)
					{
						result[0] = rangeL;
						if(rangeR != null)
						{
							result[1] = rangeR;
						}
					}
					else if(rangeR != null)
					{
						result[0] = rangeR;
					}
				}
			}
			else
			{
				//	The entire area between the two dates is not equal.
				result = new DateRangeItem[2];
				if(DateTime.Compare(date1.StartDate, date2.StartDate) > 0)
				{
					result[0] = date2;
					result[1] = date1;
				}
				else
				{
					result[0] = date1;
					result[1] = date2;
				}
			}
			if(result.Length != 0)
			{
				//	If a return value exists, then attempt to associate tags.
				if(date1.Tag != null)
				{
					//	If the first date has a tag, then use it.
					foreach(DateRangeItem dr in result)
					{
						dr.Tag = date1.Tag;
					}
				}
				else if(date2.Tag != null)
				{
					//	Otherwise, if the second date has a tag, then use it.
					foreach(DateRangeItem dr in result)
					{
						dr.Tag = date1.Tag;
					}
				}
			}

			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StartDate																															*
		//*-----------------------------------------------------------------------*
		private DateTime mStartDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the Starting Date of the Range.
		/// </summary>
		[JsonProperty(Order = 0)]
		public DateTime StartDate
		{
			get { return mStartDate; }
			set
			{
				//	If we are setting the Starting Date, then use the duration to
				//	calculate the End Date.
				mStartDate = value;
				if(mDuration.Ticks >= DateTime.MinValue.Ticks &&
					mDuration.Ticks <= DateTime.MaxValue.Ticks)
				{
					mEndDate = mStartDate.Add(mDuration);
				}
				else
				{
					mEndDate = mStartDate;
				}
				OnChange(new EventArgs());
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Tag																																		*
		//*-----------------------------------------------------------------------*
		private object mTag = null;
		/// <summary>
		/// Get/Set a generic Tag Value for this Date Range.
		/// </summary>
		[JsonIgnore]
		public object Tag
		{
			get { return mTag; }
			set { mTag = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TotalHours																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the total number of hours in the provided date range.
		/// </summary>
		/// <param name="dateRange">
		/// Reference to the date range to measure.
		/// </param>
		/// <returns>
		/// The total number of decimal hours in the provided date range, if
		/// valid. Otherwise, 0.
		/// </returns>
		public static float TotalHours(DateRangeItem dateRange)
		{
			float result = 0f;
			TimeSpan span = TimeSpan.Zero;

			if(dateRange != null)
			{
				span = dateRange.mEndDate - dateRange.mStartDate;
				result = (float)span.TotalHours;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this instance.
		/// </summary>
		/// <returns>
		/// String containing start time to end time.
		/// </returns>
		public override string ToString()
		{
			return StartDate.ToString() + " to " + EndDate.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Valid																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get a value indicating whether the elements of this instance are valid.
		/// </summary>
		[JsonIgnore]
		public bool Valid
		{
			get
			{
				return
					(mStartDate != DateTime.MinValue &&
					mEndDate != DateTime.MinValue);
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
