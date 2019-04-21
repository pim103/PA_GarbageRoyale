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
                var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, 2f))
                {
                    GameObject itemGob = hitInfo.transform.gameObject;
                    if (PhotonNetwork.IsMasterClient)
                    {
                        var charFirst = characterList.First();
                        GameObject player = charFirst.Value;
                        actionTakeItem(itemGob, player);
                    }
                    else
                    {
                        staffName = "Staff_" + itemGob.transform.position.x + "_" + ((int)itemGob.transform.position.y + 1) + "_" + itemGob.transform.position.z;
                        //Debug.Log(staffName);
                        photonView.RPC("AskTakeItem", RpcTarget.MasterClient, staffName);
                    }
                    
                }
            }
        }
        
        private void actionTakeItem(GameObject itemGob, GameObject player)
        {
            if (itemGob.GetComponent<Item>())
            {
                Item itemData = itemGob.GetComponent<Item>();
                Inventory inventoryData = player.GetComponent<Inventory>();

                Debug.Log(string.Format("Item : \n ID : {0} - Name: {1} - Damage : {2} - Type : {3}", itemData.getId(), itemData.getName(), itemData.getDamage(), itemData.getType()));
                if (inventoryData.setItemInventory(itemData.getId(), player))
                {
                    photonView.RPC("AskDisableItem", RpcTarget.All, itemGob.name);
                }
                Debug.Log(string.Format("Inventory : \n ID : {0} {1} {2} {3} {4} - Joueur : {5}", inventoryData.getItemInventory()[0], inventoryData.getItemInventory()[1], inventoryData.getItemInventory()[2], inventoryData.getItemInventory()[3], inventoryData.getItemInventory()[4], player));
            }
            else
            {
                Debug.Log("Not an item");
            }
        }
        
        [PunRPC]
        public void AskTakeItem(string objName, PhotonMessageInfo info)
        {
            Debug.Log("Infos : " + info.Sender.ActorNumber);
            actionTakeItem(GameObject.Find(objName), characterList[info.Sender.ActorNumber]);
            Debug.Log("Player Datas Inventory : " + characterList[info.Sender.ActorNumber].GetComponent<Inventory>().getItemInventory()[0]);
        }
        
        
        
        [PunRPC]
        public void AskDisableItem(string objName, PhotonMessageInfo info)
        {
            GameObject.Find(objName).SetActive(false);
        }
        
    }
}    
