using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class PlayerListMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Button readyButton;
        [SerializeField]
        private Button leftLobby;
        [SerializeField]
        private Button backToMainMenu;
        [SerializeField] 
        private Text readyText;
        [SerializeField]
        private PlayerList players;
        [SerializeField]
        private Transform contentPlayer;
        [SerializeField] 
        private GameController gc;
        [SerializeField] 
        private StartGame menuController;
        [SerializeField]
        private GameObject dialogWindow;
        [SerializeField]
        private Text dialogText;
        [SerializeField]
        private GameObject dialogButton;
        [SerializeField]
        private Button dialogButtonBtn;
        
        public List<PlayerList> listPlayers = new List<PlayerList>();

        private string[] playersNickName;

        private void Start()
        {
            dialogButton.SetActive(false);
            dialogWindow.SetActive(true);
            dialogText.text = menuController.lc.GetLocalizedValue("dialog_loading_players");
        }

        private void Awake()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                readyText.text = menuController.lc.GetLocalizedValue("ready_btn");
            }
            else
            {
                readyText.text = menuController.lc.GetLocalizedValue("launch_btn");
            }
            playersNickName = Enumerable.Repeat("", 20).ToArray();
            StartCoroutine(GetPlayers());

            readyButton.onClick.AddListener(AskToBeReady);
            leftLobby.onClick.AddListener(AskToLeftLobby);
            backToMainMenu.onClick.AddListener(AskToGoToMainMenu);
            dialogButtonBtn.onClick.AddListener(dialogBoxEnable);
        }
        
        public void AskToBeReady()
        {
            if (PhotonNetwork.IsMasterClient)
            {   
                for (int i = 0; i < gc.AvatarToUserId.Length; i++)
                {
                    if (gc.AvatarToUserId[i] != "" && gc.players[i].PlayerStats.isReadyToPlay ||
                        gc.AvatarToUserId[i] == PhotonNetwork.AuthValues.UserId || gc.AvatarToUserId[i] == "")
                    {
                        continue;
                    }
                    else
                    {
                        dialogWindow.SetActive(true);
                        dialogButton.SetActive(true);
                        dialogText.text = menuController.lc.GetLocalizedValue("dialog_players_not_ready");
                        return;
                    }
                        
                }
                gc.MasterActivateAvatarPlayer();
                return;
            }

            photonView.RPC("SetReadyPlayer", RpcTarget.MasterClient, PhotonNetwork.AuthValues.UserId);
        }

        private void dialogBoxEnable()
        {
            dialogWindow.SetActive(false);
        }
        
        private void AskToLeftLobby()
        {
            PhotonNetwork.Disconnect();
            SaveState.SaveStateGame(true);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            int index = Array.IndexOf(playersNickName, otherPlayer.NickName);
            if (index != -1)
            {
                gc.photonView.RPC("ResetPlayer", RpcTarget.All, index);
                photonView.RPC("TellPlayerLeft", RpcTarget.All, index);
            }
        }

        [PunRPC]
        public void TellPlayerLeft(int index)
        {
            playersNickName[index] = "";
            gc.AvatarToUserId[index] = "";
            listPlayers[index].gameObject.SetActive(false);
        }

        private void AskToGoToMainMenu()
        {
            PhotonNetwork.Disconnect();
            SaveState.SaveStateGame(false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator GetPlayers()
        {
            bool isGameStart = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsStart"];

            while (!gc.mineQ)
            {
                yield return new WaitForSeconds(1.0f);
            }


            if (isGameStart)
            {
                gc.JoinAsSpectator();
            }
            else
            {
                photonView.RPC("GetPlayersRPC", RpcTarget.MasterClient, PhotonNetwork.AuthValues.UserId, PhotonNetwork.LocalPlayer.NickName);
            }
        }
        
        [PunRPC]
        private void GetPlayersRPC(string userId, string name)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            playersNickName[Array.IndexOf(gc.AvatarToUserId, userId)] = name;
            bool[] isReady = Enumerable.Repeat(false, 20).ToArray();

            for(var i = 0; i < gc.AvatarToUserId.Length; i++)
            {
                if(gc.AvatarToUserId[i] != "")
                {
                    isReady[i] = gc.players[i].PlayerStats.IsReadyToPlay;
                }
            }

            photonView.RPC("ActivatePlayerRPC", RpcTarget.All, playersNickName, isReady);
        }
        
        [PunRPC]
        private void ActivatePlayerRPC(string[] players, bool[] isReady)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != "")
                {
                    listPlayers[i].gameObject.SetActive(true);
                    listPlayers[i].SetPlayerInfo(players[i]);

                    if (isReady[i])
                    {
                        listPlayers[i].gameObject.GetComponent<RawImage>().color = Color.green;
                    }
                    else
                    {
                        listPlayers[i].gameObject.GetComponent<RawImage>().color = Color.white;
                    }
                }
            }
            dialogWindow.SetActive(false);
        }

        [PunRPC]
        private void SetReadyPlayer(string userIdRequester)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            int index = Array.IndexOf(gc.AvatarToUserId, userIdRequester);
            photonView.RPC("BeReadyRPC", RpcTarget.All, index, !gc.players[index].PlayerStats.IsReadyToPlay);
        }

        [PunRPC]
        private void BeReadyRPC(int index, bool toggle)
        {
            gc.SetReady(index, toggle);
            if (toggle)
            {
                listPlayers[index].gameObject.GetComponent<RawImage>().color = Color.green;
            }
            else
            {
                listPlayers[index].gameObject.GetComponent<RawImage>().color = Color.white;
            }
        }
    }
}