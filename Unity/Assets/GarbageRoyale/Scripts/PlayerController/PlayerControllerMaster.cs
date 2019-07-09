using GarbageRoyale.Scriptable;
using GarbageRoyale.Scripts.Menu;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private bool[] coroutineIsStart;

        //Movement
        private float speed = 6.0f;
        private float jumpSpeed = 5.0f;
        private float runSpeed = 6.0f * 0.4f;
        private float crouchSpeed = 3.0f;
        private float gravity = 20.0f;

        //Camera
        private float minimumVert = -90.0f;
        private float maximumVert = 73.0f;
        private float sensHorizontal = 5.0f;
        private float sensVertical = 5.0f;

        private Texture2D filterWaterTexture;
        private Texture2D deadTexture;
        private Texture2D oiledTexture;
        private Texture2D burnedTexture;

        private bool IsGameEnd;

        private void Start()
        {
            initTexture();
            coroutineIsStart = Enumerable.Repeat(false, 20).ToArray();
            IsGameEnd = false;
        } 

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

            int playerAlive = 0;
            for (var i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                //Debug.Log(players[i].PlayerTransform.position);
                if (gc.players[i].PlayerStats.isDead)
                {
                    if(!gc.players[i].PlayerStats.isAlreadyTrigger)
                    {
                        gc.players[i].PlayerStats.isAlreadyTrigger = true;
                        DataCollector.instance.AddKillPoint(gc.players[i].PlayerGameObject, i, Time.time);

                        for(var placeInv = 0; placeInv < gc.players[i].PlayerInventory.itemInventory.Length; placeInv++)
                        {
                            se.iac.AskDropItem(placeInv, i, false);
                        }
                        for (var placeInv = 0; placeInv < gc.players[i].PlayerInventory.skillInventory.Length; placeInv++)
                        {
                            se.iac.AskDropSkill(placeInv, i);
                        }

                        photonView.RPC("UpdateDataRPC", RpcTarget.All,
                            i,
                            false,
                            0f,
                            0f,
                            0f,
                            0f,
                            false,
                            true,
                            false,
                            false,
                            false,
                            false,
                            false,
                            false,
                            false,
                            false,
                            false,
                            Vector3.zero,
                            Vector3.zero
                        );
                    }

                    continue;
                }

                playerAlive++;

                var playerAction = gc.playersActions[i];
                var player = gc.players[i];

                if (!player.PlayerGameObject.activeSelf)
                {
                    return;
                }

                //isMoving = (playerAction.horizontalAxe != 0 | playerAction.verticalAxe != 0);
                //rotationX = playerAction.rotX;
                //gc.players[i].PlayerGameObject.transform.position = playerAction.position;
                //gc.players[i].PlayerGameObject.transform.localEulerAngles = playerAction.rotation;

                if (!gc.playersActions[i].isFallen && !gc.playersActions[i].isTrap)
                {
                    isMoving = PlayerMovement(i);
                }
                rotationX = PlayerRotation(i);

                if (!coroutineIsStart[i])
                {
                    coroutineIsStart[i] = true;
                    StartCoroutine(PlayerUpdateStats(i));
                }
                //PlayerUpdateStats(i);

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
                        gc.players[i].PlayerStats.currentBreath,
                        gc.playersActions[i].headIsInWater,
                        gc.players[i].PlayerStats.isDead,
                        gc.playersActions[i].isBurning,
                        gc.playersActions[i].isOiled,
                        gc.playersActions[i].isQuiet,
                        gc.playersActions[i].isDamageBoosted,
                        gc.playersActions[i].isFallen,
                        gc.playersActions[i].isTrap,
                        gc.playersActions[i].isSlow,
                        gc.playersActions[i].isInWater,
                        gc.playersActions[i].feetIsInWater,
                        gc.playersActions[i].position,
                        gc.playersActions[i].rotation
                    );
                }

                if(gc.players[i].PlayerGameObject.transform.position.y > (12 + 4) * 8 - 5)
                {
                    gc.isGameStart = false;
                    photonView.RPC("EndGameRPC", RpcTarget.All, EndGameMenu.StateEndGame.One_Alive, i);
                    break;
                }
            }

            if(playerAlive <= 0)
            {
                gc.isGameStart = false;
                photonView.RPC("EndGameRPC", RpcTarget.All, EndGameMenu.StateEndGame.All_Dead, -1);
            }
        }

        public bool PlayerMovement(int id)
        {
            if(gc.playersActions[id].isTrap || gc.playersActions[id].isFallen)
            {
                return false;
            }

            var playerAction = gc.playersActions[id];
            var actualSpeed = speed;

            if(playerAction.isSlow)
            {
                actualSpeed /= 2;
            }

            if (gc.water.waterObject.transform.position.y > gc.players[id].PlayerGameObject.transform.position.y + 0.3f)
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
            }
            else
            {
                gc.playersActions[id].isInWater = false;
                gc.playersActions[id].feetIsInWater = false;
                gc.playersActions[id].headIsInWater = false;
            }

            RaycastHit hitInfo;
            bool isGrounded = Physics.Raycast(gc.players[id].PlayerFeet.transform.position, transform.TransformDirection(Vector3.down), out hitInfo, 0.1f);

            if (isGrounded)
            {
                gc.moveDirection[id] = new Vector3(playerAction.horizontalAxe, 0.0f, playerAction.verticalAxe);
                gc.moveDirection[id] *= actualSpeed;

                gc.moveDirection[id] = gc.players[id].PlayerGameObject.transform.TransformDirection(gc.moveDirection[id]);

                if (playerAction.wantToJump && !playerAction.headIsInWater)
                {
                    gc.players[id].PlayerChar.AddForce(Vector2.up * jumpSpeed, ForceMode.Impulse);
                }
                if(playerAction.isRunning)
                {
                    gc.moveDirection[id] *= runSpeed;
                }

                if (playerAction.isCrouched)
                {
                    gc.moveDirection[id] /= crouchSpeed;
                }
            }
            else if (!playerAction.isInWater)
            {
                float tempY = gc.moveDirection[id].y;
                gc.moveDirection[id] = new Vector3(playerAction.horizontalAxe, 0.0f, playerAction.verticalAxe);
                gc.moveDirection[id] *= actualSpeed;

                gc.moveDirection[id] = gc.players[id].PlayerGameObject.transform.TransformDirection(gc.moveDirection[id]);
                //gc.moveDirection[id].y = tempY;
            }
            else if(playerAction.isInWater)
            {
                gc.moveDirection[id] = new Vector3(playerAction.horizontalAxe, 0.0f, playerAction.verticalAxe);
                gc.moveDirection[id] *= actualSpeed;

                gc.moveDirection[id] = gc.players[id].PlayerGameObject.transform.TransformDirection(gc.moveDirection[id]);
            }

            Vector3 movement = gc.players[id].PlayerChar.velocity;
            if (playerAction.isInTransition && playerAction.wantToJump && gc.moveDirection[id].y < 6.0f)
            {
                gc.moveDirection[id].y += 3.0f;
                movement.y = gc.moveDirection[id].y;
            }
            else if (playerAction.isInWater && playerAction.wantToJump && gc.moveDirection[id].y < 3.0f)
            {
                gc.moveDirection[id].y += 2.0f;
                movement.y = gc.moveDirection[id].y;
            }
            else if (playerAction.isInWater)
            {
                movement.y /= 2;
            }
            //gc.moveDirection[id].y -= 1.0f;
            movement.x = gc.moveDirection[id].x;
            movement.z = gc.moveDirection[id].z;
            gc.players[id].PlayerChar.velocity = movement;
            //gc.players[id].PlayerChar.Move(gc.moveDirection[id] * Time.deltaTime);

            return (playerAction.horizontalAxe != 0 | playerAction.verticalAxe != 0);
        }

        public float PlayerRotation(int id)
        {
            if (gc.playersActions[id].isFallen)
            {
                return gc.rotationPlayer[id].x;
            }

            var playerAction = gc.playersActions[id];

            gc.players[id].PlayerGameObject.transform.Rotate(0, playerAction.rotationY * sensHorizontal, 0);

            float rotationX = gc.rotationPlayer[id].x;
            rotationX -= playerAction.rotationX * sensVertical;
            rotationX = Mathf.Clamp(rotationX , minimumVert, maximumVert);

            gc.rotationPlayer[id] = new Vector3(rotationX, 0, 0);
            gc.players[id].PlayerCamera.transform.localEulerAngles = gc.rotationPlayer[id];

            gc.players[id].SpotLight.transform.localEulerAngles = gc.rotationPlayer[id];

            return rotationX;
        }

        private IEnumerator PlayerUpdateStats(int id)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                yield return null;
            }

            PlayerStats ps = gc.players[id].PlayerStats;

            while(true)
            {
                if (gc.playersActions[id].headIsInWater && !gc.playersActions[id].isAmphibian)
                {
                    gc.playersActions[id].isBurning = false;
                    gc.playersActions[id].isOiled = false;
                    if (ps.currentBreath > 0)
                    {
                        ps.currentBreath -= 0.5f;
                    }
                    else if (ps.currentHp > 0)
                    {
                        ps.takeDamage(0.5f);
                    }
                }
                else
                {
                    if (ps.currentBreath < ps.defaultBreath)
                    {
                        ps.currentBreath += 0.5f;
                    }
                }

                if (gc.playersActions[id].isRunning && ps.currentStamina > 0)
                {
                    ps.currentStamina -= 2.0f;
                }
                else
                {
                    gc.playersActions[id].isRunning = false;
                }

                if (ps.currentStamina < ps.defaultStamina)
                {
                    ps.currentStamina += 0.7f;
                }

                if (gc.playersActions[id].isBurning)
                {
                    ps.takeDamage(0.5f);
                    gc.playersActions[id].timeLeftBurn -= 0.1f;
                    if (gc.playersActions[id].timeLeftBurn <= 0.0f)
                    {
                        gc.playersActions[id].isBurning = false;
                    }
                }

                if (gc.playersActions[id].isOiled && gc.playersActions[id].timeLeftOiled > 0.0f)
                {
                    gc.playersActions[id].timeLeftOiled -= 0.1f;
                }
                else
                {
                    gc.playersActions[id].isOiled = false;
                }

                if (gc.playersActions[id].isFallen && gc.playersActions[id].timeLeftFallen > 0.0f)
                {
                    gc.playersActions[id].timeLeftFallen -= 0.1f;
                }
                else if (gc.playersActions[id].isFallen)
                {
                    gc.playersActions[id].isFallen = false;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        [PunRPC]
        private void LightUpRPC(int id, bool action)
        {
            gc.players[id].SpotLight.gameObject.SetActive(action);
        }

        [PunRPC]
        private void UpdateDataRPC(int id, bool isMoving, float rotX, float h, float s, float b, bool headIsInWater, bool isDead, bool isBurning, bool isOiled, bool isQuiet, bool isDamageBoosted,
            bool isFallen,
            bool isTrap,
            bool isSlow,
            bool isInWater,
            bool feetIsInWater,
            Vector3 pos, 
            Vector3 rot)
        {
            Vector3 vec = new Vector3(rotX, 0, 0);
            //gc.players[id].SpotLight.transform.localEulerAngles = vec;

            /*  
            if (!PhotonNetwork.IsMasterClient && id != System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                gc.players[id].PlayerGameObject.transform.localEulerAngles = rot;
            }
            */
            SoundManager.Sound soundNeeded = SoundManager.Sound.Walk;

            if (gc.playersActions[id].headIsInWater)
            {
                soundNeeded = SoundManager.Sound.Swim;
            }
            else if (gc.playersActions[id].feetIsInWater)
            {
                soundNeeded = SoundManager.Sound.FeetOnWater;
            }
            else if(gc.playersActions[id].isOnMetalSheet)
            {
                soundNeeded = SoundManager.Sound.MetalSheet;
            }

            gc.soundManager.playWalkSound(id, isMoving, soundNeeded,isQuiet);

            if(isMoving)
            {
                gc.players[id].PlayerAnimator.Play("Movement");
            }

            PlayerStats ps = gc.players[id].PlayerStats;
            ps.currentHp = h;
            ps.currentStamina = s;
            ps.currentBreath = b;

            ps.isDead = isDead;
            gc.playersActions[id].headIsInWater = headIsInWater;
            gc.playersActions[id].isBurning = isBurning;
            gc.playersActions[id].isOiled = isOiled;
            gc.playersActions[id].isDamageBoosted = isDamageBoosted;
            gc.playersActions[id].isFallen = isFallen;
            gc.playersActions[id].isSlow = isSlow;
            gc.playersActions[id].isTrap = isTrap;
            gc.playersActions[id].isInWater = isInWater;
            gc.playersActions[id].feetIsInWater = feetIsInWater;

            if (isFallen)
            {
                gc.players[id].PlayerGameObject.transform.localEulerAngles = new Vector3(90.0f, gc.players[id].PlayerGameObject.transform.localEulerAngles.y, gc.players[id].PlayerGameObject.transform.localEulerAngles.z);
            } else
            {
                gc.players[id].PlayerGameObject.transform.localEulerAngles = new Vector3(0.0f, gc.players[id].PlayerGameObject.transform.localEulerAngles.y, gc.players[id].PlayerGameObject.transform.localEulerAngles.z);
            }

            int playerIndex = System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId);

            if (gc.AvatarToUserId[id] == PhotonNetwork.AuthValues.UserId)
            {
                gc.players[id].PlayerCamera.transform.localEulerAngles = new Vector3(rotX, 0, 0);
                gc.inventoryGui.updateBar(ps.currentHp, ps.currentStamina, ps.currentBreath);
            }

            if(isDead && (id == se.sm.idCamSpectate || id == playerIndex))
            {
                se.sm.SwitchCam(playerIndex);
            }
        }

        [PunRPC]
        private void EndGameRPC(EndGameMenu.StateEndGame sg, int idPlayer)
        {
            if(idPlayer == System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                sg = EndGameMenu.StateEndGame.Won;
            }

            IsGameEnd = true;
            gc.menuController.launchEndGameMenu(sg, idPlayer);
        }

        private void OnGUI()
        {
            if (!gc.endOfInit || IsGameEnd)
            {
                return;
            }

            int id = System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId);
            bool isDead = gc.players[id].PlayerStats.isDead;
            bool headIsInWater = gc.playersActions[id].headIsInWater;
            bool isBurning = gc.playersActions[id].isBurning;
            bool isOiled = gc.playersActions[id].isOiled;

            if (isDead)
            {
                //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), deadTexture);
            }
            else if (headIsInWater)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), filterWaterTexture);
            }
            else if (isBurning)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), burnedTexture);
            }
            else if (isOiled)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), oiledTexture);
            }
        }

        private void initTexture()
        {
            filterWaterTexture = gc.MakeTex(Screen.width, Screen.height, new Color(0, 0.5f, 1, 0.5f));
            deadTexture = gc.MakeTex(Screen.width, Screen.height, new Color(0.7f, 0.7f, 0.7f, 0.7f));
            oiledTexture = gc.MakeTex(Screen.width, Screen.height, new Color(1.0f, 0.9f, 1.0f, 0.7f));
            burnedTexture = gc.MakeTex(Screen.width, Screen.height, new Color(0.7f, 0.3f, 0.0f, 0.7f));
        }
    }
}