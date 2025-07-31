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
	//*	ScheduleRepetitionRate																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of scopes of repetition.
	/// </summary>
	public enum ScheduleRepetitionRate
	{
		/// <summary>
		/// The event never occurs.
		/// </summary>
		None,
		/// <summary>
		/// The event occurs once on the specified Date and Time.
		/// </summary>
		Once,
		/// <summary>
		/// The event occurs once every X days. Note. Each day = every [1] days.
		/// </summary>
		Day,
		/// <summary>
		/// The event occurs each weekday.
		/// </summary>
		Weekday,
		/// <summary>
		/// The event occurs once every X weeks on { Sunday | Monday | Tuesday |
		/// Wednesday | Thursday | Friday | Saturday }.
		/// </summary>
		Weekly,
		/// <summary>
		/// The event occurs on the X day of each Y months.
		/// </summary>
		MonthDay,
		/// <summary>
		/// The event occurs on the { First | Second | Third | Fourth | Last }
		/// { Sunday | Monday | Tuesday | Wednesday | Thursday | Friday |
		/// Saturday } of each X months.
		/// </summary>
		Month,
		/// <summary>
		/// Event occurs each {Month}{Day}.
		/// </summary>
		YearDay,
		/// <summary>
		/// Event occurs on the { First | Second | Third | Fourth | Last }
		/// { Sunday | Monday | Tuesday | Wednesday | Thursday | Friday |
		/// Saturday } of each {Month}.
		/// </summary>
		Year
	}
	//*-------------------------------------------------------------------------*

}
