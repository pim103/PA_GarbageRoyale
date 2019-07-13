using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class MainMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] 
        private Button createRoomButton;
        [SerializeField] 
        private Button joinRoomButton;
        [SerializeField]
        private Button exitRoomButton;

        [SerializeField]
        private StartGame controller;

        public bool isConnected = false;

        private List<RoomInfo> roomList;
    
        // Start is called before the first frame update
        void Start()
        {
            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;
            DataState loadedData = SaveState.LoadData();
            if (loadedData != null)
            {
                if (!PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.ConnectUsingSettings();
                    isConnected = true;
                }
            }
            
            exitRoomButton.onClick.AddListener(AskForExit);
        }

        public override void OnConnectedToMaster()
        {
            if(!PhotonNetwork.OfflineMode)
            {
                Debug.Log(PhotonNetwork.AuthValues);
                createRoomButton.interactable = true;
                joinRoomButton.interactable = true;

                createRoomButton.onClick.AddListener(AskForRoomCreation);
                joinRoomButton.onClick.AddListener(AskForRoomJoin);
                //PhotonNetwork.JoinLobby();
            }
        }

        public void AskForRoomCreation()
        {
            controller.launchSubMenu();
        }

        public void AskForRoomJoin()
        {
            controller.launchServerList();
        }

        public override void OnJoinedRoom()
        {
            controller.gameController.SetActive(true);
            controller.mainCamera.enabled = false;
            controller.mainMenu.SetActive(false);
            controller.subMenu.SetActive(false);

            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;
            exitRoomButton.interactable = false;
        }

        public void AskForExit()
        {
            PhotonNetwork.Disconnect();
            controller.launchLoginMenu();
        }
    }
}
