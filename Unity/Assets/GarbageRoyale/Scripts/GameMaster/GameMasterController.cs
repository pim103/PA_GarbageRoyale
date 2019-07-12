using System;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts.GameMaster
{
    public class GameMasterController : MonoBehaviour
    {
        [SerializeField]
        public GameObject gmGUI;

        public bool isInGMGUI = false;
        [SerializeField]
        private GameController gc;

        private void Update()
        {
            if (Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId) == -1)
            {
                return;
            }
            if (gc.players[Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].GetComponent<PlayerStats>().PlayerRole < 3)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Quote))
            {
                if (!isInGMGUI)
                {
                    gmGUI.SetActive(true);
                    isInGMGUI = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    gmGUI.SetActive(false);
                    isInGMGUI = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }

        }
    }
}