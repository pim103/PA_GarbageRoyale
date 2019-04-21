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
        private GameObject itemPrefab;
        [SerializeField]
        private Material itemMaterial;
        [SerializeField] 
        private GameObject _spawnerItems;
        [SerializeField] 
        private int itemType;
        public RoomInfo gameInfo;

        private void Start()
        {
            initItems();
        }
        
        public void initItems()
        {
            //Debug.Log(info);
            //itemType = 1; //Random.Range(1, 4);
            itemGob = Instantiate(itemPrefab, new Vector3(_spawnerItems.transform.position.x, _spawnerItems.transform.position.y + 0.7f, _spawnerItems.transform.position.z), Quaternion.identity);
            itemGob.name = "Staff_" + _spawnerItems.transform.position.x + "_" + ((int)_spawnerItems.transform.position.y + 1) + "_" + _spawnerItems.transform.position.z;
            //itemGob.AddComponent<Item>();
            itemSelf = itemGob.GetComponent<Item>();
            itemSelf.initItem(itemType);

            /*if (itemType == 2)
            {
                itemGob.GetComponent<Material>().CopyPropertiesFromMaterial(itemMaterial);
            }*/

        }
    }
}