using System;
using System.Collections.Generic;
using GarbageRoyale.Scripts.HUD;
using GarbageRoyale.Scripts.Items;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.GameMaster
{
    public class GameMasterGui : MonoBehaviourPunCallbacks
    {
        private GameController gc;
        [SerializeField]
        private Button invisibleButton;
        [SerializeField]
        private Button invincibleButton;
        [SerializeField] 
        private InputField itemTypeField;
        [SerializeField] 
        private Button addItemButton;


        private void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            addItemButton.onClick.AddListener(AddItem);
            invincibleButton.onClick.AddListener(SetInvincible);
            //invisibleButton.interactable(false);
        }

        public void SetInvincible()
        {
            return;
        }
        
        
        
        public void AddItem()
        {
            Debug.Log(Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
            Debug.Log(gc.AvatarToUserId);
            Debug.Log(PhotonNetwork.AuthValues.UserId);
            Debug.Log(itemTypeField.text);
            
            
            photonView.RPC("AskAddItem", RpcTarget.MasterClient, int.Parse(itemTypeField.text), Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
            
            itemTypeField.text = "";
        }
        
        
        [PunRPC]
        public void AskAddItem(int itemtype, int playerIndex)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            Inventory playerInventory = gc.players[playerIndex].PlayerGameObject.GetComponent<Inventory>();
            ChangeGUIGMClient(playerInventory.findPlaceInventory(), itemtype);
            photonView.RPC("AnswerAddItem", RpcTarget.All, itemtype, playerIndex, gc.items.Count, playerInventory.findPlaceInventory());
        }
        
        [PunRPC]
        public void AnswerAddItem(int itemtype, int playerIndex, int itemID, int place)
        {
            Inventory playerInventory = gc.players[playerIndex].PlayerGameObject.GetComponent<Inventory>();
            GameObject item;

            item = ObjectPooler.SharedInstance.GetPooledObject(itemtype);
            item.GetComponent<Item>().setId(itemID);
            gc.items.Add(itemID,item);
            
            Debug.Log("Test Add");
            playerInventory.itemInventory[place] = itemID;

            if (playerIndex == System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                gc.items[itemID].transform.SetParent(gc.players[playerIndex].PlayerTorch.transform.parent);
                gc.items[itemID].transform.localPosition = new Vector3(0, 0, 0);
                gc.items[itemID].transform.localRotation = gc.items[itemID].transform.parent.transform.localRotation;
            }
        }

        public void ChangeGUIGMClient(int place, int id)
        {
            Item item = gc.items[id].GetComponent<Item>();
            //Debug.Log(item.name);
            gc.GetComponent<InventoryGUI>().printSprite(place, item.itemImg);
        }
    }
}