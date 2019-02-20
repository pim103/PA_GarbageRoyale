using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class InventoryRaycastHitActions : MonoBehaviour
    {
        private CameraRaycastHitActions actionScript;

        private GameController gc;
        private Dictionary<int, GameObject> characterList = new Dictionary<int, GameObject>();
        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            characterList = gc.characterList;
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
                    if (itemGob.GetComponent<Item>())
                    {
                        Item itemData = itemGob.GetComponent<Item>();
                        var charFirst = characterList.First();
                        GameObject player = charFirst.Value;
                        Debug.Log(player);
                        Inventory inventoryData = player.GetComponent<Inventory>();

                        Debug.Log(string.Format("Item : \n ID : {0} - Name: {1} - Damage : {2} - Type : {3}", itemData.getId(), itemData.getName(), itemData.getDamage(), itemData.getType()));
                        
                        if (inventoryData.setItemInventory(itemData.getId()))
                        {
                            Destroy(itemGob);
                        }
                        Debug.Log(string.Format("Inventory : \n ID : {0} {1} {2} {3} {4} ", inventoryData.getItemInventory()[0], inventoryData.getItemInventory()[1], inventoryData.getItemInventory()[2], inventoryData.getItemInventory()[3], inventoryData.getItemInventory()[4]));
                    }
                    else
                    {
                        Debug.Log("Not an item");
                    }
                }
            }
        }
    }
}    
