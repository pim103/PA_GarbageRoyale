using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

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

        private GameObject playerCamera;

        private GameObject startDoor;
        
        private bool canMove = false;
        //List<GameObject> characterList = new List<GameObject>();
        public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
        public Dictionary <int, GameObject> lampList = new Dictionary<int, GameObject>();
        public Dictionary <int, GameObject> characterSound = new Dictionary<int, GameObject>();
        private int [][,] exploredRooms = new int[8][,];

        private GUIStyle currentStyle = null;
        private Texture2D mapTexture;
        private Texture2D playerTexture;
        private float currentPosX;
        private float currentPosY;
        private float currentPosZ;
        private bool wantToGoUp;
        private bool wantToGoDown;

        private bool isGameStart;
        private float timeLeft = 20;
        private float waterStartTimeLeft = 60;
        private bool waterStart;

        public Dictionary<string, string>[] roomLinksList;

        private int playerConnected;

        void Start()
        {
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
            playerCamera = Instantiate(cameraPrefab, new Vector3(150, 0.9f, 150), Quaternion.identity);
            canMove = false;
            
            if (PhotonNetwork.IsMasterClient)   
            {
                if (!PhotonNetwork.OfflineMode)
                {
                    startDoor = PhotonNetwork.Instantiate(startDoorPrefab.name, new Vector3(142, 0.7f, 160), Quaternion.identity);
                }
                characterList.Add(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.Instantiate(player.name, new Vector3(150, 0.7f, 150), Quaternion.identity));
                characterList[PhotonNetwork.LocalPlayer.ActorNumber].GetComponent<PlayerStats>().setId(PhotonNetwork.LocalPlayer.ActorNumber);
                characterSound.Add(PhotonNetwork.LocalPlayer.ActorNumber, Instantiate(soundObject, new Vector3(150, 0.7f, 150), Quaternion.identity));
                playerCamera.transform.SetParent(characterList[PhotonNetwork.LocalPlayer.ActorNumber].transform);
                canMove = true;
            }

            playerConnected = 1;
            roomLinksList = generator.dataGenerator.roomLinksList;
            isGameStart = false;
            wantToGoUp = false;
            waterStart = false;                
            wantToGoDown = false;
        }

        private void FixedUpdate()
        {
            if (canMove)
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

            if (Input.GetKeyUp(KeyCode.L))
            {
                photonView.RPC("TurnLightOff", RpcTarget.MasterClient);
            }
        }

        public override void OnJoinedRoom()
        {

        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            characterSound.Add(newPlayer.ActorNumber, Instantiate(soundObject, new Vector3(150, 0.7f, 150), Quaternion.identity));

            if (!PhotonNetwork.IsMasterClient) return;

            playerConnected += 1;
            
            characterList.Add(newPlayer.ActorNumber,PhotonNetwork.Instantiate(player.name, new Vector3(150, 0.7f, 150), Quaternion.identity));
            characterList[newPlayer.ActorNumber].GetComponent<PlayerStats>().setId(newPlayer.ActorNumber);
            photonView.RPC("initOwnSound", newPlayer, newPlayer.ActorNumber);

            //Instancie Chaque objet son
            foreach (KeyValuePair<int, GameObject> eachPlayer in characterSound)
            {
                if(eachPlayer.Key != newPlayer.ActorNumber)
                {
                    photonView.RPC("InstantiateOtherSound", newPlayer, eachPlayer.Key, eachPlayer.Value.transform.position.x, eachPlayer.Value.transform.position.y, eachPlayer.Value.transform.position.z);
                }
            }
            
            photonView.RPC("setCanMove", newPlayer, null);
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
        private void initOwnSound(int idPlayer)
        {
            characterSound.Add(idPlayer, Instantiate(soundObject, new Vector3(150, 2.5f, 150), Quaternion.identity));
        }

        [PunRPC]
        private void InstantiateOtherSound(int idPlayer, float x, float y, float z)
        {
            characterSound.Add(idPlayer, Instantiate(soundObject, new Vector3(x, y, z), Quaternion.identity));
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
                target = characterList[info.Sender.ActorNumber].GetComponent<PlayerMovement>();
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
            
            playerCamera.transform.position = new Vector3(posX,posY+0.2f,posZ);
        }

        [PunRPC]
        private void MovePlayerSound(int id, float posX, float posY, float posZ)
        {
            if(canMove)
            {
                characterSound[id].transform.position = new Vector3(posX, 2.5f, posZ);
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