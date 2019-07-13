using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class ElecTrapScript : MonoBehaviour
    {
        [SerializeField]
        private Item itemScript;

        private GameController gc;
        private ItemController ic;

        private bool isTrigger;
        private IEnumerator[] coroutine;

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            ic = GameObject.Find("Controller").GetComponent<ItemController>();
            isTrigger = false;
            coroutine = new IEnumerator[20];
        }

        private void Update()
        {
            if (gc.water.waterObject.transform.position.y > transform.position.y)
            {
                ic.TriggerElectricity(transform.position, itemScript.getId());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (!isTrigger && other.name.StartsWith("Player"))
            {
                int idPlayer = other.GetComponent<ExposerPlayer>().PlayerIndex;
                isTrigger = true;

                coroutine[idPlayer] = trapPlayer(idPlayer);
                StartCoroutine(coroutine[idPlayer]);
            } else if (other.gameObject.CompareTag("Rat"))
            {
                other.GetComponent<MobStats>().takeDamageFromEnv(20.0f);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (!isTrigger && other.name.StartsWith("Player"))
            {
                int idPlayer = other.GetComponent<ExposerPlayer>().PlayerIndex;
                isTrigger = false;

                StopCoroutine(coroutine[idPlayer]);
            }
        }

        private IEnumerator trapPlayer(int id)
        {
            while(true)
            {
                gc.playersActions[id].isTrap = true;
                isTrigger = true;
                gc.players[id].PlayerStats.takeDamage(10.0f);

                yield return new WaitForSeconds(1.0f);

                isTrigger = false;
                gc.playersActions[id].isTrap = false;

                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}