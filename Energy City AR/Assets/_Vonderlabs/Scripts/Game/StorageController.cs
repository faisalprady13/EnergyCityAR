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
    /// StorageController represents the storage controller
    /// </summary>
    public class StorageController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private StorageView storageView;
        private StorageModel storageModel;
        [SerializeField] private Collider areaTrigger;
        private IDisposable hitMessage;

        [SerializeField] private Transform storageTransform;
        [SerializeField] private CTAController ctaController;
        private IDisposable onboardMessage;
        private int capacity;
        private bool storing;
        private bool isOnboarding;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            storageModel = new StorageModel();
            capacity = storageView.GetCapacity();
            storageModel.Initialize(capacity);
            storing = true;
            isOnboarding = false;

            ctaController.StopCtaAnimation();

            onboardMessage = MessageBroker.Default.Receive<OnboardingCTAMessage>().Subscribe(message =>
            {
                if (message.Instruction == Onboarding.Storage)
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
                    if (message.Hit.transform == storageTransform)
                    {
                        if (isOnboarding)
                        {
                            MessageBroker.Default.Publish(
                                new OnboardingMessage() {Instruction = Onboarding.ChangeCargo});
                            isOnboarding = false;
                            onboardMessage.Dispose();
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

        #endregion

        #region METHODS

        /// <summary>
        /// Autofill the refinery slot with the given amount of oil
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        private void FillOil(int value)
        {
            int leftOnCargo = value;
            for (int i = 0; i < capacity; i++)
            {
                if (storageModel.IsEmpty(i))
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
        }


        /// <summary>
        /// extract the given amount of oil
        /// </summary>
        private void ExtractOil(int cargoSlot)
        {
            int availableOil = 0;

            int slot = cargoSlot;
            //remove on building
            for (int i = 0; i < capacity; i++)
            {
                if (storageModel.IsEmpty(i) == false)
                {
                    Empty(i);
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
                Type = OilType.CrudeOil
            });
        }

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
            storageModel.Fill(imageID);
            storageView.Fill(imageID);
        }

        /// <summary>
        /// emptying the slot
        /// </summary>
        /// <param name="imageID"></param>
        private void Empty(int imageID)
        {
            storageView.Empty(imageID);
            storageModel.Empty(imageID);
        }

        /// <summary>
        /// reset all the slots
        /// </summary>
        /// <param name="imageID"></param>
        private void EmptyAll()
        {
            for (int i = 0; i < capacity; i++)
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
            for (int i = 0; i < capacity; i++)
            {
                if (storageModel.IsEmpty(i))
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
            OilType type = cargo.GetCargoType();
            if (type == OilType.CrudeOil)
            {
                //2 things happen, store and extract
                if (storing)
                {
                    int value = cargo.GetAmount();
                    //fill
                    if (value != 0)
                    {
                        FillOil(value);
                    }

                    storing = false;
                }
                else if (!storing)
                {
                    int value = cargo.CheckEmpty();
                    //extract

                    if (value != 0)
                    {
                        ExtractOil(value);
                    }

                    storing = true;
                }
            }
        }

        #endregion
    }
}