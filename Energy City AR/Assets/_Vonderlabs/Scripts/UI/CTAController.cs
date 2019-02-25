/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/

using System;
using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

namespace Vonderlabs
{
	/// <summary>
	/// CTAController is responsible to spawn CTA
	/// </summary>
	public class CTAController : MonoBehaviour 
	{
		#region MEMBER VARIABLES
		[SerializeField] private CanvasGroup ctaCursor;
		private float rotationSpeed;
		private IDisposable cursorEffect;
		#endregion

		
		#region UNITY FUNCTIONS

		/// <summary>
		/// Start is called on the frame when a script is enabled just before
		/// any of the Update methods is called the first time.
		/// </summary>
		private void Start ()
		{
			rotationSpeed = 20f;
		}
		
		#endregion
		
		#region METHODS
		
		/// <summary>
		/// Is used to animate the cursor
		/// </summary>
		public void StartCtaAnimation()
		{
			ctaCursor.alpha = 1;
			cursorEffect = this.UpdateAsObservable().Subscribe(_ =>
				{
					ctaCursor.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
				})
				.AddTo(this);
		}

		/// <summary>
		/// Stop the cursor animation
		/// </summary>
		public void StopCtaAnimation()
		{
			ctaCursor.alpha = 0;
			cursorEffect?.Dispose();
		}
		#endregion
		
		
	}
}