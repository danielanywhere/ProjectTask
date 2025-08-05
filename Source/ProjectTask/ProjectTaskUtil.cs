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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	ProjectTaskUtil																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Utilities and functionality for the ProjectTask library.
	/// </summary>
	public class ProjectTaskUtil
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
		//*	ActiveProjectContext																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="ActiveProjectContext">ActiveProjectContext</see>.
		/// </summary>
		private static ProjectContext mActiveProjectContext = new ProjectContext();
		/// <summary>
		/// Get/Set a reference to the active file in this session.
		/// </summary>
		/// <remarks>
		/// Everything going on with items is contained here...
		/// </remarks>
		public static ProjectContext ActiveProjectContext
		{
			get { return mActiveProjectContext; }
			set { mActiveProjectContext = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AddRangeUnique																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a range of unique items in the source list to the target list.
		/// </summary>
		/// <typeparam name="T">
		/// Type to synchronize.
		/// </typeparam>
		/// <param name="sourceList">
		/// Reference to the list of source items to add when unique.
		/// </param>
		/// <param name="targetList">
		/// Reference to the list receiving the unique items.
		/// </param>
		public static void AddRangeUnique<T>(List<T> sourceList,
			List<T> targetList)
		{
			if(sourceList?.Count > 0 && targetList != null)
			{
				foreach(T item in sourceList)
				{
					if(!targetList.Contains(item))
					{
						targetList.Add(item);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Clear																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clear the contents of the specified string builder.
		/// </summary>
		/// <param name="builder">
		/// Reference to the string builder to clear.
		/// </param>
		public static void Clear(StringBuilder builder)
		{
			if(builder?.Length > 0)
			{
				builder.Remove(0, builder.Length);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CompactEqual																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the case-insensitive compact versions
		/// of the two strings are equal.
		/// </summary>
		/// <param name="leftValue">
		/// The left value to compare.
		/// </param>
		/// <param name="rightValue">
		/// The right value to compare.
		/// </param>
		/// <returns>
		/// True if the compact (without white space), case-insensitive values are
		/// equal. Otherwise, false.
		/// </returns>
		public static bool CompactEqual(string leftValue, string rightValue)
		{
			bool result = false;
			string valueL = "";
			string valueR = "";

			if(leftValue?.Length > 0 && rightValue?.Length > 0)
			{
				valueL = Regex.Replace(leftValue.ToLower(), @"\s+", "");
				valueR = Regex.Replace(rightValue.ToLower(), @"\s+", "");
				result = (valueL == valueR);
			}
			else if((leftValue?.Length == 0 && rightValue?.Length == 0) ||
				(leftValue == null && rightValue == null))
			{
				result = true;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CompactValue																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the base compact value representing the caller's string.
		/// </summary>
		/// <param name="value">
		/// The value to convert.
		/// </param>
		/// <returns>
		/// The base compact value of the supplied string.
		/// </returns>
		public static string CompactValue(string value)
		{
			string result = "";

			if(value?.Length > 0)
			{
				result = Regex.Replace(value.ToLower(), @"\s+", "");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ConvertRange																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Convert a value from one range to another.
		/// </summary>
		/// <param name="sourceStart">
		/// The starting value of the source range.
		/// </param>
		/// <param name="sourceEnd">
		/// The ending value of the source range.
		/// </param>
		/// <param name="targetStart">
		/// The starting value of the target range.
		/// </param>
		/// <param name="targetEnd">
		/// The ending value of the target range.
		/// </param>
		/// <param name="value">
		/// The value to convert.
		/// </param>
		/// <returns>
		/// The representation of the caller's source value in the target range.
		/// </returns>
		public static float ConvertRange(float sourceStart, float sourceEnd,
			float targetStart, float targetEnd, float value)
		{
			double newDiff = (double)targetEnd - (double)targetStart;
			double originalDiff = (double)sourceEnd - (double)sourceStart;
			double ratio = (originalDiff != 0d ? newDiff / originalDiff : 1d);

			return (float)(((double)value - (double)sourceStart) *
				ratio / originalDiff + (double)targetStart);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FromMultiLineString																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Convert a multiline string to a series of limited length lines.
		/// </summary>
		/// <param name="source">
		/// Reference to the source string that might have a single line of
		/// infinite length, or multiple lines.
		/// </param>
		/// <param name="maxLineLength">
		/// Maximum length of any individual line.
		/// </param>
		/// <returns>
		/// Collection of individual string segments, where one or more of those
		/// entries can contribute to a single line of text by including a space
		/// continuation at the end of the segment, or can be composed of multiple
		/// lines, by ending each segment with a non-space character.
		/// </returns>
		public static List<string> FromMultiLineString(string source,
			int maxLineLength = 60)
		{
			StringBuilder builder = new StringBuilder();
			string lineEnd = "";
			MatchCollection matches = null;
			List<string> result = new List<string>();
			string spaces = "";
			string text = "";

			if(source?.Length > 0)
			{
				matches = Regex.Matches(source, ResourceMain.rxWordSpace);
				foreach(Match matchItem in matches)
				{
					text = GetValue(matchItem, "word");
					spaces = GetValue(matchItem, "space");
					lineEnd = GetValue(matchItem, "lineend");
					if(text.Length > 0)
					{
						//	Text was found.
						if(builder.Length > 0 &&
							builder.Length + text.Length + spaces.Length > maxLineLength)
						{
							//	We can't add any more to this line. Create a new line.
							result.Add(builder.ToString());
							Clear(builder);
						}
						if(text.Length + spaces.Length > 0)
						{
							builder.Append(text + spaces);
						}
					}
					else if(lineEnd.Length > 0)
					{
						//	A line end was found.
						result.Add(builder.ToString());
						Clear(builder);
					}
				}
			}
			if(builder.Length > 0)
			{
				result.Add(builder.ToString());
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the specified group member in the provided match.
		/// </summary>
		/// <param name="match">
		/// Reference to the match to be inspected.
		/// </param>
		/// <param name="groupName">
		/// Name of the group for which the value will be found.
		/// </param>
		/// <returns>
		/// The value found in the specified group, if found. Otherwise, empty
		/// string.
		/// </returns>
		public static string GetValue(Match match, string groupName)
		{
			string result = "";

			if(match != null && match.Groups[groupName] != null &&
				match.Groups[groupName].Value != null)
			{
				result = match.Groups[groupName].Value;
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the value of the specified group member in a match found with
		/// the provided source and pattern.
		/// </summary>
		/// <param name="source">
		/// Source string to search.
		/// </param>
		/// <param name="pattern">
		/// Regular expression pattern to apply.
		/// </param>
		/// <param name="groupName">
		/// Name of the group for which the value will be found.
		/// </param>
		/// <returns>
		/// The value found in the specified group, if found. Otherwise, empty
		/// string.
		/// </returns>
		public static string GetValue(string source, string pattern,
			string groupName)
		{
			Match match = null;
			string result = "";

			if(source?.Length > 0 && pattern?.Length > 0 && groupName?.Length > 0)
			{
				match = Regex.Match(source, pattern);
				if(match.Success)
				{
					result = GetValue(match, groupName);
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the value associated with the specified property name of the
		/// provided pattern operation.
		/// </summary>
		/// <param name="type">
		/// Reference to the type for which the property value will be retrieved.
		/// </param>
		/// <param name="instance">
		/// Reference to the instance of the object from which to retrieve the
		/// property.
		/// </param>
		/// <param name="propertyName">
		/// Name of the property to retrieve.
		/// </param>
		/// <returns>
		/// Value of the specified property on the provided pattern operation
		/// item, if found. Otherwise, an empty string.
		/// </returns>
		public static object GetValue(Type type, object instance,
			string propertyName)
		{
			PropertyInfo property = null;
			object result = null;
			object item = null;

			if(type != null && propertyName?.Length > 0)
			{
				try
				{
					property = type.GetProperty(propertyName,
						System.Reflection.BindingFlags.GetProperty |
						System.Reflection.BindingFlags.IgnoreCase |
						System.Reflection.BindingFlags.Instance |
						System.Reflection.BindingFlags.Public);
					if(property != null)
					{
						item = property.GetValue(instance);
					}
					if(item != null)
					{
						result = item;
					}
				}
				catch { }
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsOpen																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified task is open.
		/// </summary>
		/// <param name="status">
		/// Reference to the task status to inspect.
		/// </param>
		/// <returns>
		/// Value indicating whether the status represents an open state.
		/// </returns>
		public static bool IsOpen(TaskStatusItem status)
		{
			bool result = false;

			if(status != null)
			{
				result =
					(status.TaskState != ProjectTaskStateEnum.Closed &&
					status.TaskState != ProjectTaskStateEnum.None);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsReferenceOnly																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified item is a reference
		/// to an object only, and not the definitive object reference.
		/// </summary>
		/// <param name="item">
		/// Reference to the object to inspect.
		/// </param>
		/// <returns>
		/// True if the item is a placeholder for the specified object only, and
		/// not the actual object reference.
		/// </returns>
		public static bool IsReferenceOnly(BaseItem item)
		{
			bool result = false;

			result = (item != null &&
				item.ExtendedProperties.Exists(x =>
					x.Name == "ReferenceOnly" && x.Value == "1"));
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return a value indicating whether the specified item is a reference
		/// to an object only, and not the definitive object reference.
		/// </summary>
		/// <param name="item">
		/// Reference to the object to inspect.
		/// </param>
		/// <returns>
		/// True if the item is a placeholder for the specified object only, and
		/// not the actual object reference.
		/// </returns>
		public static bool IsReferenceOnly(IItem item)
		{
			bool result = false;

			result = (item != null &&
				item.ExtendedProperties.Exists(x =>
					x.Name == "ReferenceOnly" && x.Value == "1"));
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsTaskType																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified task is of a task type.
		/// </summary>
		/// <param name="task">
		/// Reference to the task to inspect.
		/// </param>
		/// <returns>
		/// True if the provided item has a task type. Otherwise, false.
		/// </returns>
		public static bool IsTaskType(TaskItem task)
		{
			bool result = false;

			result = (task != null &&
				task.ItemType?.TaskType == ProjectTaskTypeEnum.Task);
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetPrecision																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the decimal precision on the provided number and return the result.
		/// </summary>
		/// <param name="number">
		/// The number to be adjusted.
		/// </param>
		/// <param name="decimalPlaces">
		/// Number of decimal places precision to allow on the provided number.
		/// </param>
		public static double SetPrecision(double number, int decimalPlaces)
		{
			string format = "0";
			double result = 0d;

			if(number != 0d && decimalPlaces > -1)
			{
				if(decimalPlaces > 0)
				{
					format += "." + new string('0', decimalPlaces);
				}
				result = ToDouble(number.ToString(format));
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Set the decimal precision on the provided number and return the result.
		/// </summary>
		/// <param name="number">
		/// The number to be adjusted.
		/// </param>
		/// <param name="decimalPlaces">
		/// Number of decimal places precision to allow on the provided number.
		/// </param>
		public static float SetPrecision(float number, int decimalPlaces)
		{
			string format = "0";
			float result = 0f;

			if(number != 0d && decimalPlaces > -1)
			{
				if(decimalPlaces > 0)
				{
					format += "." + new string('0', decimalPlaces);
				}
				result = ToFloat(number.ToString(format));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToDouble																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Double-precision floating point value. 0 if not convertible.
		/// </returns>
		public static double ToDouble(object value)
		{
			double result = 0d;
			if(value != null)
			{
				result = ToDouble(value.ToString());
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Double-precision floating point value. 0 if not convertible.
		/// </returns>
		public static double ToDouble(string value)
		{
			double result = 0d;
			try
			{
				result = double.Parse(value);
			}
			catch { }
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToFloat																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Single-precision floating point value. 0 if not convertible.
		/// </returns>
		public static float ToFloat(object value)
		{
			float result = 0f;
			if(value != null)
			{
				result = ToFloat(value.ToString());
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Single-precision floating point value. 0 if not convertible.
		/// </returns>
		public static float ToFloat(string value)
		{
			float result = 0f;
			try
			{
				result = float.Parse(value);
			}
			catch { }
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToInt																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Int32 value. 0 if not convertible.
		/// </returns>
		public static int ToInt(object value)
		{
			int result = 0;
			if(value != null)
			{
				result = ToInt(value.ToString());
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Int32 value. 0 if not convertible.
		/// </returns>
		public static int ToInt(string value)
		{
			int result = 0;
			try
			{
				result = int.Parse(value);
			}
			catch { }
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToTimeSpan																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the time span representation of the caller's decimal hours.
		/// </summary>
		/// <param name="absoluteHours">
		/// Absolute floating point decimal hours to convert.
		/// </param>
		/// <returns>
		/// Time span representing the caller's absolute hours.
		/// </returns>
		public static TimeSpan ToTimeSpan(float absoluteHours)
		{
			int days = 0;
			int hours = 0;
			int minutes = 0;
			float remainder = absoluteHours;
			TimeSpan result = TimeSpan.Zero;
			int seconds = 0;

			if(absoluteHours != 0f)
			{
				days = (int)(remainder / 24f);
				remainder -= (float)days * 24f;
				hours = (int)remainder;
				remainder -= (float)hours;
				minutes = (int)(remainder * 60f);
				remainder -= (float)minutes / 60f;
				seconds = (int)(remainder * 3600f);
				result = new TimeSpan(days, hours, minutes, seconds);
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Using a relative date and time storage value, return the 0-based time
		/// span.
		/// </summary>
		/// <param name="relativeDateTime">
		/// Relative date and time to be converted to absolute time.
		/// </param>
		/// <returns>
		/// Time span representing an absolute passage of time.
		/// </returns>
		public static TimeSpan ToTimeSpan(DateTime relativeDateTime)
		{
			return relativeDateTime - DateTime.MinValue;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
