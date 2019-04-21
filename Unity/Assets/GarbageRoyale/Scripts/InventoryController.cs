using System.Collections;
using System.Collections.Generic;
using GarbageRoyale.Scripts;
using GarbageRoyale.Scripts.HUD;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviourPunCallbacks
{
    public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
    private Inventory playerInventory;
    [SerializeField]
    private GameObject playerPrefab;
    private GameObject gtest;
    public int itemInHand;
    private InventoryGUI playerInventoryGUI;
    [SerializeField] 
    private RawImage [] rawSprites;
    [SerializeField] 
    private GameObject inventoryPrefab;

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = playerPrefab.AddComponent<Inventory>();
        playerInventory.initInventory();
        playerPrefab.AddComponent<InventoryGUI>();
        playerInventoryGUI = playerPrefab.GetComponent<InventoryGUI>();
        playerInventoryGUI.initInventoryGUI(inventoryPrefab, rawSprites);
        itemInHand = 0;

        //Debug.Log(string.Format("Inventory : {0}", playerInventory.getItemInventory()[0]));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            itemInHand = playerInventory.getItemInventory()[0];
        }

        switch (itemInHand)
        {
           case 1:
               gtest = ObjectPooler.SharedInstance.GetPooledObject(0);
               gtest.SetActive(true);
               gtest.transform.position = new Vector3(155,0.7f,155);
               break;
            case 2:
                gtest = ObjectPooler.SharedInstance.GetPooledObject(4);
                gtest.SetActive(true);
                gtest.transform.position = new Vector3(155,0.7f,155);
                break;
           default:
               break;
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