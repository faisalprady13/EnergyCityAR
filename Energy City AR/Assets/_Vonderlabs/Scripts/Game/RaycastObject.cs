/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyTouch;
using UniRx;
using UniRx.Triggers;

namespace Vonderlabs
{
    /// <summary>
    /// RaycastObject is responsible for raycasting
    /// </summary>
    public class RaycastObject : MonoBehaviour
    {
        #region MEMBER VARIABLES

        private Camera mainCamera;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            EasyTouch.On_TouchStart -= OnTouchStart;
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            MessageBroker.Default.Receive<AppStateMessage>().Subscribe(message =>
            {
                if (message.AppState == AppState.PlayGame)
                {
                    MessageBroker.Default.Publish(new CameraRequestMessage() {MainCamera = OnCameraReceived});
                }
                else if (message.AppState == AppState.PauseGame)
                {
                    EasyTouch.On_TouchStart -= OnTouchStart;
                }
                else if (message.AppState == AppState.ResetGame)
                {
                    EasyTouch.On_TouchStart -= OnTouchStart;
                }
                else if (message.AppState == AppState.EndGame)
                {
                    EasyTouch.On_TouchStart -= OnTouchStart;
                }
            }).AddTo(this);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Initialize the function
        /// </summary>
        private void Initialize()
        {
            EasyTouch.On_TouchStart += OnTouchStart;
        }

        /// <summary>
        /// Is called when tap gesture start
        /// </summary>
        /// <param name="gesture"></param>
        private void OnTouchStart(Gesture gesture)
        {
            if (gesture?.touchCount == 1)
            {
                Raycast(gesture.position.x, gesture.position.y);
            }
        }

        /// <summary>
        /// Cast a ray straight from the given position
        /// </summary>
        /// <param name="xScreen"></param>
        /// <param name="yScreen"></param>
        private void Raycast(float xScreen, float yScreen)
        {
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(xScreen, yScreen));
            Debug.DrawRay(mainCamera.transform.position, Vector3.forward, Color.magenta);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MessageBroker.Default.Publish(new RaycastHitMessage() {Hit = hit});
            }
        }

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Is called when main camera is received
        /// </summary>
        /// <param name="receivedBlock"></param>
        public void OnCameraReceived(Camera receivedCamera)
        {
            mainCamera = receivedCamera;
            Initialize();
        }

        #endregion
    }
}