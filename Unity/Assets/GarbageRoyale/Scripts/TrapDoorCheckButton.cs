using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class TrapDoorCheckButton : MonoBehaviour
    {
        private string trapDoorToOpen = "999;999";

        private int trapFloor;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            trapDoorToOpen = GameObject.Find("Controller").GetComponent<CameraRaycastHitActions>().trapDoorToOpen;
            trapFloor = GameObject.Find("Controller").GetComponent<CameraRaycastHitActions>().TrapFloor;
            string[] trap = trapDoorToOpen.Split(';');
            int trapZ = System.Convert.ToInt32(trap[0]);
            int trapX = System.Convert.ToInt32(trap[1]);

            
            if ((trapZ * 4) +(trapFloor*16) == ((int)transform.position.z) && (trapX * 4) +(trapFloor*16) == (int)transform.position.x && trapFloor*16 == (int)transform.position.y)
            {
                //Debug.Log("OH GOD YES" + buttonX + " " + buttonZ);
                Debug.Log(((int)transform.position.y/16));
                transform.GetChild(1).transform.position += new Vector3(4,0,0);
                trapDoorToOpen = "999;999";
            }
        }
    }
}
