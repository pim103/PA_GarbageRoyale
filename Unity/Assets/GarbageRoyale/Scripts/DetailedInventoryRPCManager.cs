﻿using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class DetailedInventoryRPCManager : MonoBehaviourPunCallbacks
    {
        private GameController gc;
        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        [PunRPC]
        public void AskSwapInventoryItems(int playerIndex, int oldPlace, int newPlace)
        {
            Debug.Log("yes");
            Debug.Log(" sent playerindex "+playerIndex+" oldplace "+oldPlace+" newplace "+newPlace);
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            photonView.RPC("AnswerSwapInventoryItems", RpcTarget.All,playerIndex, oldPlace, newPlace);
        }
        
        [PunRPC]
        public void testrpc()
        {
            Debug.Log("ALLO");
        }

        [PunRPC]
        public void AnswerSwapInventoryItems(int playerIndex, int oldPlace, int newPlace)
        {
            Debug.Log(" received playerindex "+playerIndex+" oldplace "+oldPlace+" newplace "+newPlace);
            Inventory playerInventory = gc.players[playerIndex].PlayerGameObject.GetComponent<Inventory>();
            int idMem = playerInventory.itemInventory[newPlace];
            playerInventory.itemInventory[newPlace] = playerInventory.itemInventory[oldPlace];
            playerInventory.itemInventory[oldPlace] = idMem;
        }

        [PunRPC]
        public void AskCraftItem(int itemtype, int playerIndex)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            photonView.RPC("AnswerCraftItem", RpcTarget.All, itemtype,playerIndex, gc.items.Count);
        }
        
        [PunRPC]
        public void AnswerCraftItem(int itemtype, int playerIndex, int itemID)
        {
            Inventory playerInventory = gc.players[playerIndex].PlayerGameObject.GetComponent<Inventory>();
            GameObject item;
            int poolID = 0;

            switch (itemtype)
            {
                case 2:
                    poolID = 1;
                    break;
                case 9:
                    poolID = 2;
                    break;
                case 10:
                    poolID = 3;
                    break;
                case 15:
                    poolID = 7;
                    break;
                default:
                    Debug.Log("Wrong Item");
                    break;
            }

            item = ObjectPooler.SharedInstance.GetPooledObject(poolID);
            item.GetComponent<Item>().setId(itemID);
            gc.items.Add(itemID,item);
            playerInventory.itemInventory[25] = itemID;

            if (playerIndex == System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                gc.items[itemID].transform.SetParent(gc.players[playerIndex].PlayerTorch.transform.parent);
                gc.items[itemID].transform.localPosition = new Vector3(0, 0, 0);
                gc.items[itemID].transform.localRotation = gc.items[itemID].transform.parent.transform.localRotation;
            }
        }
        
        [PunRPC]
        public void AskDeleteCraftItem(int playerIndex)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            photonView.RPC("AnswerDeleteCraftItem", RpcTarget.All, playerIndex);
        }
        
        [PunRPC]
        public void AnswerDeleteCraftItem(int playerIndex)
        {
            Inventory playerInventory = gc.players[playerIndex].PlayerGameObject.GetComponent<Inventory>();
            playerInventory.itemInventory[25] = -1;
        }
        
        [PunRPC]
        public void AskSwapInventorySkills(int playerIndex, int oldPlace, int newPlace)
        {
            Debug.Log("yes");
            Debug.Log(" sent playerindex "+playerIndex+" oldplace "+oldPlace+" newplace "+newPlace);
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            photonView.RPC("AnswerSwapInventorySkills", RpcTarget.All,playerIndex, oldPlace, newPlace);
        }

        [PunRPC]
        public void AnswerSwapInventorySkills(int playerIndex, int oldPlace, int newPlace)
        {
           Inventory playerInventory = gc.players[playerIndex].PlayerGameObject.GetComponent<Inventory>();
            int idMem = playerInventory.skillInventory[newPlace];
            playerInventory.skillInventory[newPlace] = playerInventory.skillInventory[oldPlace];
            playerInventory.skillInventory[oldPlace] = idMem;
        }
    }
}
