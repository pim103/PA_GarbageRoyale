using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Environment
{
    public class IceWallScript : MonoBehaviour
    {
        public float durability;
        public int id;

        private EnvironmentController ec;

        private void Start()
        {
            ec = GameObject.Find("Controller").GetComponent<EnvironmentController>();
        }

        public void ActiveIceWall()
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            durability = 100;
            StartCoroutine(UpdateStats());
        }

        private IEnumerator UpdateStats()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                yield return null;
            }

            while (durability > 0)
            {
                durability -= 1.0f;
                yield return new WaitForSeconds(0.1f);
            }

            durability = 100;

            ec.photonView.RPC("DesactiveIceWall", RpcTarget.All, id);
        }

        public void HitWall(float damage)
        {
            durability -= damage;
        }
    }
}
