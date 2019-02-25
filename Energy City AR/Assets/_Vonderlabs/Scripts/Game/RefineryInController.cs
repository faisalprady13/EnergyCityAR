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
using System.Globalization;
using UniRx;
using UniRx.Triggers;

namespace Vonderlabs
{
    /// <summary>
    /// is responsible for controlling the function of refinery input
    /// </summary>
    public class RefineryInController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private Transform refineryInTransform;
        [SerializeField] private RefineryInView refineryInView;
        [SerializeField] private Collider areaTrigger;

        [SerializeField] private CTAController ctaController;

        private RefineryModel refineryModel;
        private int amount;
        private IDisposable hitMessage;
        private bool refineryOpen;
        private bool isOnboarding;
        private IDisposable onboardMessage;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            refineryModel = new RefineryModel();
            amount = refineryInView.GetAmount();
            refineryModel.Initialize(amount);
            refineryOpen = true;
            isOnboarding = false;
            ctaController.StopCtaAnimation();
            onboardMessage = MessageBroker.Default.Receive<OnboardingCTAMessage>().Subscribe(message =>
            {
                if (message.Instruction == Onboarding.RefineryIn)
                {
                    ctaController.StartCtaAnimation();
                    isOnboarding = true;
                }
                else
                {
                    ctaController.StopCtaAnimation();
                }
            }).AddTo(this);

            //emptying
            MessageBroker.Default.Receive<AppStateMessage>().Subscribe(message =>
            {
                if (message.AppState == AppState.ResetGame)
                {
                    EmptyAll();
                }

                else if (message.AppState == AppState.PlayGame)
                {
                }

                else if (message.AppState == AppState.EndGame)
                {
                    hitMessage?.Dispose();
                }
            }).AddTo(this);

            areaTrigger.OnTriggerEnterAsObservable().Subscribe(_ =>
            {
                hitMessage = MessageBroker.Default.Receive<RaycastHitMessage>().Subscribe(message =>
                {
                    if (message.Hit.transform == refineryInTransform)
                    {
                        if (refineryOpen)
                        {
                            MessageBroker.Default.Publish(new CargoRequestMessage()
                            {
                                Cargo = OnCargoReceived
                            });
                        }
                    }
                }).AddTo(this);
            }).AddTo(this);


            MessageBroker.Default.Receive<RefineryStateMessage>().Subscribe(message =>
                {
                    refineryOpen = message.Open;
                    if (refineryOpen)
                    {
                        refineryInView.TurnOn();
                    }
                    else
                    {
                        refineryInView.TurnOff();
                    }
                })
                .AddTo(this);


            areaTrigger.OnTriggerExitAsObservable().Subscribe(_ => { hitMessage?.Dispose(); }).AddTo(this);
        }


        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            EmptyAll();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Take oil from the cargo
        /// </summary>
        /// <param name="imageID"></param>
        private void TakeOil(int amount)
        {
            MessageBroker.Default.Publish(new TakeCargoMessage()
            {
                Amount = amount
            });
        }

        /// <summary>
        /// fill the data model on the given ID
        /// </summary>
        /// <param name="imageID"></param>
        private void Fill(int imageID)
        {
            refineryModel.Fill(imageID);
            refineryInView.Fill(imageID);
        }

        /// <summary>
        /// emptying the slot
        /// </summary>
        /// <param name="imageID"></param>
        private void Empty(int imageID)
        {
            refineryInView.Empty(imageID);
            refineryModel.Empty(imageID);
        }

        /// <summary>
        /// reset all the slots
        /// </summary>
        /// <param name="imageID"></param>
        private void EmptyAll()
        {
            for (int i = 0; i < amount; i++)
            {
                Empty(i);
            }
        }

        /// <summary>
        /// Check for empty slot
        /// </summary>
        /// <returns></returns>
        private int CheckEmpty()
        {
            int result = 0;
            for (int i = 0; i < amount; i++)
            {
                if (refineryModel.IsEmpty(i))
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Autofill the refinery slot with the given amount of oil
        /// </summary>
        /// <param name="value"></param>
        private void FillOil(int value)
        {
            int leftOnCargo = value;
            for (int i = 0; i < amount; i++)
            {
                if (refineryModel.IsEmpty(i))
                {
                    Fill(i);
                    leftOnCargo--;
                }

                if (leftOnCargo == 0)
                {
                    break;
                }
            }

            if (leftOnCargo < value)
            {
                TakeOil(value - leftOnCargo);
            }

            //check if full then proceed (delay 15s then send to refinery out) also check is out empty
            if (CheckEmpty() == 0)
            {
                if (isOnboarding)
                {
                    MessageBroker.Default.Publish(
                        new OnboardingMessage() {Instruction = Onboarding.Storage});
                    onboardMessage?.Dispose();
                    isOnboarding = false;
                }

                EmptyAll();
                MessageBroker.Default.Publish(new RefineryInMessage()
                {
                    Amount = 3
                });
            }
        }

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Is called when amount of cargo received
        /// </summary>
        /// <param name="cargo"></param>
        private void OnCargoReceived(CargoController cargo)
        {
            int value = cargo.GetAmount();
            int emptySlot = CheckEmpty();
            OilType type = cargo.GetCargoType();

            if (value != 0 && emptySlot > 0 && type == OilType.CrudeOil)
            {
                FillOil(value);
            }
            else if (type != OilType.CrudeOil)
            {
                MessageBroker.Default.Publish(new NotificationMessage()
                {
                    Warning = NotificationType.WrongType
                });
            }
            else if (value == 0 && type == OilType.CrudeOil)
            {
                MessageBroker.Default.Publish(new NotificationMessage()
                {
                    Warning = NotificationType.CargoEmpty
                });
            }
            else if (value != 0 && emptySlot == 0 && type == OilType.CrudeOil)
            {
                MessageBroker.Default.Publish(new NotificationMessage()
                {
                    Warning = NotificationType.RefineryFull
                });
            }
        }

        #endregion
    }
}