using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class PauseMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] 
        private GameObject baseMenu;
        [SerializeField]
        private Button settingsButton;
        [SerializeField]
        private Button settingsReturn;
        [SerializeField] 
        private Button disconnectButton;
        [SerializeField] 
        private Button exitButton;
        
        [SerializeField] 
        private GameObject settingsMenu;
        [SerializeField] 
        private Button returnToBase;
        [SerializeField]
        private Button setFullscreen;
        [SerializeField]
        private Text textSetFullscreen;
        
        [SerializeField]
        private GameController controller;
    
        // Start is called before the first frame update
        void Start()
        {
            disconnectButton.interactable = false;
            if(!PhotonNetwork.OfflineMode)
            {
                Debug.Log(PhotonNetwork.AuthValues);
                disconnectButton.interactable = true;
            }
            settingsButton.onClick.AddListener(GoToSettings);
            exitButton.onClick.AddListener(AskForExit);
        }

        public void GoToMainPause()
        {
            settingsMenu.SetActive(false);
            baseMenu.SetActive(true);
            
            setFullscreen.onClick.AddListener(changeScreenState);
        }

        public void GoToSettings()
        {
            baseMenu.SetActive(false);
            settingsMenu.SetActive(true);
            
            returnToBase.onClick.AddListener(GoToMainPause);
        }

        public void changeScreenState()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        public void AskForExit()
        {
            Application.Quit();
        }
    }
}
