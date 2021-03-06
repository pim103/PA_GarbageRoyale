﻿using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using Photon.Pun.UtilityScripts;
using System;
using GarbageRoyale.Scripts.PrefabPlayer;
using GarbageRoyale.Scripts.PlayerController;

using System.Linq;
using GarbageRoyale.Scripts.HUD;
using GarbageRoyale.Scriptable;
using GarbageRoyale.Scripts.GameMaster;
using GarbageRoyale.Scripts.Menu;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    [RequireComponent(typeof(MazeConstructor))]               // 1

    public class GameController : MonoBehaviourPunCallbacks
    {
        public MazeConstructor generator;

        [SerializeField] 
        private GameMasterController gameMasterController;
        [SerializeField]
        private GameObject player;
        [SerializeField]
        private GameObject cameraPrefab;
        [SerializeField]
        private GameObject characterObject;
        [SerializeField]
        private GameObject soundObject;
        [SerializeField]
        private GameObject startDoorPrefab;

        [SerializeField]
        private GameObject mainCamera;
        [SerializeField]
        public AudioSource menuSound;
        [SerializeField]
        public SoundManager soundManager;

        [SerializeField]
        public StartGame menuController;

        [SerializeField]
        public InventoryGUI inventoryGui;

        [SerializeField]
        public Water water;
        
        [SerializeField] 
        public GameObject Mob;

        [SerializeField] 
        private RawImage minimapArrow;
        
        private GameObject playerCamera;

        private GameObject startDoor;
        
        private bool canMove = false;
        //public List<GameObject> characterList = new List<GameObject>();
        public Dictionary<int, GameObject> characterList = new Dictionary<int, GameObject>();
        public Dictionary <int, GameObject> mobList = new Dictionary<int, GameObject>();
        private int [][,] exploredRooms = new int[8][,];

        private GUIStyle currentStyle = null;
        private Texture2D mapTexture;
        private Texture2D trapTexture;
        private Texture2D playerTexture;
        private float currentPosX;
        private float currentPosY;
        private float currentPosZ;
        private bool wantToGoUp;
        private bool wantToGoDown;

        public bool isGameStart;
        private float timeLeft;
        private float waterStartTimeLeft;
        private bool waterStart;
        private bool pressL;

        public Dictionary<string, string>[] roomLinksList;

        private int playerConnected;

        public event Action<int> PlayerJoined;
        public event Action PlayerLeft;
        public event Action<int> OnlinePlayReady;

        [SerializeField]
        public ExposerPlayer[] players;

        [SerializeField]
        public ListPlayerIntents[] playersActions;

        public ListPlayerIntents[] playersActionsActivated;

        public Vector3[] rotationPlayer;
        public Vector3[] moveDirection;
        public string[] AvatarToUserId;
        public int[] playersScores;
        public bool endOfInit;
        public bool doorIsOpen;

        public bool triggerWater;
        public bool forceOpenDoor;
        public bool wantGoToLastFloor;

        public Dictionary<int, GameObject> pipes = new Dictionary<int, GameObject>();
        public Dictionary<GameObject, int> buttonsTrap = new Dictionary<GameObject, int>();
        public Dictionary<int, GameObject> traps = new Dictionary<int, GameObject>();
        public Dictionary<int, GameObject> doors = new Dictionary<int, GameObject>();
        public Dictionary<int, GameObject> buttonsTrapReversed = new Dictionary<int, GameObject>();

        public int nbItems;
        public Dictionary<int, bool> endInit = new Dictionary<int, bool>();

        public Dictionary<int, GameObject> items = new Dictionary<int, GameObject>();
        
        [SerializeField]
        public GameObject invHUD;
        [SerializeField]
        public GameObject playerGUI;
        [SerializeField]
        public GameObject skillsController;
        [SerializeField]
        public GameObject PlayerListScripts;

        [SerializeField]
        public GameObject[] itemList;

        public bool mineQ;
        public bool isSpectator;
        public bool endSendingScores;

        private void Awake()
        {
            nbItems = 0;
            mineQ = false;
            AvatarToUserId = Enumerable.Repeat("", 20).ToArray();
            playersScores = Enumerable.Repeat(0, 20).ToArray();
            PlayerJoined += ActivateAvatar;
            PlayerLeft += null;
            OnlinePlayReady += StartGame;
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if(!isGameStart)
            {
                endOfInit = false;
            }
        }

        void Start()
        {
            endOfInit = false;
            triggerWater = false;
            forceOpenDoor = false;

            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SendUserId", RpcTarget.MasterClient, PhotonNetwork.AuthValues.UserId);
            } else
            {
                StartCoroutine(SearchForActivateAvatar());

                waterStart = false;
                doorIsOpen = false;
                waterStartTimeLeft = 60.0f;
                timeLeft = 20.0f;
                StartCoroutine(Invincible());
            }

            // INIT MAP AND MINIMAP
            for (int i = 0; i < 8; i++)
            {
                exploredRooms[i] = new int[81, 81];
                for (int j = 0; j < 81; j++)
                {
                    exploredRooms[i][j, j] = 0;
                }
            }
            mapTexture = MakeTex(4, 4, new Color(1f, 1f, 1f, 0.5f));
            trapTexture = MakeTex(4, 4, new Color(0f, 1f, 0f, 0.5f));
            playerTexture = MakeTex(6, 6, new Color(1f, 0.0f, 0.0f, 1f));
            generator = GetComponent<MazeConstructor>();      // 2
            generator.GenerateNewMaze(81, 81, itemList.Length);

            startDoor = Instantiate(startDoorPrefab, new Vector3(142, 0, 160), Quaternion.identity);

            moveDirection = new Vector3[20];
            rotationPlayer = new Vector3[20];
            // END INIT MAP

            //playerCamera = Instantiate(cameraPrefab, new Vector3(150, 1.5f, 150), Quaternion.identity);
            canMove = false;
            pressL = false;

            //playerConnected = 1;
            roomLinksList = generator.dataGenerator.roomLinksList;
            isGameStart = false;
            //wantToGoUp = false;
            waterStart = false;
            //wantToGoDown = false;
            pipes = generator.meshGenerator.pipes;
            traps = generator.meshGenerator.trap;
            buttonsTrap = generator.meshGenerator.buttonsTrap;
            doors = generator.meshGenerator.doors;
            buttonsTrapReversed = generator.meshGenerator.buttonsTrapReversed;

            DataCollector.instance.InitMap(
                generator.dataGenerator.roomLinksList, 
                generator.dataGenerator.roomTrap, 
                generator.dataGenerator.itemRoom,
                generator.Prefabs,
                generator.floorTransition,
                generator.floors,
                generator.floorsRooms
            );
        }

        [PunRPC]
        public void testrpc()
        {
            Debug.Log("ALLO");
        }
        
        [PunRPC]
        public void TellEndUpdate()
        {
            endSendingScores = true;
            Debug.Log("Allo");
        }
        
        [PunRPC]
        private void SendUserId(string userId)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(ActivateClientAvatar(userId));
            }
        }

        private IEnumerator ActivateClientAvatar(string userId)
        {
            yield return new WaitForSeconds(0.1f);

            var i = 0;
            while (i < AvatarToUserId.Length && AvatarToUserId[i] != "")
            {
                i++;
            }

            if(!isGameStart)
            {
                AvatarToUserId[i] = userId;
                PlayerJoined?.Invoke(i); //ActivateAvatar
            }

            photonView.RPC("initAvatarUserIdRPC", RpcTarget.Others, AvatarToUserId, userId);
        }

        private IEnumerator SearchForActivateAvatar()
        {
            yield return new WaitForSeconds(0.1f);

            var i = 0;
            while(i < AvatarToUserId.Length && AvatarToUserId[i] != "")
            {
                i++;
            }

            if (PhotonNetwork.IsMasterClient && !isGameStart)
            {
                AvatarToUserId[i] = PhotonNetwork.AuthValues.UserId;
                mineQ = true;
                PlayerJoined?.Invoke(i);
            }
        }

        [PunRPC]
        private void initAvatarUserIdRPC(string[] UserIdList, string userId)
        {
            AvatarToUserId = UserIdList;

            mineQ = true;
            var i = 0;
            if(PhotonNetwork.AuthValues.UserId == userId)
            {
                while(i < AvatarToUserId.Length)
                {
                    if(PhotonNetwork.AuthValues.UserId != AvatarToUserId[i] && AvatarToUserId[i] != "")
                    {
                        ActivateAvatarRPC(i);
                    }
                    i++;
                }
            }
        }

        private void ActivateAvatar(int id)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("ActivateAvatarRPC", RpcTarget.All, id);
            }
        }

        [PunRPC]
        private void ActivateAvatarRPC(int id)
        {
            players[id].PlayerGameObject.SetActive(true);
            mainCamera.GetComponent<Camera>().enabled = true;

            foreach (Player playerPhoton in PhotonNetwork.PlayerList)
            {
                if (playerPhoton.UserId == AvatarToUserId[id])
                {
                    players[id].PlayerStats.PlayerRole = Int32.Parse(playerPhoton.CustomProperties["role"].ToString());
                    players[id].PlayerStats.PlayerName = playerPhoton.NickName;
                    gameMasterController.enabled = true;
                }
            }
        }
        
        [PunRPC]
        private void ResetPlayer(int id)
        {
            players[id].PlayerGameObject.SetActive(false);
            players[id].PlayerStats.PlayerRole = 0;
            players[id].PlayerStats.PlayerName = null;
            players[id].PlayerStats.currentHp = 0;
            players[id].PlayerStats.currentStamina = 0;
            players[id].PlayerStats.currentBreath = 0;
            players[id].PlayerStats.basicAttack = 0;
            players[id].PlayerStats.basicAttack = 0;
            players[id].PlayerStats.isDead = false;
            //gc.players[id].PlayerStats. = false;
            players[id].PlayerStats.isAlreadyTrigger = false;
            players[id].PlayerStats.isInvincible = false;
            players[id].PlayerStats.isReadyToPlay = false;
            
        }

        [PunRPC]
        public void SetReady(int index, bool toggle)
        {
            players[index].PlayerStats.IsReadyToPlay = toggle;
        }

        public void JoinAsSpectator()
        {
            isGameStart = true;
            soundManager.initAmbientSound();
            invHUD.SetActive(true);
            playerGUI.SetActive(true);
            skillsController.SetActive(true);
            PlayerListScripts.GetComponent<PlayerSoundObjects>().enabled = true;
            //playersActionsActivated[id].GetComponent<PlayerIntent>().activeScripts(id);
            menuController.roomLobby.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            mainCamera.GetComponent<Camera>().enabled = true;

            for (int i = 0; i < AvatarToUserId.Length; i++)
            {
                if (AvatarToUserId[i] != "")
                {
                    players[i].PlayerCamera.enabled = true;
                }
            }
        }
        
        [PunRPC]
        public void MasterActivateAvatarPlayer()
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            bool isStart = true;
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("IsStart", isStart);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

            for (int i = 0; i < AvatarToUserId.Length; i++)
            {
                if (AvatarToUserId[i] != "")
                {
                    photonView.RPC("launchGameRPC", RpcTarget.All, i);
                }
            }
        }

        [PunRPC]
        private void launchGameRPC(int id)
        {
            if(PhotonNetwork.AuthValues.UserId == AvatarToUserId[id])
            {
                mainCamera.SetActive(false);
                players[id].PlayerCamera.enabled = true;
                players[id].PlayerAudioListener.enabled = true;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                soundManager.initAmbientSound();
                invHUD.SetActive(true);
                playerGUI.SetActive(true);
                skillsController.SetActive(true);
                PlayerListScripts.GetComponent<PlayerSoundObjects>().enabled = true;
                //playersActionsActivated[id].GetComponent<PlayerIntent>().activeScripts(id);
                menuController.roomLobby.SetActive(false);
                
            }

            moveDirection[id] = Vector3.zero;
            rotationPlayer[id] = Vector3.zero;
            OnlinePlayReady?.Invoke(id);
        }

        [PunRPC]
        private void endOfInitRPC()
        {
            endOfInit = true;
        }

        private void StartGame(int id)
        {
            playersActionsActivated = playersActions;
            ActivateGame(id);
        }

        private void ActivateGame(int id)
        {
            isGameStart = true;

            if(playersActionsActivated == null)
            {
                return;
            }

            for(var i = 0; i < playersActionsActivated.Length; i++)
            {
                playersActionsActivated[i].enabled = true;
                playersActionsActivated[i].wantToJump = false;
                playersActionsActivated[i].wantToLightUp = true;
                playersActionsActivated[i].verticalAxe = 0.0f;
                playersActionsActivated[i].horizontalAxe = 0.0f;
                playersActionsActivated[i].rotationX = 0.0f;
                playersActionsActivated[i].rotationY = 0.0f;
                playersActionsActivated[i].wantToTurnOnTorch = false;
                playersActionsActivated[i].wantToGoDown= false;
                playersActionsActivated[i].isInTransition = false;
                playersActionsActivated[i].isInWater = false;
                playersActionsActivated[i].isOnMetalSheet = false;
                playersActionsActivated[i].headIsInWater = false;
                playersActionsActivated[i].feetIsInWater = false;
                playersActionsActivated[i].isOiled = false;
                playersActionsActivated[i].isBurning = false;
                playersActionsActivated[i].timeLeftBurn = 5.0f;
                playersActionsActivated[i].timeLeftOiled = 15.0f;
                playersActionsActivated[i].isQuiet = false;
                playersActionsActivated[i].isDamageBoosted = false;
                playersActionsActivated[i].isFallen = false;
                playersActionsActivated[i].timeLeftFallen = 2.0f;
                playersActionsActivated[i].isTrap = false;
                playersActionsActivated[i].isRunning = false;
                playersActionsActivated[i].isCrouched = false;
            }
            if (PhotonNetwork.AuthValues.UserId == AvatarToUserId[id])
            {
                photonView.RPC("endOfInitRPC", RpcTarget.All);
            }
        }

        private void FixedUpdate()
        {
            if(PhotonNetwork.IsMasterClient)
            {
                if(triggerWater)
                {
                    waterStart = true;
                    water.ToggleStartWater();
                    triggerWater = false;
                }

                if(forceOpenDoor)
                {
                    photonView.RPC("DesactiveStartDoorRPC", RpcTarget.All);
                    doorIsOpen = true;
                }

                if (wantGoToLastFloor)
                {
                    Vector3 temp = players[0].PlayerGameObject.transform.position;
                    temp.y = 113;
                    players[0].PlayerGameObject.transform.position = temp;
                    wantGoToLastFloor = false;
                }

                if (endOfInit && isGameStart)
                {
                    if(timeLeft > 0)
                    {
                        timeLeft -= Time.deltaTime;
                    }
                    else if(!doorIsOpen)
                    {
                        StartCoroutine(Invisibility());
                        photonView.RPC("DesactiveStartDoorRPC", RpcTarget.All);
                        doorIsOpen = true;
                    }
                    else if(waterStartTimeLeft > 0)
                    {
                        waterStartTimeLeft -= Time.deltaTime;
                    }
                    else if(!waterStart)
                    {
                        waterStart = true;
                        water.ToggleStartWater();
                    }
                }
            }
        }

        [PunRPC]
        public void setCanMove()
        {
            canMove = true;
        }

        public bool getCanMove()
        {
            return canMove;
        }

        [PunRPC]
        public void DesactiveStartDoorRPC()
        {
            startDoor.SetActive(false);
        }

        private IEnumerator Invincible()
        {
            while(timeLeft > 0)
            {
                for(var i = 0; i < AvatarToUserId.Length; i++)
                {
                    if(AvatarToUserId[i] != "")
                        players[i].PlayerStats.currentHp = players[i].PlayerStats.defaultHp;
                }
                yield return new WaitForSeconds(1.0f);
            }
        }

        private IEnumerator Invisibility()
        {
            photonView.RPC("ToggleInvisibilityRPC", RpcTarget.All, false);

            yield return new WaitForSeconds(10.0f);

            photonView.RPC("ToggleInvisibilityRPC", RpcTarget.All, true);
        }

        [PunRPC]
        private void ToggleInvisibilityRPC(bool toggle)
        {
            for (var i = 0; i < AvatarToUserId.Length; i++)
            {
                if(AvatarToUserId[i] != "")
                {
                    players[i].PlayerRenderer.enabled = toggle;
                }
            }
        }

        private void OnGUI()
        {
            if (!endOfInit)
            {
                return;
            }
            int playerIndex = Array.IndexOf(AvatarToUserId, PhotonNetwork.AuthValues.UserId);
            currentPosX = players[playerIndex].transform.position.x;
            currentPosY = players[playerIndex].transform.position.y;
            currentPosZ = players[playerIndex].transform.position.z;
            int size = 20;
            float posX = players[playerIndex].PlayerCamera.pixelWidth / 2 - size / 4;
            float posY = players[playerIndex].PlayerCamera.pixelHeight / 2 - size / 2;
            GUI.Label(new Rect(posX, posY, size, size), "+");
            int k = 0;
            int l = 0;
            int currentFloor = (int) currentPosY / 16;
            int imin = (int) (currentPosX / 4) - 10 - 4 * currentFloor;
            int imax = (int) (currentPosX / 4) + 10 - 4 * currentFloor;
            int jmin = (int) (currentPosZ / 4) - 10 - 4 * currentFloor;
            int jmax = (int) (currentPosZ / 4) + 10 - 4 * currentFloor;
            
            if (Input.GetKey(","))
            {
                imin = 0;
                imax = 81;
                jmin = 0;
                jmax = 81;

            }
            
            for (int i = imin;i < imax;i++)
            {
                l = 0;
                for (int j = jmin;j < jmax;j++)
                {
                    if (i > 0 && j > 0  && i < 81 - 8 * currentFloor && j < 81 - 8 * currentFloor)
                    {
                        if (generator.floors[currentFloor][j, i] != 1)
                        {
                            if ((i != (int) ((currentPosX + 1) / 4) - 4 * currentFloor ||
                                 j != (int) ((currentPosZ + 0.5) / 4) - 4 * currentFloor))
                            {
                                if (exploredRooms[currentFloor][j, i] == 1 || (currentFloor == 0 && i > (81 / 2 - 5) && i < (81 / 2 + 5) && j > 81 / 2 - 5 && j < 81 / 2 + 5 ))
                                {
                                    //Debug.Log(generator.floorsRooms[currentFloor][j, i]);
                                    if (generator.floorsRooms[currentFloor][j, i] == 3)
                                    {
                                        GUI.DrawTexture(new Rect(l * 4, k * 4, 4, 4), trapTexture);
                                    }
                                    else
                                    {
                                        GUI.DrawTexture(new Rect(l * 4, k * 4, 4, 4), mapTexture);
                                    }
                                }
                            }
                            else
                            {
                                exploredRooms[currentFloor][j, i] = 1;
                                GUI.DrawTexture(new Rect(l * 4, k * 4, 4, 4), mapTexture);
                                GUIUtility.RotateAroundPivot(players[Array.IndexOf(AvatarToUserId, PhotonNetwork.AuthValues.UserId)].transform.eulerAngles.y, new Vector2(l * 4,k*4));
                                GUI.DrawTexture(new Rect(l * 4, k * 4, 6, 6), playerTexture);
                                GUIUtility.RotateAroundPivot(players[Array.IndexOf(AvatarToUserId, PhotonNetwork.AuthValues.UserId)].transform.eulerAngles.y*-1, new Vector2(l * 4,k*4));
                                //minimapArrow.rectTransform.eulerAngles = new Vector3(minimapArrow.rectTransform.eulerAngles.x,minimapArrow.rectTransform.eulerAngles.y,players[Array.IndexOf(AvatarToUserId, PhotonNetwork.AuthValues.UserId)].transform.eulerAngles.y*-1);
                            }
                        }
                    }
                    l++;
                }
                k++;
            }
        }

        public Texture2D MakeTex( int width, int height, Color col )
        {
            Color[] pix = new Color[width * height];
            for( int i = 0; i < pix.Length; ++i )
            {
                pix[ i ] = col;
            }
            Texture2D result = new Texture2D( width, height );
            result.SetPixels( pix );
            result.Apply();
            return result;
        }

    }
    
}