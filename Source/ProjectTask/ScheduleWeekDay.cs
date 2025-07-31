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
	//*	ScheduleWeekDay																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Combinatorial enumeration of days of the week.
	/// </summary>
	[Flags]
	public enum ScheduleWeekDay
	{
		/// <summary>
		/// No Day specified.
		/// </summary>
		None = 0x00,
		/// <summary>
		/// Sunday.
		/// </summary>
		Sunday = 0x01,
		/// <summary>
		/// Monday.
		/// </summary>
		Monday = 0x02,
		/// <summary>
		/// Tuesday.
		/// </summary>
		Tuesday = 0x04,
		/// <summary>
		/// Wednesday.
		/// </summary>
		Wednesday = 0x08,
		/// <summary>
		/// Thursday.
		/// </summary>
		Thursday = 0x10,
		/// <summary>
		/// Friday.
		/// </summary>
		Friday = 0x20,
		/// <summary>
		/// Saturday.
		/// </summary>
		Saturday = 0x40
	}
	//*-------------------------------------------------------------------------*

}
