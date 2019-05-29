﻿using System.Collections.Generic;
using GarbageRoyale.Scripts.HUD;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class InventoryActionsController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameController gc;

        public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
        private Inventory playerInventory;
        private GameObject gtest;
        public string itemInHand;
        public int placeInHand = -1;
        public List<GameObject> thrownItems;
        public List<int> thrownItemsCount;
        
        AudioClip torchLightSound;

        public Dictionary<int, GameObject[]> listTorchLigt = new Dictionary<int, GameObject[]>();

        public bool Send;
        // Start is called before the first frame update
        void Start()
        {
            torchLightSound = GameObject.Find("Controller").GetComponent<SoundManager>().getTorchLightSound();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 0, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                placeInHand = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 1, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                placeInHand = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 2 ,System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                placeInHand = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 3, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                placeInHand = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 4, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                placeInHand = 4;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if(placeInHand!=-1) photonView.RPC("AskDropItem", RpcTarget.MasterClient, placeInHand, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),false);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                if(placeInHand!=-1) photonView.RPC("AskDropItem", RpcTarget.MasterClient, placeInHand, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),true);
            }

            if(Input.GetKeyDown(KeyCode.Mouse1))
            {
                if(itemInHand == "Torch") photonView.RPC("LightOnTorchRPC", RpcTarget.MasterClient, placeInHand, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                if(itemInHand == "Jerrican") photonView.RPC("DisperseOil", RpcTarget.MasterClient, placeInHand, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
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

        public void ExtinctTorch(int id)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("LightOnSpecificTorch", RpcTarget.All, id, false);
            }
        }
        
        [PunRPC]
        public void AskChangePlayerHandItem(int inventoryPlace,int playerIndex,PhotonMessageInfo info)
        {
            Inventory inventoryData = gc.players[playerIndex].GetComponent<Inventory>();
            var idItem = inventoryData.getItemInventory()[inventoryPlace];
            var itemName = "";

            if(idItem > -1)
            {
                itemName = gc.items[idItem].GetComponent<Item>().name;
            }

            photonView.RPC("AnswerChangePlayerHandItem", RpcTarget.All, itemName, playerIndex);
            photonView.RPC("setItemInHand", info.Sender, itemName);
        }
    
        [PunRPC]
        public void AnswerChangePlayerHandItem(string item, int playerIndex)
        {
            int handChild = -1;
            //Desactivate unused items
            gc.players[playerIndex].PlayerStaff.SetActive(false);
            gc.players[playerIndex].PlayerTorch.SetActive(false);
            gc.players[playerIndex].PlayerToiletPaper.SetActive(false);
            gc.players[playerIndex].PlayerJerrican.SetActive(false);
            gc.players[playerIndex].PlayerBottle.SetActive(false);

            switch (item)
            {
                case "Wooden Staff":
                    handChild = 1;
                    gc.players[playerIndex].PlayerStaff.SetActive(true);
                    break;
                case "Steel Staff":
                    handChild = 1;
                    gc.players[playerIndex].PlayerStaff.SetActive(true);
                    break;
                case "Torch":
                    handChild = 0;
                    gc.players[playerIndex].PlayerTorch.SetActive(true);
                    break;
                case "Toilet Paper":
                    handChild = 2;
                    gc.players[playerIndex].PlayerToiletPaper.SetActive(true);
                    break;
                case "Jerrican":
                    gc.players[playerIndex].PlayerJerrican.SetActive(true);
                    break;
                case "Bottle":
                    gc.players[playerIndex].PlayerBottle.SetActive(true);
                    break;
                default:
                    Debug.Log("Error, wrong item");
                    break;
            }
        }

        [PunRPC]
        public void setItemInHand(string newItemInHand)
        {
            itemInHand = newItemInHand;
        }
        
        [PunRPC]
        public void AskDropItem(int inventoryPlace,int playerIndex, bool throwItem,PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            int idItem = gc.players[playerIndex].GetComponent<Inventory>().getItemInventory()[inventoryPlace];
            if (idItem == -1)
            {
                return;
            }
            
            int typeItem = gc.items[idItem].GetComponent<Item>().type;
            string typeName = gc.items[idItem].GetComponent<Item>().name;

            photonView.RPC("AnswerDropItem", RpcTarget.All, typeItem, idItem, playerIndex, throwItem, inventoryPlace, typeName);
        }
    
        [PunRPC]
        public void AnswerDropItem(int typeItem, int idItem, int playerIndex, bool throwItem, int inventoryPlace, string typeName)
        {
            gc.players[playerIndex].GetComponent<Inventory>().itemInventory[inventoryPlace] = -1;
            int handChild = -1;

            Debug.Log("type : "+gc.items[idItem].GetComponent<Item>().type);
            gc.items[idItem].transform.parent = null;
            gc.items[idItem].SetActive(true);
            gc.items[idItem].GetComponent<Item>().resetScale();

            if (throwItem)
            {
                gc.items[idItem].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 2, 10), ForceMode.Impulse);
            }

            switch (typeName)
            {
                case "Wooden Staff":
                    handChild = 1;
                    gc.players[playerIndex].PlayerStaff.SetActive(false);
                    break;
                case "Steel Staff":
                    handChild = 1;
                    gc.players[playerIndex].PlayerStaff.SetActive(false);
                    break;
                case "Torch":
                    handChild = 0;
                    gc.players[playerIndex].PlayerTorch.SetActive(false);
                    if(gc.players[playerIndex].PlayerTorch.transform.GetChild(0).gameObject.activeSelf)
                    {
                        gc.items[idItem].transform.GetChild(0).gameObject.SetActive(true);
                    } else
                    {
                        gc.items[idItem].transform.GetChild(0).gameObject.SetActive(false);
                    }
                    break;
                case "Toilet Paper":
                    handChild = 2;
                    gc.players[playerIndex].PlayerToiletPaper.SetActive(false);
                    break;
                case "Jerrican":
                    gc.players[playerIndex].PlayerJerrican.SetActive(false);
                    break;
                case "Bottle":
                    gc.players[playerIndex].PlayerBottle.SetActive(false);
                    break;
                default:
                    Debug.Log("Error, wrong item");
                    break;
            }
            
            if (playerIndex == System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                gc.GetComponent<InventoryGUI>().deleteSprite(placeInHand);
                itemInHand = "";
                placeInHand = -1;
            }
        }
        
        [PunRPC]
        private void LightOnTorchRPC(int placeInHand, int playerIndex)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            Inventory inventoryData = gc.players[playerIndex].GetComponent<Inventory>();
            int itemId = inventoryData.getItemInventory()[placeInHand];

            if(gc.playersActions[playerIndex].headIsInWater)
            {
                return;
            }

            if (gc.players[playerIndex].PlayerTorch.activeSelf && gc.items[itemId].GetComponent<Item>().name == "Torch")
            {
                bool toggle = !gc.players[playerIndex].PlayerTorch.transform.GetChild(0).gameObject.activeSelf;
                photonView.RPC("LightOnSpecificTorch", RpcTarget.All, playerIndex, toggle);
            }
        }

        [PunRPC]
        private void LightOnSpecificTorch(int playerIndex, bool toggle)
        {
            gc.players[playerIndex].PlayerTorch.transform.GetChild(0).gameObject.SetActive(toggle);
        }

        [PunRPC]
        private void DisperseOil(int placeInHand, int playerIndex)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            Inventory inventoryData = gc.players[playerIndex].GetComponent<Inventory>();
            int itemId = inventoryData.getItemInventory()[placeInHand];

            if (gc.items[itemId].GetComponent<Item>().name == "Jerrican" && gc.players[playerIndex].PlayerJerrican.transform.childCount > 0)
            {
                photonView.RPC("DisperseSpecificOil", RpcTarget.All, playerIndex);
            }
        }

        [PunRPC]
        private void DisperseSpecificOil(int playerIndex)
        {
            var oil = gc.players[playerIndex].PlayerJerrican.transform.GetChild(0);
            oil.parent = null;
            oil.localScale = new Vector3(1f, 1f, 1f);
            oil.gameObject.SetActive(true);
        }
    }
}