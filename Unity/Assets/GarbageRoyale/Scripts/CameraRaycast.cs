using System.Collections;
using System.Collections.Generic;
using GarbageRoyale.Scripts.PlayerController;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class CameraRaycast : MonoBehaviourPunCallbacks
    {
        private CameraRaycastHitActions actionScript;
        private PlayerAttack attackScript;
        private SoundManager soundManager;
        private int openingDoorLoading;
        private bool currentlyLoading;
        private GameObject gtest;
        private Inventory playerInventory;

        [SerializeField]
        private GameController gc;

        public int cameraIndex = -1;

        // Start is called before the first frame update
        void Start()
        {
            currentlyLoading = false;
            openingDoorLoading = 0;
            //activatedCamera = gc.players[PlayerIndex].PlayerCamera;
            //soundManager = GameObject.Find("Controller").GetComponent<SoundManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if(!gc.endOfInit)
            {
                return;
            }

            var ray = gc.players[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hitInfo;
            bool touch = Physics.Raycast(ray, out hitInfo, 2f);

            if (touch)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (hitInfo.transform.name == "Button")
                    {
                        photonView.RPC("openTrapRPC", RpcTarget.MasterClient, gc.buttonsTrap[hitInfo.transform.parent.gameObject]);
                        /*actionScript = GameObject.Find("Controller").GetComponent<CameraRaycastHitActions>();
                        actionScript.type = CameraRaycastHitActions.TypeHit.Button;
                        actionScript.hitInfo = hitInfo;
                        actionScript.Send = true;
                        */
                        //soundManager.playSound(SoundManager.Sound.Button);
                    }
                    else if (hitInfo.transform.name == "DoorButton")
                    {
                        int doorId = hitInfo.transform.parent.GetComponent<OpenDoorScript>().doorId;
                        photonView.RPC("openDoorRPC", RpcTarget.MasterClient, doorId);
                        //OpenDoorScript openDoor = hitInfo.transform.parent.GetComponent<OpenDoorScript>();
                        //openDoor.openDoors(true);

                        //soundManager.playSound(SoundManager.Sound.Button);
                    }
                    else if (hitInfo.transform.name == "pipe")
                    {
                        int pipeId = hitInfo.transform.parent.GetComponent<PipeScript>().pipeIndex;
                        photonView.RPC("brokePipeRPC", RpcTarget.MasterClient, pipeId);

                    }
                    if (hitInfo.transform.name == "Mob(Clone)")
                    {
                        hitInfo.transform.GetComponent<MobStats>().takeDamage(10);
                    }
                    else if (hitInfo.transform.name.StartsWith("Player"))
                    {
                        int idHit = hitInfo.transform.gameObject.GetComponent<ExposerPlayer>().PlayerIndex;
                        Debug.Log(idHit);
                        /*
                        attackScript = GameObject.Find("Controller").GetComponent<PlayerAttack>();
                        attackScript.hitPlayer(hitInfo);
                        */
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
                            //soundManager.playSound(SoundManager.Sound.OpeningDoor);
                            currentlyLoading = true;
                        }
    
                        if (currentlyLoading)
                        {
                            openingDoorLoading++;
                            Debug.Log("opening : " + openingDoorLoading + "%");
                            if (openingDoorLoading >= 100)
                            {
                                openingDoorLoading = 0;
                                //soundManager.stopSound();
                                currentlyLoading = false;
                                //soundManager.playSound(SoundManager.Sound.EndOpeningDoor);
                                //openDoor.openDoor();
                                int doorId = openDoor.doorId;
                                photonView.RPC("openDoorRPC", RpcTarget.MasterClient, doorId);
                            }
                        }

                        if (Input.GetKeyUp(KeyCode.E))
                        {
                            //soundManager.stopSound();
                            currentlyLoading = false;
                            openingDoorLoading = 0;
                        }
                    }
                }
            } else if (openingDoorLoading > 0)
            {
                //soundManager.stopSound();
                openingDoorLoading = 0;
                currentlyLoading = false;
            }
        }

        [PunRPC]
        private void brokePipeRPC(int pipeId)
        {
            //TODO verify coord
            photonView.RPC("brokeSpecificPipeRPC", RpcTarget.AllBuffered, pipeId);
        }

        [PunRPC]
        private void brokeSpecificPipeRPC(int pipeId)
        {
            gc.pipes[pipeId].GetComponent<PipeScript>().brokePipe();
        }

        [PunRPC]
        private void openTrapRPC(int trapId)
        {
            //TODO verify coord
            photonView.RPC("openSpecificTrapRPC", RpcTarget.AllBuffered, trapId);
        }

        [PunRPC]
        private void openSpecificTrapRPC(int trapId)
        {
            gc.traps[trapId].transform.position += new Vector3(4, 0, 0);
        }

        [PunRPC]
        private void openDoorRPC(int doorId)
        {
            //TODO verify coord
            photonView.RPC("openSpecificDoorRPC", RpcTarget.AllBuffered, doorId);
        }

        [PunRPC]
        private void openSpecificDoorRPC(int doorId)
        {
            gc.doors[doorId].GetComponent<OpenDoorScript>().openDoor();
        }
    }
}    
