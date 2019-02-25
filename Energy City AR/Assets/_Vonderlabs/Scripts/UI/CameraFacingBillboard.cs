/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

namespace Vonderlabs
{
    /// <summary>
    /// CameraFacingBillboard is responsible to rotate the object towards the camera
    /// </summary>
    public class CameraFacingBillboard : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private CanvasGroup canvasUI;

        #endregion

        #region UNITY FUNCTIONS

        private void OnEnable()
        {
            MessageBroker.Default.Publish(new CameraRequestMessage() {MainCamera = OnCamReceived});
        }

        #endregion

        #region METHODS

        /// <summary>
        /// set camera to be followed
        /// </summary>
        /// <param name="myCamera"></param>
        private void FollowCam(Camera myCamera)
        {
            this.LateUpdateAsObservable().Subscribe(_ =>
            {
                canvasUI.transform.LookAt(canvasUI.transform.position + myCamera.transform.rotation * Vector3.forward,
                    myCamera.transform.rotation * Vector3.up);
            }).AddTo(this);
        }
        
        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Is called when the main camera received
        /// </summary>
        /// <param name="cam"></param>
        private void OnCamReceived(Camera cam)
        {
            FollowCam(cam);
        }

        #endregion
    }
}