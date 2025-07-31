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
using System.Linq;
using System.Text;

using static ProjectTask.ProjectTaskUtil;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	PropertyCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of PropertyItem Items.
	/// </summary>
	public class PropertyCollection : ChangeObjectCollection<PropertyItem>
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
		//* Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add the numeric equivalent of the specified property value to the
		/// supplied value and store the result in the specified property.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property to update.
		/// </param>
		/// <param name="value">
		/// Value to add to the pre-existing property value.
		/// </param>
		public void Add(string propertyName, double value)
		{
			if(propertyName?.Length > 0)
			{
				SetValue(propertyName,
					(GetDoubleValue(propertyName) +
					SetPrecision(value, mPrecision)).ToString());
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Add the numeric equivalent of the specified property value to the
		/// supplied value and store the result in the specified property.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property to update.
		/// </param>
		/// <param name="value">
		/// Value to add to the pre-existing property value.
		/// </param>
		public void Add(string propertyName, float value)
		{
			if(propertyName?.Length > 0)
			{
				SetValue(propertyName,
					(GetFloatValue(propertyName) +
					SetPrecision(value, mPrecision)).ToString());
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Add a numeric value to the specified property.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property to load.
		/// </param>
		/// <param name="value">
		/// Value to add to the specified property.
		/// </param>
		public void Add(string propertyName, int value)
		{
			if(propertyName?.Length > 0)
			{
				SetValue(propertyName,
					(GetIntValue(propertyName) + value).ToString());
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetDoubleValue																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the double-precision floating-point value of the specified
		/// property.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property to retrieve.
		/// </param>
		/// <returns>
		/// Value of the specified property, if found. Otherwise, 0.
		/// </returns>
		public double GetDoubleValue(string propertyName)
		{
			string compactName = "";
			PropertyItem property = null;
			double result = 0d;

			if(propertyName?.Length > 0)
			{
				compactName = CompactValue(propertyName);
				property = this.FirstOrDefault(x =>
					CompactValue(x.Name) == compactName);
				if(property != null && property.Value?.Length > 0)
				{
					result = SetPrecision(ToDouble(property.Value), mPrecision);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetFloatValue																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the single-precision floating-point value of the specified
		/// property.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property to retrieve.
		/// </param>
		/// <returns>
		/// Value of the specified property, if found. Otherwise, 0.
		/// </returns>
		public float GetFloatValue(string propertyName)
		{
			string compactName = "";
			PropertyItem property = null;
			float result = 0f;

			if(propertyName?.Length > 0)
			{
				compactName = CompactValue(propertyName);
				property = this.FirstOrDefault(x =>
					CompactValue(x.Name) == compactName);
				if(property != null && property.Value?.Length > 0)
				{
					result = SetPrecision(ToFloat(property.Value), mPrecision);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetIntValue																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the integer value of the specified property.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property to retrieve.
		/// </param>
		/// <returns>
		/// Value of the specified property, if found. Otherwise, 0.
		/// </returns>
		public int GetIntValue(string propertyName)
		{
			string compactName = "";
			PropertyItem property = null;
			int result = 0;

			if(propertyName?.Length > 0)
			{
				compactName = CompactValue(propertyName);
				property = this.FirstOrDefault(x =>
					CompactValue(x.Name) == compactName);
				if(property != null && property.Value?.Length > 0)
				{
					result = ToInt(property.Value);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetStringValue																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string value of the specified property.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property to retrieve.
		/// </param>
		/// <returns>
		/// Value of the specified property, if found. Otherwise, a blank string.
		/// </returns>
		public string GetStringValue(string propertyName)
		{
			string compactName = "";
			PropertyItem property = null;
			string result = "";

			if(propertyName?.Length > 0)
			{
				compactName = CompactValue(propertyName);
				property = this.FirstOrDefault(x =>
					CompactValue(x.Name) == compactName);
				if(property != null && property.Value?.Length > 0)
				{
					result = property.Value;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Precision																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Precision">Precision</see>.
		/// </summary>
		private int mPrecision = 3;
		/// <summary>
		/// Get/Set the precision, in decimal places, of values in this collection.
		/// </summary>
		public int Precision
		{
			get { return mPrecision; }
			set { mPrecision = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the value of the specified property, creating it if necessary.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property to set.
		/// </param>
		/// <param name="value">
		/// Value to place on the property.
		/// </param>
		public void SetValue(string propertyName, double value)
		{
			SetValue(propertyName,
				SetPrecision(value, mPrecision).ToString());
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Set the value of the specified property, creating it if necessary.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property to set.
		/// </param>
		/// <param name="value">
		/// Value to place on the property.
		/// </param>
		public void SetValue(string propertyName, float value)
		{
			SetValue(propertyName,
				SetPrecision(value, mPrecision).ToString());
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Set the value of the specified property, creating it if necessary.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property to set.
		/// </param>
		/// <param name="value">
		/// Value to place on the property.
		/// </param>
		public void SetValue(string propertyName, int value)
		{
			SetValue(propertyName, value.ToString());
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Set the value of the specified property, creating one if necessary.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property to set.
		/// </param>
		/// <param name="value">
		/// Value to place on the property.
		/// </param>
		public void SetValue(string propertyName, string value)
		{
			string compactName = "";
			PropertyItem property = null;

			if(propertyName?.Length > 0 && value != null)
			{
				compactName = CompactValue(propertyName);
				property = this.FirstOrDefault(x =>
					CompactValue(x.Name) == compactName);
				if(property == null)
				{
					property = new PropertyItem()
					{
						Name = propertyName
					};
					this.Add(property);
				}
				property.Value = value;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Subtract																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Subtract a numeric value from the specified property.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property to load.
		/// </param>
		/// <param name="value">
		/// Value to subtract from the specified property.
		/// </param>
		public void Subtract(string propertyName, int value)
		{
			if(propertyName?.Length > 0)
			{
				SetValue(propertyName,
					(GetIntValue(propertyName) - value).ToString());
			}
		}
		//*-----------------------------------------------------------------------*



	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	PropertyItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual property definition.
	/// </summary>
	public class PropertyItem : ChangeObjectItem
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
		//* Clone																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a deep copy of the specified property item.
		/// </summary>
		/// <param name="property">
		/// Reference to the property to be cloned.
		/// </param>
		/// <returns>
		/// Reference to a newly created clone of the caller's property.
		/// </returns>
		public static PropertyItem Clone(PropertyItem property)
		{
			PropertyItem result = null;

			if(property != null)
			{
				result = new PropertyItem()
				{
					mName = property.mName,
					mValue = property.mValue
				};
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of the property.
		/// </summary>
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		private string mValue = "";
		/// <summary>
		/// Get/Set the value of the property.
		/// </summary>
		public string Value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
