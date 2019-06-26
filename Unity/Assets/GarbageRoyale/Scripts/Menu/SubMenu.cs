using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class SubMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Slider choosePlayers;
        [SerializeField]
        private Button createRoomButton;
        [SerializeField]
        private Button backButton;
        [SerializeField]
        private Text nbPlayers;

        [SerializeField] 
        private InputField roomNameIpt;

        [SerializeField]
        private StartGame controller;

        // Start is called before the first frame update
        void Start()
        {
            createRoomButton.onClick.AddListener(AskForCreate);
            backButton.onClick.AddListener(AskForBack);

            choosePlayers.onValueChanged.AddListener(delegate { changeText(); });
            
            nbPlayers.text = choosePlayers.value.ToString();
        }

        void changeText()
        {
            nbPlayers.text = choosePlayers.value.ToString();
        }

        void AskForCreate()
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)choosePlayers.value;

            PhotonNetwork.CreateRoom(roomNameIpt.text, roomOptions);
        }

        void AskForBack()
        {
            controller.launchMainMenu();
        }

        public override void OnJoinedRoom()
        {
            controller.gameController.SetActive(true);
            /*controller.invHUD.SetActive(true);
            controller.playerGUI.SetActive(true);*/

            controller.mainCamera.enabled = false;
            controller.mainMenu.SetActive(false);
            controller.subMenu.SetActive(false);
            //PhotonNetwork.LoadLevel("ProceduralMapGeneration");
        }
    }
}