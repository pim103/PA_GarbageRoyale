using System.Collections;
using System.Collections.Generic;
using GarbageRoyale.Scripts;
using Photon.Pun;
using UnityEngine;

public class HandScript : MonoBehaviour
{
    public int currentItem;
    public int oldItem;

    private GameObject item;
    public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        characterList = GameObject.Find("Controller").GetComponent<GameController>().characterList;
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
    }
}
