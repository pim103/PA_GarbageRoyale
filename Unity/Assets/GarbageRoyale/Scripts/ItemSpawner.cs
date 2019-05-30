using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GarbageRoyale.Scripts
{
    public class ItemSpawner : MonoBehaviourPunCallbacks
    {
        private int spawnerId;
        private GameObject itemGob;
        private Item itemSelf;

        private bool isSpawned = false;

        [SerializeField]
        private GameObject spawnPosition;
 
        public int itemType;

        public RoomInfo gameInfo;

        private GameController gc;

        private void Start()
        {
            //initItems();
        }
        
        public void initItems()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            //Debug.Log(info);
            //itemType = 1; //Random.Range(1, 4);

            itemGob = Instantiate(
                gc.itemList[itemType], 
                new Vector3(spawnPosition.transform.position.x, spawnPosition.transform.position.y + 0.7f, spawnPosition.transform.position.z), 
                Quaternion.identity
            );

            //itemGob.name = "Staff_" + _spawnerItems.transform.position.x + "_" + ((int)_spawnerItems.transform.position.y + 1) + "_" + _spawnerItems.transform.position.z;
            gc.items.Add(gc.nbItems, itemGob);
            itemGob.GetComponent<Item>().setId(gc.nbItems);
            gc.nbItems++;
            //itemGob.AddComponent<Item>();
            itemSelf = itemGob.GetComponent<Item>();
            //itemSelf.type = itemType;
            /*if (itemType == 2)
            {
                itemGob.GetComponent<Material>().CopyPropertiesFromMaterial(itemMaterial);
            }*/

        }
    }
}