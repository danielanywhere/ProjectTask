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
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using static ProjectTask.ProjectTaskUtil;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	ContactCollection																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ContactItem Items.
	/// </summary>
	public class ContactCollection : ChangeObjectCollection<ContactItem>
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
		/// Add one or more contacts using a simple tuple approach.
		/// </summary>
		/// <param name="entries">
		/// One or more displayName, emailAddress tuples.
		/// </param>
		/// <returns>
		/// Reference to a list of items that have been added to the collection.
		/// </returns>
		/// <remarks>
		/// To use this method, call it similarly to:
		/// <code>project.Contacts.Add(
		/// (&quot;Pickle Featherstone&quot;, 
		/// &quot;pickle.featherstone@quirkymail.com&quot;),
		/// (&quot;Sassy Bumbleshoe&quot;, 
		/// &quot;sassy.bumbleshoe@buzzmail.io&quot;));
		/// </code>
		/// </remarks>
		public List<ContactItem> Add(
			params (string displayName, string emailAddress)[] entries)
		{
			ContactItem contact = null;
			List<ContactItem> result = new List<ContactItem>();

			if(entries?.Length > 0)
			{
				foreach((string displayName, string emailAddress) item in
					entries)
				{
					if(item.displayName?.Length > 0)
					{
						contact = new ContactItem()
						{
							DisplayName = item.displayName,
							EmailAddress = item.emailAddress
						};
						this.Add(contact);
						result.Add(contact);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	AssignSupervisor																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Assign a supervisor to the provided report.
		/// </summary>
		/// <param name="report">
		/// Name of the report whose contact record will be updated.
		/// </param>
		/// <param name="supervisor">
		/// Name of the supervisor to assign.
		/// </param>
		/// <returns>
		/// True if the operation was successful. Otherwise, false.
		/// </returns>
		/// <remarks>
		/// You can remove all spaces from the report and supervisor names, or
		/// can specify any part of either display name that is unique to make
		/// an attachment.
		/// </remarks>
		public bool AssignSupervisor(string report, string supervisor)
		{
			List<ContactItem> items = null;
			ContactItem reportItem = null;
			string reportL = "";
			ContactItem superItem = null;
			string superL = "";
			bool result = false;

			if(report?.Length > 0 && supervisor?.Length > 0)
			{
				reportL = Regex.Replace(report.ToLower(), "\\s*", "");
				superL = Regex.Replace(supervisor.ToLower(), "\\s*", "");
				//	First pass.
				items = this.FindAll(x =>
					Regex.Replace(x.DisplayName.ToLower(), "\\s*", "").
						Contains(reportL));
				if(items.Count == 1)
				{
					reportItem = items[0];
				}
				if(reportItem != null)
				{
					items = this.FindAll(x =>
						Regex.Replace(x.DisplayName.ToLower(), "\\s*", "").
							Contains(superL));
					if(items.Count == 1)
					{
						superItem = items[0];
					}
				}
				if(superItem != null)
				{
					reportItem.SupervisorContact = superItem;
					result = true;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ContactItem																															*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual contact.
	/// </summary>
	public class ContactItem : BaseItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* mDefaultOrganization_PropertyChanged																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property has changed on the DefaultOrganization object.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mDefaultOrganization_PropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("DefaultOrganization");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mSchedule_CollectionChanged																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the schedule collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Collection change event arguments.
		/// </param>
		private void mSchedule_CollectionChanged(object sender,
			CollectionChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Schedule");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mSchedule_ItemPropertyChanged																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The value of a property on a member of the schedule collection has
		/// changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mSchedule_ItemPropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("Schedule");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mSchedule_ItemAdded																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// An item has been added to the Schedule collection.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Item event arguments.
		/// </param>
		private void mSchedule_ItemAdded(object sender,
			ItemEventArgs<TimeBlockItem> e)
		{
			if(e.Data != null)
			{
				if(!ActiveProjectContext.TimeBlocks.Contains(e.Data))
				{
					ActiveProjectContext.TimeBlocks.Add(e.Data);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mSupervisorContact_PropertyChanged																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property has changed on the SupervisorContact object.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property change event arguments.
		/// </param>
		private void mSupervisorContact_PropertyChanged(object sender,
			PropertyChangeEventArgs e)
		{
			if(!e.Handled)
			{
				OnPropertyChanged("SupervisorContact");
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
		/// Create a new instance of the ContactItem item.
		/// </summary>
		public ContactItem()
		{
			ItemId = NextItemId++;
			mSchedule = new TimeBlockCollection();
			mSchedule.CollectionChanged += mSchedule_CollectionChanged;
			mSchedule.ItemAdded += mSchedule_ItemAdded;
			mSchedule.ItemPropertyChanged += mSchedule_ItemPropertyChanged;
			ActiveProjectContext.Contacts.Add(this);
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
		/// Get/Set the name of the city in which the contact is located.
		/// </summary>
		[JsonProperty(Order = 5)]
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
		//*	DefaultOrganization																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="DefaultOrganization">DefaultOrganization</see>.
		/// </summary>
		private OrganizationItem mDefaultOrganization = null;
		/// <summary>
		/// Get/Set a reference to the default organization for this contact.
		/// </summary>
		[JsonConverter(typeof(TicketConverter<OrganizationItem>))]
		[JsonProperty(Order = 13)]
		public OrganizationItem DefaultOrganization
		{
			get { return mDefaultOrganization; }
			set
			{
				bool bChanged = (mDefaultOrganization != value);
				if(bChanged && mDefaultOrganization != null)
				{
					mDefaultOrganization.PropertyChanged -=
						mDefaultOrganization_PropertyChanged;
				}
				mDefaultOrganization = value;
				if(bChanged)
				{
					if(mDefaultOrganization != null)
					{
						mDefaultOrganization.PropertyChanged +=
							mDefaultOrganization_PropertyChanged;
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
		/// Get/Set the user-readable name of the contact.
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
		/// Get/Set the default email address of the contact.
		/// </summary>
		[JsonProperty(Order = 11)]
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
		//*	PositionName																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="PositionName">PositionName</see>.
		/// </summary>
		private string mPositionName = "";
		/// <summary>
		/// Get/Set the name of the contact's position.
		/// </summary>
		[JsonProperty(Order = 3)]
		public string PositionName
		{
			get { return mPositionName; }
			set
			{
				bool bChanged = (mPositionName != value);
				mPositionName = value;
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
		/// Get/Set the name of the province in which the contact is located.
		/// </summary>
		[JsonProperty(Order = 7)]
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
		//*	Schedule																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Schedule">Schedule</see>.
		/// </summary>
		private TimeBlockCollection mSchedule = null;
		/// <summary>
		/// Get a reference to the collection of time blocks composing the
		/// contact's schedule.
		/// </summary>
		[JsonProperty(Order = 15)]
		public TimeBlockCollection Schedule
		{
			get { return mSchedule; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeCityName																								*
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
			return (mCityName?.Length > 0);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeDefaultOrganization																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the DefaultOrganization property
		/// should be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeDefaultOrganization()
		{
			return (mDefaultOrganization != null);
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
		//*	ShouldSerializePositionName																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the PositionName property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializePositionName()
		{
			return mPositionName?.Length > 0;
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
		//*	ShouldSerializeSchedule																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Schedule property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeSchedule()
		{
			return mSchedule != null;
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
		//*	ShouldSerializeSupervisorContact																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the SupervisorContact property should
		/// be serialized.
		/// </summary>
		/// <returns>
		/// A value indicating whether or not to serialize the property.
		/// </returns>
		public bool ShouldSerializeSupervisorContact()
		{
			return (mSupervisorContact != null);
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
		/// Get/Set the name of the state in which the contact is located.
		/// </summary>
		[JsonProperty(Order = 6)]
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
		/// Get/Set the street address of the contact.
		/// </summary>
		[JsonProperty(Order = 4)]
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
		//*	SupervisorContact																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="SupervisorContact">SupervisorContact</see>.
		/// </summary>
		private ContactItem mSupervisorContact = null;
		/// <summary>
		/// Get/Set a reference to the contact record of the supervisor of this
		/// contact.
		/// </summary>
		[JsonConverter(typeof(TicketConverter<ContactItem>))]
		[JsonProperty(Order = 14)]
		public ContactItem SupervisorContact
		{
			get { return mSupervisorContact; }
			set
			{
				bool bChanged = (mSupervisorContact != value);
				if(bChanged && mSupervisorContact != null)
				{
					mSupervisorContact.PropertyChanged -=
						mSupervisorContact_PropertyChanged;
				}
				mSupervisorContact = value;
				if(bChanged)
				{
					if(mSupervisorContact != null)
					{
						mSupervisorContact.PropertyChanged +=
							mSupervisorContact_PropertyChanged;
					}
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
		/// Get/Set the contact's primary telephone number.
		/// </summary>
		[JsonProperty(Order = 9)]
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
		/// Get/Set the contact's primary telephone type.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty(Order = 10)]
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
		//*	ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this item.
		/// </summary>
		/// <returns>
		/// The string representation of this contact.
		/// </returns>
		public override string ToString()
		{
			string result = "";

			if(mDisplayName?.Length > 0)
			{
				result = mDisplayName;
			}
			return result;
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
		/// Get/Set the default website address of the contact.
		/// </summary>
		[JsonProperty(Order = 12)]
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
		/// Get/Set the zip code of the contact's location.
		/// </summary>
		[JsonProperty(Order = 8)]
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
