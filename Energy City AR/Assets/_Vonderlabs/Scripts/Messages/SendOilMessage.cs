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
    /// contains amount of oil from oil source
    /// </summary>
    public class SendOilMessage
    {
        #region PROPERTIES

        public int Amount { get; set; }
        public OilType Type { get; set; }

        #endregion
    }
}