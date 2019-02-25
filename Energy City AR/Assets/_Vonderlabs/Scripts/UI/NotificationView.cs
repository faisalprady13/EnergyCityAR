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
    /// NotificationView class represents the notification UI
    /// </summary>
    public class NotificationView : MonoBehaviour
    {
        #region MEMBER VARIABLES

        private IDisposable spawnEffect;
        private IDisposable despawnEffect;
        private int speed;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Text warningText;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            speed = 2;
        }

        #endregion

        #region METHODS

        public void SetText(String text)
        {
            warningText.text = text;
        }

        /// <summary>
        /// start spawn fade animation
        /// </summary>
        /// <param name="order"></param>
        public void TurnOn()
        {
            float scale;
            spawnEffect = this.UpdateAsObservable().Subscribe(_ =>
                {
                    scale = speed * Time.deltaTime;
                    canvasGroup.alpha += scale;
                    if (canvasGroup.alpha >= 1)
                    {
                        spawnEffect?.Dispose();
                    }
                })
                .AddTo(this);
        }

        /// <summary>
        /// Start despawn fade animation
        /// </summary>
        /// <param name="order"></param>
        public void TurnOff()
        {
            float scale;
            despawnEffect = this.UpdateAsObservable().Subscribe(_ =>
                {
                    scale = speed * Time.deltaTime;
                    canvasGroup.alpha -= scale;
                    if (canvasGroup.alpha <= 0)
                    {
                        despawnEffect?.Dispose();
                    }
                })
                .AddTo(this);
        }

        /// <summary>
        /// clear message to remove multiple message
        /// </summary>
        public void ClearEffect()
        {
            spawnEffect?.Dispose();
            despawnEffect?.Dispose();
        }
        #endregion
    }
}