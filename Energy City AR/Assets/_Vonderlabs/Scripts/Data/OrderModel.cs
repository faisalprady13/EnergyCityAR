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
    /// OrderModel represents the data of an Order
    /// </summary>
    public class OrderModel
    {
        #region MEMBER VARIABLES

        private float time;
        private int amount;
        private OilType oil;
        private ResidenceType residence;

        #endregion

        #region METHODS

        /// <summary>
        /// Set the timer for the order
        /// </summary>
        /// <param name="time"></param>
        public void SetTime(float value)
        {
            time = value;
        }

        /// <summary>
        /// Get the current timer of the order
        /// </summary>
        /// <returns></returns>
        public float GetTime()
        {
            return time;
        }
        
        /// <summary>
        /// Set oil type for the order
        /// </summary>
        /// <param name="oil"></param>
        public void SetOrder(OilType type)
        {
            oil = type;
        }

        /// <summary>
        /// Get oil type for the order
        /// </summary>
        /// <returns></returns>
        public OilType GetOrder()
        {
            return oil;
        }

        /// <summary>
        /// Set amount of oil for the order
        /// </summary>
        /// <param name="value"></param>
        public void SetAmount(int value)
        {
            amount = value;
        }

        /// <summary>
        /// Get amount of oil for the order
        /// </summary>
        /// <returns></returns>
        public int GetAmout()
        {
            return amount;
        }

        /// <summary>
        /// Set the destination for the order
        /// </summary>
        /// <param name="building"></param>
        public void SetDestination(ResidenceType type)
        {
            residence = type;
        }

        /// <summary>
        /// Get the destination for the order
        /// </summary>
        /// <returns></returns>
        public ResidenceType GetDestination()
        {
            return residence;
        }

        #endregion
    }
}