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
            joinButton.interactable = false;
            joinButton.onClick.AddListener(AskForJoin);
            dialogButtonBtn.onClick.AddListener(ConfirmationDialogBox);
            backButton.onClick.AddListener(AskForBack);
        }

        void AskForJoin()
        {
            Debug.Log(roomSelected);
            PhotonNetwork.JoinRoom(roomSelected);
            joinButton.interactable = false;
            dialogWindow.SetActive(true);
            dialogText.text = "Connexion à la partie";
        }

        public override void OnJoinedRoom()
        {
            controller.gameController.SetActive(true);


            controller.mainCamera.enabled = false;
            controller.mainMenu.SetActive(false);
            controller.subMenu.SetActive(false);
            controller.launchRoomLobby();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            dialogText.text = "Erreur : Impossible de rejoindre la partie";
            joinButton.interactable = true;
            dialogButton.SetActive(true);
            
        }

        void SelectRoom(RoomListing listing, string name)
        {
            roomSelected = name;
            listing.SelectRoom();
            joinButton.interactable = true;
        }
        
        public void ConfirmationDialogBox()
        {
            dialogWindow.SetActive(false);
            dialogButton.SetActive(false);
        }

        void AskForBack()
        {
            PhotonNetwork.LeaveLobby();
            controller.launchMainMenu();
        }
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                if (info.RemovedFromList)
                {
                    Debug.Log(info.Name);
                    int index = listRooms.FindIndex(x => x.RoomInfo.Name == info.Name);
                    if (index != -1)
                    {
                        Destroy(listRooms[index].gameObject);
                        listRooms.RemoveAt(index);
                    }
                }
                else
                {
                    int index = listRooms.FindIndex(x => x.RoomInfo.Name == info.Name);
                    if (index != -1)
                    {
                        listRooms[index].SetRoomInfo(info);
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
                            listing.RoomInfo = info;
                            listRooms.Add(listing);
                        }
                    }
                }
            }
        }
    }
}