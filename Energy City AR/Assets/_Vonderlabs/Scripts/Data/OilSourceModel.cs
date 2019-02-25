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
    /// OilSourceModel represents the data model of oil source
    /// </summary>
    public class OilSourceModel
    {
        #region MEMBER VARIABLES

        private bool[] oils;
        private float[] counts;

        #endregion

        #region METHODS

        /// <summary>
        /// Is used to initialize the data model with the given amount of object
        /// </summary>
        /// <param name="amount"></param>
        public void Initialize(int amount)
        {
            oils = new bool[amount];
            counts = new float[amount];
        }

        /// <summary>
        /// Is used to check oils availability
        /// </summary
        /// <returns></returns>
        public bool IsEmpty(int ID)
        {
            return !oils[ID];
        }

        /// <summary>
        /// Is used to set filled
        /// </summary>
        /// <param name="ID"></param>
        public void Fill(int ID)
        {
            oils[ID] = true;
        }

        /// <summary>
        /// Is used to set empty
        /// </summary>
        /// <param name="ID"></param>
        public void Empty(int ID)
        {
            oils[ID] = false;
            counts[ID] = 0;
        }

        /// <summary>
        /// Set the value of the oil regeneration
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="value"></param>
        public void SetValue(int ID, float value)
        {
            counts[ID] = value;
        }

        /// <summary>
        /// Get the value of the oil regeneration
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public float GetValue(int ID)
        {
            return counts[ID];
        }

        #endregion
    }
}