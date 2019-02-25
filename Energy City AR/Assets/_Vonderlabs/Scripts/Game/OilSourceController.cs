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
    /// OilSourceController is responsible for controlling the function of the oil source
    /// </summary>
    public class OilSourceController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private int duration;
        [SerializeField] private Transform oilSourceTransform;
        [SerializeField] private OilSourceView oilSourceView;
        [SerializeField] private Collider areaTrigger;
        [SerializeField] private CTAController ctaController;
        private OilSourceModel oilSourceModel;
        private int amount;
        private IDisposable regenerationMessage;
        private IDisposable hitMessage;
        private IDisposable onboardMessage;
        private bool isOnboarding;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            oilSourceModel = new OilSourceModel();
            amount = oilSourceView.GetAmount();
            oilSourceModel.Initialize(amount);
            isOnboarding = false;
            ctaController.StopCtaAnimation();
            onboardMessage = MessageBroker.Default.Receive<OnboardingCTAMessage>().Subscribe(message =>
            {
                if (message.Instruction == Onboarding.OilSource)
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
                    Reset();
                }

                else if (message.AppState == AppState.PlayGame)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        FillData(i);
                        FillImage(i, 1f);
                    }

                    StartLoop();
                }

                else if (message.AppState == AppState.EndGame)
                {
                    regenerationMessage?.Dispose();
                }
            }).AddTo(this);

            areaTrigger.OnTriggerEnterAsObservable().Subscribe(_ =>
            {
                hitMessage = MessageBroker.Default.Receive<RaycastHitMessage>().Subscribe(message =>
                {
                    if (message.Hit.transform == oilSourceTransform)
                    {
                        if (isOnboarding)
                        {
                            MessageBroker.Default.Publish(new OnboardingMessage()
                            {
                                Instruction = Onboarding.RefineryIn
                            });
                            onboardMessage.Dispose();
                            isOnboarding = false;
                        }

                        MessageBroker.Default.Publish(new CargoRequestMessage()
                        {
                            Cargo = OnCargoReceived
                        });
                    }
                }).AddTo(this);
            }).AddTo(this);

            areaTrigger.OnTriggerExitAsObservable().Subscribe(_ => { hitMessage?.Dispose(); }).AddTo(this);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            regenerationMessage?.Dispose();
        }

        /// <summary>
        /// Is called when object is active
        /// </summary>
        private void OnEnable()
        {
            MessageBroker.Default.Publish(new OnboardingMessage()
            {
                Instruction = Onboarding.RemoveAll
            });
            Reset();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// check if there is any empty slot
        /// </summary>
        /// <returns></returns>
        private void StartLoop()
        {
            for (int i = 0; i < amount; i++)
            {
                if (oilSourceModel.IsEmpty(i) == false)
                {
                    regenerationMessage?.Dispose();
                }
                else
                {
                    AutoFill(i);
                    break;
                }
            }
        }

        /// <summary>
        /// auto fill the given slot
        /// </summary>
        /// <param name="imageID"></param>
        private void AutoFill(int imageID)
        {
            float count = oilSourceModel.GetValue(imageID);
            regenerationMessage = MessageBroker.Default.Receive<TimerMessage>().Subscribe(tick =>
                {
                    count++;
                    float percentage = count / duration;
                    FillImage(imageID, percentage);
                    oilSourceModel.SetValue(imageID, count);
                    if (count >= duration)
                    {
                        FillData(imageID);
                        StartLoop();
                    }
                })
                .AddTo(this);
        }

        /// <summary>
        /// extract the given amount of oil
        /// </summary>
        private void ExtractOil(int cargoSlot)
        {
            bool taken = false;

            int availableOil = 0;

            //remove on oil source
            for (int i = 0; i < amount; i++)
            {
                if (oilSourceModel.IsEmpty(i) == false)
                {
                    regenerationMessage?.Dispose();
                    Empty(i);
                    taken = true;
                    availableOil++;
                    cargoSlot--;

                    if (cargoSlot == 0)
                    {
                        break;
                    }
                }
            }

            //send to cargo
            if (availableOil != 0)
            {
                SendOil(availableOil);
            }

            //refill
            if (taken == true)
            {
                StartLoop();
            }
        }

        /// <summary>
        /// Publish the oil to cargo
        /// </summary>
        /// <param name="imageID"></param>
        private void SendOil(int amount)
        {
            MessageBroker.Default.Publish(new SendOilMessage()
            {
                Type = OilType.CrudeOil,
                Amount = amount
            });
        }

        /// <summary>
        /// fill the slot with the given value
        /// </summary>
        /// <param name="imageID"></param>
        /// <param name="value"></param>
        private void FillImage(int imageID, float value)
        {
            oilSourceView.Fill(imageID, value);
        }

        /// <summary>
        /// fill the data model on the given ID
        /// </summary>
        /// <param name="imageID"></param>
        private void FillData(int imageID)
        {
            oilSourceModel.Fill(imageID);
        }

        /// <summary>
        /// emptying the slot
        /// </summary>
        /// <param name="imageID"></param>
        private void Empty(int imageID)
        {
            oilSourceView.Empty(imageID);
            oilSourceModel.Empty(imageID);
        }

        /// <summary>
        /// reset all the slots
        /// </summary>
        /// <param name="imageID"></param>
        private void Reset()
        {
            hitMessage?.Dispose();
            regenerationMessage?.Dispose();
            for (int i = 0; i < amount; i++)
            {
                Empty(i);
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
            int value = cargo.CheckEmpty();
            OilType type = cargo.GetCargoType();
            if (value != 0 && type == OilType.CrudeOil)
            {
                ExtractOil(value);
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
                    Warning = NotificationType.CargoFull
                });
            }
        }

        #endregion
    }
}