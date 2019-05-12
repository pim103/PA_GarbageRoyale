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
        public GameObject subMenu;

        [SerializeField]
        public Camera mainCamera;

        [SerializeField]
        public GameObject gameController;

        // Start is called before the first frame update
        void Start()
        {
            launchMainMenu();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void launchMainMenu()
        {
            mainMenu.SetActive(true);
            subMenu.SetActive(false);
        }

        public void launchSubMenu()
        {
            mainMenu.SetActive(false);
            subMenu.SetActive(true);
        }
    }
}