using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class PhotonHelloWorldScripts : MonoBehaviourPunCallbacks
    {
        [SerializeField] 
        private Button createRoomButton;
    
        [SerializeField] 
        private Button joinRoomButton;
    
        [SerializeField] 
        private Text welcomeMessageText;
    
        // Start is called before the first frame update
        void Start()
        {
            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;
            PhotonNetwork.ConnectUsingSettings();
        }

        private void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                photonView.RPC("SayHello", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            
        }

        public override void OnConnectedToMaster()
        {
            createRoomButton.interactable = true;
            joinRoomButton.interactable = true;
        
            createRoomButton.onClick.AddListener(AskForRoomCreation);
            joinRoomButton.onClick.AddListener(AskForRoomJoin);
        }

        public void AskForRoomCreation()
        {
            PhotonNetwork.CreateRoom("Test");
        }
    
        public void AskForRoomJoin()
        {
            PhotonNetwork.LoadLevel("ProceduralMapGeneration");
            //PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinedRoom()
        {
            welcomeMessageText.text = PhotonNetwork.IsMasterClient ? "Room Created" : "Room Joined";
            PhotonNetwork.LoadLevel("ProceduralMapGeneration");
        }
        
        [PunRPC]
        public override void OnPlayerEnteredRoom(Player player)
        {
            photonView.RPC("SayHello", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber);
        }
        
        [PunRPC]
        public void SayHello(int playerNum)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                welcomeMessageText.text = $"Client {playerNum} said Hello";
                photonView.RPC("SayHello", RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {
                welcomeMessageText.text = "Server said Hello";
            }
        }
        }
}


