/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using UniRx;
using System;
using TMPro;

namespace Vonderlabs
{
    /// <summary>
    /// Is providing a timer with given amount of time
    /// </summary>
    public class TimerService
    {
        #region MEMBER VARIABLES

        private IDisposable timer;

        #endregion

        #region METHODS

        /// <summary>
        /// Is used to publish tick
        /// </summary>
        public void Start(int duration, float rateInSecond)
        {
            //first second
            MessageBroker.Default.Publish(new TimerMessage() {Value = duration});
            timer = Observable.Interval(TimeSpan.FromSeconds(rateInSecond))
                .Take(duration)
                .StartWith(-1)
                .Select(count => duration - count - 1)
                .Subscribe(
                    count => MessageBroker.Default.Publish(new TimerMessage() {Value = count}),
                    () => MessageBroker.Default.Publish(new AppStateMessage() {AppState = AppState.EndGame})
                );
        }

        /// <summary>
        /// Is used to dispose ticker message
        /// </summary>
        public void Stop()
        {
            timer?.Dispose();
        }

        #endregion
    }
}