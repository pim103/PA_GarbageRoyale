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

namespace GarbageRoyale.Scripts
{
    public class InventoryRaycastHitActions : MonoBehaviourPunCallbacks
    {
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
            if (wantUse)
            {
                var ray = gc.players[Array.IndexOf(gc.AvatarToUserId,PhotonNetwork.AuthValues.UserId)].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
                RaycastHit hitInfo;
                
                if (Physics.Raycast(ray, out hitInfo, 2f))
                {
                    if (hitInfo.transform.name == "implantOnline(Clone)")
                    {
                        photonView.RPC("AskTakeImplant", RpcTarget.MasterClient, 1);
                    }
                    else
                    {
                        GameObject itemGob = hitInfo.transform.gameObject;
                        Item itemScript = itemGob.GetComponent<Item>();

                        if (itemScript)
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
                gc.GetComponent<InventoryGUI>().printSprite(inventoryData.findPlaceInventory(), itemData.type, itemData.itemImg);
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
            if (itemData.getType() != 12)
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

            photonView.RPC("PutItemInInventory",RpcTarget.All, objName, playerIndex, isOnline, itemId);
            //Debug.Log("ID du joueur : " + info.Sender.ActorNumber);
        }

        [PunRPC]
        public void PutItemInInventory(string objName, int playerIndex, bool isOnline, int itemId)
        {
            Item itemData = gc.items[itemId].GetComponent<Item>();
            Inventory inventoryData = gc.players[playerIndex].GetComponent<Inventory>();
            bool isInInventory = false;
            if (itemData.getType()!= 12)
            {
                if (inventoryData.setItemInventory(itemData.getId()))
                {
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
        }

        /*public void takeDroppedItem(int itemId, GameObject player)
        {
            Inventory inventoryData = player.GetComponent<Inventory>();
            Debug.Log("well "+inventoryData.findPlaceInventory());
            if (player == gc.players[Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)])
            {
                gc.GetComponent<InventoryGUI>().printSprite(inventoryData.findPlaceInventory(), itemId, null);
            }
            inventoryData.setItemInventory(itemId);
            Debug.Log(string.Format("Inventory : \n ID : {0} {1} {2} {3} {4} - Joueur : {5}", inventoryData.getItemInventory()[0], inventoryData.getItemInventory()[1], inventoryData.getItemInventory()[2], inventoryData.getItemInventory()[3], inventoryData.getItemInventory()[4], player));
        }*/

        [PunRPC]
        public void ChangeGUIClient(int place, int id, PhotonMessageInfo info)
        {
            Item item = gc.items[id].GetComponent<Item>();
            //Debug.Log(item.name);
            gc.GetComponent<InventoryGUI>().printSprite(place, item.type, item.itemImg);
        }
        /*
        [PunRPC]
        public void AskDisableItem(string objName, PhotonMessageInfo info)
        {
            Debug.Log(objName);
            Debug.Log(GameObject.Find(objName).activeSelf);
            GameObject.Find(objName).SetActive(false);
        }*/

        /*[PunRPC]
        public void AskTakeImplant(int id, PhotonMessageInfo info)
        {
            Debug.Log("allo");
            Inventory inventoryData = characterList[info.Sender.ActorNumber].GetComponent<Inventory>();
            Debug.Log("allo1");
            controller = GameObject.Find("Controller");
            photonView.RPC("AnswerTakeImplant", info.Sender, inventoryData.findPlaceSkills());
            Debug.Log("allo2");
            inventoryData.setSkillInventory(0);
            Debug.Log("allo3");
            
        }
        [PunRPC]
        public void AnswerTakeImplant(int place, PhotonMessageInfo info)
        {
            gc.GetComponent<InventoryGUI>().printSkillSprite(place,1);
        }*/
    }
}    
