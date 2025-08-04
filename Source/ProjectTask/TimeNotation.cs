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
	//*	TimeNotationCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of TimeNotationItem Items.
	/// </summary>
	public class TimeNotationCollection :
		ChangeObjectCollection<TimeNotationItem>
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
		/// Add a list of time notation items using repetition rate and startTime,
		/// endTime.
		/// </summary>
		/// <param name="entries">
		/// Reference to an array of one or more repititionRate, startTime, endTime
		/// tuples.
		/// </param>
		/// <returns>
		/// Reference to a list of items that have been added to the collection.
		/// </returns>
		public List<TimeNotationItem> Add(params
			(ScheduleRepetitionRate repetitionRate,
				TimeSpan startTime, TimeSpan endTime)[] entries)
		{
			TimeNotationItem item = null;
			List<TimeNotationItem> result = new List<TimeNotationItem>();

			if(entries?.Length > 0)
			{
				foreach((ScheduleRepetitionRate repetitionRate,
					TimeSpan startTime, TimeSpan endTime) entryItem in entries)
				{
					item = new TimeNotationItem
					{
						Available = true,
						RepetitionRate = entryItem.repetitionRate,
						StartDate = DateTime.MinValue + entryItem.startTime,
						EndDate = DateTime.MinValue + entryItem.endTime
					};
					this.Add(item);
					result.Add(item);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TotalTime																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the total number of hours given in the provided collection,
		/// stopping with the specified maximum repeated cycle amount.
		/// </summary>
		/// <param name="times">
		/// Reference to the collection of times to render.
		/// </param>
		/// <param name="startTime">
		/// Date and time at which to start measuring the total time.
		/// </param>
		/// <param name="maximumHours">
		/// The optional maximum number of hours that can be rendered in the cycle.
		/// The default is 1,040, or six months composed of 40-hour workweeks.
		/// </param>
		/// <param name="maximumDays">
		/// The optional maximum number of days that can be rendered in the cycle.
		/// The default is 365, or one year.
		/// </param>
		/// <returns>
		/// The total amount of time defined by the provided collection of time
		/// notations, within the maximum scope of hours.
		/// </returns>
		public static float TotalTime(List<TimeNotationItem> times,
			DateTime startTime, float maximumHours = 1040f,
			int maximumDays = 365)
		{
			List<DateRangeItem> dateRanges = null;
			int dayIndex = 0;
			float result = 0f;
			DateTime workingTime = DateTime.MinValue;

			if(times?.Count > 0)
			{
				for(dayIndex = 0; dayIndex <= maximumDays; dayIndex ++)
				{
					workingTime = startTime + new TimeSpan(dayIndex, 0, 0, 0);
					foreach(TimeNotationItem notationItem in times)
					{
						dateRanges = TimeNotationItem.RenderDay(notationItem, workingTime);
						foreach(DateRangeItem dateTimeItem in dateRanges)
						{
							result += DateRangeItem.TotalHours(dateTimeItem);
							if(result >= maximumHours)
							{
								break;
							}
						}
						if(result >= maximumHours)
						{
							break;
						}
					}
					if(result >= maximumHours)
					{
						break;
					}
				}
				if(result > maximumHours)
				{
					result = maximumHours;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	TimeNotationItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information expressing a singular scope of time.
	/// </summary>
	public class TimeNotationItem : BaseItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	GetDayOfWeek																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the System Day of Week matching the specified Schedule Week
		/// Day enumeration member.
		/// </summary>
		/// <param name="value">
		/// Schedule Week Day enumeration member to convert.
		/// </param>
		/// <returns>
		/// System Day of Week corresponding to the caller's value.
		/// </returns>
		private static DayOfWeek GetDayOfWeek(ScheduleWeekDay value)
		{
			DayOfWeek rv = DateTime.Today.DayOfWeek;

			if((value & ScheduleWeekDay.Monday) != ScheduleWeekDay.None)
			{
				rv = DayOfWeek.Monday;
			}
			else if((value & ScheduleWeekDay.Tuesday) != ScheduleWeekDay.None)
			{
				rv = DayOfWeek.Tuesday;
			}
			else if((value & ScheduleWeekDay.Wednesday) != ScheduleWeekDay.None)
			{
				rv = DayOfWeek.Wednesday;
			}
			else if((value & ScheduleWeekDay.Thursday) != ScheduleWeekDay.None)
			{
				rv = DayOfWeek.Thursday;
			}
			else if((value & ScheduleWeekDay.Friday) != ScheduleWeekDay.None)
			{
				rv = DayOfWeek.Friday;
			}
			else if((value & ScheduleWeekDay.Saturday) != ScheduleWeekDay.None)
			{
				rv = DayOfWeek.Saturday;
			}
			else if((value & ScheduleWeekDay.Sunday) != ScheduleWeekDay.None)
			{
				rv = DayOfWeek.Sunday;
			}
			return rv;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetDayOfWeekMatching																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Day of Week in the specified
		/// Range matches one of those specified in the provided Time Notation.
		/// </summary>
		/// <param name="range">
		/// Date range containing Day of Week to be inspected.
		/// </param>
		/// <param name="time">
		/// Time Notation for which a match will be analyzed.
		/// </param>
		/// <returns>
		/// True if the Beginning Day of Week specified in the Date Range matches
		/// one of those specified by the Time Notation. False otherwise.
		/// </returns>
		/// <remarks>
		/// This method does not consider whether the span between times is
		/// correct, only whether the Day of Week found in the Begining Date of
		/// the Date Range matches one of the Days of the week specified in the
		/// Time Notation.
		/// </remarks>
		private static bool GetDayOfWeekMatching(DateRangeItem range,
			TimeNotationItem time)
		{
			DateTime dateWorking = DateTime.MinValue;
			ScheduleWeekDay dayOfWeek = ScheduleWeekDay.None;
			bool result = false;

			if(range != null)
			{
				if(time.mRepetitionRate == ScheduleRepetitionRate.Weekday)
				{
					//	Each Weekday.
					dateWorking = range.StartDate;
					result = (int)dateWorking.DayOfWeek > 0 &&
						(int)dateWorking.DayOfWeek < 6;
				}
				else
				{
					dayOfWeek = (ScheduleWeekDay)
						Enum.Parse(typeof(ScheduleWeekDay),
						range.StartDate.DayOfWeek.ToString(), true);
					if((time.RepetitionWeekday & dayOfWeek) != ScheduleWeekDay.None)
					{
						result = true;
					}
				}

			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetDaysOfMonth																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a collection of eligible days in the month indicated by the
		/// specified Date and Time Notation Item.
		/// </summary>
		/// <param name="date">
		/// Date from which the Month will be determined.
		/// </param>
		/// <param name="time">
		/// Time Notation Item containing a definition of mutliple possible dates.
		/// </param>
		/// <returns>
		/// Collection of Days in the given month that are eligible under the
		/// definition of the specified Time Notation.
		/// </returns>
		private static List<int> GetDaysOfMonth(DateTime date,
			TimeNotationItem time)
		{
			//	Eligible Day Of Week Collection.
			List<int> dowc = TimeNotationItem.GetDaysOfWeek(time);
			DateTime dw = date;       //	Get the working date.
			int mi = dw.Month;        //	Working Month Index.
			List<int> ro = new List<int>();

			//	Reset to the first day of the provided month.
			dw = dw.AddDays((double)(0 - (dw.Day - 1)));

			if((time.mRepetitionWeek & ScheduleWeekOrdinal.First) != 0)
			{
				AddRangeUnique(GetWeekMonthDays(dw, time, 1), ro);
			}
			if((time.RepetitionWeek & ScheduleWeekOrdinal.Second) != 0)
			{
				AddRangeUnique(GetWeekMonthDays(dw, time, 2), ro);
			}
			if((time.RepetitionWeek & ScheduleWeekOrdinal.Third) != 0)
			{
				AddRangeUnique(GetWeekMonthDays(dw, time, 3), ro);
			}
			if((time.RepetitionWeek & ScheduleWeekOrdinal.Fourth) != 0)
			{
				AddRangeUnique(GetWeekMonthDays(dw, time, 4), ro);
			}
			if((time.RepetitionWeek & ScheduleWeekOrdinal.Last) != 0)
			{
				AddRangeUnique(GetWeekMonthDays(dw, time, 5), ro);
			}

			return ro;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetDaysOfWeek																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return an array of System.DayOfWeek values corresponding to the
		/// specified Schedule Week Day flags.
		/// </summary>
		/// <param name="days">
		/// Schedule Week Day Flags, indicating multiple days of the week.
		/// </param>
		/// <returns>
		/// Array of System.DayOfWeek values corresponding to the selected days in
		/// the Week Day flags, if found. Otherwise, a zero length array.
		/// </returns>
		private static System.DayOfWeek[] GetDaysOfWeek(ScheduleWeekDay days)
		{
			int i = 0;
			List<int> ic = new List<int>();
			System.DayOfWeek[] ra = new System.DayOfWeek[0];

			if(days != ScheduleWeekDay.None)
			{
				for(i = 0; i < 7; i++)
				{
					if(IsDayOfWeekSelected(days, i))
					{
						//	If this day is selected, then add it to the collection.
						ic.Add(i);
					}
				}
			}
			if(ic.Count != 0)
			{
				//	If there are days of week, then convert them.
				ra = new System.DayOfWeek[ic.Count];
				for(i = 0; i < ic.Count; i++)
				{
					ra[i] = (System.DayOfWeek)ic[i];
				}
			}

			return ra;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return a collection of Integer values corresponding to System.DayOfWeek
		/// for the Days defined in the RepetitionWeekDay flags.
		/// </summary>
		/// <param name="time">
		/// Instance of a Time Notation having one or more Weekdays defined.
		/// </param>
		/// <returns>
		/// If the Time Notation defines one or more weekdays, then the
		/// collection will return the int values corresponding with the
		/// System.DayOfWeek enumeration values. Otherwise, an empty collection
		/// is returned.
		/// </returns>
		private static List<int> GetDaysOfWeek(TimeNotationItem time)
		{
			int dp = 0;
			List<int> ro = new List<int>();
			ScheduleWeekDay wd;

			for(dp = 0; dp < 7; dp++)
			{
				wd = (ScheduleWeekDay)Enum.Parse(typeof(ScheduleWeekDay),
					((System.DayOfWeek)dp).ToString(), true);
				if((time.RepetitionWeekday & wd) != ScheduleWeekDay.None)
				{
					//	Here, we found a corresponding item.
					ro.Add(dp);
				}
			}
			return ro;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetScheduleWeekDay																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the Schedule Week Day Enumeration member corresponding to the
		/// specified Day Name.
		/// </summary>
		/// <param name="name">
		/// Name of the Day for which to find the associated Schedule Week Day.
		/// </param>
		/// <returns>
		/// ScheduleWeekDay corresponding to the specified day, if found.
		/// Otherwise, ScheduleWeekDay.None.
		/// </returns>
		private static ScheduleWeekDay GetScheduleWeekDay(string name)
		{
			ScheduleWeekDay rv = ScheduleWeekDay.None;
			string tl;      //	Lower Case Value.

			try
			{
				rv = (ScheduleWeekDay)Enum.Parse(typeof(ScheduleWeekDay), name, true);
			}
			catch { }
			if(rv == ScheduleWeekDay.None)
			{
				tl = name.ToLower();
				switch(tl)
				{
					case "m":
					case "mo":
					case "mon":
						rv = ScheduleWeekDay.Monday;
						break;
					case "tu":
					case "tue":
						rv = ScheduleWeekDay.Tuesday;
						break;
					case "w":
					case "we":
					case "wed":
						rv = ScheduleWeekDay.Wednesday;
						break;
					case "th":
					case "thu":
						rv = ScheduleWeekDay.Thursday;
						break;
					case "f":
					case "fr":
					case "fri":
						rv = ScheduleWeekDay.Friday;
						break;
					case "sa":
					case "sat":
						rv = ScheduleWeekDay.Saturday;
						break;
					case "su":
					case "sun":
						rv = ScheduleWeekDay.Sunday;
						break;
				}
			}
			return rv;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetWeekMonthDays																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a collection of defined days of the month within the specified
		/// week occurrence. For example, the first Tuesday of the month might
		/// appear on the fifth day of that month.
		/// </summary>
		/// <param name="date">
		/// Working date from which to find the appropriate dates in the defined
		/// occurrences.
		/// </param>
		/// <param name="time">
		/// Time Notation defining the Week Days to find.
		/// </param>
		/// <param name="week">
		/// Index of the week for which to find the appropriate days. This value
		/// is one-based, and 5 corresponds to the last week of the month, which
		/// is either in the fourth or fifth week of that day.
		/// </param>
		/// <returns>
		/// Collection of Days of the Month corresponding to the given month,
		/// the days contained within the Time Notation, and within the occurrence
		/// specified by the week index.
		/// </returns>
		private static List<int> GetWeekMonthDays(DateTime date,
			TimeNotationItem time, int week)
		{
			//	Eligible Day Of Week Collection.
			List<int> dowc = GetDaysOfWeek(time);
			int di = 0;               //	Day of Week Comparator.
			DateTime dw = date;       //	Working Date.
			DateTime dw2;             //	Working Date.
			int dwc = 0;              //	Day of Week Count.
			int dwi = 0;              //	Day of Week Indexer.
			int dwp = 0;              //	Day of Week Position.
			int md = 0;               //	Working Month Day.
			int mi = dw.Month;        //	Working Month Index.
			List<int> ro = new List<int>();   //	Return value.

			//	Reset to the first day of the provided month.
			dw = dw.AddDays((double)(0 - (dw.Day - 1)));

			while(dowc.Count != 0)
			{
				//	Continue while we have outstanding days of the week to process.
				di = (int)dw.DayOfWeek;
				dwc = dowc.Count;
				for(dwp = 0; dwp < dwc; dwp++)
				{
					dwi = dowc[dwp];
					if(dwi == di)
					{
						//	If the currently indexed day is one of the defined days,
						//	then index to the correct week.
						dw2 = dw.AddDays((double)(7 * (week - 1)));
						if(dw2.Month == mi)
						{
							//	If we have the correct month, then we have our day.
							md = dw2.Day;
						}
						else
						{
							//	Otherwise, it was only an overflow on the fifth week.
							//	Recalculate to get the fourth week.
							dw2 = dw.AddDays((double)(7 * (week - 2)));
							if(dw2.Month == mi)
							{
								//	This 'if' probably isn't necessary, but let's see
								//	how it goes.
								md = dw2.Day;
							}
						}
						//	We found the specified instance of the defined day.
						ro.Add(md);
						//	Remove that defined day, so we don't see it next week.
						dowc.RemoveAt(dwp);
						dwp--;      //	Deindex.
						dwc--;      //	Decount.
					}
				}
				if(dowc.Count != 0)
				{
					//	If there are still items to process, then move to the next day.
					dw = dw.AddDays(1);
				}
			}

			return ro;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetWeeks																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return an array of Week Names corresponding to the
		/// specified Schedule Week Ordinal flags.
		/// </summary>
		/// <param name="ordinals">
		/// Schedule Week Ordinal Flags, indicating multiple Ordinal ranges.
		/// </param>
		/// <param name="abbreviated">
		/// Value indicating whether or not to return abbreviated values.
		/// </param>
		/// <returns>
		/// Array of string values corresponding to the selected ordinals in
		/// the ordinal flags, if found. Otherwise, a zero length array.
		/// </returns>
		private static string[] GetWeeks(ScheduleWeekOrdinal weeks,
			bool abbreviated)
		{
			List<string> tc = new List<string>();

			if((weeks & ScheduleWeekOrdinal.First) != 0)
			{
				if(abbreviated)
				{
					tc.Add("1st");
				}
				else
				{
					tc.Add("First");
				}
			}
			if((weeks & ScheduleWeekOrdinal.Second) != 0)
			{
				if(abbreviated)
				{
					tc.Add("2nd");
				}
				else
				{
					tc.Add("Second");
				}
			}
			if((weeks & ScheduleWeekOrdinal.Third) != 0)
			{
				if(abbreviated)
				{
					tc.Add("3rd");
				}
				else
				{
					tc.Add("Third");
				}
			}
			if((weeks & ScheduleWeekOrdinal.Fourth) != 0)
			{
				if(abbreviated)
				{
					tc.Add("4th");
				}
				else
				{
					tc.Add("Fourth");
				}
			}
			if((weeks & ScheduleWeekOrdinal.Last) != 0)
			{
				tc.Add("Last");
			}

			return tc.ToArray();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	IsDayOfWeekSelected																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Index of a System.DayOfWeek value
		/// is selected in the Schedule Week Day flags.
		/// </summary>
		/// <param name="days">
		/// Schedule Week Day Flags, indicating multiple days of the week.
		/// </param>
		/// <param name="index">
		/// Int32 index of the corresponding System.DayOfWeek value.
		/// </param>
		/// <returns>
		/// If the specified Day of the Week corresponds to one of the provided
		/// Week Day flags that are set, then true. Otherwise, false.
		/// </returns>
		private static bool IsDayOfWeekSelected(ScheduleWeekDay days,
			int index)
		{
			System.DayOfWeek dow = (System.DayOfWeek)index;
			ScheduleWeekDay swd = GetScheduleWeekDay(dow.ToString());

			return ((days & swd) != 0);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RenderDateDay																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render potential repeating dates that occur once every X days.
		/// </summary>
		/// <param name="scope">
		/// The absolute scope of allowable time.
		/// </param>
		/// <param name="time">
		/// Time Notation definition of the pattern of occurrences.
		/// </param>
		/// <returns>
		/// Reference to a collection of rendered time notation date trackers.
		/// </returns>
		private static TimeNotationDateCollection RenderDateDay(
			DateRangeItem scope, TimeNotationItem time)
		{
			bool bUpdated = false;
			DateRangeItem dateRange;
			RangePartEnum rangePart;
			int repeatCount = -1;
			DateTime repeatEndDate = DateTime.MinValue;
			TimeNotationDateCollection result = new TimeNotationDateCollection();

			if(scope != null && time != null &&
				time.RepetitionRate != ScheduleRepetitionRate.None &&
				time.ActivePeriod != ScheduleActivePeriod.None)
			{
				//	If this item has at least one occurrence, then continue.
				//	Configure the Active Plan.
				if(time.ActivePeriod == ScheduleActivePeriod.Count)
				{
					repeatCount = time.ActiveCount;
				}
				else if(time.ActivePeriod == ScheduleActivePeriod.EndDate)
				{
					repeatEndDate = time.ActiveEndDate;
				}
				dateRange = new DateRangeItem(time.StartDate, time.EndDate);
				while(DateTime.Compare(scope.EndDate, dateRange.StartDate) > 0 &&
					(repeatCount == -1 || repeatCount > 0) &&
					(repeatEndDate == DateTime.MinValue ||
					DateTime.Compare(repeatEndDate, dateRange.StartDate) > 0))
				{
					//	Continue while we are still in a valid scope.
					rangePart = DateRangeItem.Fit(scope, dateRange);
					if(rangePart != RangePartEnum.None)
					{
						//	If some of the current item will fit within the defined scope,
						//	then place the appropriate part.
						if(rangePart == RangePartEnum.All)
						{
							//	If the whole thing fits in the scope, then
							//	add it.
							result.Add(time, dateRange);
						}
						else
						{
							//	Otherwise, add the part that fits.
							result.Add(time, DateRangeItem.And(scope, dateRange));
						}
					}
					//	Adjust the current date to the next occurrence.
					dateRange.StartDate =
						dateRange.StartDate.AddDays(time.ActiveDayIndex);
					if(time.ActivePeriod == ScheduleActivePeriod.Count)
					{
						//	If this is a counted occurrence, then simply decrement the
						//	counter.
						repeatCount--;
					}
					bUpdated = true;
				}
				if(bUpdated)
				{
					//	When updates were made, echo those updates back to the
					//	time notation.
					time.StartDate = dateRange.StartDate;
					time.EndDate = dateRange.EndDate;
					if(time.ActivePeriod == ScheduleActivePeriod.Count)
					{
						time.ActiveCount = repeatCount;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RenderDateDayOfWeek																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render potential repeating dates that use Day Of Week values.
		/// </summary>
		/// <param name="scope">
		/// The absolute scope of allowable time.
		/// </param>
		/// <param name="time">
		/// Time Notation definition the pattern of occurrences.
		/// </param>
		/// <returns>
		/// Reference to a collection of rendered time notation date trackers.
		/// </returns>
		private static TimeNotationDateCollection RenderDateDayOfWeek(
			DateRangeItem scope, TimeNotationItem time)
		{
			bool bUpdated = false;
			DateRangeItem dateRange;
			RangePartEnum rangePart;
			int repeatCount = -1;
			DateTime repeatEndDate = DateTime.MinValue;
			TimeNotationDateCollection result = new TimeNotationDateCollection();

			if(scope != null && time != null &&
				time.RepetitionRate != ScheduleRepetitionRate.None &&
				time.ActivePeriod != ScheduleActivePeriod.None)
			{
				//	If this item has at least one occurrence, then continue.
				//	Configure the Active Plan.
				if(time.ActivePeriod == ScheduleActivePeriod.Count)
				{
					repeatCount = time.ActiveCount;
				}
				else if(time.ActivePeriod == ScheduleActivePeriod.EndDate)
				{
					repeatEndDate = time.ActiveEndDate;
				}
				dateRange = new DateRangeItem(time.StartDate, time.EndDate);
				//	Match the starting date to the template.
				if(!GetDayOfWeekMatching(dateRange, time))
				{
					//	If the first date doesn't match the template, then the
					//	first following match will be used as the first reference.
					SetNextMatchingDayOfWeek(dateRange, time);
					bUpdated = true;
				}

				while(DateTime.Compare(scope.EndDate, dateRange.StartDate) > 0 &&
					(repeatCount == -1 || repeatCount > 0) &&
					(repeatEndDate == DateTime.MinValue ||
					DateTime.Compare(repeatEndDate, dateRange.StartDate) > 0))
				{
					//	Continue while we are still in a valid scope.
					rangePart = DateRangeItem.Fit(scope, dateRange);
					if(rangePart != RangePartEnum.None)
					{
						//	If some of the current item will fit within the defined scope,
						//	then place the appropriate part.
						if(rangePart == RangePartEnum.All)
						{
							//	If the whole thing fits in the scope, then
							//	add it.
							result.Add(time, dateRange);
						}
						else
						{
							//	Otherwise, add the part that fits.
							result.Add(time, DateRangeItem.And(scope, dateRange));
						}
					}
					//	Adjust the current date to the next occurrence.
					SetNextMatchingDayOfWeek(dateRange, time);
					if(time.ActivePeriod == ScheduleActivePeriod.Count)
					{
						//	If this is a counted occurrence, then simply decrement the
						//	counter.
						repeatCount--;
					}
					bUpdated = true;
				}
				if(bUpdated)
				{
					//	Echo any updates to the time notation.
					time.StartDate = dateRange.StartDate;
					time.EndDate = dateRange.EndDate;
					if(time.ActivePeriod == ScheduleActivePeriod.Count)
					{
						time.ActiveCount = repeatCount;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RenderDateMonthDay																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render potential repeating dates that occur on the X day of each Y
		/// months.
		/// </summary>
		/// <param name="scope">
		/// The absolute scope of allowable time.
		/// </param>
		/// <param name="time">
		/// Time Notation definition the pattern of occurrences.
		/// </param>
		/// <returns>
		/// Reference to a collection of rendered time notation date trackers.
		/// </returns>
		private static TimeNotationDateCollection RenderDateMonthDay(
			DateRangeItem scope, TimeNotationItem time)
		{
			bool bUpdated = false;
			DateRangeItem dateRange;
			RangePartEnum rangePart;
			int repeatCount = -1;
			DateTime repeatEndDate = DateTime.MinValue;
			TimeNotationDateCollection result = new TimeNotationDateCollection();

			if(scope != null && time != null &&
				time.RepetitionRate != ScheduleRepetitionRate.None &&
				time.ActivePeriod != ScheduleActivePeriod.None)
			{
				//	If this item has at least one occurrence, then continue.
				//	Configure the Active Plan.
				if(time.ActivePeriod == ScheduleActivePeriod.Count)
				{
					repeatCount = time.ActiveCount;
				}
				else if(time.ActivePeriod == ScheduleActivePeriod.EndDate)
				{
					repeatEndDate = time.ActiveEndDate;
				}
				dateRange = new DateRangeItem(time.StartDate, time.EndDate);
				while(DateTime.Compare(scope.EndDate, dateRange.StartDate) > 0 &&
					(repeatCount == -1 || repeatCount > 0) &&
					(repeatEndDate == DateTime.MinValue ||
					DateTime.Compare(repeatEndDate, dateRange.StartDate) > 0))
				{
					//	Continue while we are still in a valid scope.
					rangePart = DateRangeItem.Fit(scope, dateRange);
					if(rangePart != RangePartEnum.None)
					{
						//	If some of the current item will fit within the defined scope,
						//	then place the appropriate part.
						if(rangePart == RangePartEnum.All)
						{
							//	If the whole thing fits in the scope, then
							//	add it.
							result.Add(time, dateRange);
						}
						else
						{
							//	Otherwise, add the part that fits.
							result.Add(time, DateRangeItem.And(scope, dateRange));
						}
					}
					//	Adjust the current date to the next occurrence.
					dateRange.StartDate = dateRange.StartDate.AddMonths(time.ActiveMonthIndex);
					if(time.ActivePeriod == ScheduleActivePeriod.Count)
					{
						//	If this is a counted occurrence, then simply decrement the
						//	counter.
						repeatCount--;
					}
					bUpdated = true;
				}
				if(bUpdated)
				{
					time.StartDate = dateRange.StartDate;
					time.EndDate = dateRange.EndDate;
					if(time.ActivePeriod == ScheduleActivePeriod.Count)
					{
						time.ActiveCount = repeatCount;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RenderDateOnce																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render a single occurrence date.
		/// </summary>
		/// <param name="scope">
		/// The absolute scope of allowable time.
		/// </param>
		/// <param name="time">
		/// Time Notation definition the pattern of occurrences.
		/// </param>
		/// <returns>
		/// Reference to a collection of rendered time notation date trackers.
		/// </returns>
		private static TimeNotationDateCollection RenderDateOnce(
			DateRangeItem scope, TimeNotationItem time)
		{
			DateRangeItem dateRange = null;
			DateRangeItem[] dateRangeRemainders = null;
			DateRangeItem dateRangeResult = null;
			RangePartEnum rangePart;
			TimeNotationDateCollection result = new TimeNotationDateCollection();

			if(scope != null && time != null &&
				time.ActiveCount > -1 &&
				time.EndDate.CompareTo(DateTime.MinValue) > 0)
			{
				dateRange = new DateRangeItem(time.StartDate, time.EndDate);
				rangePart = DateRangeItem.Fit(scope, dateRange);
				if(rangePart != RangePartEnum.None)
				{
					if(rangePart == RangePartEnum.All)
					{
						//	If the whole thing fits in the scope, then
						//	add it.
						result.Add(time, dateRange);
						time.StartDate = DateTime.MinValue;
						time.EndDate = DateTime.MinValue;
					}
					else
					{
						//	Otherwise, add the part that fits.
						dateRangeResult = DateRangeItem.And(scope, dateRange);
						dateRangeRemainders =
							DateRangeItem.Mask(dateRange, dateRangeResult);
						result.Add(time, dateRangeResult);
						if(dateRangeRemainders.Length > 0)
						{
							dateRangeResult =
								dateRangeRemainders[dateRangeRemainders.Length - 1];
							time.StartDate = dateRangeResult.StartDate;
							time.EndDate = dateRangeResult.EndDate;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	RenderDates																														*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Render date sets that have not been rendered.
		///// </summary>
		///// <param name="resources">
		///// Collection of resources for which the dates will be rendered, if
		///// necessary.
		///// </param>
		///// <returns>
		///// Value indicating whether or not dates were rendered on any item in the
		///// collection of resources.
		///// </returns>
		//private bool RenderDates(ScheduleResourceCollection resources)
		//{
		//	//			DateRangeItem drg;			//	Working Date Range.
		//	//			RangePart drp;	//	Working Date Part.
		//	//			int repCount = 0;													//	Repetition Counter.
		//	DateTime repEndDate = DateTime.MinValue;  //	Repetition End Date.
		//	bool rv = false;    //	Return Value.

		//	foreach(ScheduleResourceItem sri in resources)
		//	{
		//		if(!sri.DatesRendered)
		//		{
		//			//	If the dates have not been rendered on this item, then do it.
		//			rv = true;
		//			//	Clear previously rendered dates.
		//			sri.DateRanges.Clear();
		//			if(mScope.Valid)
		//			{
		//				//	If the scope is valid, then make some assignments within that
		//				//	scope.
		//				foreach(TimeNotationItem tni in sri.TimeNotations)
		//				{
		//					switch(tni.RepetitionRate)
		//					{
		//						case ScheduleRepetitionRate.None:
		//							break;

		//						case ScheduleRepetitionRate.Once:
		//							RenderDateOnce(mScope, sri, tni);
		//							break;

		//						case ScheduleRepetitionRate.Day:
		//							RenderDateDay(mScope, sri, tni);
		//							break;

		//						case ScheduleRepetitionRate.Weekday:
		//						case ScheduleRepetitionRate.Weekly:
		//						case ScheduleRepetitionRate.Month:
		//						case ScheduleRepetitionRate.Year:
		//							RenderDateDayOfWeek(mScope, sri, tni);
		//							break;

		//						case ScheduleRepetitionRate.MonthDay:
		//							RenderDateMonthDay(mScope, sri, tni);
		//							break;

		//						case ScheduleRepetitionRate.YearDay:
		//							RenderDateYearDay(mScope, sri, tni);
		//							break;
		//					}
		//				}
		//			}
		//			//	TODO: Remove availability items in relation to assigned...
		//			//	requirements, and matching roles.
		//		}
		//	}
		//	return rv;
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RenderDateYearDay																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render potential repeating dates that occur on the X day of each
		/// {month}.
		/// </summary>
		/// <param name="scope">
		/// The absolute scope of allowable time.
		/// </param>
		/// <param name="time">
		/// Time Notation definition the pattern of occurrences.
		/// </param>
		/// <returns>
		/// Reference to a collection of rendered time notation date trackers.
		/// </returns>
		private static TimeNotationDateCollection RenderDateYearDay(
			DateRangeItem scope, TimeNotationItem time)
		{
			bool bUpdated = false;
			DateRangeItem dateRange;
			RangePartEnum rangePart;
			DateTime dateWorking;
			int repeatCount = -1;
			DateTime repeatEndDate = DateTime.MinValue;
			TimeNotationDateCollection result = new TimeNotationDateCollection();

			if(scope != null && time != null &&
				time.RepetitionRate != ScheduleRepetitionRate.None &&
				time.ActivePeriod != ScheduleActivePeriod.None)
			{
				//	If this item has at least one occurrence, then continue.
				//	Configure the Active Plan.
				if(time.ActivePeriod == ScheduleActivePeriod.Count)
				{
					repeatCount = time.ActiveCount;
				}
				else if(time.ActivePeriod == ScheduleActivePeriod.EndDate)
				{
					repeatEndDate = time.ActiveEndDate;
				}
				dateRange = new DateRangeItem(time.StartDate, time.EndDate);
				while(DateTime.Compare(scope.EndDate, dateRange.StartDate) > 0 &&
					(repeatCount == -1 || repeatCount > 0) &&
					(repeatEndDate == DateTime.MinValue ||
					DateTime.Compare(repeatEndDate, dateRange.StartDate) > 0))
				{
					//	Continue while we are still in a valid scope.
					rangePart = DateRangeItem.Fit(scope, dateRange);
					if(rangePart != RangePartEnum.None)
					{
						//	If some of the current item will fit within the defined scope,
						//	then place the appropriate part.
						if(rangePart == RangePartEnum.All)
						{
							//	If the whole thing fits in the scope, then
							//	add it.
							result.Add(time, dateRange);
						}
						else
						{
							//	Otherwise, add the part that fits.
							result.Add(time, DateRangeItem.And(scope, dateRange));
						}
					}
					//	Adjust the current date to the next occurrence.
					dateWorking = dateRange.StartDate.AddYears(1);
					while(dateWorking.Month != dateRange.StartDate.Month ||
						dateWorking.Day != dateRange.StartDate.Day)
					{
						//	Process for leap year day.
						dateWorking = dateWorking.AddYears(1);
					}
					dateRange.StartDate = dateWorking;
					if(time.ActivePeriod == ScheduleActivePeriod.Count)
					{
						//	If this is a counted occurrence, then simply decrement the
						//	counter.
						repeatCount--;
					}
				}
				if(bUpdated)
				{
					time.StartDate = dateRange.StartDate;
					time.EndDate = dateRange.EndDate;
					if(time.ActivePeriod == ScheduleActivePeriod.Count)
					{
						time.ActiveCount = repeatCount;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SetNextMatchingDayOfWeek																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the Beginning Date of the specified Range to the next valid Day Of
		/// Week.
		/// </summary>
		/// <param name="range">
		/// The currently selected Date Range for which the next occurrence will
		/// be found.
		/// </param>
		/// <param name="time">
		/// The Time Notation definition the pattern of occurrences.
		/// </param>
		private static void SetNextMatchingDayOfWeek(DateRangeItem range,
			TimeNotationItem time)
		{
			bool bFound = false;
			List<int> daysOfMonth;
			int dayOfMonthIndex = 0;
			List<int> daysOfWeek = TimeNotationItem.GetDaysOfWeek(time);
			int dayOfWeekHigh = 0;
			int dayOfWeekLow = 0;
			DateTime dateWorking;            //	Working Date.
			int wdi = 0;            //	Day of Week Value.


			dateWorking = range.StartDate;
			switch(time.RepetitionRate)
			{
				case ScheduleRepetitionRate.Weekday:
					//	Each Weekday.
					dateWorking = dateWorking.AddDays(1);
					while(((int)dateWorking.DayOfWeek) < 1 ||
						((int)dateWorking.DayOfWeek) > 5)
					{
						dateWorking = dateWorking.AddDays(1);
					}
					range.StartDate = dateWorking;
					break;

				case ScheduleRepetitionRate.Weekly:
					bFound = false;       //	Match not found.
					dayOfWeekHigh = daysOfWeek.Max();
					wdi = (int)dateWorking.DayOfWeek;
					while(wdi < dayOfWeekHigh)
					{
						//	If we can index to another day in the current week,
						//	then continue.
						dateWorking = dateWorking.AddDays(1);
						wdi = (int)dateWorking.DayOfWeek;
						if(daysOfWeek.Contains(wdi))
						{
							//	If the days of week collection contains this day
							//	then let's use it.
							range.StartDate = dateWorking;
							bFound = true;    //	We found the new begin date.
							break;
						}
					}
					if(!bFound)
					{
						//	If we couldn't use another item in the same week, then we need
						//	to index to the following appropriate week.
						//	First, reset to the first specified day of week.
						dayOfWeekLow = daysOfWeek.Min();
						wdi = (int)dateWorking.DayOfWeek;
						while(wdi > dayOfWeekLow)
						{
							dateWorking = dateWorking.AddDays(-1);
							wdi = (int)dateWorking.DayOfWeek;
						}
						//	Here, we have reset to the first active day of the
						//	week we've already used.
						//	Index to the next available week.
						dateWorking =
							dateWorking.AddDays((double)(7 * time.ActiveWeekIndex));
						//	Ready to rock.
						range.StartDate = dateWorking;
					}
					break;

				case ScheduleRepetitionRate.Month:
					//	Get the days of the month that pertain to the current month.
					daysOfMonth = GetDaysOfMonth(dateWorking, time);
					dayOfMonthIndex =
						daysOfMonth.FirstOrDefault(x => x == dateWorking.Day + 1);
					if(dayOfMonthIndex != 0)
					{
						//	If we found another valid day in this month, then use it.
						dateWorking =
							dateWorking.AddDays((double)(dayOfMonthIndex - dateWorking.Day));
					}
					else
					{
						//	Otherwise, we need to index to the next available month.

						//	Reset to first day of month.
						dateWorking =
							dateWorking.AddDays((double)(0 - (dateWorking.Day - 1)));
						dateWorking = dateWorking.AddMonths(time.ActiveMonthIndex);
						//	Get the available days in that month.
						daysOfMonth = GetDaysOfMonth(dateWorking, time);
						//	Get the first available day in that month.
						dayOfMonthIndex = daysOfMonth.Min();
						if(dayOfMonthIndex != -1)
						{
							//	If we found a valid day, then use it.
							dateWorking =
								dateWorking.AddDays(
									(double)(dayOfMonthIndex - dateWorking.Day));
						}
					}
					range.StartDate = dateWorking;
					break;

				case ScheduleRepetitionRate.Year:
					//	Get the days of the month that pertain to the current month.
					daysOfMonth = GetDaysOfMonth(dateWorking, time);
					dayOfMonthIndex =
						daysOfMonth.FirstOrDefault(x => x == dateWorking.Day + 1);
					if(dayOfMonthIndex != 0)
					{
						//	If we found another valid day in this month, then use it.
						dateWorking =
							dateWorking.AddDays((double)(dayOfMonthIndex - dateWorking.Day));
					}
					else
					{
						//	Otherwise, we need to index to the next available month.

						//	Reset to first day of month.
						dateWorking =
							dateWorking.AddDays((double)(0 - (dateWorking.Day - 1)));
						dateWorking = dateWorking.AddYears(1);
						//	Get the available days in that month.
						daysOfMonth = GetDaysOfMonth(dateWorking, time);
						//	Get the first available day in that month.
						dayOfMonthIndex = daysOfMonth.Min();
						if(dayOfMonthIndex != -1)
						{
							//	If we found a valid day, then use it.
							dateWorking =
								dateWorking.AddDays(
									(double)(dayOfMonthIndex - dateWorking.Day));
						}
					}
					range.StartDate = dateWorking;
					break;
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
		//*	ActiveCount																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ActiveCount">ActiveCount</see>.
		/// </summary>
		private int mActiveCount = 0;
		/// <summary>
		/// Get/Set the number of occurrences until the event has expired.
		/// </summary>
		[JsonProperty(Order = 2)]
		public int ActiveCount
		{
			get { return mActiveCount; }
			set
			{
				bool bChanged = (mActiveCount != value);
				mActiveCount = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ActiveDayIndex																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ActiveDayIndex">ActiveDayIndex</see>.
		/// </summary>
		private int mActiveDayIndex = 0;
		/// <summary>
		/// Get/Set the index of days.
		/// </summary>
		[JsonProperty(Order = 3)]
		public int ActiveDayIndex
		{
			get { return mActiveDayIndex; }
			set
			{
				bool bChanged = (mActiveDayIndex != value);
				mActiveDayIndex = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ActiveEndDate																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ActiveEndDate">ActiveEndDate</see>.
		/// </summary>
		private DateTime mActiveEndDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the end date of the active plan.
		/// </summary>
		[JsonProperty(Order = 4)]
		public DateTime ActiveEndDate
		{
			get { return mActiveEndDate; }
			set
			{
				bool bChanged = (mActiveEndDate.CompareTo(value) != 0);
				mActiveEndDate = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ActiveMonthIndex																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ActiveMonthIndex">ActiveMonthIndex</see>.
		/// </summary>
		private int mActiveMonthIndex = 0;
		/// <summary>
		/// Get/Set the index of months.
		/// </summary>
		[JsonProperty(Order = 5)]
		public int ActiveMonthIndex
		{
			get { return mActiveMonthIndex; }
			set
			{
				bool bChanged = (mActiveMonthIndex != value);
				mActiveMonthIndex = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ActivePeriod																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ActivePeriod">ActivePeriod</see>.
		/// </summary>
		private ScheduleActivePeriod mActivePeriod =
			ScheduleActivePeriod.Indefinite;
		/// <summary>
		/// Get/Set the selection of the active plan.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty(Order = 6)]
		public ScheduleActivePeriod ActivePeriod
		{
			get { return mActivePeriod; }
			set
			{
				bool bChanged = (mActivePeriod != value);
				mActivePeriod = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ActiveWeekIndex																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ActiveWeekIndex">ActiveWeekIndex</see>.
		/// </summary>
		private int mActiveWeekIndex = 0;
		/// <summary>
		/// Get/Set the index of weeks.
		/// </summary>
		[JsonProperty(Order = 7)]
		public int ActiveWeekIndex
		{
			get { return mActiveWeekIndex; }
			set
			{
				bool bChanged = (mActiveWeekIndex != value);
				mActiveWeekIndex = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ActiveYearIndex																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ActiveYearIndex">ActiveYearIndex</see>.
		/// </summary>
		private int mActiveYearIndex = 0;
		/// <summary>
		/// Get/Set the index of years.
		/// </summary>
		[JsonProperty(Order = 8)]
		public int ActiveYearIndex
		{
			get { return mActiveYearIndex; }
			set
			{
				bool bChanged = (mActiveYearIndex != value);
				mActiveYearIndex = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Available																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Available">Available</see>.
		/// </summary>
		private bool mAvailable = true;
		/// <summary>
		/// Get/Set a value indicating whether this time is available.
		/// </summary>
		[JsonProperty(Order = 9)]
		public bool Available
		{
			get { return mAvailable; }
			set
			{
				bool bChanged = (mAvailable != value);
				mAvailable = value;
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
		/// Create a deep copy of the provided time notation item.
		/// </summary>
		/// <param name="notation">
		/// Reference to the time notation item to copy.
		/// </param>
		/// <returns>
		/// Reference to the newly cloned time notation item, fresh out of the
		/// package and ready to use, now with 30% more goodness.
		/// </returns>
		public static TimeNotationItem Clone(TimeNotationItem notation)
		{
			TimeNotationItem result = null;

			if(notation != null)
			{
				result = new TimeNotationItem()
				{
					mActiveCount = notation.mActiveCount,
					mActiveDayIndex = notation.mActiveDayIndex,
					mActiveEndDate = notation.mActiveEndDate,
					mActiveMonthIndex = notation.mActiveMonthIndex,
					mActivePeriod = notation.mActivePeriod,
					mActiveWeekIndex = notation.mActiveWeekIndex,
					mActiveYearIndex = notation.mActiveYearIndex,
					mAvailable = notation.mAvailable,
					mEndDate = notation.mEndDate,
					mNextDate = notation.mNextDate,
					mRepetitionRate = notation.mRepetitionRate,
					mRepetitionWeek = notation.mRepetitionWeek,
					mRepetitionWeekday = notation.mRepetitionWeekday,
					mStartDate = notation.mStartDate
				};
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	EndDate																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="EndDate">EndDate</see>.
		/// </summary>
		private DateTime mEndDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the date and time at which this item ends on the defining
		/// occurrence.
		/// </summary>
		[JsonProperty(Order = 10)]
		public DateTime EndDate
		{
			get { return mEndDate; }
			set
			{
				bool bChanged = (mEndDate.CompareTo(value) != 0);
				mEndDate = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	NextDate																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="NextDate">NextDate</see>.
		/// </summary>
		private DateTime mNextDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the next date and time at which this item begins on the
		/// defining occurrence.
		/// </summary>
		[JsonProperty(Order = 11)]
		public DateTime NextDate
		{
			get { return mNextDate; }
			set
			{
				bool bChanged = (mNextDate.CompareTo(value) != 0);
				mNextDate = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderDay																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render the period of a single 24-hour period using the provided time
		/// notation record and the specified date.
		/// </summary>
		/// <param name="notation">
		/// Reference to the time notation to unroll.
		/// </param>
		/// <param name="date">
		/// Date to render.
		/// </param>
		/// <returns>
		/// Reference to a collection of date range items matching the 24-hour
		/// period.
		/// </returns>
		/// <remarks>
		/// In this version, the day period ends at the next stroke of midnight.
		/// </remarks>
		public static List<DateRangeItem> RenderDay(TimeNotationItem notation,
			DateTime date)
		{
			List<DateRangeItem> result = new List<DateRangeItem>();
			DateRangeItem scope = null;
			TimeNotationDateCollection times = null;
			TimeSpan timeSpan =
				(date.Date + new TimeSpan(24, 0, 0)) - date;

			scope = new DateRangeItem(date, timeSpan);
			if(notation != null)
			{
				if(DateTime.Compare(notation.mStartDate.Date, DateTime.MinValue) == 0)
				{
					notation.mStartDate = DateTime.Today + notation.mStartDate.TimeOfDay;
				}
				if(DateTime.Compare(notation.mEndDate.Date, DateTime.MinValue) == 0)
				{
					notation.mEndDate = DateTime.Today + notation.mEndDate.TimeOfDay;
				}
				switch(notation.mRepetitionRate)
				{
					case ScheduleRepetitionRate.Day:
						times = RenderDateDay(scope, notation);
						break;
					case ScheduleRepetitionRate.Month:
					case ScheduleRepetitionRate.Weekday:
					case ScheduleRepetitionRate.Weekly:
					case ScheduleRepetitionRate.Year:
						times = RenderDateDayOfWeek(scope, notation);
						break;
					case ScheduleRepetitionRate.MonthDay:
						times = RenderDateMonthDay(scope, notation);
						break;
					case ScheduleRepetitionRate.None:
						break;
					case ScheduleRepetitionRate.Once:
						times = RenderDateOnce(scope, notation);
						break;
					case ScheduleRepetitionRate.YearDay:
						times = RenderDateYearDay(scope, notation);
						break;
				}
				if(times?.Count > 0)
				{
					foreach(TimeNotationDateItem dateItem in times)
					{
						result.Add(dateItem.DateRange);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RepetitionWeek																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="RepetitionWeek">RepetitionWeek</see>.
		/// </summary>
		private ScheduleWeekOrdinal mRepetitionWeek =
			ScheduleWeekOrdinal.None;
		/// <summary>
		/// Get/Set the week ordinal mask of the repetition plan.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty(Order = 12)]
		public ScheduleWeekOrdinal RepetitionWeek
		{
			get { return mRepetitionWeek; }
			set
			{
				bool bChanged = (mRepetitionWeek != value);
				RepetitionWeek = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RepetitionRate																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="RepetitionRate">RepetitionRate</see>.
		/// </summary>
		private ScheduleRepetitionRate mRepetitionRate =
			ScheduleRepetitionRate.None;
		/// <summary>
		/// Get/Set the repetition rate of this item.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty(Order = 13)]
		public ScheduleRepetitionRate RepetitionRate
		{
			get { return mRepetitionRate; }
			set
			{
				bool bChanged = (mRepetitionRate != value);
				mRepetitionRate = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RepetitionWeekday																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="RepetitionWeekday">RepetitionWeekday</see>.
		/// </summary>
		private ScheduleWeekDay mRepetitionWeekday = ScheduleWeekDay.None;
		/// <summary>
		/// Get/Set the weekday mask of the repetition plan.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty(Order = 14)]
		public ScheduleWeekDay RepetitionWeekday
		{
			get { return mRepetitionWeekday; }
			set
			{
				bool bChanged = (mRepetitionWeekday != value);
				mRepetitionWeekday = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeActiveCount																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ActiveCount property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeActiveCount()
		{
			return mActiveCount != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeActiveDayIndex																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ActiveDayIndex property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeActiveDayIndex()
		{
			return mActiveDayIndex != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeActiveEndDate																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ActiveEndDate property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeActiveEndDate()
		{
			return DateTime.Compare(mActiveEndDate, DateTime.MinValue) != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeActiveMonthIndex																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ActiveMonthIndex property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeActiveMonthIndex()
		{
			return mActiveMonthIndex != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeActivePeriod																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ActivePeriod property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeActivePeriod()
		{
			return mActivePeriod != ScheduleActivePeriod.None;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeActiveWeekIndex																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ActiveWeekIndex property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeActiveWeekIndex()
		{
			return mActiveWeekIndex != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeActiveYearIndex																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ActiveYearIndex property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeActiveYearIndex()
		{
			return mActiveYearIndex != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeAvailable																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Available property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeAvailable()
		{
			return true;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeEndDate																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the EndDate property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeEndDate()
		{
			return DateTime.Compare(mEndDate, DateTime.MinValue) != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeNextDate																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the NextDate property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeNextDate()
		{
			return DateTime.Compare(mNextDate, DateTime.MinValue) != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeRepetitionWeekOrdinal																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the RepetitionWeek property
		/// should be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeRepetitionWeekOrdinal()
		{
			return mRepetitionWeek != ScheduleWeekOrdinal.None;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeRepetitionRate																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the RepetitionRate property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeRepetitionRate()
		{
			return mRepetitionRate != ScheduleRepetitionRate.None;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeRepetitionWeekday																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the RepetitionWeekday property
		/// should be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeRepetitionWeekday()
		{
			return mRepetitionWeekday != ScheduleWeekDay.None;
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
		//*	StartDate																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="StartDate">StartDate</see>.
		/// </summary>
		private DateTime mStartDate = DateTime.MinValue;
		/// <summary>
		/// Get/Set the starting date and time on the defining occurrence of this
		/// item.
		/// </summary>
		[JsonProperty(Order = 15)]
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
		//*	ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this item.
		/// </summary>
		/// <returns>
		/// String representation of this Time Notation Item.
		/// </returns>
		public override string ToString()
		{
			System.DayOfWeek[] daysOfWeek;
			int index = 0;
			string[] weeks;
			StringBuilder builder = new StringBuilder();

			builder.Append("Start:   " + mStartDate.ToString() + ";\r\n");
			builder.Append("End:     " + mEndDate.ToString() + ";\r\n");
			builder.Append("Occur:   ");
			switch(mRepetitionRate)
			{
				case ScheduleRepetitionRate.None:
				case ScheduleRepetitionRate.Once:
					builder.Append("Once");
					break;
				case ScheduleRepetitionRate.Day:
					builder.Append("Once Each [");
					builder.Append(mActiveDayIndex.ToString());
					builder.Append("] Days");
					break;
				case ScheduleRepetitionRate.Weekday:
					builder.Append("Every Weekday");
					break;
				case ScheduleRepetitionRate.Weekly:
					builder.Append("Every [");
					builder.Append(mActiveWeekIndex.ToString());
					builder.Append("] Weeks on [");
					daysOfWeek = GetDaysOfWeek(mRepetitionWeekday);
					index = 0;
					foreach(System.DayOfWeek di in daysOfWeek)
					{
						if(index != 0)
						{
							builder.Append(",");
						}
						else
						{
							index++;
						}
						builder.Append(di.ToString().Substring(0, 3));
					}
					builder.Append("]");
					break;
				case ScheduleRepetitionRate.MonthDay:
					builder.Append("Every [");
					builder.Append(ActiveMonthIndex.ToString());
					builder.Append("] Months on Day [");
					builder.Append(ActiveDayIndex.ToString());
					builder.Append("]");
					break;
				case ScheduleRepetitionRate.Month:
					builder.Append("The [");
					weeks = GetWeeks(mRepetitionWeek, true);
					index = 0;
					foreach(string s in weeks)
					{
						if(index != 0)
						{
							builder.Append(",");
						}
						else
						{
							index++;
						}
						builder.Append(s);
					}
					builder.Append("] [");
					daysOfWeek = GetDaysOfWeek(mRepetitionWeekday);
					index = 0;
					foreach(System.DayOfWeek di in daysOfWeek)
					{
						if(index != 0)
						{
							builder.Append(",");
						}
						else
						{
							index++;
						}
						builder.Append(di.ToString().Substring(0, 3));
					}
					builder.Append("] of each [");
					builder.Append(ActiveMonthIndex.ToString());
					builder.Append("] Months");
					break;
				case ScheduleRepetitionRate.YearDay:
					builder.Append("Every [");
					builder.Append(((ScheduleMonth)mActiveMonthIndex).ToString());
					builder.Append("] [");
					builder.Append(ActiveDayIndex.ToString());
					builder.Append("]");
					break;
				case ScheduleRepetitionRate.Year:
					builder.Append("The [");
					weeks = GetWeeks(mRepetitionWeek, true);
					index = 0;
					foreach(string s in weeks)
					{
						if(index != 0)
						{
							builder.Append(",");
						}
						else
						{
							index++;
						}
						builder.Append(s);
					}
					builder.Append("] [");
					daysOfWeek = GetDaysOfWeek(mRepetitionWeekday);
					index = 0;
					foreach(System.DayOfWeek di in daysOfWeek)
					{
						if(index != 0)
						{
							builder.Append(",");
						}
						else
						{
							index++;
						}
						builder.Append(di.ToString().Substring(0, 3));
					}
					builder.Append("] of each [");
					builder.Append(((ScheduleMonth)mActiveMonthIndex).ToString());
					builder.Append("]");
					break;
			}
			builder.Append(";\r\n");
			//	Duration.
			builder.Append("Duration:");
			builder.Append(
				new DateRangeItem(mStartDate, mEndDate).Duration.ToString());
			builder.Append(";\r\n");
			//	Period.
			builder.Append("Period:  ");
			builder.Append(ActivePeriod.ToString() + " ");

			switch(ActivePeriod)
			{
				case ScheduleActivePeriod.Count:
					builder.Append("[");
					builder.Append(ActiveCount.ToString());
					builder.Append("] occurrences");
					break;
				case ScheduleActivePeriod.EndDate:
					builder.Append("Expire on [");
					builder.Append(ActiveEndDate.ToFileTime());
					builder.Append("]");
					break;
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
