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
using System.Collections.Generic;
using UniRx;
using UnityEditor;

namespace Vonderlabs
{
    /// <summary>
    /// OnboardingController represents the onboarding manager
    /// </summary>
    public class OnboardingController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private OnboardingView onboardingView;
        private IDisposable timer;
        private IDisposable onboardMessage;
        private Dictionary<Onboarding, bool> onboardingDictionary;

        #endregion


        #region UNITY FUNCTIONS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Awake()
        {
            onboardingDictionary = new Dictionary<Onboarding, bool>();

            onboardingDictionary.Add(Onboarding.PlaceBoard, true);
            onboardingDictionary.Add(Onboarding.RemoveAll, true);
            onboardingDictionary.Add(Onboarding.OilSource, false);
            onboardingDictionary.Add(Onboarding.Done, false);
            onboardingDictionary.Add(Onboarding.Residence, false);
            onboardingDictionary.Add(Onboarding.Storage, false);
            onboardingDictionary.Add(Onboarding.ChangeCargo, false);
            onboardingDictionary.Add(Onboarding.MoveCar, false);
            onboardingDictionary.Add(Onboarding.RefineryIn, false);
            onboardingDictionary.Add(Onboarding.RefineryOut, false);

            onboardMessage = MessageBroker.Default.Receive<OnboardingMessage>().Subscribe(message =>
            {
                bool result = false;

                Debug.Log("onboardMessage in"+message.Instruction);


                if (onboardingDictionary.TryGetValue(message.Instruction, out result))
                {
                    Debug.Log("success " + message.Instruction + " adfs " + result);
                    //success!
                    if (result)
                    {
                        if (message.Instruction == Onboarding.RemoveAll)
                        {
                            onboardingView.TurnOffPanel();
                        }
                        else
                        {
                            SetInstruction(message.Instruction);

                            onboardingView.TurnOnPanel();
                        }

                        MessageBroker.Default.Publish(new OnboardingCTAMessage
                        {
                            Instruction = message.Instruction
                        });
                    }
                }
            }).AddTo(this);
        }

        #endregion

        #region METHODS     

        /// <summary>
        /// Set the instruction for the onboarding
        /// </summary>
        /// <param name="instruction"></param>
        private void SetInstruction(Onboarding instruction)
        {
            if (instruction == Onboarding.PlaceBoard)
            {
                SetText("Detect a surface and tap to spawn gameboard");
                onboardingDictionary[instruction] = false;
                onboardingDictionary[Onboarding.MoveCar] = true;
            }
            else if (instruction == Onboarding.MoveCar)
            {
                SetText("Control the car from the left area");
                onboardingDictionary[instruction] = false;
                onboardingDictionary[Onboarding.OilSource] = true;
            }
            else if (instruction == Onboarding.OilSource)
            {
                SetText("Go to oil source and tap to collect 3 crude oils");
                onboardingDictionary[instruction] = false;
                onboardingDictionary[Onboarding.RefineryIn] = true;
            }
            else if (instruction == Onboarding.RefineryIn)
            {
                SetText("Deliver 3 crude oil to refinery to get processed");
                onboardingDictionary[instruction] = false;
                onboardingDictionary[Onboarding.Storage] = true;
            }
            else if (instruction == Onboarding.Storage)
            {
                SetText("To let oil source regenerates, storage oils here");
                onboardingDictionary[instruction] = false;
                onboardingDictionary[Onboarding.ChangeCargo] = true;
            }
            else if (instruction == Onboarding.ChangeCargo)
            {
                SetText("Change the cargo to pick different oil type");
                onboardingDictionary[instruction] = false;
                onboardingDictionary[Onboarding.RefineryOut] = true;
            }
            else if (instruction == Onboarding.RefineryOut)
            {
                SetText("Tap on the refinery to collect processed oil");
                onboardingDictionary[instruction] = false;
                onboardingDictionary[Onboarding.Residence] = true;
            }
            else if (instruction == Onboarding.Residence)
            {
                SetText("Deliver to the residence to fulfill the order");
                onboardingDictionary[instruction] = false;
                onboardingDictionary[Onboarding.Done] = true;
            }
            else if (instruction == Onboarding.Done)
            {
                SetText("You finished the tutorial! click play to start");
                int delay = 5;
                timer = MessageBroker.Default.Receive<TimerMessage>().Subscribe(_ =>
                {
                    if (delay < 0)
                    {
                        onboardingView.TurnOffPanel();
                        timer?.Dispose();
                    }

                    delay--;
                }).AddTo(this);
                onboardMessage?.Dispose();
            }
        }

        /// <summary>
        /// Set the warning text
        /// </summary>
        /// <param name="warningText"></param>
        private void SetText(String warningText)
        {
            onboardingView.SetText(warningText);
        }

        #endregion
    }
}