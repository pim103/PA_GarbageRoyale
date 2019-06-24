using Photon.Pun;
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

        public void SwitchCam(int id)
        {
            for (var i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                if (i != id && !gc.players[i].PlayerStats.isDead)
                {
                    gc.players[id].PlayerCamera.enabled = false;
                    gc.players[i].PlayerCamera.enabled = true;
                }
            }
        }
    }
}