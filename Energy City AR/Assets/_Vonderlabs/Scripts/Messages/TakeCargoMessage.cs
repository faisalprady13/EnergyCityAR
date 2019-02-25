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
    /// A message used to decrease the given amount of cargo
    /// </summary>
    public class TakeCargoMessage
    {
        #region PROPERTIES

        public int Amount { get; set; }
        public OilType Type { get; set; }

        #endregion
    }
}