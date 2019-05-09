using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.PlayerController
{
    public class PlayerControllerMaster : MonoBehaviour
    {
        [SerializeField]
        private PhotonView photonView;

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
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if(!gc.isGameStart)
            {
                return;
            }

            getParams();

            for (var i = 0; i < playersActionsActivated.Length; i++)
            {
                var playerAction = playersActions[i];
                var player = players[i];

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
                    photonView.RPC("LightUpRPC", RpcTarget.OthersBuffered, i, false);
                }
            }

            setParams();
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
            var playerAction = playersActions[id];

            if (players[id].PlayerChar.isGrounded)
            {
                // We are grounded, so recalculate
                // move direction directly from axes

                moveDirection[id] = new Vector3(playerAction.horizontalAxe, 0.0f, playerAction.verticalAxe);
                moveDirection[id] *= speed;

                moveDirection[id] = players[id].PlayerGameObject.transform.TransformDirection(moveDirection[id]);

                if (playerAction.wantToJump && !playerAction.isInWater)
                {
                    moveDirection[id].y = jumpSpeed;
                }
            }
            else if (!playerAction.isInWater)
            {
                float tempY = moveDirection[id].y;
                moveDirection[id] = new Vector3(playerAction.horizontalAxe, 0.0f, playerAction.verticalAxe);
                moveDirection[id] *= speed;

                moveDirection[id] = players[id].PlayerGameObject.transform.TransformDirection(moveDirection[id]);
                moveDirection[id].y = tempY;
            }
            else
            {
                moveDirection[id] = new Vector3(playerAction.horizontalAxe, 0.0f, playerAction.verticalAxe);
                moveDirection[id] *= speed;

                moveDirection[id] = players[id].PlayerGameObject.transform.TransformDirection(moveDirection[id]);
            }

            if (playerAction.isInTransition && playerAction.wantToJump && moveDirection[id].y < 6.0f)
            {
                moveDirection[id].y += 6.0f;
            }
            if (playerAction.isInTransition && playerAction.wantToGoDown)
            {
                moveDirection[id].y += 0.0f;
            }
            else if (playerAction.isInTransition)
            {
                moveDirection[id].y += 9.8f * Time.deltaTime;
            }
            else if (playerAction.isInWater && playerAction.wantToJump && moveDirection[id].y < 3.0f)
            {
                moveDirection[id].y += 3.0f;
            }

            moveDirection[id].y -= gravity * Time.deltaTime;

            players[id].PlayerChar.Move(moveDirection[id] * Time.deltaTime);
        }

        public void PlayerRotation(int id)
        {
            var playerAction = playersActions[id];

            players[id].PlayerGameObject.transform.Rotate(0, playerAction.rotationY * sensHorizontal, 0);

            float rotationX = rotationPlayer[id].x;
            rotationX -= Mathf.Clamp(playerAction.rotationX * sensVertical, minimumVert, maximumVert);
            float rotationY = players[id].transform.localEulerAngles.y;

            rotationPlayer[id] = new Vector3(rotationX, 0, 0);
            players[id].PlayerCamera.transform.localEulerAngles = rotationPlayer[id];

            players[id].SpotLight.transform.localEulerAngles = rotationPlayer[id];
            //photonView.RPC("RotateLampRPC", RpcTarget.OthersBuffered, id, rotationX);
        }

        [PunRPC]
        private void LightUpRPC(int id, bool action)
        {
            players[id].SpotLight.gameObject.SetActive(action);
        }

        [PunRPC]
        private void RotateLampRPC(int id, float rotX)
        {
            if(gc.AvatarToUserId[id] == PhotonNetwork.AuthValues.UserId)
            {
                Vector3 vec = new Vector3(rotX, 0, 0);
                players[id].SpotLight.transform.localEulerAngles = vec;
            }
        }
    }
}