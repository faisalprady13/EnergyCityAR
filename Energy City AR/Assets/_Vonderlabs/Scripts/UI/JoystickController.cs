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
    /// JoystickController is responsible to control the joystick UI
    /// </summary>
    public class JoystickController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private CanvasGroup joystick;
        [SerializeField] private CanvasGroup area;
        private IDisposable onboardMessage;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            joystick.alpha = 0;
            joystick.interactable = false;
            joystick.blocksRaycasts = false;

            MessageBroker.Default.Receive<AppStateMessage>().Subscribe(message =>
            {
                if (message.AppState == AppState.PlayGame)
                {
                    joystick.alpha = 1;
                    joystick.interactable = true;
                    joystick.blocksRaycasts = true;
                }
                else
                {
                    joystick.alpha = 0;
                    joystick.interactable = false;
                    joystick.blocksRaycasts = false;
                }
            });

            onboardMessage = MessageBroker.Default.Receive<OnboardingCTAMessage>().Subscribe(message =>
            {
                if (message.Instruction == Onboarding.MoveCar)
                {
                    area.transform.SetAsFirstSibling();
                    area.alpha = 1;
                }
                else if (message.Instruction == Onboarding.OilSource)
                {
                    area.alpha = 0;
                    onboardMessage?.Dispose();
                }
            }).AddTo(this);
        }

        #endregion
    }
}