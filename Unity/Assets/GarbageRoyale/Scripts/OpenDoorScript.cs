using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class OpenDoorScript : MonoBehaviour
    {
        private DoorController doorController;
        public bool isOpen = false;
        private int doorToOpenByOtherX;
        private int doorToOpenByOtherZ;
        
        private void Start()
        {
            doorController = GameObject.Find("Controller").GetComponent<DoorController>();
        }
        private void Update()
        {
            if (doorController.doorX == (int)transform.position.x && doorController.doorZ == (int)transform.position.z)
            {
                openDoors(false);
                doorController.SetDoorToOpenByOther(999,999);
            }
        }

        public void openDoors(bool sendToOthers)
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                transform.GetChild(2).transform.Rotate(new Vector3(0,-90,0));
                transform.GetChild(3).transform.Rotate(new Vector3(0,90,0));
                if(sendToOthers) doorController.SetDoorToOpen((int)transform.position.x + ";" + (int)transform.position.z);
            }
            else
            {
                transform.GetChild(2).transform.Rotate(new Vector3(0,90,0));
                transform.GetChild(3).transform.Rotate(new Vector3(0,-90,0));
                if(sendToOthers) doorController.SetDoorToOpen((int)transform.position.x + ";" + (int)transform.position.z);
            }
        }
    }
    
    
}
