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

using Newtonsoft.Json;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	TicketConverter																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Serializes and deserializes a property reference using its Ticket.
	/// </summary>
	public class TicketConverter<T> : JsonConverter where T : new()
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
		//* CanConvert																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the provided object type can be
		/// converted.
		/// </summary>
		/// <param name="objectType">
		/// Reference to the type of object to be tested.
		/// </param>
		/// <returns>
		/// True if the object can be converted. Otherwise, false.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(T);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ReadJson																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Read JSON content from the file.
		/// </summary>
		/// <param name="reader">
		/// Reference to the active JSON reader.
		/// </param>
		/// <param name="objectType">
		/// Reference to the object type to be read. 
		/// </param>
		/// <param name="existingValue">
		/// Reference to the existing value in the target.
		/// </param>
		/// <param name="serializer">
		/// Reference to the active serializer.
		/// </param>
		/// <returns>
		/// Reference to the specified 
		/// </returns>
		/// <exception cref="Exception">
		/// </exception>
		public override object ReadJson(JsonReader reader, Type objectType,
			object existingValue, JsonSerializer serializer)
		{
			PropertyInfo property = null;
			T result = default(T);

			if(reader != null && reader.TokenType != JsonToken.Null &&
				serializer != null)
			{
				if(reader.TokenType != JsonToken.String)
				{
					throw new Exception(
						$"Expecting a string value for {typeof(T).Name} Ticket.");
				}
				else
				{
					//	Generate the object as a reference until everything is loaded.
					result = new T();
					property = typeof(T).GetProperty("ItemTicket");
					property?.SetValue(result, (string)reader.Value);
					property = typeof(T).GetProperty("ExtendedProperties");
					property?.PropertyType.
						GetMethod("SetValue")?.
							Invoke(property.GetValue(result),
							new object[] { "ReferenceOnly", 1 });
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* WriteJson																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Write the JSON data for this value.
		/// </summary>
		/// <param name="writer">
		/// Reference to the JSON writer.
		/// </param>
		/// <param name="value">
		/// Reference to the value to be written.
		/// </param>
		/// <param name="serializer">
		/// Reference to the active serializer.
		/// </param>
		public override void WriteJson(JsonWriter writer, object value,
			JsonSerializer serializer)
		{
			PropertyInfo property = null;
			string ticketValue = "";

			if(writer != null && serializer != null)
			{
				if(value != null)
				{
					property = typeof(T).GetProperty("ItemTicket");
					ticketValue = property?.GetValue(value)?.ToString();
					writer.WriteValue(ticketValue);
				}
				else
				{
					writer.WriteNull();
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
