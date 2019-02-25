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
using HedgehogTeam.EasyTouch;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;

namespace Vonderlabs
{
    /// <summary>
    /// PlayerController class is responsible for controlling the main player
    /// </summary>
    public class TruckController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private float speed;
        [SerializeField] private Rigidbody truckRigidBody;
        [SerializeField] private Transform pivot;
        [SerializeField] private float offset;
        private Camera mainCamera;
        private IDisposable updateMessage;

        private bool isOnboarding;
        private bool isMoveOnboarding;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            isOnboarding = true;
            isMoveOnboarding = true;

            MessageBroker.Default.Receive<AppStateMessage>().Subscribe(message =>
            {
                if (message.AppState == AppState.PlayGame)
                {
                    if (isOnboarding)
                    {
                        MessageBroker.Default.Publish(new OnboardingMessage() {Instruction = Onboarding.MoveCar});
                        isOnboarding = false;
                    }

                    updateMessage = this.UpdateAsObservable().Subscribe(_ =>
                    {
                        float moveHorizontal = ETCInput.GetAxis("Horizontal");
                        float moveVertical = ETCInput.GetAxis("Vertical");

                        if (moveHorizontal != 0.0f || moveVertical != 0.0f)
                        {
                            if (pivot != null)
                            {
                                if (isMoveOnboarding)
                                {
                                    MessageBroker.Default.Publish(new OnboardingMessage()
                                        {Instruction = Onboarding.OilSource});
                                    isMoveOnboarding = false;
                                }

                                MovePlayer(moveHorizontal, moveVertical);
                            }
                        }
                    }).AddTo(this);
                }
                else
                {
                    updateMessage?.Dispose();
                }
            });
        }

        private void OnEnable()
        {
            MessageBroker.Default.Publish(new CameraRequestMessage() {MainCamera = OnCameraReceived});
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Is used to move the player relative to camera direction
        /// </summary>
        /// <param name="moveHorizontal"></param>
        /// <param name="moveVertical"></param>
        private void MovePlayer(float moveHorizontal, float moveVertical)
        {
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            //relative to camera
            Transform mainCamTransform = mainCamera.transform;
            mainCamTransform.eulerAngles = new Vector3(0, mainCamTransform.eulerAngles.y, 0);
            Vector3 relativeMovement = mainCamTransform.TransformVector(movement);

            RotatePlayer(relativeMovement);
            truckRigidBody.AddForce(relativeMovement * speed);
        }

        /// <summary>
        /// Is used to make the player faces the moving direction
        /// </summary>
        /// <param name="movement"></param>
        private void RotatePlayer(Vector3 movement)
        {
            pivot.transform.rotation = Quaternion.LookRotation(movement);

            truckRigidBody.transform.rotation =
                pivot.transform.rotation * Quaternion.Euler(0, offset, 0); //90 degree offset
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
        }

        #endregion
    }
}