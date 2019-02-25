/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/
using UnityEngine;
using System.Collections;
using System;

namespace Vonderlabs
{
	/// <summary>
	/// Message used to despawn a truck
	/// </summary>
	public class DespawnTruckMessage
	{
		#region PROPERTIES
		public Transform Truck { get; set; }
		#endregion
	}
}