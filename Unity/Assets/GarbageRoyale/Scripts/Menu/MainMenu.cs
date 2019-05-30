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
        private Button offlineRoomButton;
        [SerializeField] 
        private Button createRoomButton;
        [SerializeField] 
        private Button joinRoomButton;
        [SerializeField]
        private Button exitRoomButton;

        [SerializeField]
        private StartGame controller;
    
        // Start is called before the first frame update
        void Start()
        {
            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;
            
            //PhotonNetwork.ConnectUsingSettings();
            
            offlineRoomButton.onClick.AddListener(AskForOffline);
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
            }
        }

        public void AskForRoomCreation()
        {
            controller.launchSubMenu();
        }
    
        public void AskForRoomJoin()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinedRoom()
        {
            controller.gameController.SetActive(true);
            controller.mainCamera.enabled = false;
            controller.mainMenu.SetActive(false);
            controller.subMenu.SetActive(false);
            /*controller.invHUD.SetActive(true);
            controller.playerGUI.SetActive(true);*/

            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;
            offlineRoomButton.interactable = false;
            exitRoomButton.interactable = false;
        }

        public void AskForOffline()
        {
            if(PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.CreateRoom("offlineRoom");
        }

        public void AskForExit()
        {
            Application.Quit();
        }
    }
}
