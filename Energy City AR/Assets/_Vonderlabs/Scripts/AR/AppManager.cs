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
using UnityEngine.Serialization;

namespace Vonderlabs
{
    /// <summary>
    /// Available type of trucks
    /// </summary>
    public enum TruckType
    {
        Tanker,
        HotDog,
        YellowTanker
    }

    /// <summary>
    /// Possible app's status
    /// </summary>
    public enum AppState
    {
        PlaceBoard,
        ChooseTruck,
        ResetGame,
        PlayGame,
        PauseGame,
        EndGame,
        Exit
    }

    /// <summary>
    /// Available building types
    /// </summary>
    public enum ResidenceType
    {
        ResidenceA,
        ResidenceB
    }

    /// <summary>
    /// Available oil types
    /// </summary>
    public enum OilType
    {
        CrudeOil,
        ProcessedOil
    }

    /// <summary>
    /// Possible type of order
    /// </summary>
    public enum OrderType
    {
        normal
    }

    /// <summary>
    /// Possible notifications
    /// </summary>
    public enum NotificationType
    {
        RefineryFull,
        CargoEmpty,
        WrongType,
        CargoFull,
        WrongDestination
    }


    /// <summary>
    /// Possible onboarding instructions
    /// </summary>
    public enum Onboarding
    {
        PlaceBoard,
        MoveCar,
        OilSource,
        RefineryIn,
        Storage,
        ChangeCargo,
        RefineryOut,
        Residence,
        Done,
        RemoveAll
    }

    /// <summary>
    /// AppManager class represents the game manager
    /// </summary>
    public class AppManager : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private int duration;
        private int tutorialDuration;
        private float rateInSecond;

        private SpawnerService spawnerService;
        private TimerService timerService;

        private IDisposable onboardMessage;
        private bool isOnboarding;

        private int currentTime;
        private IDisposable timerMessage;
        private bool isPaused;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            //Screen.orientation = ScreenOrientation.Portrait;
            Screen.orientation = ScreenOrientation.LandscapeRight; 
       
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            timerService = new TimerService();
            spawnerService = new SpawnerService();
            spawnerService.Start();
            rateInSecond = 1;
            tutorialDuration = 999;
            isOnboarding = true;
            isPaused = false;


            onboardMessage = MessageBroker.Default.Receive<OnboardingMessage>().Subscribe(message =>
            {
                if (message.Instruction == Onboarding.Done)
                {
                    StopTimer();
                    MessageBroker.Default.Publish(new AppStateMessage()
                    {
                        AppState = AppState.ResetGame
                    });
                    onboardMessage?.Dispose();
                }
            }).AddTo(this);

            timerMessage = MessageBroker.Default.Receive<TimerMessage>().Subscribe(message =>
            {
                currentTime = (int) message.Value;
            }).AddTo(this);

            MessageBroker.Default.Receive<AppStateMessage>().Subscribe(message =>
            {
                if (message.AppState == AppState.Exit)
                {
                    StopApp();
                }

                else if (message.AppState == AppState.PlayGame)
                {
                    if (isOnboarding)
                    {
                        StartTimer(tutorialDuration);
                        isOnboarding = false;
                    }
                    else if (isPaused)
                    {
                        StartTimer(currentTime);
                        isPaused = false;
                    }
                    else
                    {
                        StartTimer(duration);
                    }
                }

                else if (message.AppState == AppState.PauseGame)
                {
                    StopTimer();
                    isPaused = true;
                }

                else if (message.AppState == AppState.ResetGame)
                {
                    isPaused = false;
                }

                else if (message.AppState == AppState.EndGame)
                {
                    StopTimer();
                }
            }).AddTo(this);

            this.UpdateAsObservable().Subscribe(_ =>
            {
                if (Input.GetKeyDown("escape"))
                {
                    MessageBroker.Default.Publish(new AppStateMessage()
                    {
                        AppState = AppState.Exit
                    });
                }
            }).AddTo(this);
            
            //start
            StartApp();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// is responsible to start the app
        /// </summary>
        private void StartApp()
        {
            MessageBroker.Default.Publish(new AppStateMessage() {AppState = AppState.PlaceBoard});
            MessageBroker.Default.Publish(new OnboardingMessage() {Instruction = Onboarding.PlaceBoard});
        }

        /// <summary>
        /// Start the global timer
        /// </summary>
        private void StartTimer(int time)
        {
            timerService.Start(time, rateInSecond);
        }

        /// <summary>
        /// Stop the global timer
        /// </summary>
        private void StopTimer()
        {
            timerService.Stop();
        }

        /// <summary>
        /// Is responsible to stop the app
        /// </summary>
        private void StopApp()
        {
            timerService.Stop();
            spawnerService?.Stop();
            Application.Quit();
        }

        #endregion
    }
}