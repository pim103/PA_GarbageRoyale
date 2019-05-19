using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.PlayerController
{
    public class PlayerControllerMaster : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameController gc;

        private ExposerPlayer[] players;
        private ListPlayerIntents[] playersActions;
        private ListPlayerIntents[] playersActionsActivated;

        public Vector3[] rotationPlayer;

        public Vector3[] moveDirection;

        //Movement
        private float speed = 6.0f;
        private float jumpSpeed = 8.0f;
        private float gravity = 20.0f;

        //Camera
        private float minimumVert = -90.0f;
        private float maximumVert = 90.0f;
        private float sensHorizontal = 5.0f;
        private float sensVertical = 5.0f;

        void FixedUpdate()
        {
            //getParams();

            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if(!gc.isGameStart)
            {
                return;
            }

            for (var i = 0; i < gc.playersActionsActivated.Length; i++)
            {
                var playerAction = gc.playersActions[i];
                var player = gc.players[i];

                if (!player.PlayerGameObject.activeSelf)
                {
                    return;
                }

                PlayerMovement(i);
                PlayerRotation(i);

                if (playerAction.wantToLightUp && !player.SpotLight.gameObject.activeSelf)
                {
                    player.SpotLight.gameObject.SetActive(true);
                    photonView.RPC("LightUpRPC", RpcTarget.Others, i, true);
                }
                else if (!playerAction.wantToLightUp && player.SpotLight.gameObject.activeSelf)
                {
                    player.SpotLight.gameObject.SetActive(false);
                    photonView.RPC("LightUpRPC", RpcTarget.Others, i, false);
                }
            }

            //setParams();
        }

        private void getParams()
        {
            players = gc.players;
            playersActionsActivated = gc.playersActionsActivated;
            playersActions = gc.playersActions;
            rotationPlayer = gc.rotationPlayer;
            moveDirection = gc.moveDirection;
        }

        private void setParams()
        {
            gc.players = players;
            gc.playersActionsActivated = playersActionsActivated;
            gc.playersActions = playersActions;
            gc.rotationPlayer = rotationPlayer;
            gc.moveDirection = moveDirection;
        }

        public void PlayerMovement(int id)
        {
            var playerAction = gc.playersActions[id];

            if (gc.players[id].PlayerChar.isGrounded)
            {
                gc.moveDirection[id] = new Vector3(playerAction.horizontalAxe, 0.0f, playerAction.verticalAxe);
                gc.moveDirection[id] *= speed;

                gc.moveDirection[id] = gc.players[id].PlayerGameObject.transform.TransformDirection(gc.moveDirection[id]);

                if (playerAction.wantToJump && !playerAction.isInWater)
                {
                    gc.moveDirection[id].y = jumpSpeed;
                }
            }
            else if (!playerAction.isInWater)
            {
                float tempY = gc.moveDirection[id].y;
                gc.moveDirection[id] = new Vector3(playerAction.horizontalAxe, 0.0f, playerAction.verticalAxe);
                gc.moveDirection[id] *= speed;

                gc.moveDirection[id] = gc.players[id].PlayerGameObject.transform.TransformDirection(gc.moveDirection[id]);
                gc.moveDirection[id].y = tempY;
            }
            else
            {
                gc.moveDirection[id] = new Vector3(playerAction.horizontalAxe, 0.0f, playerAction.verticalAxe);
                gc.moveDirection[id] *= speed;

                gc.moveDirection[id] = gc.players[id].PlayerGameObject.transform.TransformDirection(gc.moveDirection[id]);
            }

            if (playerAction.isInTransition && playerAction.wantToJump && gc.moveDirection[id].y < 6.0f)
            {
                gc.moveDirection[id].y += 6.0f;
            }
            if (playerAction.isInTransition && playerAction.wantToGoDown)
            {
                gc.moveDirection[id].y += 0.0f;
            }
            else if (playerAction.isInTransition)
            {
                gc.moveDirection[id].y += 9.8f * Time.deltaTime;
            }
            else if (playerAction.isInWater && playerAction.wantToJump && gc.moveDirection[id].y < 3.0f)
            {
                gc.moveDirection[id].y += 3.0f;
            }

            gc.moveDirection[id].y -= gravity * Time.deltaTime;

            gc.players[id].PlayerChar.Move(gc.moveDirection[id] * Time.deltaTime);
        }

        public void PlayerRotation(int id)
        {
            var playerAction = gc.playersActions[id];

            gc.players[id].PlayerGameObject.transform.Rotate(0, playerAction.rotationY * sensHorizontal, 0);

            float rotationX = gc.rotationPlayer[id].x;
            rotationX -= Mathf.Clamp(playerAction.rotationX * sensVertical, minimumVert, maximumVert);
            float rotationY = gc.players[id].transform.localEulerAngles.y;

            gc.rotationPlayer[id] = new Vector3(rotationX, 0, 0);
            gc.players[id].PlayerCamera.transform.localEulerAngles = gc.rotationPlayer[id];

            gc.players[id].SpotLight.transform.localEulerAngles = gc.rotationPlayer[id];

            if (PhotonNetwork.IsMasterClient && rotationX != gc.players[id].SpotLight.transform.localEulerAngles.x)
            {
                photonView.RPC("RotateLampRPC", RpcTarget.Others, id, rotationX);
            }
        }

        [PunRPC]
        private void LightUpRPC(int id, bool action)
        {
            gc.players[id].SpotLight.gameObject.SetActive(action);
        }

        [PunRPC]
        private void RotateLampRPC(int id, float rotX)
        {
            Debug.Log("Rotate id : " + id);
            if (gc.AvatarToUserId[id] != PhotonNetwork.AuthValues.UserId)
            {
                Vector3 vec = new Vector3(rotX, 0, 0);
                gc.players[id].SpotLight.transform.localEulerAngles = vec;
            }
        }
    }
}