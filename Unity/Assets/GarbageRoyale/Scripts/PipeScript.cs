using GarbageRoyale.Scripts.PrefabPlayer;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class PipeScript : MonoBehaviour
    {
        private CameraRaycastHitActions ray;

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

        // Start is called before the first frame update
        void Start()
        {
            ray = GameObject.Find("Controller").GetComponent<CameraRaycastHitActions>();
            isBroken = false;
            isExplode = false;
            canTakeDamage = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!isBroken && ray.xTrap == (int)transform.position.x && ray.yTrap == (int)transform.position.y && ray.zTrap == (int)transform.position.z)
            {
                pipe.SetActive(false);
                brokenPipe.SetActive(true);
                isBroken = true;
            } else if(isExplode)
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

        private void OnTriggerEnter(Collider other)
        {
            if (isExplode && canTakeDamage && other.name.StartsWith("Player"))
            {
                Debug.Log(other.GetComponent<ExposerPlayer>().PlayerIndex);
                Debug.Log("damage");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (isBroken && !isExplode && other.name.StartsWith("Player"))
            {
                explosion();
                //events.triggerExplosion((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
            } else if(isExplode && canTakeDamage && other.name.StartsWith("Player"))
            {
                Debug.Log(other.GetComponent<ExposerPlayer>().PlayerIndex);
                Debug.Log("damage");
            }
        }

        private void explosion()
        {
            brokenPipe.transform.GetChild(0).gameObject.SetActive(false);
            Explosion.SetActive(true);
            isExplode = true;
            canTakeDamage = true;
            /*
            gameObject.SetActive(false);

            GameObject explo;
            explo = ObjectPooler.SharedInstance.GetPooledObject(6);
            explo.SetActive(true);
            explo.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            explo.GetComponent<Explosion>().audioSource = audioSource;

            isExplode = true;

            audioSource.GetComponent<AudioSource>().Stop();
            audioSource.GetComponent<AudioSource>().PlayOneShot(explosionSound);
            */
        }
    }
}