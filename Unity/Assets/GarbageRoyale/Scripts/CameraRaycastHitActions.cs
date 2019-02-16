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
                foreach (var link in roomLinksList[0])
                {
                    //Debug.Log(link.Key+ " " + link.Value);
                    
                }
                Send = !Send;
            }
        }
    }
}