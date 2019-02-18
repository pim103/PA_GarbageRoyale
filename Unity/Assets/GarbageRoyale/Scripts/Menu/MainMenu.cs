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

        private StartGame controller;
    
        // Start is called before the first frame update
        void Start()
        {
            controller = GameObject.Find("Scripts").GetComponent<StartGame>();

            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;
            exitRoomButton.interactable = false;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.UseAlternativeUdpPorts = true;
        }

        private void Update()
        {
            
        }

        public override void OnConnectedToMaster()
        {
            createRoomButton.interactable = true;
            joinRoomButton.interactable = true;
            exitRoomButton.interactable = true;

            createRoomButton.onClick.AddListener(AskForRoomCreation);
            joinRoomButton.onClick.AddListener(AskForRoomJoin);
            exitRoomButton.onClick.AddListener(AskForExit);
        }

        public void AskForRoomCreation()
        {
            controller.launchSubMenu();
        }
    
        public void AskForRoomJoin()
        {
            PhotonNetwork.LoadLevel("ProceduralMapGeneration");
        }

        public void AskForExit()
        {
            Application.Quit();
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel("ProceduralMapGeneration");
        }
    }
}


