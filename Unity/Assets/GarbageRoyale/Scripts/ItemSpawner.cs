using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GarbageRoyale.Scripts
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] 
        private GameObject itemPrefab;
        
        [SerializeField]
        private Material itemMaterial;
        [SerializeField] 
        private GameObject _spawnerItems;
        
        private GameObject itemGob;
        private Item itemSelf;
           
        
        // Start is called before the first frame update
        void Start()
        {
            itemGob = Instantiate(itemPrefab, new Vector3(_spawnerItems.transform.position.x, _spawnerItems.transform.position.y + 0.7f, _spawnerItems.transform.position.z), Quaternion.identity);
            
            itemSelf = itemGob.GetComponent<Item>().initItem(itemGob.GetComponent<Item>().getType());
            switch (itemSelf.getType())
            {
                case 1:                    
                    itemGob.GetComponent<Renderer>().material = itemMaterial;
                    itemMaterial.shader = Shader.Find("_Color");
                    itemMaterial.SetColor("_Color", new Color(156, 74, 0, 255));
                    break;
                case 2:
                    itemMaterial.SetColor("_Color", new Color(100, 100, 100, 255));
                    itemGob.GetComponent<Renderer>().material = itemMaterial;
                    break;
                default:
                    itemGob.GetComponent<Renderer>().material = itemMaterial;
                    break; 
            }

            Debug.Log(itemSelf.getName());
            /*itemSelf = itemSelf.initItem(itemPrefab.GetComponent<Item>().getType());
            Debug.Log(itemSelf.getName());*/
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
