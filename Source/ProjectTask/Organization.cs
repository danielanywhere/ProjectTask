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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using static ProjectTask.ProjectTaskUtil;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	OrganizationCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of OrganizationItem Items.
	/// </summary>
	public class OrganizationCollection :
		ChangeObjectCollection<OrganizationItem>
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
	//*	OrganizationItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a single organization.
	/// </summary>
	public class OrganizationItem : BaseItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* mDefaultContact_PropertyChanged																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property has changed on the DefaultContact object.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mDefaultContact_PropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("DefaultContact");
			}
		}
		//*-----------------------------------------------------------------------*

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
		/// Create a new instance of the OrganizationItem item.
		/// </summary>
		public OrganizationItem()
		{
			ItemId = NextItemId++;
			ActiveProjectContext.Organizations.Add(this);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CityName																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="CityName">CityName</see>.
		/// </summary>
		private string mCityName = "";
		/// <summary>
		/// Get/Set the name of the city in which the organization is located.
		/// </summary>
		[JsonProperty(Order = 6)]
		public string CityName
		{
			get { return mCityName; }
			set
			{
				bool bChanged = (mCityName != value);
				mCityName = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DefaultContact																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="DefaultContact">DefaultContact</see>.
		/// </summary>
		private ContactItem mDefaultContact = null;
		/// <summary>
		/// Get/Set a reference to the default contact for this organization.
		/// </summary>
		[JsonConverter(typeof(TicketConverter<ContactItem>))]
		[JsonProperty(Order = 4)]
		public ContactItem DefaultContact
		{
			get { return mDefaultContact; }
			set
			{
				bool bChanged = (mDefaultContact != value);
				if(bChanged && mDefaultContact != null)
				{
					mDefaultContact.PropertyChanged -= mDefaultContact_PropertyChanged;
				}
				mDefaultContact = value;
				if(bChanged)
				{
					if(mDefaultContact != null)
					{
						mDefaultContact.PropertyChanged += mDefaultContact_PropertyChanged;
					}
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DisplayName																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="DisplayName">DisplayName</see>.
		/// </summary>
		private string mDisplayName = "";
		/// <summary>
		/// Get/Set the user-readable name of the organization.
		/// </summary>
		[JsonProperty(Order = 2)]
		public string DisplayName
		{
			get { return mDisplayName; }
			set
			{
				bool bChanged = (mDisplayName != value);
				mDisplayName = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	EmailAddress																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="EmailAddress">EmailAddress</see>.
		/// </summary>
		private string mEmailAddress = "";
		/// <summary>
		/// Get/Set the default email address of the organization.
		/// </summary>
		[JsonProperty(Order = 12)]
		public string EmailAddress
		{
			get { return mEmailAddress; }
			set
			{
				bool bChanged = (mEmailAddress != value);
				mEmailAddress = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProvinceName																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ProvinceName">ProvinceName</see>.
		/// </summary>
		private string mProvinceName = "";
		/// <summary>
		/// Get/Set the name of the province in which the organization is located.
		/// </summary>
		[JsonProperty(Order = 8)]
		public string ProvinceName
		{
			get { return mProvinceName; }
			set
			{
				bool bChanged = (mProvinceName != value);
				mProvinceName = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RelationType																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="RelationType">RelationType</see>.
		/// </summary>
		private OrganizationRelationType mRelationType =
			OrganizationRelationType.None;
		/// <summary>
		/// Get/Set the type of relation that organization has to this.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty(Order = 3)]
		public OrganizationRelationType RelationType
		{
			get { return mRelationType; }
			set
			{
				bool bChanged = (mRelationType != value);
				mRelationType = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeCityName																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the CityName property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeCityName()
		{
			return mCityName?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeDefaultContact																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the DefaultContact property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeDefaultContact()
		{
			return mDefaultContact != null;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeDisplayName																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the DisplayName property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeDisplayName()
		{
			return mDisplayName?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeEmailAddress																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the EmailAddress property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeEmailAddress()
		{
			return mEmailAddress?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeProvinceName																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ProvinceName property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeProvinceName()
		{
			return mProvinceName?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeRelationType																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the RelationType property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeRelationType()
		{
			return mRelationType != OrganizationRelationType.None;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeStateName																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the StateName property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeStateName()
		{
			return mStateName?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeStreetAddress																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the StreetAddress property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeStreetAddress()
		{
			return mStreetAddress?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeTelephoneNumber																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the TelephoneNumber property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeTelephoneNumber()
		{
			return mTelephoneNumber?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeTelephoneType																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the TelephoneType property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeTelephoneType()
		{
			return mTelephoneType != TelephoneTypeEnum.None;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeWebsiteAddress																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the WebsiteAddress property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeWebsiteAddress()
		{
			return mWebsiteAddress?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShouldSerializeZipCode																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the ZipCode property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeZipCode()
		{
			return mZipCode?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StateName																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="StateName">StateName</see>.
		/// </summary>
		private string mStateName = "";
		/// <summary>
		/// Get/Set the name of the state in which the organization is located.
		/// </summary>
		[JsonProperty(Order = 7)]
		public string StateName
		{
			get { return mStateName; }
			set
			{
				bool bChanged = (mStateName != value);
				mStateName = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StreetAddress																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="StreetAddress">StreetAddress</see>.
		/// </summary>
		private string mStreetAddress = "";
		/// <summary>
		/// Get/Set the street address of the organization.
		/// </summary>
		[JsonProperty(Order = 5)]
		public string StreetAddress
		{
			get { return mStreetAddress; }
			set
			{
				bool bChanged = (mStreetAddress != value);
				mStreetAddress = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TelephoneNumber																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TelephoneNumber">TelephoneNumber</see>.
		/// </summary>
		private string mTelephoneNumber = "";
		/// <summary>
		/// Get/Set the organization's primary telephone number.
		/// </summary>
		[JsonProperty(Order = 10)]
		public string TelephoneNumber
		{
			get { return mTelephoneNumber; }
			set
			{
				bool bChanged = (mTelephoneNumber != value);
				mTelephoneNumber = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TelephoneType																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TelephoneType">TelephoneType</see>.
		/// </summary>
		private TelephoneTypeEnum mTelephoneType = TelephoneTypeEnum.None;
		/// <summary>
		/// Get/Set the organization's primary telephone type.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty(Order = 11)]
		public TelephoneTypeEnum TelephoneType
		{
			get { return mTelephoneType; }
			set
			{
				bool bChanged = (mTelephoneType != value);
				mTelephoneType = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WebsiteAddress																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="WebsiteAddress">WebsiteAddress</see>.
		/// </summary>
		private string mWebsiteAddress = "";
		/// <summary>
		/// Get/Set the default website address of the organization.
		/// </summary>
		[JsonProperty(Order = 13)]
		public string WebsiteAddress
		{
			get { return mWebsiteAddress; }
			set
			{
				bool bChanged = (mWebsiteAddress != value);
				mWebsiteAddress = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ZipCode																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ZipCode">ZipCode</see>.
		/// </summary>
		private string mZipCode = "";
		/// <summary>
		/// Get/Set the zip code of the organization's location.
		/// </summary>
		[JsonProperty(Order = 9)]
		public string ZipCode
		{
			get { return mZipCode; }
			set
			{
				bool bChanged = (mZipCode != value);
				mZipCode = value;
				if(bChanged)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
