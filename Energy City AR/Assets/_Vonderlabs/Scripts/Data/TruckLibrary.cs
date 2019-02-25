/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Vonderlabs
{
	/// <summary>
	/// Is responsible for the creation of truck library asset
	/// </summary>
	[CreateAssetMenu(fileName = "Truck Library", menuName = "Custom/Truck Library")]
	public class TruckLibrary : SerializedScriptableObject
	{
		public Dictionary<TruckType, Transform> library = new Dictionary<TruckType, Transform>();
	}
}