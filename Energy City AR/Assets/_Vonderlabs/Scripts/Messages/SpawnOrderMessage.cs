/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using System;

namespace Vonderlabs
{
    /// <summary>
    /// Message used to spawn an order
    /// </summary>
    public class SpawnOrderMessage
    {
        #region PROPERTIES

        public OrderType Type { get; set; }
        public Action<OrderController> Order { get; set; }
        public Transform Parent { get; set; }

        #endregion
    }
}