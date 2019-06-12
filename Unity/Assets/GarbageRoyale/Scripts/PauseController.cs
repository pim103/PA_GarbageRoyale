using System;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField]
        public GameObject pauseGUI;

        public bool isInEscapeMenu = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                
                if (!isInEscapeMenu)
                {
                    pauseGUI.SetActive(true);
                    isInEscapeMenu = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    pauseGUI.SetActive(false);
                    isInEscapeMenu = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                
            }
        }
    }
}