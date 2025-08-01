using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectTask
{
	//*-------------------------------------------------------------------------*
	//*	IParentCollection																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Interface to a parent collection.
	/// </summary>
	public interface IParentCollection
	{
		//*-----------------------------------------------------------------------*
		//* ProjectFile																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Reference to the project file assigned to this collection.
		/// </summary>
		ProjectTask.ProjectFile ProjectFile { get; set; }
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
