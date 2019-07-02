using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class MazeConstructor : MonoBehaviourPunCallbacks

    {
        //1
        public bool showDebug;
        [SerializeField] public GameObject [] Prefabs;
        [SerializeField] public GameObject floorTransition;
        [SerializeField] private Material mazeMat1;
        [SerializeField] private Material mazeMat2;

        public MazeDataGenerator dataGenerator;
        public MazeMeshGenerator meshGenerator;

        public int[][,] floors;
        public int[][,] floorsRooms;
        public Dictionary<string, int>[] trapId = new Dictionary<string, int>[8];
        public Dictionary<string, int>[] itemRoom = new Dictionary<string, int>[8];

        public List<NavMeshSurface> Surfaces;
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
    
        public void GenerateNewMaze(int sizeRows, int sizeCols, int nbItems)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                floors = dataGenerator.FromDimensions(sizeRows, sizeCols);
                floorsRooms = dataGenerator.RoomData(sizeRows, sizeCols, floors, nbItems);
                trapId = dataGenerator.roomTrap;
                itemRoom = dataGenerator.itemRoom;

                for (int i = 0; i < 8; i++)
                {
                    DisplayMaze(i * 16,floors[i], floorsRooms[i], i);
                }

                foreach (var surface in Surfaces)
                {
                    /*surface.collectObjects = CollectObjects.Volume;
                    surface.size = new Vector3(8*16,3,8*16);*/
                    surface.BuildNavMesh();
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
    
        private void DisplayMaze(int ypos, int[,] maze, int[,] rooms, int level)
        {
            GameObject go = new GameObject();
            go.transform.position = Vector3.zero;
            go.name = "Procedural Maze";

            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = meshGenerator.FromData(maze,ypos,Prefabs, rooms, floorTransition, trapId[level], itemRoom[level], false);
    
            MeshCollider mc = go.AddComponent<MeshCollider>();
            mc.sharedMesh = mf.mesh;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.materials = new Material[2] {mazeMat1, mazeMat2};

            go.AddComponent<NavMeshSurface>();
            Surfaces.Add(go.GetComponent<NavMeshSurface>());
            
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
            var strTrap = "";
            var strItemRoom = "";

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, floors);
            
            strFloors = Convert.ToBase64String(ms.ToArray());
            
            BinaryFormatter bf2 = new BinaryFormatter();
            MemoryStream ms2 = new MemoryStream();
            bf2.Serialize(ms2, floorsRooms);
            strRooms = Convert.ToBase64String(ms2.ToArray());

            BinaryFormatter bf3 = new BinaryFormatter();
            MemoryStream ms3 = new MemoryStream();
            bf3.Serialize(ms3, trapId);
            strTrap = Convert.ToBase64String(ms3.ToArray());

            BinaryFormatter bf4 = new BinaryFormatter();
            MemoryStream ms4 = new MemoryStream();
            bf4.Serialize(ms4, itemRoom);
            strItemRoom = Convert.ToBase64String(ms4.ToArray());

            photonView.RPC("GenerateClientMaze", info.Sender, strFloors, strRooms, strTrap, strItemRoom);
        }
        
        [PunRPC]
        public void GenerateClientMaze(string strFloors, string strRooms, string strTrap, string strItemRoom)
        {
            int[][,] maze = floors;
            int[][,] rooms = floorsRooms;
            
            if (PhotonNetwork.IsMasterClient) return;

            BinaryFormatter bf = new BinaryFormatter();
            Byte[] by = Convert.FromBase64String(strFloors);
            MemoryStream sr = new MemoryStream(by);
            maze = (int[][,])bf.Deserialize(sr);
            
            BinaryFormatter bf2 = new BinaryFormatter();
            Byte[] by2 = Convert.FromBase64String(strRooms);
            MemoryStream sr2 = new MemoryStream(by2);
            rooms = (int[][,])bf2.Deserialize(sr2);

            BinaryFormatter bf3 = new BinaryFormatter();
            Byte[] by3 = Convert.FromBase64String(strTrap);
            MemoryStream sr3 = new MemoryStream(by3);
            trapId = (Dictionary<string, int>[])bf3.Deserialize(sr3);

            BinaryFormatter bf4 = new BinaryFormatter();
            Byte[] by4 = Convert.FromBase64String(strItemRoom);
            MemoryStream sr4 = new MemoryStream(by4);
            itemRoom = (Dictionary<string, int>[])bf4.Deserialize(sr4);

            floors = maze;
            floorsRooms = rooms;
            for (int i = 0; i < 8; i++)
            {
                DisplayMaze(i*16,maze[i], rooms[i], i);
            }
        }

    }
   }

