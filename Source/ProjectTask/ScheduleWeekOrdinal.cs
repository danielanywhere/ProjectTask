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
	//*	ScheduleWeekOrdinal																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of ordinal week-based scopes for month activities.
	/// </summary>
	[Flags]
	public enum ScheduleWeekOrdinal
	{
		/// <summary>
		/// No position.
		/// </summary>
		None = 0x00,
		/// <summary>
		/// First occurrence of that day within the month.
		/// </summary>
		First = 0x01,
		/// <summary>
		/// Second occurrence of that day within the month.
		/// </summary>
		Second = 0x02,
		/// <summary>
		/// Third occurrence of that day within the month.
		/// </summary>
		Third = 0x04,
		/// <summary>
		/// Fourth occurrence of that day within the month.
		/// </summary>
		Fourth = 0x08,
		/// <summary>
		/// Last occurrence of the specified day within the month.
		/// </summary>
		Last = 0x10
	}
	//*-------------------------------------------------------------------------*

}
