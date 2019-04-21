using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class TrapDoorCheckButton : MonoBehaviour
    {
        private string trapDoorToOpen = "999;999";
        private bool isOpen;
        private CameraRaycastHitActions ray;

        private AudioClip trapSound;

        private int trapFloor;
        // Start is called before the first frame update
        void Start()
        {
            isOpen = false;
            ray = GameObject.Find("Controller").GetComponent<CameraRaycastHitActions>();
            trapSound = GameObject.Find("Controller").GetComponent<SoundManager>().getTrapSound();
        }

        // Update is called once per frame
        void Update()
        {
            if(!isOpen)
            {
                trapDoorToOpen = ray.trapDoorToOpen;
                trapFloor = ray.TrapFloor;
                string[] trap = trapDoorToOpen.Split(';');
                int trapZ = System.Convert.ToInt32(trap[0]);
                int trapX = System.Convert.ToInt32(trap[1]);


                if ((trapZ * 4) + (trapFloor * 16) == ((int)transform.position.z) && (trapX * 4) + (trapFloor * 16) == (int)transform.position.x && trapFloor * 16 == (int)transform.position.y)
                {
                    //Debug.Log("OH GOD YES" + buttonX + " " + buttonZ);
                    //Debug.Log(((int)transform.position.y/16));
                    transform.GetChild(1).transform.position += new Vector3(4, 0, 0);
                    isOpen = true;

                    GameObject crateSound;
                    crateSound = ObjectPooler.SharedInstance.GetPooledObject(2);
                    crateSound.SetActive(true);
                    crateSound.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    crateSound.GetComponent<AudioSource>().PlayOneShot(trapSound);
                }
            }
        }
    }
}
