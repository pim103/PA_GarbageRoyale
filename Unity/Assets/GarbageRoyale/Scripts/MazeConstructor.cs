using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class MazeConstructor : MonoBehaviourPunCallbacks

    {
        //1
        public bool showDebug;
        [SerializeField] private GameObject [] Prefabs;
        [SerializeField] private GameObject floorTransition;
        [SerializeField] private Material mazeMat1;
        [SerializeField] private Material mazeMat2;
        [SerializeField] private Material startMat;
        [SerializeField] private Material treasureMat;

        public MazeDataGenerator dataGenerator;
        private MazeMeshGenerator meshGenerator;

        public int[][,] floors;
        public int[][,] floorsRooms;

        //2
        public int[,] data
        {
            get; private set;
        }

        //3
        void Awake()
        {
            meshGenerator = new MazeMeshGenerator();

            dataGenerator = new MazeDataGenerator();

            // default to walls surrounding a single empty cell
            data = new int[81,81];
            floors = new int[8][,];
            floorsRooms = new int[8][,];

        }
    
        public void GenerateNewMaze(int sizeRows, int sizeCols)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                floors = dataGenerator.FromDimensions(sizeRows, sizeCols);
                floorsRooms = dataGenerator.RoomData(sizeRows, sizeCols, floors);
                for (int i = 0; i < 8; i++)
                {
                    DisplayMaze(i * 16,floors[i], floorsRooms[i]);
                }
                
            }
            else
            {
                photonView.RPC("SendData", RpcTarget.MasterClient);
                //PhotonNetwork.JoinRandomRoom();
            }
            
        }

        void OnGUI()
        {
            //1
            if (!showDebug)
            {
                return;
            }

            //2
            int[,] maze = data;
            int rMax = maze.GetUpperBound(0);
            int cMax = maze.GetUpperBound(1);

            string msg = "";

            //3
            for (int i = rMax; i >= 0; i--)
            {
                for (int j = 0; j <= cMax; j++)
                {
                    if (maze[i, j] == 0)
                    {
                        msg += "....";
                    }
                    else
                    {
                        msg += "==";
                    }
                }
                msg += "\n";
            }

            //4
            GUI.Label(new Rect(20, 20, 500, 500), msg);
        }
    
        private void DisplayMaze(int ypos, int[,] maze, int[,] rooms)
        {
            GameObject go = new GameObject();
            go.transform.position = Vector3.zero;
            go.name = "Procedural Maze";

            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = meshGenerator.FromData(maze,ypos,Prefabs, rooms, floorTransition);
    
            MeshCollider mc = go.AddComponent<MeshCollider>();
            mc.sharedMesh = mf.mesh;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.materials = new Material[2] {mazeMat1, mazeMat2};
        }
    
        /*+-[PunRPC]
        public void GetDatas(int[,] d,string str)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //welcomeMessageText.text = $"Client {playerNum} said Hello";
                //photonView.RPC("SayHello", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber);
                photonView.RPC("getDatas", RpcTarget.Others, data, "hey");
            }
            else
            {
                data = d;
                Debug.Log(str);
            }
        }*/
        
        [PunRPC]
        public void SendData(PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            var strFloors = "";
            var strRooms = "";
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, floors);
            
            strFloors = Convert.ToBase64String(ms.ToArray());
            
            BinaryFormatter bf2 = new BinaryFormatter();
            MemoryStream ms2 = new MemoryStream();
            bf2.Serialize(ms2, floorsRooms);
            strRooms = Convert.ToBase64String(ms2.ToArray());
            
            /*for (var i = 0; i < 8; i++)
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, floors[i]);
        
                strarray[i] = Convert.ToBase64String(ms.ToArray());
            }
                BinaryFormatter ba = new BinaryFormatter();
                MemoryStream ma = new MemoryStream();
                ba.Serialize(ma, strarray);
            
                str = Convert.ToBase64String(ma.ToArray());*/
            photonView.RPC("GenerateClientMaze", info.Sender, strFloors, strRooms);
        }
        
        [PunRPC]
        public void GenerateClientMaze(string strFloors, string strRooms)
        {
            int[][,] maze = floors;
            int[][,] rooms = floorsRooms;
            //Debug.Log(strRooms);
            if (PhotonNetwork.IsMasterClient) return;
            BinaryFormatter bf = new BinaryFormatter();
            Byte[] by = Convert.FromBase64String(strFloors);
            MemoryStream sr = new MemoryStream(by);
            maze = (int[][,])bf.Deserialize(sr);
            
            BinaryFormatter bf2 = new BinaryFormatter();
            Byte[] by2 = Convert.FromBase64String(strRooms);
            MemoryStream sr2 = new MemoryStream(by2);
            rooms = (int[][,])bf2.Deserialize(sr2);
            
            /*BinaryFormatter bf = new BinaryFormatter();
            Byte[] by = Convert.FromBase64String(str);
            MemoryStream sr = new MemoryStream(by);

            strarray = (string[])bf.Deserialize(sr);
            
            for (int i = 0; i < 8; i++)
            {
                BinaryFormatter ba = new BinaryFormatter();
                Byte[] bb = Convert.FromBase64String(strarray[i]);
                MemoryStream sa = new MemoryStream(bb);

                maze[] = (int[,])ba.Deserialize(sa);
            }*/
            floors = maze;
            floorsRooms = rooms;
            for (int i = 0; i < 8; i++)
            {
                DisplayMaze(i*16,maze[i], rooms[i]);
            }
        }

    }
   }

