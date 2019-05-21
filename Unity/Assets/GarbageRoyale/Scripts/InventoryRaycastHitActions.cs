using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GarbageRoyale.Scripts.HUD;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
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
            if (Input.GetKey(KeyCode.F))
            {
                wantUse = true;
            }
        }

        private void FixedUpdate()
        {
            if (wantUse)
            {
                var ray = gc.players[System.Array.IndexOf(gc.AvatarToUserId,PhotonNetwork.AuthValues.UserId)].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
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
                        /*if (PhotonNetwork.IsMasterClient)
                        {
                            //var charFirst = characterList.First();
                            Debug.Log(hitInfo.transform.name);
                            GameObject player =
                                gc.players[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].gameObject;
                            actionTakeItem(itemGob, player, true);
                        }
                        else
                        {*/
                            staffName = "Staff_" + itemGob.transform.position.x + "_" + ((int)itemGob.transform.position.y + 1) + "_" + itemGob.transform.position.z;
                            //Debug.Log(staffName);
                            photonView.RPC("AskTakeItem", RpcTarget.MasterClient, staffName, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                            //Item itemData = itemGob.GetComponent<Item>();
                        //}
                    }
                }
            }

            wantUse = false;
        }

        private void actionTakeItem(GameObject itemGob, GameObject player, bool isMaster)
        {
            if (itemGob.GetComponent<Item>() != null)
            {
                Item itemData = itemGob.GetComponent<Item>();
                Inventory inventoryData = player.GetComponent<Inventory>();

                Debug.Log(string.Format("Item : \n ID : {0} - Name: {1} - Damage : {2} - Type : {3}", itemData.getId(), itemData.getName(), itemData.getDamage(), itemData.getType()));
                if(isMaster)
                {
                    controller = GameObject.Find("Controller");
                    controller.GetComponent<InventoryGUI>().printSprite(inventoryData.findPlaceInventory(), itemData.getId());
                }
                if (inventoryData.setItemInventory(itemData.getId()))
                {
                    itemGob.SetActive(false);
                    //photonView.RPC("AskDisableItem", RpcTarget.All, itemGob.name);
                    
                }
                Debug.Log(string.Format("Inventory : \n ID : {0} {1} {2} {3} {4} - Joueur : {5}", inventoryData.getItemInventory()[0], inventoryData.getItemInventory()[1], inventoryData.getItemInventory()[2], inventoryData.getItemInventory()[3], inventoryData.getItemInventory()[4], player));
            }
            else
            {
                Debug.Log("Not an item");
            }
        }
        
        [PunRPC]
        public void AskTakeItem(string objName, int playerIndex, PhotonMessageInfo info)
        {
            //photonView.RPC("ChangeGUIClient", info.Sender, gc.players[playerIndex].GetComponent<Inventory>().findPlaceInventory(), GameObject.Find(objName).GetComponent<Item>().getId());
            photonView.RPC("PutItemInInventory",RpcTarget.All, objName, playerIndex);
            //Debug.Log("ID du joueur : " + info.Sender.ActorNumber);
        }

        [PunRPC]
        public void PutItemInInventory(string objName, int playerIndex)
        {
            actionTakeItem(GameObject.Find(objName), gc.players[playerIndex].PlayerGameObject, false);
        }

        [PunRPC]
        public void ChangeGUIClient(int place, int id, PhotonMessageInfo info)
        {
            controller = GameObject.Find("Controller");
            controller.GetComponent<InventoryGUI>().printSprite(place, id);
        }
        /*
        [PunRPC]
        public void AskDisableItem(string objName, PhotonMessageInfo info)
        {
            Debug.Log(objName);
            Debug.Log(GameObject.Find(objName).activeSelf);
            GameObject.Find(objName).SetActive(false);
        }*/

        [PunRPC]
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
            controller.GetComponent<InventoryGUI>().printSkillSprite(place,1);
        }
    }
}    
