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

namespace Vonderlabs
{
    /// <summary>
    /// NotificationController is responsible for managing in game feedback
    /// </summary>
    public class NotificationController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private NotificationView notificationView;
        private IDisposable timer;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            MessageBroker.Default.Receive<NotificationMessage>().Subscribe(message =>
            {
                notificationView.ClearEffect();
                timer?.Dispose();

                NotificationType warning = message.Warning;
                if (warning == NotificationType.CargoEmpty)
                {
                    SetText("your cargo is empty");
                }
                else if (warning == NotificationType.RefineryFull)
                {
                    SetText("the refinery is full, take the oil");
                }
                else if (warning == NotificationType.WrongType)
                {
                    SetText("wrong cargo type, go to cargo changer");
                }
                else if (warning == NotificationType.CargoFull)
                {
                    SetText("your cargo is full, put it on storage");
                }
                else if (warning == NotificationType.WrongDestination)
                {
                    SetText("wrong destination, look at the order above");
                }

                notificationView.TurnOn();
                int delay = 2;
                timer = MessageBroker.Default.Receive<TimerMessage>().Subscribe(_ =>
                {
                    if (delay < 0)
                    {
                        notificationView.TurnOff();
                        timer?.Dispose();
                    }

                    delay--;
                }).AddTo(this);
            }).AddTo(this);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Set the warning text
        /// </summary>
        /// <param name="warningText"></param>
        private void SetText(String warningText)
        {
            notificationView.SetText(warningText);
        }

        #endregion
    }
}