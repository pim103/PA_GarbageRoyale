using System.Collections;
using System.Collections.Generic;
using GarbageRoyale.Scripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InventoryActionsController : MonoBehaviourPunCallbacks
{
    public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
    private Inventory playerInventory;
    private GameObject gtest;
    public int itemInHand;

    public bool Send;
    // Start is called before the first frame update
    void Start()
    {
        characterList = GameObject.Find("Controller").GetComponent<GameController>().characterList;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //itemInHand = playerInventory.getItemInventory()[0];
            photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 5);
        }

        if (Send)
        {
            photonView.RPC("ChangeItemInHandOthers", RpcTarget.MasterClient, itemInHand);
            Send = false;
        }
        
    }
    
    [PunRPC]
    public void AskChangePlayerHandItem(int inventoryPlace,PhotonMessageInfo info)
    {
        Debug.Log("testask");
        
        Inventory inventoryData = characterList[info.Sender.ActorNumber].GetComponent<Inventory>();
        photonView.RPC("AnswerChangePlayerHandItem", info.Sender, inventoryData.getItemInventory()[0]);
    }
    
    [PunRPC]
    public void AnswerChangePlayerHandItem(int item,PhotonMessageInfo info)
    {
        itemInHand = item;
    }
    [PunRPC]
    public void ChangeItemInHandOthers(int item,PhotonMessageInfo info)
    {
        Debug.Log("ui "+item);
        int poolID = 0;
        GameObject putitem;
        switch (item)
        {
            case 1:
                poolID = 0;
                break;
            case 4:
                poolID = 1;
                break;
        }
        
        putitem = ObjectPoolerPhoton.SharedInstance.GetPooledObject(poolID);
        putitem.SetActive(true);
        putitem.transform.SetParent(characterList[info.Sender.ActorNumber].transform.GetChild(8).transform);
        putitem.transform.localPosition = new Vector3(0,0,0);
    }
    // Update is called once per frame
    /*public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        characterList[newPlayer.ActorNumber].AddComponent<Inventory>();
        characterList[newPlayer.ActorNumber].GetComponent<Inventory>().initInventory();
    }*/
        
    
}