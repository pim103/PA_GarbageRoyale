using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class EndGameMenu : MonoBehaviour
    {
        [SerializeField]
        private Text EndMessage;

        [SerializeField]
        private Button BackToMenuButton;
        [SerializeField]
        private GameObject dialogWindow;
        [SerializeField]
        private Text dialogText;
        [SerializeField]
        private GameObject dialogButton;
        [SerializeField]
        private Button dialogButtonBtn;

        

        [SerializeField]
        private StartGame controller;
        [SerializeField]
        private GameController gc;

        public enum StateEndGame
        {
            None,
            All_Dead,
            One_Alive,
            End_With_Many_Alive,
            Won
        }

        private void Start()
        {
            UpdateScores();
            dialogWindow.SetActive(true);
            dialogText.text = controller.lc.GetLocalizedValue("dialog_end_send_score");
            dialogButton.SetActive(false);
            gc.endSendingScores = false;
            BackToMenuButton.interactable = false;
            BackToMenuButton.onClick.AddListener(BackToMenu);
        }

        private void UpdateScores()
        {
            for (int i = 0; i < gc.AvatarToUserId.Length; i++)
            {
                if (gc.AvatarToUserId[i] != "")
                {
                    StartCoroutine(SendScores.SendScore(gc.AvatarToUserId[i], gc.playersScores[i]));
                }
            }
            gc.photonView.RPC("TellEndUpdate", RpcTarget.All);
        }

        private void Update()
        {
            if (gc.endSendingScores)
            {
                BackToMenuButton.interactable = true;
                dialogWindow.SetActive(false);
            }
        }

        private void BackToMenu()
        {
            PhotonNetwork.Disconnect();
            SaveState.SaveStateGame(false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void SetEndMessage(StateEndGame sg, int idPlayer)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            switch (sg)
            {
                case StateEndGame.All_Dead:
                    EndMessage.text = controller.lc.GetLocalizedValue("end_all_dead");
                    break;
                case StateEndGame.One_Alive:
                    EndMessage.text = controller.lc.GetLocalizedValue("end_one_alive").Replace("${player}", idPlayer.ToString());
                    break;
                case StateEndGame.Won:
                    EndMessage.text = controller.lc.GetLocalizedValue("end_won");
                    break;
            }
        }
    }
}