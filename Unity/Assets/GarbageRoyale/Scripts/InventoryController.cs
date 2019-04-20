using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InventoryController : MonoBehaviourPunCallbacks
{
    public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
    private Inventory playerInventory;
    [SerializeField]
    private GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = playerPrefab.AddComponent<Inventory>();
        playerInventory.initInventory();
        //Debug.Log(string.Format("Inventory : {0}", playerInventory.getItemInventory()[0]));
    }

    // Update is called once per frame
    /*public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        characterList[newPlayer.ActorNumber].AddComponent<Inventory>();
        characterList[newPlayer.ActorNumber].GetComponent<Inventory>().initInventory();
    }*/
        
    
}