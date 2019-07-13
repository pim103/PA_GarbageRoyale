using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class WolfTrapScript : MonoBehaviour
    {
        [SerializeField]
        public GameObject leftPanel;

        [SerializeField]
        public GameObject rightPanel;

        [SerializeField]
        private GameObject rope;

        [SerializeField]
        private GameObject centerRotation;

        [SerializeField]
        private Rigidbody rigid;

        private GameController gc;
        private ItemController ic;

        private bool isTrigger;

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            ic = GameObject.Find("Controller").GetComponent<ItemController>();
            isTrigger = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (!isTrigger && other.name.StartsWith("Player"))
            {
                int idPlayer = other.GetComponent<ExposerPlayer>().PlayerIndex;
                isTrigger = true;

                ic.ActiveWolfTrap(transform.GetComponent<Item>().getId(), isTrigger);
                StartCoroutine(trapPlayer(idPlayer));
            } else if (!isTrigger && other.gameObject.CompareTag("Rat"))
            {
                isTrigger = true;

                ic.ActiveWolfTrap(transform.GetComponent<Item>().getId(), isTrigger);
                other.GetComponent<MobStats>().takeDamageFromEnv(20.0f);
            }
        }

        private IEnumerator trapPlayer(int id)
        {
            gc.playersActions[id].isTrap = true;
            gc.players[id].PlayerStats.takeDamage(10.0f);

            yield return new WaitForSeconds(3.0f);
            isTrigger = false;
            gc.playersActions[id].isTrap = false;

            ic.ActiveWolfTrap(transform.GetComponent<Item>().getId(), isTrigger);
        }
    }
}