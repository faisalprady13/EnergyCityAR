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
    /// is responsible for controlling the UI for the refinery out
    /// </summary>
    public class RefineryOutView : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private Image[] oilImages;
        [SerializeField] private CanvasGroup refineryCanvas;

        #endregion

        #region METHODS

        /// <summary>
        /// turn on canvas
        /// </summary>
        public void TurnOn()
        {
            refineryCanvas.alpha = 1;
        }

        /// <summary>
        /// turn off canvas
        /// </summary>
        public void TurnOff()
        {
            refineryCanvas.alpha = 0;
        }


        /// <summary>
        /// Fill the image
        /// </summary>
        /// <param name="imageID"></param>
        /// <param name="amount"></param>
        public void Fill(int imageID, float amount)
        {
            oilImages[imageID].fillAmount = amount;
        }

        /// <summary>
        /// emptying the image
        /// </summary>
        public void Empty(int imageID)
        {
            oilImages[imageID].fillAmount = 0;
        }

        /// <summary>
        /// Is called to get length of array
        /// </summary>
        /// <returns></returns>
        public int GetCapacity()
        {
            return oilImages.Length;
        }

        #endregion
    }
}