using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Menu
{
    public class StartGame : MonoBehaviour
    {
        [SerializeField]
        private Canvas mainMenu;
        [SerializeField]
        private Canvas subMenu;

        private Canvas main;
        private Canvas sub;

        // Start is called before the first frame update
        void Start()
        {
            main = Instantiate(mainMenu);
            main.enabled = false;
            sub = Instantiate(subMenu);
            sub.enabled = false;

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
            main.enabled = true;
            sub.enabled = false;
        }

        public void launchSubMenu()
        {
            main.enabled = false;
            sub.enabled = true;
        }
    }
}