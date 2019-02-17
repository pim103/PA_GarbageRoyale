using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class CameraRaycastHitActions : MonoBehaviourPunCallbacks
    {
        public RaycastHit hitInfo;
        public bool Send = false;
        private Dictionary<string, string>[] roomLinksList;
        
        // Start is called before the first frame update
        void Start()
        {
            if (PhotonNetwork.IsMasterClient) return;
            
        }

        // Update is called once per frame
        void Update()
        {
            if (Send)
            {
                roomLinksList = GameObject.Find("Controller").GetComponent<GameController>().roomLinksList;
                Debug.Log(hitInfo.transform.name + " ");
                if (hitInfo.transform.GetComponent<Item>())
                {
                    GameObject script = hitInfo.transform.gameObject;
                    Item itemData = script.GetComponent<Item>();
                    Inventory inventoryData = script.GetComponent<Inventory>();
                    
                    Debug.Log(string.Format("Item : \n ID : {0} - Name: {1} - Damage : {2} - Type : {3}", itemData.getId(), itemData.getName(), itemData.getDamage(), itemData.getType()));
                    inventoryData.setItemInventory(itemData.getId());
                    Debug.Log(string.Format("Inventory : \n ID : {0} {1} {2} {3} {4} ", inventoryData.getItemInventory()[0], inventoryData.getItemInventory()[1], inventoryData.getItemInventory()[2], inventoryData.getItemInventory()[3], inventoryData.getItemInventory()[4]));
                }
                foreach (var link in roomLinksList[0])
                {
                    //Debug.Log(link.Key+ " " + link.Value);
                    
                }
                Send = !Send;
            }
        }
    }
}