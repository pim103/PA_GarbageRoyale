using System.Collections.Generic;
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

namespace GarbageRoyale.Scripts
{
    [RequireComponent(typeof(MazeConstructor))]               // 1

    public class GameController : MonoBehaviourPunCallbacks
    {
        public MazeConstructor generator;
        
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
        public InventoryGUI inventoryGui;

        [SerializeField]
        public Water water;

        private GameObject playerCamera;

        private GameObject startDoor;
        
        private bool canMove = false;
        //public List<GameObject> characterList = new List<GameObject>();
        public Dictionary<int, GameObject> characterList = new Dictionary<int, GameObject>();
        //public Dictionary <int, GameObject> mobList = new Dictionary<int, GameObject>();
        private int [][,] exploredRooms = new int[8][,];

        private GUIStyle currentStyle = null;
        private Texture2D mapTexture;
        private Texture2D playerTexture;
        private float currentPosX;
        private float currentPosY;
        private float currentPosZ;
        private bool wantToGoUp;
        private bool wantToGoDown;

        public bool isGameStart;
        private float timeLeft = 20;
        private float waterStartTimeLeft = 9999999999;
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
        public bool endOfInit;

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

        private void Awake()
        {
            nbItems = 0;
            AvatarToUserId = Enumerable.Repeat("", 8).ToArray();
            PlayerJoined += ActivateAvatar;
            PlayerLeft += null;
            OnlinePlayReady += StartGame;
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            endOfInit = false;
        }

