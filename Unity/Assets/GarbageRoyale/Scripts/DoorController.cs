using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class DoorController : MonoBehaviourPunCallbacks
    {
        public string doorToOpen = "999;999";
        public int doorX=999;
        public int doorZ=999;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (doorToOpen != "999;999")
            {
                photonView.RPC("OpenByOtherPlayer", RpcTarget.Others, doorToOpen);
                doorToOpen = "999;999";
            }
        }

        public void SetDoorToOpen(string door)
        {
            doorToOpen = door;
        }
        
        public void SetDoorToOpenByOther(int newdoorX, int newdoorZ)
        {
            doorX = newdoorX;
            doorZ = newdoorZ;
        }
    
        [PunRPC]
        public void OpenByOtherPlayer(string door)
        {
            string[] doorCoords = door.Split(';');
            doorZ = System.Convert.ToInt32(doorCoords[0]);
            doorX = System.Convert.ToInt32(doorCoords[1]);
        }
    }
}
