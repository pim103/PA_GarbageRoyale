using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Menu
{
    public class StartGame : MonoBehaviour
    {
        [SerializeField]
        public GameObject mainMenu;
        [SerializeField]
        public GameObject registerMenu;
        [SerializeField]
        public GameObject loginMenu;
        [SerializeField]
        public GameObject subMenu;
        [SerializeField] 
        public GameObject lobbyMenu;
        [SerializeField]
        public GameObject EndGameMenu;

        [SerializeField]
        public Camera mainCamera;

        [SerializeField]
        public GameObject gameController;
        [SerializeField]
        public GameObject invHUD;
        [SerializeField]
        public GameObject playerGUI;

        // Start is called before the first frame update
        void Start()
        {
            launchLoginMenu();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void launchMainMenu()
        {
            ResetAllMenu();
            mainMenu.SetActive(true);
        }

        public void launchSubMenu()
        {
            ResetAllMenu();
            subMenu.SetActive(true);
        }
        
        public void launchLobbyMenu()
        {
            ResetAllMenu();
            lobbyMenu.SetActive(true);
        }
        
        public void launchRegisterMenu()
        {
            ResetAllMenu();
            registerMenu.SetActive(true);
        }
        
        public void launchLoginMenu()
        {
            ResetAllMenu();
            loginMenu.SetActive(true);
        }

        public void launchEndGameMenu(EndGameMenu.StateEndGame sg, int idPlayer)
        {
            ResetAllMenu();
            gameController.SetActive(false);
            EndGameMenu.SetActive(true);
            EndGameMenu egm = EndGameMenu.GetComponent<EndGameMenu>();
            egm.SetEndMessage(sg, idPlayer);
        }

        private void ResetAllMenu()
        {
            loginMenu.SetActive(false);
            mainMenu.SetActive(false);
            registerMenu.SetActive(false);
            subMenu.SetActive(false);
            EndGameMenu.SetActive(false);
        }
    }
}