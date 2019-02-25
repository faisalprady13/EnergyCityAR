/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Vonderlabs
{
    /// <summary>
    /// CargoModel represents the data model of the cargo
    /// </summary>
    public class CargoModel
    {
        #region MEMBER VARIABLES

        private bool[] oils;
        private bool activated;
        private OilType type;

        #endregion

        #region METHODS

        /// <summary>
        /// Is used to initialize the data model
        /// </summary>
        /// <param name="amount"></param>
        public void Initialize(int amount, OilType oilType, bool active)
        {
            oils = new bool[amount];
            type = oilType;
            activated = active;
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

        /// <summary>
        /// Set cargo type
        /// </summary>
        /// <param name="oilType"></param>
        public void SetCargoType(OilType oilType)
        {
            type = oilType;
        }

        /// <summary>
        /// Get the current cargo type;
        /// </summary>
        public OilType GetCargoType()
        {
            return type;
        }

        /// <summary>
        /// return true if the model is activated
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return activated;
        }

        /// <summary>
        /// activate the model
        /// </summary>
        public void Activate()
        {
            activated = true;
        }

        /// <summary>
        /// deactivate the model
        /// </summary>
        public void Deactivate()
        {
            activated = false;
        }
        #endregion
    }
}