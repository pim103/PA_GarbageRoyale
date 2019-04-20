using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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
        if (PhotonNetwork.IsMasterClient)
        {
            characterList[PhotonNetwork.LocalPlayer.ActorNumber].AddComponent<Inventory>();
            characterList[PhotonNetwork.LocalPlayer.ActorNumber].GetComponent<Inventory>().initInventory();
        }
        //Debug.Log(string.Format("Inventory : {0}", playerInventory.getItemInventory()[0]));
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
