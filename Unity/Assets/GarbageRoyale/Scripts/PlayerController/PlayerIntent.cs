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
        private GameController gc;

        [SerializeField]
        private PlayerControllerMaster pcm;

        // Update is called once per frame
        void Update()
        {
            if (gc.AvatarToUserId[PlayerIndex] != PhotonNetwork.AuthValues.UserId)
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

            if(rotationX != Input.GetAxis("Mouse Y"))
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
        }

        void FixedUpdate()
        {
            if (gc.AvatarToUserId[PlayerIndex] != PhotonNetwork.AuthValues.UserId)
            {
                return;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                pcm.PlayerMovement(PlayerIndex);
                pcm.PlayerRotation(PlayerIndex);
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
    }
}