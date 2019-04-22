using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class Gaz : MonoBehaviour
    {
        private bool isExplode;
        EnvironmentEvent events;
        AudioClip explosionSound;

        public GameObject audioSource;

        // Start is called before the first frame update
        void Start()
        {
            events = GameObject.Find("Controller").GetComponent<EnvironmentEvent>();
            isExplode = false;
            explosionSound = GameObject.Find("Controller").GetComponent<SoundManager>().getExplosionSound();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(!isExplode && events.gazX == (int)transform.position.x && events.gazY == (int)transform.position.y && events.gazZ == (int)transform.position.z)
            {
                events.resetGazExplode();
                explosion();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!isExplode && other.name == "torch(Clone)" && other.transform.GetChild(0).gameObject.activeSelf == true)
            {
                events.triggerExplosion((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
            }
        }

        private void explosion()
        {
            gameObject.SetActive(false);

            GameObject explo;
            explo = ObjectPooler.SharedInstance.GetPooledObject(6);
            explo.SetActive(true);
            explo.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            explo.GetComponent<Explosion>().audioSource = audioSource;

            isExplode = true;

            audioSource.GetComponent<AudioSource>().Stop();
            audioSource.GetComponent<AudioSource>().PlayOneShot(explosionSound);
        }
    }
}