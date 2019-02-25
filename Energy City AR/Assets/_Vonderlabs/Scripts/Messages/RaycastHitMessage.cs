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
    /// Message that contains RaycastHit
    /// </summary>
    public class RaycastHitMessage : MonoBehaviour
    {
        #region PROPERTIES

        public RaycastHit Hit { get; set; }

        #endregion
    }
}