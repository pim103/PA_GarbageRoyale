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

        private StartGame controller;
    
        // Start is called before the first frame update
        void Start()
        {
            controller = GameObject.Find("Scripts").GetComponent<StartGame>();

            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;

            PhotonNetwork.ConnectUsingSettings();

            offlineRoomButton.onClick.AddListener(AskForOffline);
            exitRoomButton.onClick.AddListener(AskForExit);
        }

        public override void OnConnectedToMaster()
        {
            if(!PhotonNetwork.OfflineMode)
            {
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
            PhotonNetwork.LoadLevel("ProceduralMapGeneration");
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
