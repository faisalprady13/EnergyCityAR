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
	/// Is responsible for the creation of order library asset
	/// </summary>
	[CreateAssetMenu(fileName = "Order Library", menuName = "Custom/Order Library")]
	public class OrderLibrary : SerializedScriptableObject
	{
		public Dictionary<OrderType, RectTransform> library = new Dictionary<OrderType, RectTransform>();
	}
}