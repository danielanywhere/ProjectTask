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

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	TimeNotationDateCollection																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of TimeNotationDateItem Items.
	/// </summary>
	public class TimeNotationDateCollection : List<TimeNotationDateItem>
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
		/// <param name="timeNotation">
		/// Reference to the time notation to associate.
		/// </param>
		/// <param name="dateRange">
		/// Reference to the date range being tracked.
		/// </param>
		/// <returns>
		/// Reference to a time notation / date range association.
		/// </returns>
		public TimeNotationDateItem Add(TimeNotationItem timeNotation,
			DateRangeItem dateRange)
		{
			TimeNotationDateItem result = new TimeNotationDateItem
			{
				TimeNotation = timeNotation,
				DateRange = DateRangeItem.Clone(dateRange)
			};
			this.Add(result);

			return result;
		}
		//*-----------------------------------------------------------------------*



	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	TimeNotationDateItem																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Tracking information for a time notation and an individual resolved date
	/// range.
	/// </summary>
	public class TimeNotationDateItem
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
		//*	DateRange																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="DateRange">DateRange</see>.
		/// </summary>
		private DateRangeItem mDateRange = null;
		/// <summary>
		/// Get/Set a reference to the captured date range.
		/// </summary>
		public DateRangeItem DateRange
		{
			get { return mDateRange; }
			set { mDateRange = value; }
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
		/// Get/Set a reference to the time notation being tracked.
		/// </summary>
		public TimeNotationItem TimeNotation
		{
			get { return mTimeNotation; }
			set { mTimeNotation = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
