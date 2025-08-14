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
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	MultilineString																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// A list of single-line strings that can support a multi-line document.
	/// </summary>
	public class MultilineString : ChangeObjectCollection<string>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		/// <summary>
		/// The maximum line width allowed for lines with spaces.
		/// </summary>
		private const int mMaxLineWidth = 80;

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Implicit MultilineString = string																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Cast the string instance to a MultilineString.
		/// </summary>
		/// <param name="value">
		/// The string value to convert.
		/// </param>
		/// <returns>
		/// Reference to the new MultilineString representing the caller's value.
		/// </returns>
		public static implicit operator MultilineString(string value)
		{
			MultilineString result = new MultilineString();

			result.SetValue(value);
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Clone																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a deep copy of the caller's multiline string.
		/// </summary>
		/// <param name="multilineString">
		/// Reference to the multiline string to copy.
		/// </param>
		/// <returns>
		/// Reference to the newly created multiline string.
		/// </returns>
		public static MultilineString Clone(MultilineString multilineString)
		{
			MultilineString result = null;

			if(multilineString != null)
			{
				result = new MultilineString();
				foreach(string stringItem in multilineString)
				{
					result.Add(stringItem);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the value of this multiline string from the provided content.
		/// </summary>
		/// <param name="content">
		/// A line of text to convert to multiline format.
		/// </param>
		public void SetValue(string content)
		{
			List<string> lines = null;

			this.Clear();
			if(content?.Length > 0)
			{
				lines = ProjectTaskUtil.FromMultiLineString(content, mMaxLineWidth);
				this.AddRange(lines);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the aggregated version of this multi-line string.
		/// </summary>
		/// <returns>
		/// Single string representing all of the content of this multi-line
		/// string.
		/// </returns>
		/// <remarks>
		/// <para>
		/// In a multi-line string, at the end of each item, a space
		/// indicates that the content of the current line will be continued on
		/// the next. Otherwise, the next item represents the beginning of the
		/// next line.
		/// </para>
		/// <para>
		/// In this version, the last line is not terminated with a line-feed.
		/// </para>
		/// </remarks>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			int count = 0;
			int index = 0;

			if(this?.Count > 0)
			{
				count = this.Count;
				foreach(string item in this)
				{
					if(index + 1 < count && !item.EndsWith(" "))
					{
						builder.AppendLine(item);
					}
					else
					{
						builder.Append(item);
					}
					index++;
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
