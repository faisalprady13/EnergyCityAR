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
using UnityEngine.UI;

namespace Vonderlabs
{
    /// <summary>
    /// MenuController class is responsible to manage all the menu 
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private CanvasGroup pauseMenu;
        [SerializeField] private CanvasGroup pauseButtonPanel;
        [SerializeField] private Button playOnPauseButton;
        [SerializeField] private Button exitOnPauseButton;
        [SerializeField] private Button resetOnPauseButton;
        [SerializeField] private Button pauseButton;

        [SerializeField] private Text pointText;
        [SerializeField] private CanvasGroup pointPanel;
        [SerializeField] private Text timerText;
        [SerializeField] private CanvasGroup timerPanel;
        [SerializeField] private Button startButton;
        [SerializeField] private CanvasGroup startMenu;
        private IDisposable timerMessage;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private CanvasGroup endMenu;
        [SerializeField] private Text endScoreText;
        private IDisposable pointMessage;
        private PointModel pointModel;
        private bool isOnboarding;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            pointModel = new PointModel();

            TurnOff(startMenu);
            TurnOff(timerPanel);
            TurnOff(pointPanel);
            TurnOff(endMenu);
            TurnOff(pauseMenu);
            TurnOff(pauseButtonPanel);

            isOnboarding = true;


            MessageBroker.Default.Receive<OnboardingMessage>().Subscribe(message =>
            {
                if (message.Instruction == Onboarding.Done)
                {
                    isOnboarding = false;
                }
            }).AddTo(this);

            MessageBroker.Default.Receive<AppStateMessage>().Subscribe(message =>
            {
                if (message.AppState == AppState.ResetGame)
                {
                    timerMessage?.Dispose();
                    pointMessage?.Dispose();
                    pointModel.SetPoint(0);
                    pointText.text = pointModel.GetPoint().ToString("0");
                    TurnOff(pointPanel);
                    TurnOff(timerPanel);
                    TurnOn(startMenu);
                    TurnOff(endMenu);
                    TurnOff(pauseMenu);
                    TurnOff(pauseButtonPanel);

                    timerMessage = MessageBroker.Default.Receive<TimerMessage>().Subscribe(tick =>
                        {
                            timerText.text = tick.Value.ToString("0");
                        })
                        .AddTo(this);

                    pointMessage = MessageBroker.Default.Receive<PointMessage>().Subscribe(point =>
                    {
                        int currentPoint = pointModel.GetPoint() + point.Point;
                        pointText.text = currentPoint.ToString("0");
                        pointModel.SetPoint(currentPoint);
                    }).AddTo(this);
                }
                else if (message.AppState == AppState.PlayGame)
                {
                    if (!isOnboarding)
                    {
                        TurnOn(timerPanel);
                        TurnOn(pointPanel);
                    }

                    TurnOff(startMenu);
                    TurnOff(pauseMenu);
                    TurnOn(pauseButtonPanel);
                }
                else if (message.AppState == AppState.EndGame)
                {
                    timerMessage?.Dispose();
                    pointMessage?.Dispose();
                    endScoreText.text = "Your Score:" + pointModel.GetPoint().ToString("0");
                    TurnOn(endMenu);
                    TurnOff(pauseButtonPanel);
                }
                else if (message.AppState == AppState.PauseGame)
                {
                    TurnOn(pauseMenu);
                    TurnOff(pauseButtonPanel);
                }
                else
                {
                    TurnOff(startMenu);
                    TurnOff(timerPanel);
                    TurnOff(pointPanel);
                }
            }).AddTo(this);

            pauseButton.OnClickAsObservable().Subscribe(_ =>
            {
                MessageBroker.Default.Publish(new AppStateMessage()
                {
                    AppState = AppState.PauseGame
                });
            }).AddTo(this);

            startButton.OnClickAsObservable().Subscribe(_ =>
            {
                MessageBroker.Default.Publish(new AppStateMessage()
                {
                    AppState = AppState.PlayGame
                });
            }).AddTo(this);

            playOnPauseButton.OnClickAsObservable().Subscribe(_ =>
            {
                MessageBroker.Default.Publish(new AppStateMessage()
                {
                    AppState = AppState.PlayGame
                });
            }).AddTo(this);


            resetOnPauseButton.OnClickAsObservable().Subscribe(_ =>
            {
                MessageBroker.Default.Publish(new AppStateMessage()
                {
                    AppState = AppState.ResetGame
                });
            }).AddTo(this);

            resetButton.OnClickAsObservable().Subscribe(_ =>
            {
                MessageBroker.Default.Publish(new AppStateMessage()
                {
                    AppState = AppState.ResetGame
                });
            }).AddTo(this);

            exitButton.OnClickAsObservable().Subscribe(_ =>
            {
                MessageBroker.Default.Publish(new AppStateMessage()
                {
                    AppState = AppState.Exit
                });
            }).AddTo(this);

            exitOnPauseButton.OnClickAsObservable().Subscribe(_ =>
            {
                MessageBroker.Default.Publish(new AppStateMessage()
                {
                    AppState = AppState.Exit
                });
            }).AddTo(this);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Show interactive UI
        /// </summary>
        /// <param name="canvasGroup"></param>
        private void TurnOn(CanvasGroup canvasGroup)
        {
            canvasGroup.transform.SetAsLastSibling();
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// Hide UI
        /// </summary>
        /// <param name="canvasGroup"></param>
        private void TurnOff(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        #endregion
    }
}