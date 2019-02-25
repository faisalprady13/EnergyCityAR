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
	/// Message used to despawn an order
	/// </summary>
	public class DespawnOrderMessage
	{
		#region PROPERTIES
		public Transform Order { get; set; }
		#endregion
	}
}