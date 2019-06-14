using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using GarbageRoyale.Scripts.PrefabPlayer;
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

        private GameController gc;

        private bool isInInventory = false;
        private bool isInEscapeMenu = false;
        
        private SkillsController skillsController;

        private void Start()
        {
            if (scripts.gc.AvatarToUserId[PlayerIndex] != PhotonNetwork.AuthValues.UserId)
            {
                return;
            }

            gc = GameObject.Find("Controller").GetComponent<GameController>();
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

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isInEscapeMenu = !isInEscapeMenu;
            }
            
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isInInventory = !isInInventory;
            }

            if (isInInventory || isInEscapeMenu)
            {
                return;
            }

            if (horizontalAxe != Input.GetAxis("Horizontal"))
            {
                horizontalAxe = Input.GetAxis("Horizontal");
                photonView.RPC("WantToMoveHorizontalRPC", RpcTarget.MasterClient, Input.GetAxis("Horizontal"));
            }

            if (verticalAxe != Input.GetAxis("Vertical"))
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

            if (rotationY != Input.GetAxis("Mouse X"))
            {
                rotationY = Input.GetAxis("Mouse X");
                photonView.RPC("WantToRotateYRPC", RpcTarget.MasterClient, Input.GetAxis("Mouse X"));
            }

            if (Input.GetButtonDown("Jump"))
            {
                wantToJump = true;
                photonView.RPC("WantToJumpRPC", RpcTarget.MasterClient, true);
            }
            else if (Input.GetButtonUp("Jump"))
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
                ActivateSkill(0);
                
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ActivateSkill(1);
            }
            
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log("Running");
                isRunning = true;
                photonView.RPC("WantToRunRPC", RpcTarget.MasterClient, true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                Debug.Log("End Running");
                isRunning = false;
                photonView.RPC("WantToRunRPC", RpcTarget.MasterClient, false);
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Debug.Log("Crouching");
                isCrouched = true;
                photonView.RPC("WantToCrouchRPC", RpcTarget.MasterClient, true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                Debug.Log("End Crouching");
                isCrouched = false;
                photonView.RPC("WantToCrouchRPC", RpcTarget.MasterClient, false);
            }
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
        
        [PunRPC]
        void WantToRunRPC(bool action)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                isRunning = action;
            }
        }
        
        [PunRPC]
        void WantToCrouchRPC(bool action)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                isCrouched = action;
            }
        }

        void ActivateSkill(int place)
        {
            var ray = scripts.gc.players[Array.IndexOf(scripts.gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hitInfo;
            bool touch = Physics.Raycast(ray, out hitInfo, 2f);
            int Hitid = -1;

            if (touch)
            {
                if (hitInfo.transform.name.StartsWith("Player"))
                {
                    Hitid = hitInfo.transform.gameObject.GetComponent<ExposerPlayer>().PlayerIndex;
                }
            }

            skillsController.photonView.RPC("AskSkillActivation", RpcTarget.MasterClient, place, Array.IndexOf(scripts.gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId), Hitid);
        }
    }
}