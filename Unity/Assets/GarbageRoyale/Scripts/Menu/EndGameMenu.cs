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
        private StartGame controller;

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
            BackToMenuButton.onClick.AddListener(BackToMenu);
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
                    EndMessage.text = "Fin de la partie\nTout le monde est mort";
                    break;
                case StateEndGame.One_Alive:
                    EndMessage.text = "Fin de la partie\nLe joueur : " + idPlayer + " a réussi à s'échapper";
                    break;
                case StateEndGame.Won:
                    EndMessage.text = "Fin de la partie\nVous avez gagné !";
                    break;
            }
        }
    }
}