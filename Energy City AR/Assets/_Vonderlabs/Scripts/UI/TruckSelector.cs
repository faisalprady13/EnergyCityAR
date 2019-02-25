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
using UnityEngine.UI;

namespace Vonderlabs
{
    /// <summary>
    /// TruckSelector is responsible to select a truck based on UI input
    /// </summary>
    public class TruckSelector : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private Button button1;
        [SerializeField] private Button button2;
        [SerializeField] private Button button3;
        [SerializeField] private Button confirmButton;
        [SerializeField] private CanvasGroup truckSelectorUI;
        private Transform mainBoardTransform;
        private Transform mainTruck;
        private IDisposable rotateAnimation;

        #endregion

        #region UNITY METHODS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            truckSelectorUI.alpha = 0;
            truckSelectorUI.interactable = false;
            truckSelectorUI.blocksRaycasts = false;

            MessageBroker.Default.Receive<AppStateMessage>().Subscribe(message =>
            {
                if (message.AppState == AppState.ChooseTruck)
                {
                    MessageBroker.Default.Publish(new BoardRequestMessage()
                    {
                        MainBoard = OnBoardReceived
                    });
                }
                else
                {
                    truckSelectorUI.alpha = 0;
                    truckSelectorUI.interactable = false;
                    truckSelectorUI.blocksRaycasts = false;
                }
            }).AddTo(this);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Is responsible to initialize the truck selection UI
        /// </summary>
        /// <param name="mainBoard"></param>
        private void Initialize()
        {
            truckSelectorUI.alpha = 1;
            truckSelectorUI.interactable = true;
            truckSelectorUI.blocksRaycasts = true;
            SpawnTruck(TruckType.HotDog);

            button1.OnClickAsObservable().Subscribe(_ =>
            {
                SpawnTruck(TruckType.HotDog);
            }).AddTo(this);

            button2.OnClickAsObservable().Subscribe(_ =>
            {
                SpawnTruck(TruckType.Tanker);
            }).AddTo(this);

            button3.OnClickAsObservable().Subscribe(_ =>
            {
                SpawnTruck(TruckType.YellowTanker);
            }).AddTo(this);

            confirmButton.OnClickAsObservable().Subscribe(_ =>
            {
                StopRotateAnimation();
                MessageBroker.Default.Publish(new AppStateMessage() {AppState = AppState.ResetGame});
            }).AddTo(this);
        }

        /// <summary>
        /// Spawn a new truck on the center of the AR board    
        /// </summary>
        /// <param name="truck"></param>
        private void SpawnTruck(TruckType truck)
        {
            DespawnTruck(mainTruck);
            MessageBroker.Default.Publish(new SpawnTruckMessage() {Type = truck, Truck = OnTruckReceived});
        }

        /// <summary>
        /// Is responsible to despawn the current truck
        /// </summary>
        /// <param name="truck"></param>
        private void DespawnTruck(Transform truck)
        {
            StopRotateAnimation();
            MessageBroker.Default.Publish(new DespawnTruckMessage() {Truck = null});
        }

        /// <summary>
        /// Is responsible to move an object to the center of main board
        /// </summary>
        /// <param name="objectToCenter"></param>
        private void MoveToCenter(Transform objectToCenter)
        {
            objectToCenter.Translate(mainBoardTransform.position.x, mainBoardTransform.position.y + 0.7f,
                mainBoardTransform.position.z);
        }

        /// <summary>
        /// Is responsible to start rotate animation
        /// </summary>
        /// <param name="objectToRotate"></param>
        private void StartRotateAnimation(Transform objectToRotate)
        {
            rotateAnimation = this.UpdateAsObservable().Subscribe(_ =>
            {
                objectToRotate.Rotate(0.0f, 50.0f * Time.deltaTime, 0.0f);
            });
        }

        /// <summary>
        /// Is responsible to stop the rotate animation
        /// </summary>
        private void StopRotateAnimation()
        {
            rotateAnimation?.Dispose();
        }

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Is called when the spawned truck received
        /// </summary>
        /// <param name="receivedTruck"></param>
        private void OnTruckReceived(Transform receivedTruck)
        {
            mainTruck = receivedTruck;
            MoveToCenter(mainTruck);
            StartRotateAnimation(mainTruck);
        }

        /// <summary>
        /// Is called when main game board received
        /// </summary>
        private void OnBoardReceived(Transform mainBoard)
        {
            mainBoardTransform = mainBoard;
            Initialize();
        }

        #endregion
    }
}