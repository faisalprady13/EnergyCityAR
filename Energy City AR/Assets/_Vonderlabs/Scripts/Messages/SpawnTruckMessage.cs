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
	/// Message used to spawn a truck
	/// </summary>
	public class SpawnTruckMessage
	{
		#region PROPERTIES
		public TruckType Type { get; set; }
		public Action<Transform> Truck { get; set; }
		#endregion
	}
}