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
	//*	ScheduleActivePeriod																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of occurrence limits for repeated times.
	/// </summary>
	/// <remarks>
	/// Any repeated time notation can occur indefinitely, or within a
	/// pre-defined limit of occurrences.
	/// </remarks>
	public enum ScheduleActivePeriod
	{
		/// <summary>
		/// The period is never active.
		/// </summary>
		None,
		/// <summary>
		/// The event has no end date.
		/// </summary>
		Indefinite,
		/// <summary>
		/// The event ends after X occurrences.
		/// </summary>
		Count,
		/// <summary>
		/// The event ends on Date X.
		/// </summary>
		EndDate
	}
	//*-------------------------------------------------------------------------*

}
