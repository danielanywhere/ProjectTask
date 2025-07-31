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
	//*	BudgetTypeEnum																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of known budget resource types.
	/// </summary>
	[Flags]
	public enum BudgetTypeEnum
	{
		/// <summary>
		/// No budget type defined or unknown.
		/// </summary>
		None =					0x00,
		/// <summary>
		/// Internal time is being budgeted.
		/// </summary>
		InternalTime =	0x01,
		/// <summary>
		/// External time is being budgeted.
		/// </summary>
		ExternalTime =	0x02,
		/// <summary>
		/// A specific date budget is being approved.
		/// </summary>
		Date =					0x04,
		/// <summary>
		/// Some amount of money is needed for this element.
		/// </summary>
		Money =					0x08,
		/// <summary>
		/// Facilities are needed for this element.
		/// </summary>
		Facilities =		0x10,
		/// <summary>
		/// Equipment is needed for this element.
		/// </summary>
		Equipment =			0x20
	}
	//*-------------------------------------------------------------------------*

}
