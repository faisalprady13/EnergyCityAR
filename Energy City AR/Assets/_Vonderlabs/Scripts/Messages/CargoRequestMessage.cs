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

namespace Vonderlabs
{
    /// <summary>
    /// Message contains cargo information
    /// </summary>
    public class CargoRequestMessage
    {
        #region PROPERTIES

        public Action<CargoController> Cargo { get; set; }

        #endregion
    }
}