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
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

namespace Vonderlabs
{
    /// <summary>
    /// ARController class is responsible for controlling the AR function
    /// </summary>
    public class ARController : MonoBehaviour
    {
        #region MEMBER VARIABLES

        [SerializeField] [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        GameObject m_PlacedPrefab;

        [SerializeField] private Camera mainCamera;
        ARSessionOrigin m_SessionOrigin;

        [SerializeField] private ARPlaneManager arPlaneManager;
        [SerializeField] private ARPointCloudManager arPointCloudManager;
        private List<ARPlane> arPlanes;

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        private Transform mainTruck;
        private Transform mainBoard;
        [SerializeField] private float positionOffset;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The prefab to instantiate on touch.
        /// </summary>
        public GameObject placedPrefab
        {
            get { return m_PlacedPrefab; }
            set { m_PlacedPrefab = value; }
        }

        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject spawnedBoard { get; private set; }

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            m_SessionOrigin = GetComponent<ARSessionOrigin>();
            MessageBroker.Default.Receive<CameraRequestMessage>().Subscribe(message =>
            {
                message.MainCamera.Invoke(mainCamera);
            }).AddTo(this);
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            arPlanes = new List<ARPlane>();
            MessageBroker.Default.Receive<AppStateMessage>().Subscribe(message =>
            {
                this.UpdateAsObservable().Subscribe(_ =>
                {
                    if (Input.touchCount > 0 && message.AppState == AppState.PlaceBoard)
                    {
                        Touch touch = Input.GetTouch(0);

                        PlaceBoard(touch);
                    }
                }).AddTo(this);
            });
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Place an AR object on the given position
        /// </summary>
        /// <param name="touch">contains position</param>
        private void PlaceBoard(Touch touch)
        {
            if (m_SessionOrigin.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = s_Hits[0].pose;

                if (spawnedBoard == null)
                {
                    spawnedBoard = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    spawnedBoard.transform.position = new Vector3(spawnedBoard.transform.position.x,
                        spawnedBoard.transform.position.y, spawnedBoard.transform.position.z + positionOffset);
                    mainBoard = spawnedBoard.transform;

                    MessageBroker.Default.Receive<BoardRequestMessage>().Subscribe(message =>
                    {
                        message.MainBoard.Invoke(getMainBoard());
                    });
                    MessageBroker.Default.Publish(new AppStateMessage() {AppState = AppState.ChooseTruck});

                    //remove after spawn
                    arPlaneManager.enabled = false;
                    arPointCloudManager.enabled = false;
                    arPlaneManager.GetAllPlanes(arPlanes);
                    if (arPlanes.Count > 0)
                    {
                        foreach (var plane in arPlanes)
                        {
                            Destroy(plane.gameObject);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Is called to get the main AR board
        /// </summary>
        /// <returns></returns>
        private Transform getMainBoard()
        {
            return mainBoard;
        }

        #endregion
    }
}