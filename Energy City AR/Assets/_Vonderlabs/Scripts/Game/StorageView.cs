/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Vonderlabs
{
	/// <summary>
	/// is responsible for controlling the UI for the storage
	/// </summary>
	public class StorageView : MonoBehaviour
	{
		#region MEMBER VARIABLES

		[SerializeField] private Image[] oilImages;

		#endregion

		#region METHODS

		/// <summary>
		/// fill the image
		/// </summary>
		/// <param name="amount"></param>
		public void Fill(int imageID)
		{
			oilImages[imageID].fillAmount = 1;
		}

		/// <summary>
		/// emptying the image
		/// </summary>
		public void Empty(int imageID)
		{
			oilImages[imageID].fillAmount = 0;
		}

		/// <summary>
		/// Is called to get length of array
		/// </summary>
		/// <returns></returns>
		public int GetCapacity()
		{
			return oilImages.Length;
		}

		#endregion
	}
}