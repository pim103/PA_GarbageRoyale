using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class SurfaceFlameScript : MonoBehaviour
    {
        private float timeLeftBurning;

        private GameController gc;

        private bool[] isInZone;
        private IEnumerator[] coroutine;

        // Start is called before the first frame update
        void Awake()
        {
            timeLeftBurning = 10.0f;
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            coroutine = new IEnumerator[20];
        }

        private void Update()
        {
            if(timeLeftBurning > 0)
            {
                timeLeftBurning -= Time.deltaTime;
            }
            else
            {
                gameObject.SetActive(false);
            }

            if (gc.water.waterObject.transform.position.y > transform.position.y)
            {
                gameObject.SetActive(false);
            }
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

                if (gc.playersActions[id].isOiled)
                {
                    gc.playersActions[id].isOiled = false;
                    gc.playersActions[id].isBurning = true;
                    gc.playersActions[id].timeLeftBurn = 5.0f;
                }
            } else if (other.gameObject.CompareTag("Rat"))
            {
                other.GetComponent<MobStats>().takeDamageFromEnv(25.0f);
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
                gc.players[id].PlayerStats.takeDamage(5f);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}