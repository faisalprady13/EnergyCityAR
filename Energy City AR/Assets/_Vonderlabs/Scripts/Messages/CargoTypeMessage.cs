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
    /// CargoTypeMessage is a message containing type of the cargo
    /// </summary>
    public class CargoTypeMessage
    {
        #region PROPERTIES

        public OilType Type { get; set; }

        #endregion
    }
}