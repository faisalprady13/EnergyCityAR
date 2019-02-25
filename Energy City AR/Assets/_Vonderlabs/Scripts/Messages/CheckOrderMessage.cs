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
    /// CheckOrderMessage is used to check an order
    /// </summary>
    public class CheckOrderMessage
    {
        #region PROPERTIES

        public ResidenceType Residence { get; set; }
        public OilType Oil { get; set; }
        public int Amount { get; set; }

        #endregion
    }
}