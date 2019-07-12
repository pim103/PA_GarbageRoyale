using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Environment
{
    public class EnvironmentController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameController gc;

        public Dictionary<int, GameObject> iceWalls;

        private void Start()
        {
            iceWalls = new Dictionary<int, GameObject>();
        }

        [PunRPC]
        public void DesactiveIceWall(int idWall)
        {
            iceWalls[idWall].SetActive(false);
        }
    }
}
