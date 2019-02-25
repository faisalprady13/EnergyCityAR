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
    /// is responsible for controlling the function of the refinery out
    /// </summary>
    public class RefineryOutController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private Transform refineryOutTransform;
        [SerializeField] private RefineryOutView refineryOutView;
        [SerializeField] private Collider areaTrigger;
        [SerializeField] private CTAController ctaController;
        private RefineryModel refineryOutModel;
        private int capacity;
        private IDisposable hitMessage;
        private bool isOnboarding;
        private IDisposable onboardMessage;
        [SerializeField] private int duration;
        private IDisposable processMessage;

        #endregion

        #region UNITY FUNCTIONS

        private void OnEnable()
        {
            Reset();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            refineryOutModel = new RefineryModel();
            capacity = refineryOutView.GetCapacity();
            refineryOutModel.Initialize(capacity);
            isOnboarding = false;
            ctaController.StopCtaAnimation();
            onboardMessage = MessageBroker.Default.Receive<OnboardingCTAMessage>().Subscribe(message =>
            {
                if (message.Instruction == Onboarding.RefineryOut)
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
                    if (message.Hit.transform == refineryOutTransform)
                    {
                        if (isOnboarding)
                        {
                            MessageBroker.Default.Publish(new OnboardingMessage() {Instruction = Onboarding.Residence});
                            isOnboarding = false;
                            onboardMessage?.Dispose();
                        }

                        MessageBroker.Default.Publish(new CargoRequestMessage()
                        {
                            Cargo = OnCargoReceived
                        });
                    }
                }).AddTo(this);
            }).AddTo(this);

            areaTrigger.OnTriggerExitAsObservable().Subscribe(_ => { hitMessage?.Dispose(); }).AddTo(this);

            MessageBroker.Default.Receive<RefineryInMessage>().Subscribe(message =>
            {
                MessageBroker.Default.Publish(new RefineryStateMessage()
                {
                    Open = false
                });

                refineryOutView.TurnOn();

                float delay = 0.0f;
                float progress = 0.0f;
                processMessage = MessageBroker.Default.Receive<TimerMessage>().Subscribe(_ =>
                {
                    if (delay >= duration)
                    {
                        for (int i = 0; i < capacity; i++)
                        {
                            FillData(i);
                        }

                        processMessage?.Dispose();
                    }

                    delay++;
                    progress = delay / duration;

                    for (int i = 0; i < capacity; i++)
                    {
                        Fill(i, progress);
                    }
                }).AddTo(this);
            }).AddTo(this);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// extract the given amount of oil
        /// </summary>
        private void ExtractOil(int cargoSlot)
        {
            bool taken = false;
            int availableOil = 0;
            int slot = cargoSlot;
//remove on building
            for (int i = 0; i < capacity; i++)
            {
                if (refineryOutModel.IsEmpty(i) == false)
                {
                    Empty(i);
                    taken = true;
                    availableOil++;
                    slot--;
                    if (slot == 0)
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

//something is taken
            if (taken)
            {
                if (GetEmptySlot() == capacity)
                {
                    MessageBroker.Default.Publish(new RefineryStateMessage()
                    {
                        Open = true
                    });
                    refineryOutView.TurnOff();
                }
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
                Amount = amount,
                Type = OilType.ProcessedOil
            });
        }


        /// <summary>
        /// fill the data model on the given ID
        /// </summary>
        /// <param name="imageID"></param>
        private void FillData(int imageID)
        {
            refineryOutModel.Fill(imageID);
        }

        /// <summary>
        /// fill the canvas on the given ID
        /// </summary>
        /// <param name="imageID"></param>
        private void Fill(int imageID, float value)
        {
            refineryOutView.Fill(imageID, value);
        }

        /// <summary>
        /// emptying the slot
        /// </summary>
        /// <param name="imageID"></param>
        private void Empty(int imageID)
        {
            refineryOutView.Empty(imageID);
            refineryOutModel.Empty(imageID);
        }

        /// <summary>
        /// reset all the slots
        /// </summary>
        /// <param name="imageID"></param>
        private void Reset()
        {
            for (int i = 0; i < capacity; i++)
            {
                Empty(i);
            }
        }

        /// <summary>
        /// Get amount of empty slot
        /// </summary>
        /// <returns></returns>
        private int GetEmptySlot()
        {
            int result = 0;
            for (int i = 0; i < capacity; i++)
            {
                if (refineryOutModel.IsEmpty(i))
                {
                    result++;
                }
            }

            return result;
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
            if (value != 0 && type == OilType.ProcessedOil)
            {
                ExtractOil(value);
            }
            else if (type != OilType.ProcessedOil)
            {
                MessageBroker.Default.Publish(new NotificationMessage()
                {
                    Warning = NotificationType.WrongType
                });
            }
            else if (value == 0 && type == OilType.ProcessedOil)
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