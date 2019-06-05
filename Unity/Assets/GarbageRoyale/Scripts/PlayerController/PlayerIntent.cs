﻿using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GarbageRoyale.Scripts.PlayerController {
    public class PlayerIntent : ListPlayerIntents
    {
        [FormerlySerializedAs("PlayerActorId")]
        [SerializeField]
        public int PlayerIndex;

        [SerializeField]
        private PhotonView photonView;

        [SerializeField]
        private ScriptExposer scripts;

        private bool isInInventory = false;

        private SkillsController skillsController;

        private void Start()
        {
            if (scripts.gc.AvatarToUserId[PlayerIndex] != PhotonNetwork.AuthValues.UserId)
            {
                return;
            }

            skillsController = GameObject.Find("SkillsController").GetComponent<SkillsController>();
            scripts.cr.cameraIndex = PlayerIndex;
        }

        // Update is called once per frame
        void Update()
        {
            if (scripts.gc.AvatarToUserId[PlayerIndex] != PhotonNetwork.AuthValues.UserId)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isInInventory = !isInInventory;
            }
            if (isInInventory)
            {
                return;
            }

            if(horizontalAxe != Input.GetAxis("Horizontal"))
            {
                horizontalAxe = Input.GetAxis("Horizontal");
                photonView.RPC("WantToMoveHorizontalRPC", RpcTarget.MasterClient, Input.GetAxis("Horizontal"));
            }
            
            if(verticalAxe != Input.GetAxis("Vertical"))
            {
                verticalAxe = Input.GetAxis("Vertical");
                photonView.RPC("WantToMoveVerticalRPC", RpcTarget.MasterClient, Input.GetAxis("Vertical"));
            }

            /*
            if(Input.GetKeyDown(KeyCode.Z))
            {
                wantToGoForward = true;
                photonView.RPC("WantToMoveForwardRPC", RpcTarget.MasterClient, wantToGoForward);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                wantToGoBackward = true;
                photonView.RPC("WantToMoveBackwardRPC", RpcTarget.MasterClient, wantToGoBackward);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                wantToGoLeft = true;
                photonView.RPC("WantToMoveLeftRPC", RpcTarget.MasterClient, wantToGoLeft);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                wantToGoRight = true;
                photonView.RPC("WantToMoveRightRPC", RpcTarget.MasterClient, wantToGoRight);
            }
            */

            if (rotationX != Input.GetAxis("Mouse Y"))
            {
                rotationX = Input.GetAxis("Mouse Y");
                photonView.RPC("WantToRotateXRPC", RpcTarget.MasterClient, Input.GetAxis("Mouse Y"));
            }

            if(rotationY != Input.GetAxis("Mouse X"))
            {
                rotationY = Input.GetAxis("Mouse X");
                photonView.RPC("WantToRotateYRPC", RpcTarget.MasterClient, Input.GetAxis("Mouse X"));
            }

            if (Input.GetButtonDown("Jump"))
            {
                wantToJump = true;
                photonView.RPC("WantToJumpRPC", RpcTarget.MasterClient, true);
            }
            else if(Input.GetButtonUp("Jump"))
            {
                wantToJump = false;
                photonView.RPC("WantToJumpRPC", RpcTarget.MasterClient, false);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                photonView.RPC("WantToLightUpRPC", RpcTarget.MasterClient);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                photonView.RPC("WantToUseTorch", RpcTarget.MasterClient);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                //scripts.gc.playersActions[PlayerIndex].isQuiet = true;
                skillsController.photonView.RPC("AskSkillActivation",RpcTarget.MasterClient,0,Array.IndexOf(scripts.gc.AvatarToUserId,PhotonNetwork.AuthValues.UserId));
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                //scripts.gc.playersActions[PlayerIndex].isQuiet = true;
                skillsController.photonView.RPC("AskSkillActivation",RpcTarget.MasterClient,1,Array.IndexOf(scripts.gc.AvatarToUserId,PhotonNetwork.AuthValues.UserId));
            }
        }

        void FixedUpdate()
        {
            if (scripts.gc.AvatarToUserId[PlayerIndex] != PhotonNetwork.AuthValues.UserId)
            {
                return;
            }
            /*
            if (!PhotonNetwork.IsMasterClient)
            {
                scripts.pcm.PlayerMovement(PlayerIndex);
                scripts.pcm.PlayerRotation(PlayerIndex);
            }
            */
        }

        [PunRPC]
        void WantToMoveHorizontalRPC(float axe)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                horizontalAxe = axe;
            }
        }

        [PunRPC]
        void WantToMoveVerticalRPC(float axe)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                verticalAxe = axe;
            }
        }

        [PunRPC]
        void WantToRotateXRPC(float rot)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                rotationX = rot;
            }
        }

        [PunRPC]
        void WantToRotateYRPC(float rot)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                rotationY = rot;
            }
        }

        [PunRPC]
        void WantToJumpRPC(bool action)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                wantToJump = action;
            }
        }

        [PunRPC]
        void WantToLightUpRPC()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (wantToLightUp)
                {
                    wantToLightUp = false;
                }
                else
                {
                    wantToLightUp = true;
                }
            }
        }

        [PunRPC]
        void WantToUseTorch()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (wantToLightUp)
                {
                    wantToTurnOnTorch = false;
                }
                else
                {
                    wantToTurnOnTorch = true;
                }
            }
        }
    }
}