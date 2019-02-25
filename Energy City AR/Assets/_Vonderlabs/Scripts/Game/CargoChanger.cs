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

namespace Vonderlabs
{
    /// <summary>
    /// CargoChanger is used to change type of the cargo
    /// </summary>
    public class CargoChanger : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private Collider areaTrigger;
        private IDisposable hitMessage;
        private bool isOnboarding;
        [SerializeField] private CTAController ctaController;
        private IDisposable onboardMessage;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            isOnboarding = false;

            ctaController.StopCtaAnimation();


            onboardMessage = MessageBroker.Default.Receive<OnboardingCTAMessage>().Subscribe(message =>
            {
                if (message.Instruction == Onboarding.ChangeCargo)
                {
                    ctaController.StartCtaAnimation();
                    isOnboarding = true;
                }
                else
                {
                    ctaController.StopCtaAnimation();
                }
            }).AddTo(this);

            areaTrigger.OnTriggerEnterAsObservable().Subscribe(_ =>
            {
                hitMessage = MessageBroker.Default.Receive<RaycastHitMessage>().Subscribe(message =>
                {
                    if (message.Hit.collider == areaTrigger)
                    {
                        if (isOnboarding)
                        {
                            MessageBroker.Default.Publish(
                                new OnboardingMessage() {Instruction = Onboarding.RefineryOut});
                            isOnboarding = false;
                            onboardMessage?.Dispose();
                        }

                        MessageBroker.Default.Publish(new CargoRequestMessage()
                        {
                            Cargo = OnCargoReceived
                        });
                    }
                });
            });
            areaTrigger.OnTriggerExitAsObservable().Subscribe(_ => { hitMessage?.Dispose(); }).AddTo(this);
        }

        #endregion

        #region METHODS

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Is called when cargo received
        /// </summary>
        /// <param name="cargo"></param>
        private void OnCargoReceived(CargoController cargo)
        {
            if (cargo.GetCargoType() == OilType.CrudeOil)
            {
                MessageBroker.Default.Publish(new CargoTypeMessage()
                {
                    Type = OilType.ProcessedOil
                });
            }
            else if (cargo.GetCargoType() == OilType.ProcessedOil)
            {
                MessageBroker.Default.Publish(new CargoTypeMessage()
                {
                    Type = OilType.CrudeOil
                });
            }
        }

        #endregion
    }
}