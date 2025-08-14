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
	//*	public class ItemEventArgs<T>																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Specialized item event args in the data type of the item.
	/// </summary>
	/// <typeparam name="T">
	/// Type of item to represent in the event handler.
	/// </typeparam>
	public class ItemEventArgs<T> : EventArgs
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
		/// Create a new Instance of the ItemEventArgs Item.
		/// </summary>
		public ItemEventArgs()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ItemEventArgs Item.
		/// </summary>
		/// <param name="data">
		/// Reference to the item to pass.
		/// </param>
		public ItemEventArgs(T data)
		{
			mData = data;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Data																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Data">Data</see>.
		/// </summary>
		private T mData = default(T);
		/// <summary>
		/// Get/Set the native value of the item.
		/// </summary>
		public T Data
		{
			get { return mData; }
			set { mData = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Handled																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Handled">Handled</see>.
		/// </summary>
		private bool mHandled = false;
		/// <summary>
		/// Get/Set a value indicating whether this change has been handled.
		/// </summary>
		public bool Handled
		{
			get { return mHandled; }
			set { mHandled = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*


}
