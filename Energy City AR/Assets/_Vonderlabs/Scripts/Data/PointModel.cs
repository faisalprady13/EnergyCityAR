/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/
using UnityEngine;
using System.Collections;

namespace Vonderlabs
{
	/// <summary>
	/// PointModel represents the data model of the scoring system
	/// </summary>
	public class PointModel
	{
		#region MEMBER VARIABLES

		private int point;

		#endregion

		#region METHODS

		/// <summary>
		/// Set the point's value
		/// </summary>
		/// <param name="value"></param>
		public void SetPoint(int value)
		{
			point = value;
		}

		/// <summary>
		/// Get the point's value
		/// </summary>
		/// <returns></returns>
		public int GetPoint()
		{
			return point;
		}
		
		#endregion

	}
}