using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class InventoryActionsController : MonoBehaviourPunCallbacks
    {
        public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
        private Inventory playerInventory;
        private GameObject gtest;
        public int itemInHand;
        public int placeInHand;
        public List<GameObject> thrownItems;
        public List<int> thrownItemsCount;

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
                placeInHand = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 1);
                placeInHand = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 2);
                placeInHand = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 3);
                placeInHand = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 4);
                placeInHand = 4;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if(itemInHand!=0) photonView.RPC("AskDropItem", RpcTarget.MasterClient, placeInHand);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if(itemInHand!=0) photonView.RPC("AskThrowItem", RpcTarget.MasterClient, placeInHand);
            }

            if (Send)
            {
                photonView.RPC("ChangeItemInHandOthers", RpcTarget.MasterClient, itemInHand);
                Send = false;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var item in thrownItems)
                {
                    if(thrownItemsCount[thrownItems.IndexOf(item)]<30 && thrownItemsCount[thrownItems.IndexOf(item)] >= 0){
                        item.transform.Translate(new Vector3(0,0,4)*Time.deltaTime);
                        item.transform.GetChild(0).Rotate(new Vector3(360,0,0)*Time.deltaTime,Space.Self);
                        thrownItemsCount[thrownItems.IndexOf(item)]+=1;
                    }
                    else if(thrownItemsCount[thrownItems.IndexOf(item)]>0)
                    {
                        item.transform.Translate(0,-1,0);
                        item.transform.GetChild(0).rotation = Quaternion.identity;
                        thrownItemsCount[thrownItems.IndexOf(item)] = -1;
                    }
                }
            }
        }
    
        [PunRPC]
        public void AskChangePlayerHandItem(int inventoryPlace,PhotonMessageInfo info)
        {
            Inventory inventoryData = characterList[info.Sender.ActorNumber].GetComponent<Inventory>();
            photonView.RPC("AnswerChangePlayerHandItem", info.Sender, inventoryData.getItemInventory()[inventoryPlace]);
        }
    
        [PunRPC]
        public void AnswerChangePlayerHandItem(int item,PhotonMessageInfo info)
        {
            itemInHand = item;
        }
        [PunRPC]
        public void ChangeItemInHandOthers(int item,PhotonMessageInfo info)
        {
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
            putitem.transform.localEulerAngles = new Vector3(0,0,0);
        }
    
        [PunRPC]
        public void AskDropItem(int inventoryPlace,PhotonMessageInfo info)
        {
            Inventory inventoryData = characterList[info.Sender.ActorNumber].GetComponent<Inventory>();
            characterList[info.Sender.ActorNumber].transform.GetChild(8).GetChild(0).parent = null;
            inventoryData.getItemInventory()[inventoryPlace] = 0;
            photonView.RPC("AnswerDropItem", info.Sender);
        }
    
        [PunRPC]
        public void AskThrowItem(int inventoryPlace,PhotonMessageInfo info)
        {
            Transform child;
            GameObject newParent = new GameObject("thrownObjectParent");
            Inventory inventoryData = characterList[info.Sender.ActorNumber].GetComponent<Inventory>();
            child = characterList[info.Sender.ActorNumber].transform.GetChild(8).GetChild(0);
            newParent.transform.position = child.transform.position;
            newParent.transform.rotation = child.transform.rotation;
            child.parent = newParent.transform;
            newParent.transform.Translate(0,1,0);
            inventoryData.getItemInventory()[inventoryPlace] = 0;
            photonView.RPC("AnswerDropItem", info.Sender);
            /*for (int i = 0; i < 100; i++)
        {
            child.Translate(Vector3.forward*Time.deltaTime,Space.Self);
            
        }*/
            thrownItems.Add(newParent);
            thrownItemsCount.Add(0);
        }
    
        [PunRPC]
        public void AnswerDropItem()
        {
            itemInHand = 0;
        }
    
    
    }
}