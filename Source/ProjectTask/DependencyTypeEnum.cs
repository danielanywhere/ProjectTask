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
	//*	DependencyTypeEnum																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of the known project and task dependency types.
	/// </summary>
	public enum DependencyTypeEnum
	{
		/// <summary>
		/// No project dependency specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// Start this project a specified amount of time after starting the
		/// other project or task.
		/// </summary>
		StartAfter,
		/// <summary>
		/// Start this project on completion of the other project or task.
		/// </summary>
		StartOnCompletion,
		/// <summary>
		///	Start the target project or task when triggered by completion of the
		///	project or task. Used for industrial automation.
		/// </summary>
		TriggerFallingEdge,
		/// <summary>
		/// Start the target project or task when triggered by the start of the
		/// project or task. Used for industrial automation.
		/// </summary>
		TriggerRisingEdge
	}
	//*-------------------------------------------------------------------------*

}
