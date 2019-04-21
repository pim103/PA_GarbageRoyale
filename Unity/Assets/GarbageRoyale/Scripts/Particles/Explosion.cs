using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts
{

    public class Explosion : MonoBehaviour
    {
        BoxCollider bx;
        private float explosionTimer = 1;
        public GameObject audioSource;

        // Start is called before the first frame update
        void Start()
        {
            bx = GetComponent<BoxCollider>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(explosionTimer > 0)
            {
                explosionTimer -= Time.deltaTime;
            }
            else
            {
                bx.gameObject.SetActive(false);
            }

            if(!audioSource.GetComponent<AudioSource>().isPlaying)
            {
                audioSource.SetActive(false);
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "otherPerso(Clone)")
            {
                PlayerStats ps = other.GetComponent<PlayerStats>();
                ps.takeDamage(30.0f);
            }
        }
    }
}