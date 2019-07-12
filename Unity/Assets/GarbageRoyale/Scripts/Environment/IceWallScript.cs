using GarbageRoyale.Scripts.PrefabPlayer;
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

        private float cdTorch;

        private void Start()
        {
            ec = GameObject.Find("Controller").GetComponent<EnvironmentController>();
            cdTorch = 0.1f;
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
                //durability -= 1.0f;
                yield return new WaitForSeconds(0.1f);
            }

            durability = 100;

            ec.photonView.RPC("DesactiveIceWall", RpcTarget.All, id);
        }

        public void HitWall(float damage)
        {
            durability -= damage;
        }

        public void OnTriggerStay(Collider other)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            bool isPlayer = false;
            bool isTorch = false;

            bool torchProximity = false;

            if (other.name.StartsWith("Player"))
            {
                isPlayer = true;
            }
            else if (other.name.StartsWith("torch") || other.name.StartsWith("toiletPaper"))
            {
                isTorch = true;
            }
            else
            {
                return;
            }

            if (isPlayer)
            {
                if (other.GetComponent<ExposerPlayer>().PlayerTorch.transform.GetChild(0).gameObject.activeSelf && other.GetComponent<ExposerPlayer>().PlayerTorch.activeSelf)
                {
                    torchProximity = true;
                }
            }
            else if (isTorch)
            {
                if (other.transform.GetChild(0).gameObject.activeSelf)
                {
                    torchProximity = true;
                }
            }

            if(torchProximity)
            {
                cdTorch -= Time.deltaTime;

                if(cdTorch <= 0)
                {
                    cdTorch = 0.1f;
                    durability -= 0.1f;
                }
            }
        }
    }
}
