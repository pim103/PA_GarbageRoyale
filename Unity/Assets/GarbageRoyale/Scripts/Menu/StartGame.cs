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
            //launchLoginMenu();
            launchLoginMenu();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void launchMainMenu()
        {
            loginMenu.SetActive(false);
            mainMenu.SetActive(true);
            registerMenu.SetActive(false);
            subMenu.SetActive(false);
        }

        public void launchSubMenu()
        {
            loginMenu.SetActive(false);
            mainMenu.SetActive(false);
            registerMenu.SetActive(false);
            subMenu.SetActive(true);
        }
        
        public void launchRegisterMenu()
        {
            loginMenu.SetActive(false);
            mainMenu.SetActive(false);
            registerMenu.SetActive(true);
            subMenu.SetActive(false);
        }
        
        public void launchLoginMenu()
        {
            loginMenu.SetActive(true);
            mainMenu.SetActive(false);
            registerMenu.SetActive(false);
            subMenu.SetActive(false);
        }
    }
}