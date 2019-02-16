using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

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

        private GameObject playerCamera;
        
        private bool canMove = false;
        //List<GameObject> characterList = new List<GameObject>();
        public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
        public Dictionary <int, GameObject> lampList = new Dictionary<int, GameObject>();
        public Dictionary<int, GameObject> characterSound = new Dictionary<int, GameObject>();

        private GUIStyle currentStyle = null;
        private Texture2D mapTexture;
        private Texture2D playerTexture;
        private float currentPosX;
        private float currentPosY;
        private float currentPosZ;
        private bool wantToGoUp;

        private bool isGameStart;

        public Dictionary<string, string>[] roomLinksList;

        void Start()
        {
            mapTexture = MakeTex(4, 4, new Color(1f, 1f, 1f, 0.5f));
            playerTexture = MakeTex(4, 4, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            generator = GetComponent<MazeConstructor>();      // 2
            generator.GenerateNewMaze(81, 81);
            playerCamera = Instantiate(cameraPrefab, new Vector3(150, 0.9f, 150), Quaternion.identity);

            if (PhotonNetwork.IsMasterClient)
            {
                characterList.Add(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.Instantiate(player.name, new Vector3(150, 0.7f, 150), Quaternion.identity));
                characterSound.Add(PhotonNetwork.LocalPlayer.ActorNumber, Instantiate(soundObject, new Vector3(150, 2.5f, 150), Quaternion.identity));
                playerCamera.transform.SetParent(characterList[PhotonNetwork.LocalPlayer.ActorNumber].transform);
            }

            roomLinksList = generator.dataGenerator.roomLinksList;
            isGameStart = false;
            canMove = true;
            wantToGoUp = false;
        }

        private void FixedUpdate()
        {
            Water water;

            photonView.RPC("SendSoundPosition", RpcTarget.MasterClient, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

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

                photonView.RPC("MovePlayer", RpcTarget.MasterClient,Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), wantToGoUp);
                photonView.RPC("SendCameraPosition", RpcTarget.MasterClient,Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            } else
            {
                if (Input.GetKeyDown(KeyCode.G) && !isGameStart)
                {
                    water = GetComponent<Water>();
                    isGameStart = true;
                    water.setStartWater(isGameStart);
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
            characterSound.Add(newPlayer.ActorNumber, Instantiate(soundObject, new Vector3(150, 2.5f, 150), Quaternion.identity));

            if (!PhotonNetwork.IsMasterClient) return;

            characterList.Add(newPlayer.ActorNumber,PhotonNetwork.Instantiate(player.name, new Vector3(150, 0.7f, 150), Quaternion.identity));
            photonView.RPC("initOwnSound", newPlayer, newPlayer.ActorNumber);

            //Instancie Chaque objet son
            foreach (KeyValuePair<int, GameObject> eachPlayer in characterSound)
            {
                if(eachPlayer.Key != newPlayer.ActorNumber)
                {
                    photonView.RPC("InstantiateOtherSound", newPlayer, eachPlayer.Key, eachPlayer.Value.transform.position.x, eachPlayer.Value.transform.position.y, eachPlayer.Value.transform.position.z);
                }
            }
        }

        [PunRPC]
        private void initOwnSound(int idPlayer)
        {
            characterSound.Add(idPlayer, Instantiate(soundObject, new Vector3(150, 2.5f, 150), Quaternion.identity));
        }

        [PunRPC]
        private void InstantiateOtherSound(int idPlayer, float x, float y, float z)
        {
            Debug.Log("Other id" + idPlayer);
            characterSound.Add(idPlayer, Instantiate(soundObject, new Vector3(x, y, z), Quaternion.identity));
        }

        [PunRPC]
        private void MovePlayer(float axeX, float axeZ, bool wantToGoUp, PhotonMessageInfo info)
        {
            PlayerMovement target;
            PlayerStats targetStats;
            
            if (!PhotonNetwork.IsMasterClient) return;

            target = characterList[info.Sender.ActorNumber].GetComponent<PlayerMovement>();
            targetStats = characterList[info.Sender.ActorNumber].GetComponent<PlayerStats>();
            if (!targetStats.getIsDead())
            {
                //Debug.Log("Player "+ info.Sender.ActorNumber+" asked to move! X: " + axeX + " Z : " + axeZ);
                target = characterList[info.Sender.ActorNumber].GetComponent<PlayerMovement>();
                target.Movement(axeX, axeZ, wantToGoUp);
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

            //playerCamera.transform.Rotate(0, axeX * 10.0f, 0);
            
            /*float rotationX = 0;
            rotationX -= axeY * 10.0f;
            rotationX =
                Mathf.Clamp(rotationX, -90.0f,
                    90.0f);

            /characterList[info.Sender.ActorNumber].transform.GetChild(1).localEulerAngles = new Vector3(rotationX, 0, 0);*/
            
        }
        
        [PunRPC]
        private void TurnLightOff(PhotonMessageInfo info)
        {
            Debug.Log("ui");
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
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Bouge le son pour : " + id);
            }
            characterSound[id].transform.position = new Vector3(posX, 2.5f, posZ);
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
            
            for (int i = (int)(currentPosX/4) - 10; i < (int)(currentPosX/4) + 10 ; i++)
            {
                l = 0;
                for (int j = (int)(currentPosZ/4) - 10; j < (int)(currentPosZ/4) + 10; j++)
                {
                    if (i > 0 && j > 0)
                    {
                        if (generator.floors[0][j, i] != 1)
                        {
                            if(i != (int)((currentPosX+1)/4) || j != (int)((currentPosZ+0.5)/4)) GUI.DrawTexture(new Rect(l * 4, k * 4, 4, 4), mapTexture);
                            else GUI.DrawTexture(new Rect(l * 4, k * 4, 4, 4), playerTexture);
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