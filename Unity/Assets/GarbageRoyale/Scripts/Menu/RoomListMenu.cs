using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class RoomListMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Transform contentList;
        [SerializeField] 
        private RoomListing roomListing;
        [SerializeField]
        private Button joinButton;
        [SerializeField]
        private Button backButton;
        [SerializeField]
        private StartGame controller;
        [SerializeField]
        private GameObject dialogWindow;
        [SerializeField]
        private Text dialogText;
        [SerializeField]
        private GameObject dialogButton;
        [SerializeField]
        private Button dialogButtonBtn;

        private string roomSelected;
        
        //private List<RoomInfo> listOfRooms = new List<RoomInfo>();
        
        public List<RoomListing> listRooms = new List<RoomListing>();
        
        void Start()
        {
            joinButton.onClick.AddListener(AskForJoin);
            backButton.onClick.AddListener(AskForBack);
        }

        private void Awake()
        {
            PhotonNetwork.JoinLobby();
        }

        void AskForJoin()
        {
            Debug.Log(roomSelected);
            PhotonNetwork.JoinRoom(roomSelected);
            dialogWindow.SetActive(true);
            dialogText.text = "Connexion à la partie";
        }

        public override void OnJoinedRoom()
        {
            controller.gameController.SetActive(true);
            /*controller.invHUD.SetActive(true);
            controller.playerGUI.SetActive(true);*/

            controller.mainCamera.enabled = false;
            controller.mainMenu.SetActive(false);
            controller.subMenu.SetActive(false);
            controller.launchRoomLobby();
            //PhotonNetwork.LoadLevel("ProceduralMapGeneration");
        }
        
        void SelectRoom(RoomListing listing, string name)
        {
            roomSelected = name;
            listing.SelectRoom();
        }

        void AskForBack()
        {
            controller.launchMainMenu();
        }
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                if (info.RemovedFromList)
                {
                    /*int index = listRooms.FindIndex(x => x.RoomInfo.Name == info.Name);
                    if (index != -1)
                    {
                        Destroy(listRooms[index].gameObject);
                        listRooms.RemoveAt(index);
                    }*/
                }
                else
                {
                    RoomListing listing = Instantiate(roomListing, contentList);
                    if (listing != null)
                    {
                        listing.SetRoomInfo(info);
                        listing.GetComponent<Button>().onClick.AddListener(delegate
                        {
                            SelectRoom(listing, info.Name);
                        });
                        listRooms.Add(listing);
                    }
                }
            }
        }
    }
}