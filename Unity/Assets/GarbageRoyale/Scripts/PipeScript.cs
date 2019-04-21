using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class PipeScript : MonoBehaviour
    {
        private CameraRaycastHitActions ray;
        private AudioClip gazSound;
        bool isBroken;

        // Start is called before the first frame update
        void Start()
        {
            ray = GameObject.Find("Controller").GetComponent<CameraRaycastHitActions>();
            gazSound = GameObject.Find("Controller").GetComponent<SoundManager>().getGazSound();
            isBroken = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!isBroken && ray.xTrap == (int)transform.position.x && ray.yTrap == (int)transform.position.y && ray.zTrap == (int)transform.position.z)
            {
                GameObject BrokenPipe;
                GameObject Particle;
                isBroken = true;

                gameObject.SetActive(false);

                BrokenPipe = ObjectPooler.SharedInstance.GetPooledObject(1);
                BrokenPipe.SetActive(true);
                BrokenPipe.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                BrokenPipe.transform.localEulerAngles = transform.localEulerAngles;

                Particle = ObjectPooler.SharedInstance.GetPooledObject(3);
                Particle.SetActive(true);
                Particle.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

                GameObject crateSound;
                crateSound = ObjectPooler.SharedInstance.GetPooledObject(2);
                crateSound.SetActive(true);
                crateSound.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                AudioSource audioSource = crateSound.GetComponent<AudioSource>();
                audioSource.clip = gazSound;
                audioSource.loop = true;
                audioSource.Play();

                Particle.transform.GetChild(0).gameObject.GetComponent<Gaz>().audioSource = crateSound;
            }
        }
    }
}