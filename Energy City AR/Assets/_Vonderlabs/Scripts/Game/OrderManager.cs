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
using UniRx.Triggers;
using Random = UnityEngine.Random;

namespace Vonderlabs
{
    /// <summary>
    /// OrderManager class is responsible for managing the order system
    /// </summary>
    public class OrderManager : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] private CanvasGroup orderPanel;
        [SerializeField] private int minTime;
        [SerializeField] private int maxTime;
        private List<OrderController> orders;
        private IDisposable timerMessage;
        private OrderSpawnService orderSpawnService;
        private float panelWidth;
        private float panelHeight;
        private float panelXpos;
        private float panelYpos;
        private IDisposable despawnEffect;
        private IDisposable spawnEffect;
        private float speed;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            orderSpawnService = new OrderSpawnService();
            orderSpawnService.Start();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            timerMessage?.Dispose();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            orders = new List<OrderController>();
            speed = 1;
            panelWidth = orderPanel.rectTransform().sizeDelta.x;
            panelHeight = orderPanel.rectTransform().sizeDelta.y;
            panelXpos = orderPanel.rectTransform().anchoredPosition.x;
            panelYpos = orderPanel.rectTransform().anchoredPosition.y;
            int randomValue = Random.Range(minTime, maxTime);

            MessageBroker.Default.Receive<AppStateMessage>().Subscribe(message =>
            {
                if (message.AppState == AppState.PlayGame)
                {
                    //use order spawn service to spawn new
                    timerMessage = MessageBroker.Default.Receive<TimerMessage>().Subscribe(tick =>
                    {
                        //first spawn
                        if (orders.Count <= 0)
                        {
                            SpawnOrder(OrderType.normal, orderPanel.transform);
                            randomValue = Random.Range(minTime, maxTime);
                        }

                        if (orders.Count < 3)
                        {
                            if (randomValue <= 0)
                            {
                                SpawnOrder(OrderType.normal, orderPanel.transform);
                                randomValue = Random.Range(minTime, maxTime);
                            }

                            randomValue--;
                        }
                    }).AddTo(this);
                }
                else
                {
                    timerMessage?.Dispose();
                    DespawnOrder(null);
                    orders.Clear();
                }
            }).AddTo(this);

            MessageBroker.Default.Receive<OrderTimesUpMessage>().Subscribe(message =>
            {
                if (message.TimesUp)
                {
                    int index = 0;
                    foreach (var order in orders)
                    {
                        if (order.GetTime() <= 0)
                        {
                            orders.RemoveAt(index);
                            StartDespawnAnimation(order);
                            DecreaseScore(1);
                            break;
                        }

                        index++;
                    }
                }
            }).AddTo(this);

            MessageBroker.Default.Receive<CheckOrderMessage>().Subscribe(message =>
            {
                int cargoAmount = message.Amount;
                OilType cargoOilType = message.Oil;
                ResidenceType cargoDestination = message.Residence;

                Debug.Log(cargoDestination);

                CheckOrder(cargoDestination, cargoOilType, cargoAmount);
            }).AddTo(this);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Is called to check if the received cargo match the order
        /// </summary>
        /// <param name="cargoDestination"></param>
        /// <param name="cargoOilType"></param>
        /// <param name="cargoAmount"></param>
        private void CheckOrder(ResidenceType cargoDestination, OilType cargoOilType, int cargoAmount)
        {
            int index = 0;
            bool isWrongDestination = true;
            foreach (var order in orders)
            {
                if (cargoDestination == order.GetDestination() && cargoOilType == order.GetOrder())
                {
                    int orderAmount = order.GetAmount();
                    int deliveredAmount = 0;
                    while (orderAmount != 0)
                    {
                        deliveredAmount++;
                        cargoAmount--;
                        orderAmount--;

                        if (cargoAmount == 0)
                        {
                            break;
                        }
                    }

                    if (orderAmount != 0)
                    {
                        order.SetOrder(order.GetOrder(), orderAmount);
                    }
                    else if (orderAmount == 0)
                    {
                        //order done, publish reward, remove this order
                        order.StopTimer();
                        orders.RemoveAt(index);
                        StartDespawnAnimation(order);
                        IncreaseScore(3);
                    }

                    //decrease cargo with delivered amount
                    MessageBroker.Default.Publish(new TakeCargoMessage()
                    {
                        Amount = deliveredAmount,
                        Type = cargoOilType
                    });
                    isWrongDestination = false;
                    break;
                }

                index++;
            }

            if (isWrongDestination)
            {
                MessageBroker.Default.Publish(new NotificationMessage()
                {
                    Warning = NotificationType.WrongDestination
                });
            }
        }


        /// <summary>
        /// Use to decrease score
        /// </summary>
        private void DecreaseScore(int score)
        {
            MessageBroker.Default.Publish(new PointMessage()
            {
                Point = -score
            });
        }

        /// <summary>
        /// Use to increase score
        /// </summary>
        private void IncreaseScore(int score)
        {
            MessageBroker.Default.Publish(new PointMessage()
            {
                Point = score
            });
        }

        /// <summary>
        /// Is called to spawn new order
        /// </summary>
        /// <param name="orderType"></param>
        /// <param name="canvasOrder"></param>
        private void SpawnOrder(OrderType orderType, Transform canvasOrder)
        {
            MessageBroker.Default.Publish(new SpawnOrderMessage()
            {
                Type = orderType,
                Parent = canvasOrder,
                Order = OnOrderControllerReceived
            });
        }

        /// <summary>
        /// Is called to despawn an order
        /// </summary>
        /// <param name="order"></param>
        private void DespawnOrder(OrderController order)
        {
            Transform toDespawn;
            if (order == null)
            {
                toDespawn = null;
            }
            else
            {
                toDespawn = order.transform;
            }

            MessageBroker.Default.Publish(new DespawnOrderMessage()
            {
                Order = toDespawn
            });

            RescaleOrderPanel();
        }


        /// <summary>
        /// Rescale orders panel based on the current amount of order
        /// </summary>
        private void RescaleOrderPanel()
        {
            int orderAmount = orders.Count;
            if (orderAmount > 0)
            {
                orderPanel.rectTransform().sizeDelta = new Vector2(panelWidth * (orders.Count), panelHeight);

                orderPanel.rectTransform().anchoredPosition =
                    new Vector2(panelXpos + (panelWidth / 2 * (orders.Count - 1)), panelYpos);
            }
        }

        /// <summary>
        /// Is used to set type of oil for an order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="oil"></param>
        private void SetOrder(OrderController order, OilType oil, int amount)
        {
            order.SetOrder(oil, amount);
        }

        /// <summary>
        /// Is used to set the destination for an order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="residence"></param>
        private void SetDestination(OrderController order, ResidenceType residence)
        {
            order.SetDestination(residence);
        }

        /// <summary>
        /// start spawn fade animation
        /// </summary>
        /// <param name="order"></param>
        public void StartSpawnAnimation(OrderController order)
        {
            CanvasGroup canvasGroup = order.GetCanvasGroup();
            float scale;
            spawnEffect = this.UpdateAsObservable().Subscribe(_ =>
                {
                    scale = speed * Time.deltaTime;
                    canvasGroup.alpha += scale;
                    if (canvasGroup.alpha >= 1)
                    {
                        spawnEffect?.Dispose();
                    }
                })
                .AddTo(this);
        }

        /// <summary>
        /// Start despawn fade animation
        /// </summary>
        /// <param name="order"></param>
        public void StartDespawnAnimation(OrderController order)
        {
            CanvasGroup canvasGroup = order.GetCanvasGroup();
            float scale;
            despawnEffect = this.UpdateAsObservable().Subscribe(_ =>
                {
                    scale = speed * Time.deltaTime;
                    canvasGroup.alpha -= scale;
                    if (canvasGroup.alpha <= 0)
                    {
                        DespawnOrder(order);
                        despawnEffect?.Dispose();
                    }
                })
                .AddTo(this);
        }

        /// <summary>
        /// Show interactive UI
        /// </summary>
        /// <param name="canvasGroup"></param>
        private void TurnOn(CanvasGroup canvasGroup)
        {
            canvasGroup.transform.SetAsLastSibling();
            canvasGroup.alpha = 1;
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

        #region EVENT HANDLERS

        /// <summary>
        /// Is called when spawned order controller received
        /// </summary>
        /// <param name="orderController"></param>
        private void OnOrderControllerReceived(OrderController orderController)
        {
            TurnOff(orderController.GetCanvasGroup());
            StartSpawnAnimation(orderController);
            orders.Add(orderController);
            RescaleOrderPanel();
            //set the order's information randomly


            ResidenceType randomResidence =
                (ResidenceType) Random.Range(0, System.Enum.GetValues(typeof(ResidenceType)).Length);

            SetOrder(orderController, OilType.ProcessedOil, 2);
            SetDestination(orderController, randomResidence);
        }

        #endregion
    }
}