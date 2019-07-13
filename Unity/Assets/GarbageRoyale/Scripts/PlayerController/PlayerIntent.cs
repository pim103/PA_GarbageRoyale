using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using GarbageRoyale.Scripts.GameMaster;
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
        
        private SkillsController skillsController;

        private void Start()
        {
            if (!PhotonNetwork.OfflineMode)
            {
                if (scripts.gc.AvatarToUserId[PlayerIndex] != PhotonNetwork.AuthValues.UserId)
                {
                    return;
                }
            }
            
            skillsController = GameObject.Find("SkillsController").GetComponent<SkillsController>();
            scripts.cr.cameraIndex = PlayerIndex;

            isInInventory = false;
            isInEscapeMenu = false;
            isInGMGUI = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (scripts.gc.AvatarToUserId[PlayerIndex] != PhotonNetwork.AuthValues.UserId)
            {
                //Debug.Log("Error indexes");
                return;
            }

            if (Input.GetKeyDown(KeyCode.Tab) && (!isInGMGUI || !isInEscapeMenu))
            {
                isInInventory = !isInInventory;
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isInEscapeMenu = !isInEscapeMenu;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                if (Input.GetKeyDown(KeyCode.Quote) && (!isInInventory || !isInEscapeMenu))
                {
                    isInGMGUI = !isInGMGUI;
                }
            }
            
            if (horizontalAxe != Input.GetAxis("Horizontal"))
            {
                if (!isInInventory && !isInEscapeMenu)
                {
                    horizontalAxe = Input.GetAxis("Horizontal");
                    photonView.RPC("WantToMoveHorizontalRPC", RpcTarget.MasterClient, Input.GetAxis("Horizontal"));
                }
                else
                {
                    horizontalAxe = 0;
                    photonView.RPC("WantToMoveHorizontalRPC", RpcTarget.MasterClient, 0f);
                }
            }

            if (verticalAxe != Input.GetAxis("Vertical"))
            {
                if (!isInInventory && !isInEscapeMenu)
                {
                    verticalAxe = Input.GetAxis("Vertical");
                    photonView.RPC("WantToMoveVerticalRPC", RpcTarget.MasterClient, Input.GetAxis("Vertical"));
                }
                else
                {
                    verticalAxe = 0;
                    photonView.RPC("WantToMoveVerticalRPC", RpcTarget.MasterClient, 0f);
                }
            }

            if (rotationX != Input.GetAxis("Mouse Y"))
            {
                if (!isInInventory && !isInEscapeMenu && !isInGMGUI)
                {
                    rotationX = Input.GetAxis("Mouse Y");
                    photonView.RPC("WantToRotateXRPC", RpcTarget.MasterClient, Input.GetAxis("Mouse Y"));
                }
                else
                {
                    rotationX = 0;
                    photonView.RPC("WantToRotateXRPC", RpcTarget.MasterClient, 0f);
                }
            }

            if (rotationY != Input.GetAxis("Mouse X"))
            {
                if (!isInInventory && !isInEscapeMenu && !isInGMGUI)
                {
                    rotationY = Input.GetAxis("Mouse X");
                    photonView.RPC("WantToRotateYRPC", RpcTarget.MasterClient, Input.GetAxis("Mouse X"));
                }
                else
                {
                    rotationY = 0;
                    photonView.RPC("WantToRotateYRPC", RpcTarget.MasterClient, 0f);
                }
            }
            if (isInInventory || isInEscapeMenu || isInGMGUI)
            {
                return;
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

            if(Input.GetMouseButtonDown(0))
            {
                photonView.RPC("WantToPunchRPC", RpcTarget.MasterClient, true);
                wantToPunch = true;
            } else
            {
                wantToPunch = false;
            }
            
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isCrouched || Input.GetKeyDown(KeyCode.LeftShift) && !wantToJump)
            {
                //Debug.Log("Running");
                isRunning = true;
                photonView.RPC("WantToRunRPC", RpcTarget.MasterClient, true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) || isCrouched || wantToJump)
            {
                //Debug.Log("End Running");
                isRunning = false;
                photonView.RPC("WantToRunRPC", RpcTarget.MasterClient, false);
            }

            if (Input.GetKeyDown(KeyCode.LeftControl) && !isRunning)
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

        private void FixedUpdate()
        {
            if (scripts.gc.AvatarToUserId[PlayerIndex] != PhotonNetwork.AuthValues.UserId && !scripts.gc.players[PlayerIndex].PlayerStats.isDead)
            {
                return;
            }

            if(!PhotonNetwork.IsMasterClient)
            {
                scripts.pcm.PlayerMovement(PlayerIndex);
            }

            float rotX = scripts.pcm.PlayerRotation(PlayerIndex);

            photonView.RPC("AskUpdateRotPos", RpcTarget.MasterClient, null, scripts.gc.players[PlayerIndex].PlayerGameObject.transform.localEulerAngles, rotX, PlayerIndex);
        }

        [PunRPC]
        private void AskUpdateRotPos(Vector3 position, Vector3 rotation, float rotX, int idPlayer)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            this.position = position;
            this.rotation = rotation;
            this.rotX = rotX;
            scripts.gc.players[idPlayer].PlayerGameObject.transform.localEulerAngles = rotation;
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

        [PunRPC]
        void WantToPunchRPC(bool action)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                wantToPunch = action;
            }
        }

        void ActivateSkill(int place)
        {
            skillsController.photonView.RPC("AskSkillActivation", RpcTarget.MasterClient, place, Array.IndexOf(scripts.gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
        }
    }
}