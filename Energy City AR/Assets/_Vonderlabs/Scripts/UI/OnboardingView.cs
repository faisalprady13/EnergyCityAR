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
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

namespace Vonderlabs
{
    /// <summary>
    /// OnboardingView represents the onboarding canvas
    /// </summary>
    public class OnboardingView : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Text warningText;

        #endregion


        #region METHODS

        /// <summary>
        /// Set the text for the notification
        /// </summary>
        /// <param name="text"></param>
        public void SetText(String text)
        {
            warningText.text = text;
        }

        /// <summary>
        /// start spawn panel fade animation
        /// </summary>
        /// <param name="order"></param>
        public void TurnOnPanel()
        {

            canvasGroup.alpha = 1;
        }

        /// <summary>
        /// Start despawn panel fade animation
        /// </summary>
        /// <param name="order"></param>
        public void TurnOffPanel()
        {
            canvasGroup.alpha = 0;
        }

        #endregion
    }
}