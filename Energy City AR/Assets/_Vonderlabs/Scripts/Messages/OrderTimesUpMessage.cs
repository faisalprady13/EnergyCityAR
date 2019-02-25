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
	/// Is a message contains time status of an order
	/// </summary>
	public class OrderTimesUpMessage
	{
		#region PROPERTIES

		public bool TimesUp { get; set; }
		#endregion
	}
}