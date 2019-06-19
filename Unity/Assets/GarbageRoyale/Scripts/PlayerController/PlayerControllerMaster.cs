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
        private float jumpSpeed = 8.0f;
        private float runSpeed = 6.0f * 0.4f;
        private float crouchSpeed = 3.0f;
        private float gravity = 20.0f;

        //Camera
        private float minimumVert = -90.0f;
        private float maximumVert = 90.0f;
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
            coroutineIsStart = Enumerable.Repeat(false, 10).ToArray();
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
                if (gc.players[i].PlayerStats.isDead)
                {
                    if(!gc.players[i].PlayerStats.isAlreadyTrigger)
                    {
                        gc.players[i].PlayerStats.isAlreadyTrigger = true;
                        DataCollector.instance.AddKillPoint(gc.players[i].PlayerGameObject, i, Time.time);

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
                               false
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

                if (!gc.playersActions[i].isFallen && !gc.playersActions[i].isTrap)
                {
                    isMoving = PlayerMovement(i);
                }

                //rotationX = PlayerRotation(i);
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
                        gc.playersActions[i].isDamageBoosted
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
            var playerAction = gc.playersActions[id];

            var actualSpeed = speed;

            if(playerAction.isSlow)
            {
                actualSpeed /= 2;
            }

            if (gc.players[id].PlayerChar.isGrounded)
            {
                gc.moveDirection[id] = new Vector3(playerAction.horizontalAxe, 0.0f, playerAction.verticalAxe);
                gc.moveDirection[id] *= actualSpeed;

                gc.moveDirection[id] = gc.players[id].PlayerGameObject.transform.TransformDirection(gc.moveDirection[id]);

                if (playerAction.wantToJump && !playerAction.headIsInWater)
                {
                    gc.moveDirection[id].y = jumpSpeed;
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
                gc.moveDirection[id].y = tempY;
            }
            else
            {
                gc.moveDirection[id] = new Vector3(playerAction.horizontalAxe, 0.0f, playerAction.verticalAxe);
                gc.moveDirection[id] *= actualSpeed;

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
            rotationX -= playerAction.rotationX * sensVertical;
            rotationX = Mathf.Clamp(rotationX , minimumVert, maximumVert);
            //Debug.Log(playerAction.rotationX);

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
                    gc.players[id].PlayerGameObject.transform.localEulerAngles = new Vector3(0f, gc.players[id].PlayerGameObject.transform.localEulerAngles.y, 0.0f);
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
        private void UpdateDataRPC(int id, bool isMoving, float rotX, float h, float s, float b, bool headIsInWater, bool isDead, bool isBurning, bool isOiled, bool isQuiet, bool isDamageBoosted)
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

            if (gc.AvatarToUserId[id] == PhotonNetwork.AuthValues.UserId)
            {
                gc.players[id].PlayerCamera.transform.localEulerAngles = new Vector3(rotX, 0, 0);
                gc.inventoryGui.updateBar(ps.currentHp, ps.currentStamina, ps.currentBreath);
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
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), deadTexture);
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