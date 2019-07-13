using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.PlayerController
{
    public class SpectateMode : MonoBehaviour
    {
        [SerializeField]
        private GameController gc;

        [HideInInspector]
        public int idCamSpectate;

        [SerializeField]
        private GameObject canvasDead;

        bool wantToSeeNextPlayer;
        bool wantToSeePreviousPlayer;

        public enum Order
        {
            None,
            Previous,
            Next,
        }

        private void Start()
        {
            idCamSpectate = -1;
            wantToSeeNextPlayer = false;
            wantToSeePreviousPlayer = false;
        }

        private void Update()
        {
            if(!gc.isGameStart)
            {
                return;
            }

            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                wantToSeePreviousPlayer = true;
            }
            else if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                wantToSeeNextPlayer = true;
            }
        }

        private void FixedUpdate()
        {
            if(wantToSeeNextPlayer)
            {
                SwitchCam(Order.Next);
                wantToSeeNextPlayer = false;
            }
            if(wantToSeePreviousPlayer)
            {
                SwitchCam(Order.Previous);
                wantToSeePreviousPlayer = false;
            }
        }

        public void SwitchCam(Order order)
        {
            int idPlayer = Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId);

            if (!gc.players[idPlayer].PlayerStats.isDead || !gc.isSpectator)
            {
                return;
            }

            int startLoop = 0;
            int inc = 1;

            switch(order)
            {
                case Order.None:
                    startLoop = 0;
                    inc = 1;
                    break;
                case Order.Previous:
                    startLoop = idPlayer;
                    inc = -1;
                    break;
                case Order.Next:
                    startLoop = idPlayer;
                    inc = 1;
                    break;
            }

            int idOtherPlayer;
            for (var i = startLoop; i - startLoop < gc.AvatarToUserId.Length && i - startLoop > -gc.AvatarToUserId.Length; i += inc)
            {
                if (i >= gc.AvatarToUserId.Length)
                {
                    idOtherPlayer = i - gc.AvatarToUserId.Length;
                }
                else if (i < 0)
                {
                    idOtherPlayer = i + gc.AvatarToUserId.Length;
                }
                else
                {
                    idOtherPlayer = i;
                }

                if(gc.AvatarToUserId[idOtherPlayer] == "")
                {
                    continue;
                }

                if (idOtherPlayer != idPlayer && !gc.players[idOtherPlayer].PlayerStats.isDead)
                {
                    if(idCamSpectate == -1)
                    {
                        idCamSpectate = idPlayer;
                    }
                    gc.players[idCamSpectate].PlayerCamera.enabled = false;
                    gc.players[idOtherPlayer].PlayerCamera.enabled = true;
                    idCamSpectate = idOtherPlayer;
                }
            }
        }

        public void WantToSwitchCam()
        {
            StartCoroutine(WaitingTransition());
        }

        private IEnumerator WaitingTransition()
        {
            canvasDead.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            canvasDead.SetActive(false);

            SwitchCam(Order.None);
        }
    }
}