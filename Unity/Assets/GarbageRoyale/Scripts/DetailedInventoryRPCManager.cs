using System;
using System.Collections.Generic;
using GarbageRoyale.Scripts.Items;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class DetailedInventoryRPCManager : MonoBehaviourPunCallbacks
    {
        private GameController gc;
        
        public List<int> craftingList;
        
        private RawImage CraftingResultSlot;
        
        private InventorySpritesExposer spritesExposer;
        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            spritesExposer = GameObject.Find("DetailedInventory").GetComponent<InventorySpritesExposer>();
            CraftingResultSlot = spritesExposer.CraftingResult;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        [PunRPC]
        public void AskSwapInventoryItems(int playerIndex, int oldPlace, int newPlace, bool isMaster, PhotonMessageInfo info)
        {
            Debug.Log("yes");
            Debug.Log(" sent playerindex "+playerIndex+" oldplace "+oldPlace+" newplace "+newPlace);
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            AnswerSwapInventoryItems(playerIndex, oldPlace, newPlace, true);
            if (!isMaster)
            {
                photonView.RPC("AnswerSwapInventoryItems", info.Sender, playerIndex, oldPlace, newPlace, false);
            }
        }
        
        [PunRPC]
        public void testrpc()
        {
            Debug.Log("ALLO");
        }

        [PunRPC]
        public void AnswerSwapInventoryItems(int playerIndex, int oldPlace, int newPlace, bool isMaster)
        {
            Debug.Log(" received playerindex "+playerIndex+" oldplace "+oldPlace+" newplace "+newPlace);
            Inventory playerInventory = gc.players[playerIndex].PlayerGameObject.GetComponent<Inventory>();
            int idMem = playerInventory.itemInventory[newPlace];
            playerInventory.itemInventory[newPlace] = playerInventory.itemInventory[oldPlace];
            playerInventory.itemInventory[oldPlace] = idMem;
            
            craftingList.Clear();
            for (int j = 20; j < 25; j++)
            {
                //Debug.Log("eheh "+playerInventory.itemInventory[j]);
                if (playerInventory.itemInventory[j] != -1)
                {
                    Debug.Log("eh "+gc.items[playerInventory.itemInventory[j]].GetComponent<Item>().type);
                    craftingList.Add(gc.items[playerInventory.itemInventory[j]].GetComponent<Item>().type);

                }
            }

            if (playerIndex == System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                bool isCrafting = true;
                if (craftingList.Count == 2)
                {
                    if (craftingList.Contains((int) ItemController.TypeItem.WoodenStaff) &&
                        craftingList.Contains((int) ItemController.TypeItem.ToiletPaper))
                    {
                        photonView.RPC("AskCraftItem", RpcTarget.MasterClient, (int) ItemController.TypeItem.SteelStaff,
                            Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),isMaster);

                        //CraftingResultSlot.texture = gc.inventoryGui.rawSprites[1].texture;

                    }
                    else if (craftingList.Contains((int) ItemController.TypeItem.Bottle) &&
                             craftingList.Contains((int) ItemController.TypeItem.Jerrican))
                    {
                        photonView.RPC("AskCraftItem", RpcTarget.MasterClient, (int) ItemController.TypeItem.OilBottle,
                            Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),isMaster);
                        //CraftingResultSlot.texture = gc.inventoryGui.rawSprites[7].texture;
                        
                    }
                    else if (craftingList.Contains((int) ItemController.TypeItem.OilBottle) &&
                             craftingList.Contains((int) ItemController.TypeItem.ToiletPaper))
                    {
                        photonView.RPC("AskCraftItem", RpcTarget.MasterClient, (int) ItemController.TypeItem.Molotov,
                            Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),isMaster);
                        //CraftingResultSlot.texture = gc.inventoryGui.rawSprites[8].texture;
                    }
                    else
                    {
                        isCrafting = false;
                    }
                }
                else if (craftingList.Count == 3)
                {
                    if (craftingList.Contains((int) ItemController.TypeItem.Bottle) &&
                        craftingList.Contains((int) ItemController.TypeItem.ToiletPaper) &&
                        craftingList.Contains((int) ItemController.TypeItem.Jerrican))
                    {
                        photonView.RPC("AskCraftItem", RpcTarget.MasterClient, (int) ItemController.TypeItem.Molotov,
                            Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),isMaster);
                        //CraftingResultSlot.texture = gc.inventoryGui.rawSprites[8].texture;
                    }
                    else if (craftingList.Contains((int) ItemController.TypeItem.MetalSheet) &&
                             craftingList.Contains((int) ItemController.TypeItem.Rope) &&
                             craftingList.Contains((int) ItemController.TypeItem.NailBox))
                    {
                        photonView.RPC("AskCraftItem", RpcTarget.MasterClient,
                            (int) ItemController.TypeItem.WolfTrap,
                            Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));

                            //CraftingResultSlot.texture = gc.inventoryGui.rawSprites[9].texture;
                    }
                    else
                    {
                        isCrafting = false;
                    }
                }
                else if (craftingList.Count == 5)
                {
                    if (craftingList.Contains((int) ItemController.TypeItem.Torch)
                        && craftingList.Contains((int) ItemController.TypeItem.MetalSheet)
                        && craftingList.Contains((int) ItemController.TypeItem.WoodenStaff)
                        && craftingList.Contains((int) ItemController.TypeItem.ToiletPaper)
                        && craftingList.Contains((int) ItemController.TypeItem.Rope)
                    )
                    {
                        photonView.RPC("AskCraftItem", RpcTarget.MasterClient, (int) ItemController.TypeItem.ManifTrap,
                            Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),isMaster);
                        //CraftingResultSlot.texture = gc.inventoryGui.rawSprites[10].texture;
                        
                    }
                }
                else
                {
                    isCrafting = false;
                }

                if (!isCrafting)
                {
                    if (playerIndex == System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
                    {
                        Debug.Log("notCrafted");
                        //CraftingResultSlot.texture = null;
                        //playerInventory.itemInventory[25] = -1;
                        //photonView.RPC("AskDeleteCraftItem", RpcTarget.All,System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                    }
                }
            }
        }

        [PunRPC]
        public void AskCraftItem(int itemtype, int playerIndex, bool isMaster, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            
            AnswerCraftItem(itemtype,playerIndex, gc.items.Count);
            if (!isMaster)
            {
                photonView.RPC("AnswerCraftItem", info.Sender, itemtype,playerIndex, gc.items.Count);
            }
        }
        
        [PunRPC]
        public void AnswerCraftItem(int itemtype, int playerIndex, int itemID)
        {
            Inventory playerInventory = gc.players[playerIndex].PlayerGameObject.GetComponent<Inventory>();
            GameObject item;
            int poolID = 0;
            int textureID = 0;
            switch (itemtype)
            {
                case (int)ItemController.TypeItem.SteelStaff:
                    poolID = 1;
                    textureID = 1;
                    break;
                case (int)ItemController.TypeItem.OilBottle:
                    poolID = 2;
                    textureID = 7;
                    break;
                case (int)ItemController.TypeItem.Molotov:
                    poolID = 3;
                    textureID = 8;
                    break;
                case (int)ItemController.TypeItem.WolfTrap:
                    poolID = 7;
                    textureID = 9;
                    break;
                case (int)ItemController.TypeItem.ManifTrap:
                    poolID = 9;
                    textureID = 10;
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
                
                Debug.Log("allo");
                CraftingResultSlot.texture = gc.inventoryGui.rawSprites[textureID].texture;
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
