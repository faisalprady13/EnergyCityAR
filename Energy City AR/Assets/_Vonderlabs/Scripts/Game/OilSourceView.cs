/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Vonderlabs
{
    /// <summary>
    /// OilSourceView is responsible for controlling the UI for the oil source
    /// </summary>
    public class OilSourceView : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private Image[] oilImages;

        #endregion

        #region METHODS

        /// <summary>
        /// fill the image with the given amount between 0 and 1
        /// </summary>
        /// <param name="amount"></param>
        public void Fill(int imageID, float amount)
        {
            Image image = oilImages[imageID];
            image.fillAmount = amount;
        }

        /// <summary>
        /// emptying the image
        /// </summary>
        public void Empty(int imageID)
        {
            Image image = oilImages[imageID];
            image.fillAmount = 0;
        }

        /// <summary>
        /// Is called to get length of array
        /// </summary>
        /// <returns></returns>
        public int GetAmount()
        {
            return oilImages.Length;
        }

        #endregion
    }
}