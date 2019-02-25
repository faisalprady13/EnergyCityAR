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
    /// Message that contains main board
    /// </summary>
    public class BoardRequestMessage
    {
        #region PROPERTIES

        public Action<Transform> MainBoard { get; set; }

        #endregion
    }
}