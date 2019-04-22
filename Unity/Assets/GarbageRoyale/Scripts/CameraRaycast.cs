using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class CameraRaycast : MonoBehaviour
    {
        private CameraRaycastHitActions actionScript;
        private PlayerAttack attackScript;
        private SoundManager soundManager;
        private int openingDoorLoading;
        private bool currentlyLoading;
        private GameObject gtest;
        private Inventory playerInventory;

        // Start is called before the first frame update
        void Start()
        {
            currentlyLoading = false;
            soundManager = GameObject.Find("Controller").GetComponent<SoundManager>();
        }

        // Update is called once per frame
        void Update()
        {
            var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hitInfo;
            bool touch = Physics.Raycast(ray, out hitInfo, 2f);

            if (touch)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (hitInfo.transform.name == "otherPerso(Clone)")
                    {
                        attackScript = GameObject.Find("Controller").GetComponent<PlayerAttack>();
                        attackScript.hitPlayer(hitInfo);
                    }
                    else if (hitInfo.transform.name == "Button")
                    {
                        actionScript = GameObject.Find("Controller").GetComponent<CameraRaycastHitActions>();
                        actionScript.type = CameraRaycastHitActions.TypeHit.Button;
                        actionScript.hitInfo = hitInfo;
                        actionScript.Send = true;

                        soundManager.playSound(SoundManager.Sound.Button);
                    }
                    else if (hitInfo.transform.name == "DoorButton")
                    {
                        OpenDoorScript openDoor = hitInfo.transform.parent.GetComponent<OpenDoorScript>();
                        openDoor.openDoors(true);

                        soundManager.playSound(SoundManager.Sound.Button);
                    }
                    else if (hitInfo.transform.name == "pipe(Clone)")
                    {
                        actionScript = GameObject.Find("Controller").GetComponent<CameraRaycastHitActions>();
                        actionScript.hitInfo = hitInfo;
                        actionScript.type = CameraRaycastHitActions.TypeHit.Pipe;
                        actionScript.Send = true; ;
                        soundManager.playSound(SoundManager.Sound.Pipe);
                    }
                    if (hitInfo.transform.name == "Mob(Clone)")
                    {
                        Debug.Log("ui");
                        hitInfo.transform.GetComponent<MobStats>().takeDamage(10);
                    }
                    /*gtest = ObjectPooler.SharedInstance.GetPooledObject(0);
                    gtest.SetActive(true);
                    gtest.transform.position = hitInfo.transform.position;*/
                }

                if (hitInfo.transform.name == "LeftDoor" || hitInfo.transform.name == "RightDoor")
                {
                    OpenDoorScript openDoor = hitInfo.transform.parent.parent.GetComponent<OpenDoorScript>();
                    if (openDoor.isOpen)
                    {
                        Debug.Log("press E");
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            soundManager.playSound(SoundManager.Sound.OpeningDoor);
                            currentlyLoading = true;
                        }
    
                        if (currentlyLoading)
                        {
                            openingDoorLoading++;
                            Debug.Log("opening : " + openingDoorLoading + "%");
                            if (openingDoorLoading >= 100)
                            {
                                openingDoorLoading = 0;
                                soundManager.stopSound();
                                currentlyLoading = false;
                                soundManager.playSound(SoundManager.Sound.EndOpeningDoor);
                                openDoor.openDoors(true);
                            }
                        }
    
                        if (Input.GetKeyUp(KeyCode.E))
                        {
                            soundManager.stopSound();
                            currentlyLoading = false;
                            openingDoorLoading = 0;
                        }
                    }
                }
            } else if (openingDoorLoading > 0)
            {
                soundManager.stopSound();
                openingDoorLoading = 0;
                currentlyLoading = false;
            }
        }
    }
}    
