using System;
using System.Collections.Generic;
using GarbageRoyale.Scripts.HUD;
using GarbageRoyale.Scripts.InventoryScripts;
using GarbageRoyale.Scripts.Items;
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
        
        private RawImage[] SkillSlots;
        
        private RawImage CraftingResultSlot;

        private GameObject DropArea;

        private RectTransform[] ItemRects = new RectTransform[20];
        
        private RectTransform[] CraftingRects = new RectTransform[20];
        
        private RectTransform[] SkillRects = new RectTransform[20];
        
        private RectTransform CraftingResultRect = new RectTransform();
        private RectTransform DropAreaRect = new RectTransform();

        private GameController gc;

        private InventoryActionsController iac;
        private InventorySpritesExposer spritesExposer;

        public int invIndex;

        public List<int> craftingList;

        private DetailedInventoryRPCManager RpcManager;

        private bool isGettingCraftedItem = false;

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            iac = GameObject.Find("Controller").GetComponent<InventoryActionsController>();
            RpcManager = GameObject.Find("DetailedInventory").GetComponent<DetailedInventoryRPCManager>();
            spritesExposer = GameObject.Find("DetailedInventory").GetComponent<InventorySpritesExposer>();
            ItemSlots = spritesExposer.ItemSlots;
            CraftingSlots = spritesExposer.CraftingSlots;
            CraftingResultSlot = spritesExposer.CraftingResult;
            SkillSlots = spritesExposer.skillSlots;
            DropArea = spritesExposer.DropArea;
            
            for (int i = 0; i<20; i++)
            {
                ItemRects[i] = ItemSlots[i].transform as RectTransform;
            }
            for (int i = 0; i<5; i++)
            {
                CraftingRects[i] = CraftingSlots[i].transform as RectTransform;
            }
            for (int i = 0; i<2; i++)
            {
                SkillRects[i] = SkillSlots[i].transform as RectTransform;
            }
            CraftingResultRect = CraftingResultSlot.transform as RectTransform;
            DropAreaRect = DropArea.transform as RectTransform;
            
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
                Inventory playerInventory = gc.players[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerInventory;
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
                        isGettingCraftedItem = true;
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(CraftingRects[i], Input.mousePosition))
                {
                    CraftingResultSlot.texture = null;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //gc = GameObject.Find("Controller").GetComponent<GameController>();
            Inventory playerInventory = gc.players[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerInventory;
            transform.localPosition = Vector3.zero;
            if (invIndex < 100)
            {
                for (int i = 0; i < 20; i++)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(ItemRects[i], Input.mousePosition))
                    {
                        if (isGettingCraftedItem)
                        {
                            RpcManager.photonView.RPC("AskGetCraftedItem", RpcTarget.MasterClient, gc.items[playerInventory.itemInventory[invIndex]].GetComponent<Item>().type, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),  playerInventory.itemInventory[invIndex],  i,  PhotonNetwork.IsMasterClient);
                            isGettingCraftedItem = false;
                        }
                        else
                        {
                            RpcManager.photonView.RPC("AskSwapInventoryItems", RpcTarget.MasterClient,
                                System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId), invIndex, i,
                                PhotonNetwork.IsMasterClient);
                        }

                        RawImage rawImg = GameObject.Find("ItemImg_" + i).GetComponent<RawImage>();
                        Texture textureMem = rawImg.texture;
                        rawImg.texture = GetComponent<RawImage>().texture;
                        GetComponent<RawImage>().texture = textureMem;
                        return;
                        
                    }
                }
                if (isGettingCraftedItem)
                {
                    RpcManager.photonView.RPC("AskGetCraftedItem", RpcTarget.MasterClient, gc.items[playerInventory.itemInventory[invIndex]].GetComponent<Item>().type, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),  playerInventory.itemInventory[invIndex],  -1,  PhotonNetwork.IsMasterClient);
                    isGettingCraftedItem = false;
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(CraftingRects[i], Input.mousePosition))
                        {
                            Debug.Log("playerindex " +
                                      System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId) +
                                      " oldplace " + invIndex + " newplace " + i);
                            RpcManager.photonView.RPC("AskSwapInventoryItems", RpcTarget.MasterClient,
                                System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId), invIndex,
                                20 + i, PhotonNetwork.IsMasterClient);
                            //ItemSlots[i].transform.position = ItemRects[i].position;
                            RawImage rawImg = GameObject.Find("CraftingImg_" + i).GetComponent<RawImage>();
                            Texture textureMem = rawImg.texture;
                            rawImg.texture = GetComponent<RawImage>().texture;
                            GetComponent<RawImage>().texture = textureMem;
                            //Inventory playerInventory = gc.players[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerInventory;
                            /*int idMem = playerInventory.itemInventory[20+i];
                            playerInventory.itemInventory[20+i] = playerInventory.itemInventory[invIndex];
                            playerInventory.itemInventory[invIndex] = idMem;*/

                        }
                        else
                        {
                            RpcManager.photonView.RPC("AskSwapInventoryItems", RpcTarget.MasterClient,
                                System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId), invIndex,
                                invIndex, PhotonNetwork.IsMasterClient);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(SkillRects[i], Input.mousePosition))
                    {
                        RpcManager.photonView.RPC("AskSwapInventorySkills", RpcTarget.MasterClient,System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId), invIndex - 100,i);
                        //ItemSlots[i].transform.position = ItemRects[i].position;
                        RawImage rawImg = GameObject.Find("SkillImg_" + i).GetComponent<RawImage>();
                        Texture textureMem = rawImg.texture;
                        rawImg.texture = GetComponent<RawImage>().texture;
                        GetComponent<RawImage>().texture = textureMem;
                    }
                }
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(DropAreaRect, Input.mousePosition))
            {
                if (invIndex < 100)
                {
                    iac.photonView.RPC("AskDropItem", RpcTarget.MasterClient, invIndex,
                        System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId), false);
                }
                else
                {
                    iac.photonView.RPC("AskDropSkill", RpcTarget.MasterClient, invIndex-100, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                }
            }
        }

        [PunRPC]
        public void AskSwapInventoryItems(int playerIndex, int oldPlace, int newPlace)
        {
            //Debug.Log("yes");
            Debug.Log(" sent playerindex "+playerIndex+" oldplace "+oldPlace+" newplace "+newPlace);
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            photonView.RPC("AnswerSwapInventoryItems", RpcTarget.All,playerIndex, oldPlace, newPlace);
        }

        [PunRPC]
        public void AnswerSwapInventoryItems(int playerIndex, int oldPlace, int newPlace)
        {
            Debug.Log(" received playerindex "+playerIndex+" oldplace "+oldPlace+" newplace "+newPlace);
            Inventory playerInventory = gc.players[playerIndex].PlayerInventory;
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
        
        [PunRPC]
        public void AskSwapInventorySkills(int playerIndex, int oldPlace, int newPlace)
        {
            //Debug.Log("yes");
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
            Inventory playerInventory = gc.players[playerIndex].PlayerInventory;
            int idMem = playerInventory.skillInventory[newPlace];
            playerInventory.skillInventory[newPlace] = playerInventory.skillInventory[oldPlace];
            playerInventory.skillInventory[oldPlace] = idMem;

            if (playerIndex ==System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                RawImage rawImg = GameObject.Find("SkillImg_" + newPlace).GetComponent<RawImage>();
                Texture textureMem = rawImg.texture;
                RawImage oldPlaceImg = GameObject.Find("SkillImg_" + oldPlace).GetComponent<RawImage>();
                rawImg.texture = oldPlaceImg.texture;
                oldPlaceImg.texture = textureMem;
            }
        }
    }
}
