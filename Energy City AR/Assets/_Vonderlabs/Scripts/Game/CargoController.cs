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
using System.Threading;
using UniRx;
using UniRx.Triggers;

namespace Vonderlabs
{
    /// <summary>
    /// CargoController is responsible for controlling the truck cargo
    /// </summary>
    public class CargoController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private CargoView oilView;
        [SerializeField] private CargoView processedView;
        [SerializeField] private Transform truckTransform;
        private CargoModel oilModel;
        private CargoModel processedModel;
        private int oilCapacity;
        private int processedCapacity;
        private IDisposable updateMessage;
        private IDisposable typeMessage;
        private IDisposable cargoRequestMessage;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            oilModel = new CargoModel();
            processedModel = new CargoModel();
            oilCapacity = oilView.GetAmount();
            processedCapacity = processedView.GetAmount();
            //default type
            oilModel.Initialize(oilCapacity, OilType.CrudeOil, true);
            processedModel.Initialize(processedCapacity, OilType.ProcessedOil, false);
            TurnOff();
            Reset();
            typeMessage = MessageBroker.Default.Receive<CargoTypeMessage>().Subscribe(message =>
            {
                OilType cargoType = message.Type;
                SetCargoType(cargoType);
            }).AddTo(this);
            this.UpdateAsObservable().Subscribe(_ =>
                {
                    oilView.SetPosition(truckTransform);
                    processedView.SetPosition(truckTransform);
                })
                .AddTo(this);


            cargoRequestMessage = MessageBroker.Default.Receive<CargoRequestMessage>().Subscribe(message =>
                {
                    message.Cargo.Invoke(GetComponent<CargoController>());
                })
                .AddTo(this);
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            MessageBroker.Default.Receive<AppStateMessage>().Subscribe(message =>
            {
                if (message.AppState == AppState.ResetGame)
                {
                    Reset();
                    TurnOff();
                }
                else if (message.AppState == AppState.PlayGame)
                {
                    TurnOnOil();
                }

                else if (message.AppState == AppState.EndGame)
                {
                    TurnOff();
                }
            }).AddTo(this);

            EnableReceiveOilMessage();
            EnableTakeOilMessage();
        }

        /// <summary>
        /// Is called when the object is disabled
        /// </summary>
        private void OnDisable()
        {
            cargoRequestMessage?.Dispose();
            typeMessage?.Dispose();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// enable oil receive function
        /// </summary>
        private void EnableReceiveOilMessage()
        {
            MessageBroker.Default.Receive<SendOilMessage>().Subscribe(message =>
            {
                if (message.Type == GetCargoType())
                {
                    int receivedOil = message.Amount;
                    int index = 0;

                    while (receivedOil > 0)
                    {
                        if (IsEmpty(index))
                        {
                            Fill(index);
                            receivedOil--;
                        }

                        index++;
                    }
                }
            }).AddTo(this);
        }

        /// <summary>
        /// enable take oil message
        /// </summary>    
        private void EnableTakeOilMessage()
        {
            MessageBroker.Default.Receive<TakeCargoMessage>().Subscribe(message =>
            {
                if (message.Type == GetCargoType())
                {
                    int taken = message.Amount;
                    for (int i = 0; i < GetCapacity(); i++)
                    {
                        if (IsEmpty(i) == false)
                        {
                            Empty(i);
                            taken--;
                        }

                        if (taken == 0)
                        {
                            break;
                        }
                    }
                }
            }).AddTo(this);
        }

        /// <summary>
        /// fill the slot
        /// </summary>
        /// <param name="imageID"></param>
        private void Fill(int imageID)
        {
            if (oilModel.IsActive())
            {
                oilModel.Fill(imageID);
                oilView.Fill(imageID);
            }
            else if (processedModel.IsActive())
            {
                processedModel.Fill(imageID);
                processedView.Fill(imageID);
            }
        }

        /// <summary>
        /// emptying the slot
        /// </summary>
        /// <param name="imageID"></param>
        private void Empty(int imageID)
        {
            if (oilModel.IsActive())
            {
                oilModel.Empty(imageID);
                oilView.Empty(imageID);
            }
            else if (processedModel.IsActive())
            {
                processedModel.Empty(imageID);
                processedView.Empty(imageID);
            }
        }

        /// <summary>
        /// reset all the slots
        /// </summary>
        /// <param name="imageID"></param>
        private void Reset()
        {
            for (int i = 0; i < oilCapacity; i++)
            {
                oilModel.Empty(i);
                oilView.Empty(i);
            }

            for (int i = 0; i < processedCapacity; i++)
            {
                processedModel.Empty(i);
                processedView.Empty(i);
            }
        }

        /// <summary>
        /// Check for empty slot
        /// </summary>
        /// <returns></returns>
        public int CheckEmpty()
        {
            int result = 0;

            if (oilModel.IsActive())
            {
                for (int i = 0; i < oilCapacity; i++)
                {
                    if (oilModel.IsEmpty(i))
                    {
                        result++;
                    }
                }
            }
            else if (processedModel.IsActive())
            {
                for (int i = 0; i < processedCapacity; i++)
                {
                    if (processedModel.IsEmpty(i))
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Check if the given slot is empty
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsEmpty(int index)
        {
            bool result = false;

            if (oilModel.IsActive())
            {
                result = oilModel.IsEmpty(index);
            }
            else if (processedModel.IsActive())
            {
                result = processedModel.IsEmpty(index);
            }

            return result;
        }

        /// <summary>
        /// get capacity
        /// </summary>
        /// <returns></returns>
        private int GetCapacity()
        {
            int result = 0;

            if (oilModel.IsActive())
            {
                result = oilCapacity;
            }
            else if (processedModel.IsActive())
            {
                result = processedCapacity;
            }

            return result;
        }

        /// <summary>
        /// check amount of cargo
        /// </summary>
        /// <returns></returns>
        public int GetAmount()
        {
            int result = oilCapacity - CheckEmpty();
            return result;
        }

        /// <summary>
        /// get current type of cargo
        /// </summary>
        /// <returns></returns>
        public OilType GetCargoType()
        {
            OilType result = OilType.CrudeOil;

            if (oilModel.IsActive())
            {
                result = OilType.CrudeOil;
            }
            else if (processedModel.IsActive())
            {
                result = OilType.ProcessedOil;
            }

            return result;
        }


        /// <summary>
        /// Set cargo type
        /// </summary>
        /// <param name="type"></param>
        private void SetCargoType(OilType type)
        {
            if (type == OilType.CrudeOil)
            {
                TurnOnOil();
            }
            else if (type == OilType.ProcessedOil)
            {
                TurnOnProcessed();
            }
        }


        /// <summary>
        /// turn off all canvas
        /// </summary>
        private void TurnOff()
        {
            processedView.TurnOff();
            oilView.TurnOff();
        }

        /// <summary>
        /// turn on oil cargo
        /// </summary>
        private void TurnOnOil()
        {
            processedView.TurnOff();
            processedModel.Deactivate();
            oilView.TurnOn();
            oilModel.Activate();
        }

        /// <summary>
        /// turn on processed cargo
        /// </summary>
        private void TurnOnProcessed()
        {
            processedView.TurnOn();
            processedModel.Activate();
            oilView.TurnOff();
            oilModel.Deactivate();
        }

        #endregion
    }
}