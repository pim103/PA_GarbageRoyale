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
    public class GameMasterGUI : MonoBehaviourPunCallbacks
    {
        private GameController gc;
        [SerializeField]
        private Button invisibleButton;
        [SerializeField]
        private Text invisibleText;
        [SerializeField]
        private Button teleportToTheEnd;
        [SerializeField]
        private Button stopWater;
        [SerializeField]
        private Button openTheGate;
        [SerializeField]
        private Button invincibleButton;
        [SerializeField] 
        private InputField teleportToThemTypeField;
        [SerializeField] 
        private Button teleportToThemButton;
        [SerializeField] 
        private InputField teleportToMeTypeField;
        [SerializeField] 
        private Button teleportToMeButton;
        [SerializeField] 
        private InputField itemTypeField;
        [SerializeField] 
        private Button addItemButton;
        [SerializeField] 
        private Text waterSpeedText;
        [SerializeField] 
        private Slider sliderWaterSpeed;
        [SerializeField] 
        private Button applyWaterSpeedButon;


        private void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            sliderWaterSpeed.value = gc.GetComponent<Water>().getSpeedWater();
            addItemButton.onClick.AddListener(AddItem);
            invincibleButton.onClick.AddListener(SetInvincible);
            invisibleButton.onClick.AddListener(SetInvisible);
            applyWaterSpeedButon.onClick.AddListener(SetWaterSpeedUp);
            stopWater.onClick.AddListener(StopWater);
            openTheGate.onClick.AddListener(OpenTheGate);
            teleportToTheEnd.onClick.AddListener(GoToLastFloor);
            //invisibleButton.interactable(false);
        }

        private void Update()
        {
            waterSpeedText.text = sliderWaterSpeed.value + " / " + sliderWaterSpeed.maxValue;
        }

        public void SetWaterSpeedUp()
        {
            gc.GetComponent<Water>().setSpeedWater(sliderWaterSpeed.value);
        }

        public void SetInvisible()
        {
            photonView.RPC("SetGMInvisible", RpcTarget.All);
        }
        
        public void StopWater()
        {
            gc.triggerWater = !gc.triggerWater;
        }
        
        public void OpenTheGate()
        {
            gc.forceOpenDoor = !gc.forceOpenDoor;
        }

        public void GoToLastFloor()
        {
            gc.wantGoToLastFloor = true;
        }

        public void SetInvincible()
        {
            gc.players[Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerStats.isInvincible = true;
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
            item.GetComponent<Item>().setThrowerId(playerIndex);
            
            //Debug.Log(item.GetComponent<Item>().getThrowerId());

            if (playerIndex == System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                gc.items[itemID].transform.SetParent(gc.players[playerIndex].PlayerTorch.transform.parent);
                gc.items[itemID].transform.localPosition = new Vector3(0, 0, 0);
                gc.items[itemID].transform.localRotation = gc.items[itemID].transform.parent.transform.localRotation;
            }
        }

        [PunRPC]
        public void SetGMInvisible()
        {
            gc.players[0].PlayerRenderer.enabled = !gc.players[0].PlayerRenderer.enabled;
            if (gc.players[0].PlayerRenderer.enabled)
            {
                invisibleText.text = "Devenir invisible";
            }
            else
            {
                invisibleText.text = "Devenir visible";
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