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

        [SerializeField]
        private ScriptExposer se;

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
            bool isMoving = false;
            float rotationX = 0.0f;

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

                isMoving = PlayerMovement(i);
                rotationX = PlayerRotation(i);
                PlayerUpdateStats(i);

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

                if(gc.endOfInit)
                {
                    photonView.RPC("UpdateDataRPC", RpcTarget.All,
                        i,
                        isMoving,
                        rotationX,
                        gc.players[i].PlayerStats.currentHp,
                        gc.players[i].PlayerStats.currentStamina,
                        gc.players[i].PlayerStats.currentBreath
                    );
                }
            }
        }

        public bool PlayerMovement(int id)
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
            
            if (gc.water.waterObject.transform.position.y > gc.players[id].PlayerGameObject.transform.position.y + 0.4f)
            {
                gc.playersActions[id].isInWater = true;
                gc.playersActions[id].feetIsInWater = false;
                gc.playersActions[id].headIsInWater = true;
                se.iac.ExtinctTorch(id);
            }
            else if (gc.water.waterObject.transform.position.y > gc.players[id].PlayerGameObject.transform.position.y - 1f)
            {
                gc.playersActions[id].isInWater = true;
                gc.playersActions[id].feetIsInWater = true;
                gc.playersActions[id].headIsInWater = false;
            } else
            {
                gc.playersActions[id].isInWater = false;
                gc.playersActions[id].feetIsInWater = false;
                gc.playersActions[id].headIsInWater = false;
            }

            return (playerAction.horizontalAxe != 0 | playerAction.verticalAxe != 0);
        }

        public float PlayerRotation(int id)
        {
            var playerAction = gc.playersActions[id];

            gc.players[id].PlayerGameObject.transform.Rotate(0, playerAction.rotationY * sensHorizontal, 0);

            float rotationX = gc.rotationPlayer[id].x;
            rotationX -= Mathf.Clamp(playerAction.rotationX * sensVertical, minimumVert, maximumVert);

            gc.rotationPlayer[id] = new Vector3(rotationX, 0, 0);
            gc.players[id].PlayerCamera.transform.localEulerAngles = gc.rotationPlayer[id];

            gc.players[id].SpotLight.transform.localEulerAngles = gc.rotationPlayer[id];
            
            return rotationX;
        }

        private void PlayerUpdateStats(int id)
        {
            PlayerStats ps = gc.players[id].PlayerStats;
            
            if (gc.playersActions[id].headIsInWater)
            {
                if (ps.currentBreath > 0)
                {
                    ps.currentBreath -= 0.1f;
                }
                else if (ps.currentHp > 0)
                {
                    ps.takeDamage(0.2f);
                }
            }
            else
            {
                if (ps.currentBreath < ps.defaultBreath)
                {
                    ps.currentBreath += 1;
                }
            }

            if (ps.currentStamina < ps.defaultStamina)
            {
                ps.currentStamina += 0.3f;
            }
        }

        [PunRPC]
        private void LightUpRPC(int id, bool action)
        {
            gc.players[id].SpotLight.gameObject.SetActive(action);
        }

        [PunRPC]
        private void UpdateDataRPC(int id, bool isMoving, float rotX, float h, float s, float b)
        {
            Vector3 vec = new Vector3(rotX, 0, 0);
            gc.players[id].SpotLight.transform.localEulerAngles = vec;
            SoundManager.Sound soundNeeded = SoundManager.Sound.Walk;

            if (gc.playersActions[id].headIsInWater)
            {
                soundNeeded = SoundManager.Sound.Swim;
            }
            else if (gc.playersActions[id].feetIsInWater)
            {
                soundNeeded = SoundManager.Sound.FeetOnWater;
            }

            gc.soundManager.playWalkSound(id, isMoving, soundNeeded);

            PlayerStats ps = gc.players[id].PlayerStats;
            ps.currentHp = h;
            ps.currentStamina = s;
            ps.currentBreath = b;

            if (gc.AvatarToUserId[id] == PhotonNetwork.AuthValues.UserId)
            {
                gc.players[id].PlayerCamera.transform.localEulerAngles = new Vector3(rotX, 0, 0);
                gc.inventoryGui.updateBar(ps.currentHp, ps.currentStamina, ps.currentBreath);
            }
        }
    }
}