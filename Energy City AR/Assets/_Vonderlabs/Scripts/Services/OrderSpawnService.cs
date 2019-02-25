/*
------------------------------------------------------------------
Copyright (c) 2018 Vonderlabs
This software is the proprietary information of Vonderlabs
All rights reserved.
------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using System;
using UniRx;
using PathologicalGames;
using System.Collections.Generic;

namespace Vonderlabs
{
    /// <summary>
    /// Is responsible for spawning a new order
    /// </summary>
    public class OrderSpawnService
    {
        #region MEMBER VARIABLES

        private OrderLibrary orderLibrary;
        private IDisposable spawnMessage;
        private IDisposable despawnMessage;
        private SpawnPool orderPool;

        #endregion
        #region METHODS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        public void Start()
        {
            orderLibrary = Resources.Load("Order Library") as OrderLibrary;
            orderPool = PoolManager.Pools["Orders"];
            spawnMessage = MessageBroker.Default.Receive<SpawnOrderMessage>().Subscribe(message =>
            {
                
                if (orderLibrary.library.ContainsKey(message.Type))
                {
                    Transform spawned = Spawn(orderLibrary.library[message.Type], message.Parent);
                    message.Order.Invoke(spawned.GetComponent<OrderController>());
                }
                else
                {
                    throw new KeyNotFoundException("The requested key " + message.Type + " could not be found.");
                }
            });

            despawnMessage = MessageBroker.Default.Receive<DespawnOrderMessage>().Subscribe(message =>
            {
                if (message.Order == null)
                {
                    DespawnAll();
                }
                else
                {
                    Despawn(message.Order);
                }
            });
        }

        /// <summary>
        /// Dispose all messages
        /// </summary>
        public void Stop()
        {
            spawnMessage.Dispose();
            despawnMessage.Dispose();
        }

       
        /// <summary>
        /// Spawn object from the pool
        /// </summary>
        /// <param name="newOrder"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private RectTransform Spawn(RectTransform newOrder, Transform parent = null)
        {
            RectTransform spawnedOrder = orderPool.Spawn(newOrder, Vector3.zero, Quaternion.identity, parent) as RectTransform;
            return spawnedOrder;
        }

        /// <summary>
        /// Despawn the given object
        /// </summary>
        /// <param name="objectToDespawn"></param>
        private void Despawn(Transform objectToDespawn)
        {
            orderPool.Despawn(objectToDespawn);
        }

        /// <summary>
        /// Despawn all spawned object
        /// </summary>
        private void DespawnAll()
        {
            orderPool.DespawnAll();
        }

        #endregion
    }
}