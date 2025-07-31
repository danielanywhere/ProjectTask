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
	//*	OrganizationRelationType																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of types of relationship another organization might have to
	/// this one.
	/// </summary>
	[Flags]
	public enum OrganizationRelationType
	{
		/// <summary>
		/// No relation type defined or unknown.
		/// </summary>
		None =				0x0000,
		/// <summary>
		/// The organization identified is this organization.
		/// </summary>
		This =				0x0001,
		/// <summary>
		/// Vendor organization.
		/// </summary>
		Vendor =			0x0002,
		/// <summary>
		/// Customer organization.
		/// </summary>
		Customer =		0x0004,
		/// <summary>
		/// Partner orgnization.
		/// </summary>
		Partner =			0x0008,
		/// <summary>
		/// Parent organization.
		/// </summary>
		Parent =			0x0010,
		/// <summary>
		/// Subsidiary organization.
		/// </summary>
		Subsidiary =	0x0020,
		/// <summary>
		/// Contract work.
		/// </summary>
		Contract =		0x0040,
		/// <summary>
		/// Service work.
		/// </summary>
		Service =			0x0080,
		/// <summary>
		/// Product sales.
		/// </summary>
		Product =			0x0100
	}
	//*-------------------------------------------------------------------------*

}
