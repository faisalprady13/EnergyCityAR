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
using UnityEngine.UI;

namespace Vonderlabs
{
    /// <summary>
    /// Is responsible for controlling a single order
    /// </summary>
    public class OrderController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        private OrderModel orderModel;
        [SerializeField] private OrderView orderView;
        [SerializeField] private int orderTime;
        private IDisposable timerMessage;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            orderModel = new OrderModel();

            int time = orderTime;

            SetTime(time);

            timerMessage = MessageBroker.Default.Receive<TimerMessage>().Subscribe(message =>
                {
                    time--;
                    SetTime(time);
                    //destroy when time is up
                    if (time <= 0)
                    {
                        //time is up

                        MessageBroker.Default.Publish(new OrderTimesUpMessage() {TimesUp = true});
                        timerMessage?.Dispose();
                    }
                })
                .AddTo(this);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            timerMessage?.Dispose();
        }

        /// <summary>
        /// Is called when object destroyed
        /// </summary>
        private void OnDestroy()
        {
            timerMessage?.Dispose();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Is called to stop timer
        /// </summary>
        public void StopTimer()
        {
            timerMessage?.Dispose();
        }

        /// <summary>
        /// Set time for the order
        /// </summary>
        /// <param name="value"></param>
        private void SetTime(float value)
        {
            orderView.SetTime(value);
            orderModel.SetTime(value);
        }

        /// <summary>
        /// Get the current timer of the order
        /// </summary>
        /// <returns></returns>
        public float GetTime()
        {
            return orderModel.GetTime();
        }

        /// <summary>
        /// Set oil type for the order
        /// </summary>
        /// <param name="oil"></param>
        public void SetOrder(OilType oil, int value)
        {
            string oiltype = "";
            if (oil == OilType.CrudeOil)
            {
                oiltype = "Crude oil";
            }

            orderView.SetOrder(oiltype, value);
            orderModel.SetOrder(oil);
            orderModel.SetAmount(value);
        }

        /// <summary>
        /// Get oil type for the order
        /// </summary>
        /// <returns></returns>
        public OilType GetOrder()
        {
            return orderModel.GetOrder();
        }

        /// <summary>
        /// Get amount of oil for the order
        /// </summary>
        /// <returns></returns>
        public int GetAmount()
        {
            return orderModel.GetAmout();
        }

        /// <summary>
        /// Set the destination for the order
        /// </summary>
        /// <param name="residence"></param>
        public void SetDestination(ResidenceType residence)
        {
            orderView.SetDestination(residence.ToString());
            orderModel.SetDestination(residence);
        }

        /// <summary>
        /// Get the destination for the order
        /// </summary>
        /// <returns></returns>
        public ResidenceType GetDestination()
        {
            return orderModel.GetDestination();
        }

        /// <summary>
        /// Get canvas group
        /// </summary>
        /// <returns></returns>
        public CanvasGroup GetCanvasGroup()
        {
            return orderView.GetCanvasGroup();
        }

        #endregion
    }
}