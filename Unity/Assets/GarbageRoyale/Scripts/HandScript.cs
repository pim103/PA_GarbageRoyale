using System.Collections;
using System.Collections.Generic;
using GarbageRoyale.Scripts;
using Photon.Pun;
using UnityEngine;

public class HandScript : MonoBehaviour
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
        currentItem = characterList[PhotonNetwork.LocalPlayer.ActorNumber].GetComponent<InventoryController>().itemInHand;
        switch (currentItem)
        {
            case 1:
                if (currentItem != oldItem)
                {
                    item = ObjectPooler.SharedInstance.GetPooledObject(0);
                    item.SetActive(true);
                    item.transform.SetParent(transform.GetChild(0).transform);
                    item.transform.localPosition = new Vector3(0,0,0);
                    item.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    oldItem = currentItem;
                }
                break;
            case 2:
                item = ObjectPooler.SharedInstance.GetPooledObject(3);
                item.SetActive(true);
                item.transform.position = new Vector3(155,0.7f,155);
                break;
            case 4:
                if (currentItem != oldItem)
                {
                    item = ObjectPooler.SharedInstance.GetPooledObject(5);
                    item.SetActive(true);
                    item.transform.SetParent(transform.GetChild(0).transform);
                    item.transform.localPosition = new Vector3(0,0,0);
                    item.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    oldItem = currentItem;
                }
                break;
            default:
                break;
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
