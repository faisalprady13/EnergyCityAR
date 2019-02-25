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
	/// Message that contains main camera
	/// </summary>
	public class CameraRequestMessage
	{
		#region PROPERTIES
		public Action<Camera> MainCamera { get; set; }
		#endregion
	}
}