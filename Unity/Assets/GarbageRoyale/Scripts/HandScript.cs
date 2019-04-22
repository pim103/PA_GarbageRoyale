using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class HandScript : MonoBehaviourPunCallbacks
    {
        public int currentItem;
        public int oldItem;

        private bool pressV;
        private GameObject item;
        private GameObject crateSound;
        public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();

        AudioClip torchLightSound;

        // Start is called before the first frame update
        void Start()
        {
            characterList = GameObject.Find("Controller").GetComponent<GameController>().characterList;
            pressV = false;
            torchLightSound = GameObject.Find("Controller").GetComponent<SoundManager>().getTorchLightSound();
            crateSound = null;
        }

        // Update is called once per frame
        void Update()
        {
            currentItem = GameObject.Find("Controller").GetComponent<GameController>().GetComponent<InventoryActionsController>().itemInHand;
            putInHand();
        }
    
        public void putInHand()
        {
            int poolID = 0;

            if (currentItem == 0 && oldItem !=0)
            {
                /*foreach (Transform child in transform.GetChild(0).transform)
            {
                child.gameObject.SetActive(false);
                child.parent = null;
                Debug.Log(child.gameObject.name);
            }*/
                int count = transform.GetChild(0).childCount;
                for(int i = 0; i < count; i++)
                {
                    Transform child = transform.GetChild(0).GetChild(i);
                    child.gameObject.SetActive(false);
                    child.parent = null;
                    Debug.Log(child.gameObject.name);
                
                }
                /*transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(0).parent = null;*/
                crateSound = null;
                oldItem = currentItem;
            }
            else if (currentItem != oldItem)
            {
                crateSound = null;
                switch (currentItem)
                {
                    case 1:
                        poolID = 0;
                        break;
                    case 4:
                        poolID = 5;
                        break;
                }
                if (oldItem != 0)
                {
                    item.transform.parent = null;
                    item.SetActive(false);
                }
                item = ObjectPooler.SharedInstance.GetPooledObject(poolID);
                item.SetActive(true);
                item.transform.SetParent(transform.GetChild(0).transform);
                item.transform.localPosition = new Vector3(0,0,0);
                item.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                oldItem = currentItem;
                GameObject.Find("Controller").GetComponent<GameController>().GetComponent<InventoryActionsController>().Send = true;
            }

            if(currentItem == 4 && crateSound == null)
            {
                crateSound = ObjectPooler.SharedInstance.GetPooledObject(2);
                crateSound.SetActive(true);
                crateSound.transform.SetParent(transform.GetChild(0).transform);
                crateSound.transform.localPosition = new Vector3(0, 0, 0);
                crateSound.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                AudioSource audio = crateSound.GetComponent<AudioSource>();
                audio.clip = torchLightSound;
                audio.loop = true;
            }
            if (currentItem == 4 && Input.GetKeyDown(KeyCode.V))
            {
                pressV = true;
            }

            if(currentItem != 4 && crateSound != null)
            {
                crateSound.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            if(pressV)
            {
                triggerTorch();
            }

            pressV = false;
        }

        private void triggerTorch()
        {
            //ParticleSystem flame = item.transform.GetChild(0).GetComponent<ParticleSystem>();
            GameObject flame = item.transform.GetChild(0).gameObject;
            AudioSource audio = crateSound.GetComponent<AudioSource>();
        
            if(flame.activeSelf)
            {
                flame.SetActive(false);
                Debug.Log("Eteignage de torche");
                audio.Stop();
            } else
            {
                flame.SetActive(true);
                if (!audio.isPlaying)
                {
                    Debug.Log("Allumage de torche");
                    audio.Play();
                }
            }
            pressV = false;
        }

    
    }
}
