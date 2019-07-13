using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{

    public class NailAreaScript : MonoBehaviour
    {
        public bool isEmpty;
        private IEnumerator[] coroutine;

        private GameController gc;

        private void Start()
        {
            isEmpty = false;
            coroutine = new IEnumerator[20];

            gc = GameObject.Find("Controller").GetComponent<GameController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (other.name.StartsWith("Player"))
            {
                int id = other.GetComponent<ExposerPlayer>().PlayerIndex;
                coroutine[id] = DealDamage(id);

                StartCoroutine(coroutine[id]);
            } else if (other.gameObject.CompareTag("Rat"))
            {
                other.GetComponent<MobStats>().takeDamageFromEnv(30.0f);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (other.name.StartsWith("Player"))
            {
                int id = other.GetComponent<ExposerPlayer>().PlayerIndex;
                StopCoroutine(coroutine[id]);
                gc.playersActions[id].isSlow = false;
            }
        }

        private IEnumerator DealDamage(int id)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                yield return null;
            }

            while(true)
            {
                gc.players[id].PlayerStats.takeDamage(0.5f);
                gc.playersActions[id].isSlow = true;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}