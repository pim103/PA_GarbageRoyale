using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class PauseMenu : MonoBehaviour
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
            if (Screen.fullScreen)
            {
                textSetFullscreen.text = "Passer en mode fenêtré";
            }
            else
            {
                textSetFullscreen.text = "Passer en mode Plein écran";
            }
            baseMenu.SetActive(true);
            settingsMenu.SetActive(false);
            
            settingsButton.onClick.AddListener(GoToSettings);
            disconnectButton.onClick.AddListener(BackToMenu);
            settingsReturn.onClick.AddListener(GoToMainPause);
            exitButton.onClick.AddListener(AskForExit);
        }

        public void GoToMainPause()
        {
            settingsMenu.SetActive(false);
            baseMenu.SetActive(true);
            
        }

        public void GoToSettings()
        {
            baseMenu.SetActive(false);
            settingsMenu.SetActive(true);

            setFullscreen.onClick.AddListener(ChangeScreenState);
            returnToBase.onClick.AddListener(GoToMainPause);
        }

        public void ChangeScreenState()
        {
            Debug.Log(Screen.fullScreen);
            Screen.fullScreen = !Screen.fullScreen;
            ChangeButtonScreenStatText();
            Debug.Log(Screen.fullScreen);
        }

        public void ChangeButtonScreenStatText()
        {
            if (!Screen.fullScreen)
            {
                textSetFullscreen.text = "Passer en mode fenêtré";
            }
            else
            {
                textSetFullscreen.text = "Passer en mode Plein écran";
            }
        }

        public void ResumeGame()
        {
            controller.GetComponent<PauseController>().isInEscapeMenu = false;
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void AskForExit()
        {
            Application.Quit();
        }
    }
}
