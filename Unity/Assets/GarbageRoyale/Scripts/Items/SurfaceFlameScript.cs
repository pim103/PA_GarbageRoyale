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
            isInZone = Enumerable.Repeat(false, 10).ToArray();
            timeLeftBurning = 10.0f;
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            coroutine = new IEnumerator[10];
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
            /*
            for (var i = 0; i < isInZone.Length; i++)
            {
                if (isInZone[i])
                {
                    gc.players[i].PlayerStats.takeDamage(0.05f);
                }
            }
            */
            if (gc.water.waterObject.transform.position.y > transform.position.y)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name.StartsWith("Player"))
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
            if (other.name.StartsWith("Player"))
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
                gc.playersActions[id].isSlow = true;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}