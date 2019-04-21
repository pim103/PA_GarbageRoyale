using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class EnvironmentEvent : MonoBehaviourPunCallbacks
    {
        public int gazX;
        public int gazY;
        public int gazZ;

        // Start is called before the first frame update
        void Start()
        {
            gazX = -1;
            gazY = -1;
            gazZ = -1;
        }

        public void triggerExplosion(int x, int y, int z)
        {
            photonView.RPC("verifyTriggerExplosion", RpcTarget.MasterClient, x, y, z);
        }

        [PunRPC]
        private void verifyTriggerExplosion(int x, int y, int z, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            photonView.RPC("explode", RpcTarget.All, x, y, z);
        }

        [PunRPC]
        private void explode(int x, int y, int z, PhotonMessageInfo info)
        {
            gazX = x;
            gazY = y;
            gazZ = z;
        }

        public void resetGazExplode()
        {
            gazX = -1;
            gazY = -1;
            gazZ = -1;
        }
    }
}