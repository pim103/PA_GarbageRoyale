using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class OilAreaScript : MonoBehaviour
    {

        [SerializeField]
        private GameObject oil;

        [SerializeField]
        private GameObject flame;

        [SerializeField]
        private BoxCollider bx;

        private float timeToBurn;
        bool isBurning;
        private GameController gc;

        private bool[] isInZone;
        private IEnumerator[] coroutine;

        private void Start()
        {
            isInZone = Enumerable.Repeat(false, 10).ToArray();
            isBurning = false;
            timeToBurn = 50.0f;
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            coroutine = new IEnumerator[10];
        }

        private void Update()
        {
            if (isBurning)
            {
                if (timeToBurn > 0.0f)
                {
                    timeToBurn -= Time.deltaTime;
                }
                else
                {
                    bx.enabled = false;
                    flame.SetActive(false);
                    oil.SetActive(false);
                }
                /*
                for(var i = 0; i < isInZone.Length; i++)
                {
                    if(isInZone[i])
                    {
                        gc.players[i].PlayerStats.takeDamage(0.1f);
                    }
                }
                */
            }

            if(gc.water.waterObject.transform.position.y > transform.position.y)
            {
                bx.enabled = false;
                flame.SetActive(false);
                oil.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isBurning && (other.name.StartsWith("torch") || other.name.StartsWith("toiletPaper")) && other.transform.GetChild(0).gameObject.activeSelf)
            {
                bx.enabled = false;
                flame.SetActive(true);
                isBurning = true;
                bx.size += Vector3.forward;
                bx.enabled = true;
            }
            else if (isBurning && other.name.StartsWith("Player"))
            {
                int id = other.GetComponent<ExposerPlayer>().PlayerIndex;
                isInZone[id] = true;

                coroutine[id] = DealDamage(id);
                StartCoroutine(coroutine[id]);

                if (gc.playersActions[id].isOiled)
                {
                    gc.playersActions[id].isOiled = false;
                    gc.playersActions[id].isBurning = true;
                    gc.playersActions[id].timeLeftBurn = 5.0f;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (isBurning && other.name.StartsWith("Player"))
            {
                int id = other.GetComponent<ExposerPlayer>().PlayerIndex;
                isInZone[id] = false;
                StopCoroutine(coroutine[id]);
            }
        }

        private IEnumerator DealDamage(int id)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                yield return null;
            }

            while (true)
            {
                gc.players[id].PlayerStats.takeDamage(1f);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}