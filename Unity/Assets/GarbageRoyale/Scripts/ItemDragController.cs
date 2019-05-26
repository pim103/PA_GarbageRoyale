using System;
using GarbageRoyale.Scripts.HUD;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class ItemDragController : MonoBehaviour, IDragHandler,IEndDragHandler
    {
        private RawImage[] ItemSlots;

        private RectTransform[] ItemRects = new RectTransform[20];

        private GameController gc;

        public int invIndex;
        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            ItemSlots = GameObject.Find("DetailedInventory").GetComponent<InventorySpritesExposer>().ItemSlots;
            for (int i = 0; i<20; i++)
            {
                ItemRects[i] = ItemSlots[i].transform as RectTransform;
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.localPosition = Vector3.zero;
            for (int i = 0; i<20; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(ItemRects[i], Input.mousePosition))
                {
                    //ItemSlots[i].transform.position = ItemRects[i].position;
                    RawImage rawImg = GameObject.Find("ItemImg_" + i).GetComponent<RawImage>();
                    Texture textureMem = rawImg.texture;
                    rawImg.texture = GetComponent<RawImage>().texture;
                    GetComponent<RawImage>().texture = textureMem;
                    Inventory playerInventory = gc.players[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerGameObject.GetComponent<Inventory>();
                    int idMem = playerInventory.itemInventory[i];
                    playerInventory.itemInventory[i] = playerInventory.itemInventory[invIndex];
                    playerInventory.itemInventory[invIndex] = idMem;
                }
            }
        }
    }
}
