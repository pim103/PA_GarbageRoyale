using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class OpenDoorScript : MonoBehaviour
    {
        public int doorId;
        public bool isOpen = false;
        
        public void openDoor()
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                transform.GetChild(2).transform.Rotate(new Vector3(0,-90,0));
                transform.GetChild(3).transform.Rotate(new Vector3(0,90,0));
            }
            else
            {
                transform.GetChild(2).transform.Rotate(new Vector3(0,90,0));
                transform.GetChild(3).transform.Rotate(new Vector3(0,-90,0));
            }
        }
    }
    
    
}
