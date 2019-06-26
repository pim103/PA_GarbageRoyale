using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GarbageRoyale.Scripts.HUD;
using GarbageRoyale.Scripts.Items;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.InventoryScripts
{
    public class InventoryRaycastHitActions : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameObject itemPanel;
        [SerializeField]
        private Text itemDataText;
        private CameraRaycastHitActions actionScript;

        private GameController gc;
        private Dictionary<int, GameObject> characterList = new Dictionary<int, GameObject>();

        private string staffName;
        private GameObject controller;

        private bool wantUse = false;

        private int place;
        
        // Start is called before the first frame update
        
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            characterList = gc.characterList;
            itemPanel.SetActive(false);
            Debug.Log(characterList);
        }
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && !wantUse)
            {
                wantUse = true;
            }
        }

        private void FixedUpdate()
        {
            if(!gc.endOfInit)
            {
                return;
            }

            bool touch;

            var ray = gc.players[Array.IndexOf(gc.AvatarToUserId,PhotonNetwork.AuthValues.UserId)].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hitInfo;

            if (touch = Physics.Raycast(ray, out hitInfo, 2f))
            {
                if (hitInfo.transform.gameObject.GetComponent<Item>())
                {
                    itemPanel.SetActive(true);
                    GameObject itemGob = hitInfo.transform.gameObject;
                    Item itemData = itemGob.GetComponent<Item>();
                    if (itemData.getType() != (int) ItemController.TypeItem.Implant)
                    {
                        if (!hitInfo.transform.gameObject.GetComponent<Item>().isThrow)
                        {
                            itemDataText.text = itemData.name + " (Press F to take)";
                        }
                    }
                    else
                    {
                        itemDataText.text = itemGob.GetComponent<Skill>().name + " (Press F to take)";
                    }
                        
                }
                else
                {
                    itemPanel.SetActive(false);
                    itemDataText.text = "";
                }
            }
            else
            {
                itemPanel.SetActive(false);
                itemDataText.text = "";
            }

            if (wantUse)
            {   
                if (touch)
                {
                    if (hitInfo.transform.name == "implantOnline(Clone)")
                    {
                        photonView.RPC("AskTakeImplant", RpcTarget.MasterClient, 1);
                    }
                    else
                    {
                        GameObject itemGob = hitInfo.transform.gameObject;
                        Item itemScript = itemGob.GetComponent<Item>();

                        if (itemScript && itemScript.isPickable)
                        {
                            staffName = "Staff_" + itemGob.transform.position.x + "_" +
                                        ((int) itemGob.transform.position.y + 1) + "_" +
                                        itemGob.transform.position.z;
                            //Debug.Log(staffName);
                            photonView.RPC("AskTakeItem", RpcTarget.MasterClient, staffName,
                            Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),false,itemScript.getId());
                        } else
                        {
                            Debug.Log("Not an item");
                        }
                    }
                }
            }

            wantUse = false;
        }

        private void actionTakeItem(GameObject itemGob, GameObject player, bool isMaster)
        {
            Item itemData = itemGob.GetComponent<Item>();
            Inventory inventoryData = player.GetComponent<Inventory>();

            Debug.Log(string.Format("Item : \n ID : {0} - Name: {1} - Damage : {2} - Type : {3}", itemData.getId(), itemData.getName(), itemData.getDamage(), itemData.getType()));
            if(isMaster)
            {
                gc.GetComponent<InventoryGUI>().printSprite(inventoryData.findPlaceInventory(), itemData.itemImg);
            }
            if (inventoryData.setItemInventory(itemData.getId()))
            {
                itemGob.SetActive(false);
                //photonView.RPC("AskDisableItem", RpcTarget.All, itemGob.name);
                
            }
            Debug.Log(string.Format("Inventory : \n ID : {0} {1} {2} {3} {4} - Joueur : {5}", inventoryData.getItemInventory()[0], inventoryData.getItemInventory()[1], inventoryData.getItemInventory()[2], inventoryData.getItemInventory()[3], inventoryData.getItemInventory()[4], player));
            
        }
        
        [PunRPC]
        public void AskTakeItem(string objName, int playerIndex, bool isOnline, int itemId, PhotonMessageInfo info)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            Debug.Log(gc.items[itemId].GetComponent<Item>().type);
            Item itemData = gc.items[itemId].GetComponent<Item>();
            
            if(!itemData.isPickable)
            {
                return;
            }

            if (itemData.getType() != (int)ItemController.TypeItem.Implant)
            {
                place = gc.players[playerIndex].GetComponent<Inventory>().findPlaceInventory();
            }
            else
            {
                place = gc.players[playerIndex].GetComponent<Inventory>().findPlaceSkills();
            }

            photonView.RPC("ChangeGUIClient", info.Sender,place,
                gc.items[itemId].GetComponent<Item>().getId()
            );

            photonView.RPC("PutItemInInventory", RpcTarget.All, objName, playerIndex, isOnline, itemId);
            //Debug.Log("ID du joueur : " + info.Sender.ActorNumber);
        }

        [PunRPC]
        public void PutItemInInventory(string objName, int playerIndex, bool isOnline, int itemId)
        {
            Item itemData = gc.items[itemId].GetComponent<Item>();
            Inventory inventoryData = gc.players[playerIndex].GetComponent<Inventory>();
            bool isInInventory = false;
            if (itemData.getType()!= (int)ItemController.TypeItem.Implant)
            {
                if (inventoryData.setItemInventory(itemData.getId()))
                {
                    itemData.setThrowerId(playerIndex);
                    isInInventory = true;
                }
            }
            else
            {
                if (inventoryData.setSkillInventory(itemData.getId()))
                {
                    isInInventory = true;
                }
            }

            if (isInInventory)
            {
                gc.items[itemId].transform.SetParent(gc.players[playerIndex].PlayerTorch.transform.parent);
                gc.items[itemId].transform.localPosition = new Vector3(0, 0, 0);
                gc.items[itemId].transform.localRotation =
                gc.items[itemId].transform.parent.transform.localRotation;
                gc.items[itemId].SetActive(false);
            }
            //Debug.Log(itemData.getThrowerId());
        }

        [PunRPC]
        public void ChangeGUIClient(int place, int id, PhotonMessageInfo info)
        {
            //Debug.Log(item.name);
            Item item = gc.items[id].GetComponent<Item>();
            if(item.type != (int)ItemController.TypeItem.Implant){
                gc.GetComponent<InventoryGUI>().printSprite(place, item.itemImg);
            }
            else
            {
                Skill skill = gc.items[id].GetComponent<Skill>();
                gc.GetComponent<InventoryGUI>().printSkillSprite(place, skill.type);
            }

        }
    }
}    
