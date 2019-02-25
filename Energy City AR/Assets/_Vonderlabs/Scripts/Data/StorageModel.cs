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
    /// represents the data model storage
    /// </summary>
    public class StorageModel
    {
        #region MEMBER VARIABLES

        private bool[] oils;

        #endregion

        #region METHODS

        /// <summary>
        /// Is used to initialize the data model with the given amount of object
        /// </summary>
        /// <param name="amount"></param>
        public void Initialize(int amount)
        {
            oils = new bool[amount];
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
        }

        #endregion
    }
}