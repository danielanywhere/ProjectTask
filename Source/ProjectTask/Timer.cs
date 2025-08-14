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
	//*	TimerCollection																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of TimerItem Items.
	/// </summary>
	public class TimerCollection : ChangeObjectCollection<TimerItem>
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
		//*	Sum																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the sum of hours elapsed in the caller's collection of timers.
		/// </summary>
		/// <param name="timers">
		/// Reference to the list of timers whose time will be summed.
		/// </param>
		/// <returns>
		/// The sum of the provided timers, in decimal hours.
		/// </returns>
		public static float Sum(List<TimerItem> timers)
		{
			float result = 0f;

			if(timers?.Count > 0)
			{
				foreach(TimerItem timerItem in timers)
				{
					result = (float)(timerItem.EndTime - timerItem.StartTime).TotalHours;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*



	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	TimerItem																																*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual timer.
	/// </summary>
	public class TimerItem : BaseItem
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
		/// Create a new instance of the TimerItem item.
		/// </summary>
		public TimerItem()
		{
			ItemId = NextItemId++;
			ActiveProjectContext.Timers.Add(this);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	EndTime																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="EndTime">EndTime</see>.
		/// </summary>
		private DateTime mEndTime = DateTime.MinValue;
		/// <summary>
		/// Get/Set the date and time upon which the timer was stopped.
		/// </summary>
		[JsonProperty(Order = 3)]
		public DateTime EndTime
		{
			get { return mEndTime; }
			set
			{
				bool bChanged = (mEndTime.CompareTo(value) != 0);
				mEndTime = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//	TODO: GetTimeSpan
		//*-----------------------------------------------------------------------*
		//*	GetTimeSpan																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the time elapsed from start to end or since the timer was
		/// started.
		/// </summary>
		/// <param name="timer">
		/// Reference to the timer to inspect.
		/// </param>
		/// <returns>
		/// Reference to a time span reflecting the time elapsed from start to end,
		/// if the timer has been stopped, or since the start, if the timer is
		/// still running.
		/// </returns>
		public static TimeSpan GetTimeSpan(TimerItem timer)
		{
			return new TimeSpan();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	IsRunning																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="IsRunning">IsRunning</see>.
		/// </summary>
		private bool mIsRunning = false;
		/// <summary>
		/// Get/Set a value indicating whether the timer is running.
		/// </summary>
		[JsonProperty(Order = 4)]
		public bool IsRunning
		{
			get { return mIsRunning; }
			set
			{
				bool bChanged = (mIsRunning != value);
				mIsRunning = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeEndTime																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the EndTime property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeEndTime()
		{
			return DateTime.Compare(mEndTime, DateTime.MinValue) != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeIsRunning																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the IsRunning property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeIsRunning()
		{
			return true;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeStartTime																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the StartTime property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeStartTime()
		{
			return DateTime.Compare(mStartTime, DateTime.MinValue) != 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StartTime																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="StartTime">StartTime</see>.
		/// </summary>
		private DateTime mStartTime = DateTime.MinValue;
		/// <summary>
		/// Get/Set the date and time upon which the timer was started.
		/// </summary>
		[JsonProperty(Order = 2)]
		public DateTime StartTime
		{
			get { return mStartTime; }
			set
			{
				bool bChanged = (mStartTime.CompareTo(value) != 0);
				mStartTime = value;
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
