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
	//*	BudgetStatusEnum																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of known budget states.
	/// </summary>
	public enum BudgetStatusEnum
	{
		/// <summary>
		/// No budget status known or undefined.
		/// </summary>
		None = 0,
		/// <summary>
		/// The budget amount has been approved.
		/// </summary>
		Approved,
		/// <summary>
		/// The budget has been declined.
		/// </summary>
		Declined,
		/// <summary>
		/// The budget can be approved if significant reductions are made.
		/// </summary>
		Reduction,
		/// <summary>
		/// This item is waiting for a budget decision.
		/// </summary>
		Waiting
	}
	//*-------------------------------------------------------------------------*

}
