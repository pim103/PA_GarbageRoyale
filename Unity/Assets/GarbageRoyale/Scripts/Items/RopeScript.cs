﻿using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{

    public class RopeScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject previewCube;

        [SerializeField]
        private GameObject previewCube2;

        [SerializeField]
        private Renderer material;

        [SerializeField]
        public MeshCollider mc;

        private GameController gc;
        private ItemController ic;

        public int idItem;
        public int idTrap;

        public bool inEditMode;
        private bool toggleCube;
        private bool toggleEditMode;

        private bool canSetPos2;
        public bool isDeployed;

        private float ropeDistance;

        private Vector3 savePos1;
        private Vector3 savePos2;

        private Vector3 scalePreview;
        private Quaternion rotationPreview;

        private Color redColor;
        private Color greenColor;

        private bool canTrigger;

        // Start is called before the first frame update
        void Awake()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            idTrap = -1;
            canTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name.StartsWith("Player"))
            {
                ExposerPlayer ep = other.GetComponent<ExposerPlayer>();

                if (idTrap != -1)
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        return;
                    }

                    TrapInterface item = gc.items[idTrap].GetComponent<TrapInterface>();
                    item.TriggerTrap(ep.PlayerIndex);
                }
                else if(canTrigger)
                {
                    gc.playersActions[ep.PlayerIndex].isFallen = true;
                    gc.playersActions[ep.PlayerIndex].timeLeftFallen = 2.0f;
                    StartCoroutine(TriggerRope());
                }
            }
        }

        private IEnumerator TriggerRope()
        {
            canTrigger = false;

            yield return new WaitForSeconds(3.0f);

            canTrigger = true;
        }
    }
}