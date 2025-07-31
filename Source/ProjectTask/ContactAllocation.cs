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
	//*	ContactAllocationCollection																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ContactAllocationItem Items.
	/// </summary>
	public class ContactAllocationCollection : List<ContactAllocationItem>
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


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ContactAllocationItem																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about time allocations made to a contact.
	/// </summary>
	public class ContactAllocationItem
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
		//*	Contact																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Contact">Contact</see>.
		/// </summary>
		private ContactItem mContact = null;
		/// <summary>
		/// Get/Set a reference to the contact for which this allocation is
		/// prepared.
		/// </summary>
		public ContactItem Contact
		{
			get { return mContact; }
			set { mContact = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FreeBusyConnections																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="FreeBusyConnections">FreeBusyConnections</see>.
		/// </summary>
		private FreeBusyCollection mFreeBusyConnections =
			new FreeBusyCollection();
		/// <summary>
		/// Get a reference to the collection of free/busy clips for this contact.
		/// </summary>
		public FreeBusyCollection FreeBusyConnections
		{
			get { return mFreeBusyConnections; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Tasks																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Tasks">Tasks</see>.
		/// </summary>
		private List<TaskItem> mTasks = new List<TaskItem>();
		/// <summary>
		/// Get a reference to the collection of tasks associated with this
		/// contact.
		/// </summary>
		public List<TaskItem> Tasks
		{
			get { return mTasks; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TimeBlocks																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TimeBlocks">TimeBlocks</see>.
		/// </summary>
		private TimeBlockCollection mTimeBlocks = new TimeBlockCollection();
		/// <summary>
		/// Get a reference to the collection of time blocks being scheduled.
		/// </summary>
		public TimeBlockCollection TimeBlocks
		{
			get { return mTimeBlocks; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
