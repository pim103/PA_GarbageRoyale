using System.Collections;
using System.Collections.Generic;
using GarbageRoyale.Scripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InventoryController : MonoBehaviourPunCallbacks
{
    public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
    private Inventory playerInventory;
    [SerializeField]
    private GameObject playerPrefab;
    private GameObject gtest;
    public int itemInHand;

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = playerPrefab.AddComponent<Inventory>();
        playerInventory.initInventory();
        itemInHand = 0;

        //Debug.Log(string.Format("Inventory : {0}", playerInventory.getItemInventory()[0]));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            itemInHand = playerInventory.getItemInventory()[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            itemInHand = playerInventory.getItemInventory()[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            itemInHand = playerInventory.getItemInventory()[2];
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            itemInHand = playerInventory.getItemInventory()[3];
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            itemInHand = playerInventory.getItemInventory()[4];
        }
    }

    // Update is called once per frame
    /*public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        characterList[newPlayer.ActorNumber].AddComponent<Inventory>();
        characterList[newPlayer.ActorNumber].GetComponent<Inventory>().initInventory();
    }*/
        
    
}