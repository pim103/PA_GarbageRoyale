using System;
using System.Collections.Generic;
using GarbageRoyale.Scripts.HUD;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class ItemDragController : MonoBehaviourPunCallbacks, IDragHandler,IEndDragHandler
    {
        private RawImage[] ItemSlots;
        
        private RawImage[] CraftingSlots;
        
        private RawImage CraftingResultSlot;

        private RectTransform[] ItemRects = new RectTransform[20];
        
        private RectTransform[] CraftingRects = new RectTransform[20];
        
        private RectTransform CraftingResultRect = new RectTransform();

        private GameController gc;

        private InventorySpritesExposer spritesExposer;

        public int invIndex;

        public List<int> craftingList;

        private DetailedInventoryRPCManager RpcManager;
        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            RpcManager = GameObject.Find("DetailedInventory").GetComponent<DetailedInventoryRPCManager>();
            spritesExposer = GameObject.Find("DetailedInventory").GetComponent<InventorySpritesExposer>();
            ItemSlots = spritesExposer.ItemSlots;
            CraftingSlots = spritesExposer.CraftingSlots;
            CraftingResultSlot = spritesExposer.CraftingResult;
            
            for (int i = 0; i<20; i++)
            {
                ItemRects[i] = ItemSlots[i].transform as RectTransform;
            }
            for (int i = 0; i<5; i++)
            {
                CraftingRects[i] = CraftingSlots[i].transform as RectTransform;
            }
            CraftingResultRect = CraftingResultSlot.transform as RectTransform;
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint(CraftingResultRect, Input.mousePosition))
            {
                Inventory playerInventory = gc.players[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerGameObject.GetComponent<Inventory>();
                if (playerInventory.itemInventory[25] != -1)
                {
                    for (int j = 20; j < 25; j++)
                    {
                        if (playerInventory.itemInventory[j] != -1)
                        {
                            gc.items[playerInventory.itemInventory[j]].SetActive(false);
                            playerInventory.itemInventory[j] = -1;
                            
                        }

                        CraftingSlots[j-20].texture = null;
                    }
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //gc = GameObject.Find("Controller").GetComponent<GameController>();
            Inventory playerInventory = gc.players[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerGameObject.GetComponent<Inventory>();
            transform.localPosition = Vector3.zero;
            for (int i = 0; i<20; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(ItemRects[i], Input.mousePosition))
                {
                    //Debug.Log("playerindex "+System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)+" oldplace "+invIndex+" newplace "+i);
                    //RpcManager.photonView.RPC("testrpc",RpcTarget.MasterClient);
                    RpcManager.photonView.RPC("AskSwapInventoryItems", RpcTarget.MasterClient,System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),invIndex,i);
                    //ItemSlots[i].transform.position = ItemRects[i].position;
                    RawImage rawImg = GameObject.Find("ItemImg_" + i).GetComponent<RawImage>();
                    Texture textureMem = rawImg.texture;
                    rawImg.texture = GetComponent<RawImage>().texture;
                    GetComponent<RawImage>().texture = textureMem;
                    
                    /*int idMem = playerInventory.itemInventory[i];
                    playerInventory.itemInventory[i] = playerInventory.itemInventory[invIndex];
                    playerInventory.itemInventory[invIndex] = idMem;
                    Debug.Log("newPlace " + playerInventory.itemInventory[i]+ " oldplace " + idMem);*/
                }
            }
            for (int i = 0; i<5; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(CraftingRects[i], Input.mousePosition))
                {
                    Debug.Log("playerindex "+System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)+" oldplace "+invIndex+" newplace "+i);
                    RpcManager.photonView.RPC("AskSwapInventoryItems", RpcTarget.MasterClient,System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),invIndex,20+i);
                    //ItemSlots[i].transform.position = ItemRects[i].position;
                    RawImage rawImg = GameObject.Find("CraftingImg_" + i).GetComponent<RawImage>();
                    Texture textureMem = rawImg.texture;
                    rawImg.texture = GetComponent<RawImage>().texture;
                    GetComponent<RawImage>().texture = textureMem;
                    //Inventory playerInventory = gc.players[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerGameObject.GetComponent<Inventory>();
                    /*int idMem = playerInventory.itemInventory[20+i];
                    playerInventory.itemInventory[20+i] = playerInventory.itemInventory[invIndex];
                    playerInventory.itemInventory[invIndex] = idMem;*/
                    
                }
            }
            craftingList.Clear();
            for (int j = 20; j < 25; j++)
            {
                //Debug.Log("eheh "+playerInventory.itemInventory[j]);
                if (playerInventory.itemInventory[j] != -1)
                {
                    //Debug.Log("eh "+gc.items[playerInventory.itemInventory[j]].GetComponent<Item>().type);
                    craftingList.Add(gc.items[playerInventory.itemInventory[j]].GetComponent<Item>().type);
                    
                }
            }
            if (craftingList.Contains(1) && craftingList.Contains(5) && craftingList.Count == 2)
            {
                RpcManager.photonView.RPC("AskCraftItem",RpcTarget.All,2,System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                CraftingResultSlot.texture = gc.inventoryGui.rawSprites[1].texture;
                //playerInventory.itemInventory[25] = 2;
                //Debug.Log("yees");
            }
            else
            {
                CraftingResultSlot.texture = null;
                //playerInventory.itemInventory[25] = -1;
                RpcManager.photonView.RPC("AskDeleteCraftItem",RpcTarget.All,System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
            }
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

            if (playerIndex ==System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                RawImage rawImg = GameObject.Find("ItemImg_" + newPlace).GetComponent<RawImage>();
                Texture textureMem = rawImg.texture;
                RawImage oldPlaceImg = GameObject.Find("ItemImg_" + oldPlace).GetComponent<RawImage>();
                rawImg.texture = oldPlaceImg.texture;
                oldPlaceImg.texture = textureMem;
            }
        }
    }
}
