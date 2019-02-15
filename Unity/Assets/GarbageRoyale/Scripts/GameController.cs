using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

        private GameObject playerCamera;

        //List<GameObject> characterList = new List<GameObject>();
        public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
 
        private GUIStyle currentStyle = null;
        private Texture2D mapTexture;
        private Texture2D playerTexture;
        private float currentPosX;
        private float currentPosY;
        private float currentPosZ;
        private bool wantToGoUp;

        private bool isGameStart;

        void Start()
        {
            mapTexture = MakeTex(4, 4, new Color(0f, 0f, 0f, 0.5f));
            playerTexture = MakeTex(4, 4, new Color(1f, 1f, 1f, 0.5f));
            generator = GetComponent<MazeConstructor>();      // 2
            generator.GenerateNewMaze(81, 81);
            playerCamera = Instantiate(cameraPrefab, new Vector3(150, 0.9f, 150), Quaternion.identity);

            if (PhotonNetwork.IsMasterClient)
            {
                characterList.Add(PhotonNetwork.LocalPlayer.ActorNumber,PhotonNetwork.Instantiate(player.name, new Vector3(150, 0.7f, 150), Quaternion.identity));
                playerCamera.transform.SetParent(characterList[PhotonNetwork.LocalPlayer.ActorNumber].transform);
            }

            isGameStart = false;
            wantToGoUp = false;
            //characterList.Add(player);
            //OnlinePlayerManager.RefreshInstance(ref LocalPlayer, PlayerPrefab);
        }

        private void FixedUpdate()
        {
            Water water;

            if (!PhotonNetwork.IsMasterClient)
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
        }

        public override void OnJoinedRoom()
        {
            
        }


        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            characterList.Add(newPlayer.ActorNumber,PhotonNetwork.Instantiate(player.name, new Vector3(150, 0.7f, 150), Quaternion.identity));
            //if(PhotonNetwork.IsMasterClient) PhotonNetwork.Instantiate(player.name, new Vector3(150, 0.7f, 150), Quaternion.identity);
            //PhotonNetwork.Instantiate(player.name, new Vector3(150, 0.7f, 150), Quaternion.identity);
            //Instantiate(characterObject, new Vector3(150, 0.7f, 150), Quaternion.identity);
            //OnlinePlayerManager.RefreshInstance(ref LocalPlayer, PlayerPrefab);
        }

        [PunRPC]
        private void MovePlayer(float axeX, float axeZ, bool wantToGoUp,PhotonMessageInfo info)
        {
            PlayerMovement target;
            PlayerStats targetStats;
            if(!PhotonNetwork.IsMasterClient) return;

            target = characterList[info.Sender.ActorNumber].GetComponent<PlayerMovement>();
            targetStats = characterList[info.Sender.ActorNumber].GetComponent<PlayerStats>();
            if(!targetStats.getIsDead())
            {
                target.Movement(axeX, axeZ, wantToGoUp);
            }
        }
        
        [PunRPC]
        private void SendCameraPosition(float axeX, float axeY,PhotonMessageInfo info)
        {
            PlayerMovement target;
            if(!PhotonNetwork.IsMasterClient) return;
            //Debug.Log("Player "+ info.Sender.ActorNumber+" asked to move the camera! X: " + axeX + " Z : " + axeY);
            characterList[info.Sender.ActorNumber].transform.Rotate(0, axeX * 10.0f, 0);
            photonView.RPC("RotatePlayerCamera", info.Sender, characterList[info.Sender.ActorNumber].transform.position.x, characterList[info.Sender.ActorNumber].transform.position.y, characterList[info.Sender.ActorNumber].transform.position.z, axeX, axeY);
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
                        if (generator.floors[0][j, i] == 0)
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