        void Start()
        {
            endOfInit = false;
            if(!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SendUserId", RpcTarget.MasterClient, PhotonNetwork.AuthValues.UserId);
            } else
            {
                StartCoroutine(SearchForActivateAvatar());
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
            playerTexture = MakeTex(4, 4, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            generator = GetComponent<MazeConstructor>();      // 2
            generator.GenerateNewMaze(81, 81);

            moveDirection = new Vector3[10];
            rotationPlayer = new Vector3[10];
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
        }

        [PunRPC]
        public void testrpc()
        {
            Debug.Log("ALLO");
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

            AvatarToUserId[i] = userId;

            photonView.RPC("initAvatarUserIdRPC", RpcTarget.Others, AvatarToUserId, userId);

            PlayerJoined?.Invoke(i);
        }

        private IEnumerator SearchForActivateAvatar()
        {
            yield return new WaitForSeconds(0.1f);

            var i = 0;
            while(i < AvatarToUserId.Length && AvatarToUserId[i] != "")
            {
                i++;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                AvatarToUserId[i] = PhotonNetwork.AuthValues.UserId;
                PlayerJoined?.Invoke(i);
            }
        }

        [PunRPC]
        private void initAvatarUserIdRPC(string[] UserIdList, string userId)
        {
            AvatarToUserId = UserIdList;

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
                if (Input.GetKeyDown(KeyCode.U))
                {
                    waterStart = true;
                    water.setStartWater(true);
                }
            }
            /*if (canMove)
            {
                photonView.RPC("SendSoundPosition", RpcTarget.MasterClient, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }

            if (!PhotonNetwork.IsMasterClient && canMove)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    wantToGoUp = true;
                }
                else if (Input.GetKeyUp(KeyCode.Space))
                {
                    wantToGoUp = false;
                }
                if(Input.GetKeyDown(KeyCode.LeftShift))
                {
                    wantToGoDown = true;
                }
                else if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    wantToGoDown = false;
                }

                photonView.RPC("MovePlayer", RpcTarget.MasterClient,Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), wantToGoUp, wantToGoDown);
                photonView.RPC("SendCameraPosition", RpcTarget.MasterClient,Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            } else
            {
                if(playerConnected == StaticSwitchScene.gameSceneNbPlayers && timeLeft > 0)
                {
                    timeLeft -= Time.deltaTime;
                } else if(timeLeft <= 0 && !isGameStart)
                {
                    PhotonNetwork.Destroy(startDoor);
                    isGameStart = true;
                } else if(isGameStart && waterStartTimeLeft > 0)
                {
                    waterStartTimeLeft -= Time.deltaTime;
                    
                } else if(waterStartTimeLeft <= 0 && !waterStart)
                {
                    Water water = GetComponent<Water>();
                    waterStart = true;
                    water.setStartWater(true);
                }

                if(Input.GetKeyDown(KeyCode.U))
                {
                    Water water = GetComponent<Water>();
                    waterStart = true;
                    water.setStartWater(true);
                }
            }

            if(pressL)
            {
                photonView.RPC("TurnLightOff", RpcTarget.MasterClient);
            }
            pressL = false;*/
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

        /*[PunRPC]
        private void initOwnSound(int idPlayer)
        {
            characterSound.Add(idPlayer, Instantiate(soundObject, new Vector3(150, 2.5f, 150), Quaternion.identity));
            characterSoundWalk.Add(idPlayer, Instantiate(soundObject, new Vector3(150, 2.5f, 150), Quaternion.identity));
        }

        [PunRPC]
        private void InstantiateOtherSound(int idPlayer, float x, float y, float z)
        {
            characterSound.Add(idPlayer, Instantiate(soundObject, new Vector3(x, y, z), Quaternion.identity));
            characterSoundWalk.Add(idPlayer, Instantiate(soundObject, new Vector3(x, y, z), Quaternion.identity));
        }

        [PunRPC]
        private void MovePlayer(float axeX, float axeZ, bool wantToGoUp, bool wantToGoDown, PhotonMessageInfo info)
        {
            PlayerMovement target;
            PlayerStats targetStats;
            
            if (!PhotonNetwork.IsMasterClient) return;

            target = characterList[info.Sender.ActorNumber].GetComponent<PlayerMovement>();
            targetStats = characterList[info.Sender.ActorNumber].GetComponent<PlayerStats>();
            if (!targetStats.getIsDead())
            {
                target.Movement(axeX, axeZ, wantToGoUp, wantToGoDown, false);
            }
        }

        [PunRPC]
        private void SendSoundPosition(float axeX, float axeY, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            photonView.RPC("MovePlayerSound", RpcTarget.AllBuffered, info.Sender.ActorNumber, characterList[info.Sender.ActorNumber].transform.position.x, characterList[info.Sender.ActorNumber].transform.position.y, characterList[info.Sender.ActorNumber].transform.position.z);
        }

        [PunRPC]
        private void SendCameraPosition(float axeX, float axeY,PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            characterList[info.Sender.ActorNumber].transform.Rotate(0, axeX * 10.0f, 0);
            photonView.RPC("RotatePlayerCamera", info.Sender, characterList[info.Sender.ActorNumber].transform.position.x, characterList[info.Sender.ActorNumber].transform.position.y, characterList[info.Sender.ActorNumber].transform.position.z, axeX, axeY);
        }
        
        [PunRPC]
        private void TurnLightOff(PhotonMessageInfo info)
        {
            Light playerLight = characterList[info.Sender.ActorNumber].transform.GetChild(1).GetComponent<Light>();
            if (playerLight.enabled) playerLight.enabled = false;
            else playerLight.enabled = true;

        }

        [PunRPC]
        private void RotatePlayerCamera(float posX, float posY, float posZ, float axeX, float axeY)
        {
            currentPosX = posX;
            currentPosY = posY;
            currentPosZ = posZ;
            playerCamera.transform.Rotate(0, axeX * 10.0f, 0);
            
            float rotationX = 0;
            rotationX -= axeY * 10.0f;
            rotationX =
                Mathf.Clamp(rotationX, -90.0f,
                    90.0f);

            float rotationY = playerCamera.transform.localEulerAngles.y;

            playerCamera.transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
            
            playerCamera.transform.position = new Vector3(posX,posY+0.8f,posZ);
        }

        [PunRPC]
        private void MovePlayerSound(int id, float posX, float posY, float posZ)
        {
            if(canMove)
            {
                characterSound[id].transform.position = new Vector3(posX, 2.5f, posZ);
                characterSoundWalk[id].transform.position = new Vector3(posX, 2.5f, posZ);
            }
        }

        private void OnGUI()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                currentPosX = characterList[PhotonNetwork.LocalPlayer.ActorNumber].transform.position.x;
                currentPosY = characterList[PhotonNetwork.LocalPlayer.ActorNumber].transform.position.y;
                currentPosZ = characterList[PhotonNetwork.LocalPlayer.ActorNumber].transform.position.z;
            }
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
                                    GUI.DrawTexture(new Rect(l * 4, k * 4, 4, 4), mapTexture);
                                }
                            }
                            else
                            {
                                exploredRooms[currentFloor][j, i] = 1;
                                GUI.DrawTexture(new Rect(l * 4, k * 4, 4, 4), playerTexture);

                            }
                        }
                    }
                    l++;
                }
                k++;
            }
        }*/

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