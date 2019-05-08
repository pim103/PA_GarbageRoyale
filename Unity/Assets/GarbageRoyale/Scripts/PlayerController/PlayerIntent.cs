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

        // Update is called once per frame
        void Update()
        {
            if(PlayerNumbering.SortedPlayers.Length <= PlayerIndex ||
                PlayerNumbering.SortedPlayers[PlayerIndex].ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return;
            }

            horizontalAxe = Input.GetAxis("Horizontal");
            photonView.RPC("WantToMoveHorizontalRPC", RpcTarget.MasterClient, Input.GetAxis("Horizontal"));

            verticalAxe = Input.GetAxis("Vertical");
            photonView.RPC("WantToMoveVerticalRPC", RpcTarget.MasterClient, Input.GetAxis("Vertical"));

            rotationX = Input.GetAxis("Mouse Y");
            photonView.RPC("WantToRotateXRPC", RpcTarget.MasterClient, Input.GetAxis("Mouse Y"));

            rotationY = Input.GetAxis("Mouse X");
            photonView.RPC("WantToRotateYRPC", RpcTarget.MasterClient, Input.GetAxis("Mouse X"));

            if (Input.GetButton("Jump"))
            {
                wantToJump = true;
                photonView.RPC("WantToJumpRPC", RpcTarget.MasterClient, true);
            }
            else
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
            if (PlayerNumbering.SortedPlayers.Length <= PlayerIndex ||
                PlayerNumbering.SortedPlayers[PlayerIndex].ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return;
            }
            /*
            if (!PhotonNetwork.IsMasterClient)
            {
                pc.PlayerMovement(PlayerIndex);
                pc.PlayerRotation(PlayerIndex);
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