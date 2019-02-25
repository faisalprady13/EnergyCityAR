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
    /// is responsible for controlling the UI for the refinery in
    /// </summary>
    public class RefineryInView : MonoBehaviour
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
        /// fill the image
        /// </summary>
        /// <param name="imageID"></param>
        public void Fill(int imageID)
        {
            Image image = oilImages[imageID];
            image.fillAmount = 1;
        }

        /// <summary>
        /// emptying the image
        /// </summary>
        /// <param name="imageID"></param>
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