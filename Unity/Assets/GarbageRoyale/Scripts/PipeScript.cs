using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class PipeScript : MonoBehaviour
    {
        public int pipeIndex;

        bool isBroken;
        bool isExplode;
        bool canTakeDamage;

        private float explosionTimer = 1.5f;

        [SerializeField]
        private GameObject pipe;

        [SerializeField]
        private GameObject brokenPipe;

        [SerializeField]
        private GameObject Explosion;

        [SerializeField]
        private BoxCollider bx;

        // Start is called before the first frame update
        void Start()
        {
            isBroken = false;
            isExplode = false;
            canTakeDamage = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(isExplode)
            {
                if(explosionTimer > 0)
                {
                    explosionTimer -= Time.deltaTime;
                }
                else
                {
                    Explosion.SetActive(false);
                    canTakeDamage = false;
                }
            }
        }

        public void brokePipe()
        {
            if (!isBroken)
            {
                bx.enabled = true;
                pipe.SetActive(false);
                brokenPipe.SetActive(true);
                isBroken = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isExplode && other.name.StartsWith("Player"))
            {
                if(PhotonNetwork.IsMasterClient)
                {
                    GameController gc = GameObject.Find("Controller").GetComponent<GameController>();
                    int id = other.GetComponent<ExposerPlayer>().PlayerIndex;
                    gc.players[id].PlayerStats.takeDamage(30.0f);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            bool isPlayer = false;
            bool isTorch = false;

            if(other.name.StartsWith("Player"))
            {
                isPlayer = true;
            }
            else if(other.name.StartsWith("torch"))
            {
                isTorch = true;
            } else
            {
                return;
            }

            if(isBroken && !isExplode)
            {
                if(isPlayer)
                {

                    if (other.GetComponent<ExposerPlayer>().PlayerTorch.transform.GetChild(0).gameObject.activeSelf && other.GetComponent<ExposerPlayer>().PlayerTorch.activeSelf)
                    {
                        bx.enabled = false;
                        explosion();
                        StartCoroutine("DesactivateBx");
                    }
                }
                else if(isTorch)
                {
                    if(other.transform.GetChild(0).gameObject.activeSelf)
                    {
                        bx.enabled = false;
                        explosion();
                        StartCoroutine("DesactivateBx");
                    }
                }
            }
        }
        
        private void explosion()
        {
            bx.enabled = true;
            brokenPipe.transform.GetChild(0).gameObject.SetActive(false);
            Explosion.SetActive(true);
            isExplode = true;
            canTakeDamage = true;
        }

        private IEnumerator DesactivateBx()
        {
            yield return new WaitForSeconds(1.5f);

            bx.enabled = false;
        }
    }
}