/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Vonderlabs
{
    /// <summary>
    /// OrderView represents the UI of an order
    /// </summary>
    public class OrderView : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private Text time;
        [SerializeField] private Text order;
        [SerializeField] private Text destination;
        [SerializeField] private CanvasGroup orderPanel;

        #endregion

        #region METHODS

        /// <summary>
        /// Set time for the order
        /// </summary>
        /// <param name="value"></param>
        public void SetTime(float value)
        {
            time.text = "Time left: " + value.ToString("0");
        }

        /// <summary>
        /// Set oil type and amount for the order
        /// </summary>
        /// <param name="oil"></param>
        public void SetOrder(string oil, int amount)
        {
            order.text = "Order: " + amount + " " + oil;
        }

        /// <summary>
        /// Set the destination for the order
        /// </summary>
        /// <param name="building"></param>
        public void SetDestination(string building)
        {
            destination.text = "Goal: " + building;
        }

        /// <summary>
        /// Get canvas group
        /// </summary>
        /// <returns></returns>
        public CanvasGroup GetCanvasGroup()
        {
            return orderPanel;
        }

        #endregion
    }
}