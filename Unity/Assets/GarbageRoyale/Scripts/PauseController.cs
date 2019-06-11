using System;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField]
        public GameObject pauseGUI;

        private bool isInEscapeMenu = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                
                if (!isInEscapeMenu)
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    pauseGUI.SetActive(true);
                    isInEscapeMenu = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    transform.localScale = new Vector3(0f, 0f, 0f);
                    pauseGUI.SetActive(false);
                    isInEscapeMenu = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                
            }
            
            if (isInEscapeMenu)
            {
                return;
            }
        }
    }
}