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
    /// ResidenceController is responsible to controlling the residence
    /// </summary>
    public class ResidenceController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private Collider areaTrigger;
        [SerializeField] private Transform residenceTransform;
        [SerializeField] private CTAController ctaController;
        [SerializeField] private ResidenceType ResidenceType;
        private IDisposable onboardMessage;
        private IDisposable hitMessage;
        private OilType cargoType;
        private int cargoAmount;
        private bool isOnboarding;

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
                if (message.Instruction == Onboarding.Residence)
                {
                    ctaController.StartCtaAnimation();
                    isOnboarding = true;
                }
                else
                {
                    ctaController.StopCtaAnimation();
                }
            }).AddTo(this);


            MessageBroker.Default.Receive<AppStateMessage>().Subscribe(message =>
            {
                if (message.AppState == AppState.ResetGame)
                {
                }

                else if (message.AppState == AppState.PlayGame)
                {
                }

                else if (message.AppState == AppState.EndGame)
                {
                }
            }).AddTo(this);


            areaTrigger.OnTriggerEnterAsObservable().Subscribe(_ =>
            {
                hitMessage = MessageBroker.Default.Receive<RaycastHitMessage>().Subscribe(message =>
                {
                    if (message.Hit.transform == residenceTransform)
                    {
                        if (isOnboarding)
                        {
                            MessageBroker.Default.Publish(new OnboardingMessage() {Instruction = Onboarding.Done});
                            isOnboarding = false;
                            onboardMessage?.Dispose();
                        }

                        RequestCargo();
                    }
                }).AddTo(this);
            }).AddTo(this);
            areaTrigger.OnTriggerExitAsObservable().Subscribe(_ => { hitMessage?.Dispose(); }).AddTo(this);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Request the current cargoController
        /// </summary>
        private void RequestCargo()
        {
            MessageBroker.Default.Publish(new CargoRequestMessage()
            {
                Cargo = OnCargoReceived
            });
        }

        /// <summary>
        /// Get type of the building
        /// </summary>
        /// <returns></returns>
        private ResidenceType GetBuildingType()
        {
            return ResidenceType;
        }

        /// <summary>
        /// Check if the delivered same as the requested order
        /// </summary>
        /// <param name="oil"></param>
        /// <param name="amount"></param>
        private void CheckOrder(OilType oil, int amount)
        {
            if (amount > 0)
            {
                MessageBroker.Default.Publish(new CheckOrderMessage()
                {
                    Amount = amount,
                    Residence = GetBuildingType(),
                    Oil = oil
                });
            }
        }

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Is called when cargo received
        /// </summary>
        /// <param name="cargo"></param>
        private void OnCargoReceived(CargoController cargo)
        {
            CheckOrder(cargo.GetCargoType(), cargo.GetAmount());
        }

        #endregion
    }
}