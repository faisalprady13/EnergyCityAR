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
    /// Is responsible for spawning a new truck
    /// </summary>
    public class SpawnerService
    {
        #region MEMBER VARIABLES

        private TruckLibrary truckLibrary;
        private IDisposable spawnMessage;
        private IDisposable despawnMessage;
        private SpawnPool truckPool;

        #endregion

        #region METHODS

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        public void Start()
        {
            truckLibrary = Resources.Load("Truck Library") as TruckLibrary;
            truckPool = PoolManager.Pools["Trucks"];

            spawnMessage = MessageBroker.Default.Receive<SpawnTruckMessage>().Subscribe(message =>
            {
                if (truckLibrary.library.ContainsKey(message.Type))
                {
                    Transform spawned = Spawn(truckLibrary.library[message.Type]);
                    message.Truck.Invoke(spawned.GetComponent<Transform>());
                }
                else
                {
                    throw new KeyNotFoundException("The requested key " + message.Type + " could not be found.");
                }
            });

            despawnMessage = MessageBroker.Default.Receive<DespawnTruckMessage>().Subscribe(message =>
            {
                if (message.Truck == null)
                {
                    DespawnAll();
                }
                else
                {
                    Despawn(message.Truck);
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
        /// Spawn the given object from the pool
        /// </summary>
        /// <param name="newBlock">block to spawn</param>
        /// <returns>spawned block</returns>
        private Transform Spawn(Transform newTruck)
        {
            Transform spawnedTruck = truckPool.Spawn(newTruck, Vector3.zero, Quaternion.identity);
            return spawnedTruck;
        }

        /// <summary>
        /// Despawn the given object
        /// </summary>
        /// <param name="truckToDespawn"></param>
        private void Despawn(Transform truckToDespawn)
        {
            truckPool.Despawn(truckToDespawn);
        }

        /// <summary>
        /// Despawn all spawned object
        /// </summary>
        private void DespawnAll()
        {
            truckPool.DespawnAll();
        }

        #endregion
    }
}