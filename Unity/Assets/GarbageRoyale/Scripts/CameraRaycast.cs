﻿using System;
using System.Collections;
using System.Collections.Generic;
using GarbageRoyale.Scripts.InventoryScripts;
using GarbageRoyale.Scripts.PlayerController;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GarbageRoyale.Scripts
{
    public class CameraRaycast : MonoBehaviourPunCallbacks
    {
        private CameraRaycastHitActions actionScript;
        private PlayerAttack attackScript;
        private SoundManager soundManager;
        private int openingDoorLoading;
        private bool currentlyLoading;
        private GameObject gtest;
        private Inventory playerInventory;

        [SerializeField]
        private GameController gc;

        public int cameraIndex = -1;

        private int lastDoorId = -1;

        // Start is called before the first frame update
        void Start()
        {
            currentlyLoading = false;
            openingDoorLoading = 0;
            //activatedCamera = gc.players[PlayerIndex].PlayerCamera;
            //soundManager = GameObject.Find("Controller").GetComponent<SoundManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if(!gc.endOfInit)
            {
                return;
            }

            var ray = gc.players[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hitInfo;
            bool touch = Physics.Raycast(ray, out hitInfo, 2f);

            if (touch)
            {
                if (hitInfo.transform.name == "LeftDoor" || hitInfo.transform.name == "RightDoor")
                {
                    OpenDoorScript openDoor = hitInfo.transform.parent.parent.GetComponent<OpenDoorScript>();
                    int doorId = openDoor.doorId;
                    lastDoorId = doorId;

                    if (openDoor.isOpen)
                    {
                        Debug.Log("press E");
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            photonView.RPC("soundOpenDoorRPC", RpcTarget.MasterClient, doorId, true);
                            currentlyLoading = true;
                        }

                        if (currentlyLoading)
                        {
                            openingDoorLoading++;
                            Debug.Log("opening : " + openingDoorLoading + "%");
                            if (openingDoorLoading >= 100)
                            {
                                openingDoorLoading = 0;
                                currentlyLoading = false;
                                photonView.RPC("openDoorRPC", RpcTarget.MasterClient, doorId, true, false);
                            }
                        }

                        if (Input.GetKeyUp(KeyCode.E))
                        {
                            photonView.RPC("soundOpenDoorRPC", RpcTarget.MasterClient, doorId, false);
                            currentlyLoading = false;
                            openingDoorLoading = 0;
                        }
                    }
                }

                if(Input.GetKeyDown(KeyCode.F))
                {
                    if (hitInfo.transform.name == "Button")
                    {
                        photonView.RPC("openTrapRPC", RpcTarget.MasterClient, gc.buttonsTrap[hitInfo.transform.parent.gameObject]);
                    }
                    else if (hitInfo.transform.name == "DoorButton")
                    {
                        int doorId = hitInfo.transform.parent.GetComponent<OpenDoorScript>().doorId;
                        photonView.RPC("openDoorRPC", RpcTarget.MasterClient, doorId, false, true);
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    attackScript = GameObject.Find("Controller").GetComponent<PlayerAttack>();
                    Debug.Log(hitInfo.transform.name);
                    if (hitInfo.transform.name == "pipe")
                    {
                        int pipeId = hitInfo.transform.parent.GetComponent<PipeScript>().pipeIndex;
                        photonView.RPC("brokePipeRPC", RpcTarget.MasterClient, pipeId);
                    }
                    else if (hitInfo.transform.name == "Mob(Clone)" || hitInfo.transform.name == "GIANT_RAT_LEGACY")
                    {
                        //hitInfo.transform.GetComponent<MobStats>().takeDamage(Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                        //Debug.Log("Test saucisse de rat");
                        photonView.RPC("HitMobRPC",RpcTarget.MasterClient,hitInfo.transform.GetComponent<MobStats>().id,Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                    }
                    else if (hitInfo.transform.name.StartsWith("Player"))
                    {
                        int idHit = hitInfo.transform.gameObject.GetComponent<ExposerPlayer>().PlayerIndex;
                        attackScript.hitPlayer(hitInfo, idHit);
                    }
                }
            } else if (openingDoorLoading > 0)
            {
                openingDoorLoading = 0;
                currentlyLoading = false;
                if(lastDoorId != -1)
                {
                    photonView.RPC("soundOpenDoorRPC", RpcTarget.MasterClient, lastDoorId, false);
                    lastDoorId = -1;
                }
            }
        }

        [PunRPC]
        private void brokePipeRPC(int pipeId)
        {
            //TODO verify coord
            photonView.RPC("brokeSpecificPipeRPC", RpcTarget.All, pipeId);
        }

        [PunRPC]
        private void brokeSpecificPipeRPC(int pipeId)
        {
            gc.pipes[pipeId].GetComponent<PipeScript>().brokePipe();
        }

        [PunRPC]
        private void openTrapRPC(int trapId)
        {
            //TODO verify coord
            photonView.RPC("openSpecificTrapRPC", RpcTarget.All, trapId);
        }

        [PunRPC]
        private void openSpecificTrapRPC(int trapId)
        {
            gc.traps[trapId].transform.position += new Vector3(4, 0, 0);
            gc.buttonsTrapReversed[trapId].GetComponent<AudioSource>().Play();
        }

        [PunRPC]
        private void openDoorRPC(int doorId, bool playEndSong, bool playButtonSong)
        {
            //TODO verify coord
            photonView.RPC("openSpecificDoorRPC", RpcTarget.All, doorId, playEndSong, playButtonSong);
        }

        [PunRPC]
        private void openSpecificDoorRPC(int doorId, bool playEndSong, bool playButtonSong)
        {
            OpenDoorScript ods = gc.doors[doorId].GetComponent<OpenDoorScript>();
            ods.openDoor();

            if(playEndSong)
            {
                ods.PlayEndOpeningSound();
            }
            if(playButtonSong)
            {
                ods.PlayButtonSound();
            }
        }

        [PunRPC]
        private void soundOpenDoorRPC(int doorId, bool playSound)
        {
            //TODO verify coord
            photonView.RPC("soundOpenSpecificDoorRPC", RpcTarget.All, doorId, playSound);
        }

        [PunRPC]
        private void soundOpenSpecificDoorRPC(int doorId, bool playSound)
        {
            OpenDoorScript ods = gc.doors[doorId].GetComponent<OpenDoorScript>();
            if(playSound)
            {
                ods.PlayOpenSound();
            }
            else
            {
                ods.StopOpenSound();
            }
        }

        [PunRPC]
        private void HitMobRPC(int mobID, int playerIndex)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            gc.mobList[mobID].transform.GetChild(0).GetComponent<MobStats>().takeDamage(playerIndex);
        }

        [PunRPC]
        private void MobDeath(int mobID)
        {
            photonView.RPC("MobDeathAll",RpcTarget.All,mobID,Random.Range(0, 6));
        }
        
        [PunRPC]
        private void MobDeathAll(int mobID, int type)
        {
            MobStats stats = gc.mobList[mobID].GetComponent<MobStats>();
            stats.isDead = true;
            if (!stats.isRotateMob)
            {
                stats.rotateDeadMob();
                stats.lootSkill(type);
                stats.gameObject.SetActive(false);
            }
        }
    }
}    
