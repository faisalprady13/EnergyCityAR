/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/
using UnityEngine;
using System.Collections;

namespace Vonderlabs
{
    /// <summary>
    /// Message used to send global game tick
    /// </summary>
    public class TimerMessage
    {
        #region PROPERTIES
        public float Value { get; set; }
        #endregion
    }
}