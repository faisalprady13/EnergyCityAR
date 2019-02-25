/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

namespace Vonderlabs
{
    /// <summary>
    /// CargoView represents the UI for the cargo
    /// </summary>
    public class CargoView : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private CanvasGroup oilCanvas;

        [SerializeField] private Image[] oilImages;

        #endregion

        #region METHODS

        /// <summary>
        /// turn on canvas
        /// </summary>
        public void TurnOn()
        {
            oilCanvas.transform.SetAsLastSibling();
            oilCanvas.alpha = 1;
        }

        /// <summary>
        /// turn off canvas
        /// </summary>
        public void TurnOff()
        {
            oilCanvas.alpha = 0;
            oilCanvas.interactable = false;
            oilCanvas.blocksRaycasts = false;
        }

        /// <summary>
        /// Set the UI position same as the target
        /// </summary>
        /// <param name="canvasUI"></param>
        /// <param name="target"></param>
        public void SetPosition(Transform target)
        {
            oilCanvas.transform.position =
                new Vector3(target.position.x, oilCanvas.transform.position.y, target.position.z);
        }


        /// <summary>
        /// fill the image with the given amount between 0 and 1
        /// </summary>
        /// <param name="amount"></param>
        public void Fill(int imageID)
        {
            Image image = oilImages[imageID];
            image.fillAmount = 1;
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