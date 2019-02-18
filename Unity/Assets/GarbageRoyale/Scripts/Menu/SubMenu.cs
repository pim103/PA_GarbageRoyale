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

        private StartGame controller;

        // Start is called before the first frame update
        void Start()
        {
            controller = GameObject.Find("Scripts").GetComponent<StartGame>();

            createRoomButton.onClick.AddListener(AskForCreate);
            backButton.onClick.AddListener(AskForBack);

            choosePlayers.onValueChanged.AddListener(delegate { changeText(); });
            
            nbPlayers.text = choosePlayers.value.ToString();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void changeText()
        {
            nbPlayers.text = choosePlayers.value.ToString();
        }

        void AskForCreate()
        {
            StaticSwitchScene.gameSceneNbPlayers = choosePlayers.value;
            PhotonNetwork.CreateRoom("Test");
        }

        void AskForBack()
        {
            controller.launchMainMenu();
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel("ProceduralMapGeneration");
        }
    }
}