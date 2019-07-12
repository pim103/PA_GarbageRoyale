using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
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
        private PlayerList players;
        [SerializeField]
        private Transform contentPlayer;
        [SerializeField] 
        private GameController gc;
        [SerializeField] 
        private StartGame menuController;
        
        public List<PlayerList> listPlayers = new List<PlayerList>();

        private string[] playersNickName;

        

        private void Awake()
        {
            playersNickName = Enumerable.Repeat("", 20).ToArray();
            StartCoroutine(GetPlayers());

            readyButton.onClick.AddListener(AskToBeReady);
            leftLobby.onClick.AddListener(AskToLeftLobby);
            backToMainMenu.onClick.AddListener(AskToGoToMainMenu);
        }
        
        public void AskToBeReady()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (PhotonNetwork.CurrentRoom.MaxPlayers != PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    return;
                }
                for (int i = 0; i < gc.AvatarToUserId.Length; i++)
                {
                    if (gc.AvatarToUserId[i] != "" && gc.players[i].PlayerStats.isReadyToPlay ||
                        gc.AvatarToUserId[i] == PhotonNetwork.AuthValues.UserId || gc.AvatarToUserId[i] == "")
                    {
                        continue;
                    }
                    else
                    {
                        return;
                    }
                        
                }
                gc.MasterActivateAvatarPlayer();
                return;
            }
            photonView.RPC("SetReadyPlayer", RpcTarget.MasterClient, PhotonNetwork.AuthValues.UserId);
        }
        
        private void AskToLeftLobby()
        {
            PhotonNetwork.LeaveRoom();
            menuController.launchServerList();
        }
        
        private void AskToGoToMainMenu()
        {
            PhotonNetwork.LeaveRoom();
            menuController.launchMainMenu();
        }

        private IEnumerator GetPlayers()
        {
            while (!gc.mineQ)
            {
                yield return new WaitForSeconds(1.0f);
            }
            photonView.RPC("GetPlayersRPC", RpcTarget.MasterClient, PhotonNetwork.AuthValues.UserId, PhotonNetwork.LocalPlayer.NickName);
        }
        
        [PunRPC]
        private void GetPlayersRPC(string userId, string name)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            playersNickName[Array.IndexOf(gc.AvatarToUserId, userId)] = name;
            photonView.RPC("ActivatePlayerRPC", RpcTarget.All, playersNickName);
        }
        
        [PunRPC]
        private void ActivatePlayerRPC(string[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != "")
                {
                    Debug.Log("E");
                    listPlayers[i].gameObject.SetActive(true);
                    listPlayers[i].SetPlayerInfo(players[i]);
                }
            }
        }

        [PunRPC]
        private void SetReadyPlayer(string userIdRequester)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            int index = Array.IndexOf(gc.AvatarToUserId, userIdRequester);
            photonView.RPC("BeReadyRPC", RpcTarget.All, index);
        }

        [PunRPC]
        private void BeReadyRPC(int index)
        {
            gc.SetReady(index);
            if (gc.players[index].PlayerStats.IsReadyToPlay)
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