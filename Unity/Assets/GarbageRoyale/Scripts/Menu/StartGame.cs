using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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
        public GameObject serverListMenu;
        [SerializeField] 
        public GameObject roomLobby;
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
            DataState loadedData = SaveState.LoadData();
            if (loadedData != null)
            {
                PhotonNetwork.AuthValues.UserId = loadedData.UserId;
                //Debug.Log(PhotonNetwork.AuthValues.UserId);
                PhotonNetwork.NickName = loadedData.NickName;
                
                StartCoroutine(LoadRole());
                
                if (loadedData.IsInMenu)
                {
                    PhotonNetwork.LeaveLobby();
                    launchServerList();
                }
                else if(loadedData.endGame)
                {
                    launchMainMenu();
                }
                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true; 
            }
            else
            {
                launchLoginMenu();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true; 
            }
        }

        IEnumerator LoadRole()
        {
            if (!mainMenu.GetComponent<MainMenu>().isConnected)
            {
                yield return new WaitForSeconds(1.0f);
            }
            DataState loadedData = SaveState.LoadData();
            if (loadedData != null)
            {
                PhotonNetwork.SetPlayerCustomProperties(new ExitGames.Client.Photon.Hashtable()
                {
                    {"role", loadedData.Role}
                });
            }
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
        
        public void launchServerList()
        {
            ResetAllMenu();
            PhotonNetwork.JoinLobby();
            serverListMenu.SetActive(true);
        }
        
        public void launchRoomLobby()
        {
            ResetAllMenu();
            roomLobby.SetActive(true);
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

        private void OnApplicationQuit()
        {
            SaveState.DeleteData();
        }

        private void ResetAllMenu()
        {
            loginMenu.SetActive(false);
            mainMenu.SetActive(false);
            serverListMenu.SetActive(false);
            roomLobby.SetActive(false);
            registerMenu.SetActive(false);
            subMenu.SetActive(false);
            EndGameMenu.SetActive(false);
        }
    }
}