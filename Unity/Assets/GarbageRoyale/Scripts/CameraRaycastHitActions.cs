using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
//UNUSED
namespace GarbageRoyale.Scripts
{
    public class CameraRaycastHitActions : MonoBehaviourPunCallbacks
    {
        public RaycastHit hitInfo;
        public bool Send = false;
        public string trapDoorToOpen = "999;999";
        public int TrapFloor = 0;
        public TypeHit type;

        public int xTrap;
        public int yTrap;
        public int zTrap;

        private Dictionary<string, string>[] roomLinksList;

        public enum TypeHit
        {
            Button,
            Pipe,
        }
        
        // Start is called before the first frame update
        void Start()
        {
            xTrap = -1;
            yTrap = -1;
            zTrap = -1;
            if (PhotonNetwork.IsMasterClient) return;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (Send)
            {
                switch(type)
                {
                    case TypeHit.Button:
                        photonView.RPC("AskTrapDoorOpening", RpcTarget.MasterClient, (int)hitInfo.transform.position.x,(int)hitInfo.transform.position.z, (int)hitInfo.transform.position.y);
                        break;
                    case TypeHit.Pipe:
                        photonView.RPC("AskBrokenPipe", RpcTarget.MasterClient, (int)hitInfo.transform.position.x, (int)hitInfo.transform.position.z, (int)hitInfo.transform.position.y);
                        break;
                    default:
                        break;
                }

                Send = false;
            }
        }

        private bool CheckTrapButton(int hitPosX, int hitPosZ, int hitPosY)
        {
            roomLinksList = GameObject.Find("Controller").GetComponent<GameController>().roomLinksList;
            //Debug.Log(hitPosX+((hitPosY/18)*16));
            foreach (var link in roomLinksList[hitPosY/18])
            {
                //Debug.Log(link.Key+ " " + link.Value);
                string[] buttonpos = link.Value.Split(';');
                int buttonZ = System.Convert.ToInt32(buttonpos[0]);
                int buttonX = System.Convert.ToInt32(buttonpos[1]);
                //Debug.Log((int)hitInfo.transform.position.x+ " " +buttonX*4);
                //Debug.Log(buttonX*4 + "=" + (hitPosX+((hitPosY/18)*16)));
                if(hitPosX == buttonX*4+((hitPosY/18)*16) && hitPosZ == buttonZ*4+((hitPosY/18)*16))
                {
                    //Debug.Log("yay" + link.Key);
                    trapDoorToOpen = link.Key;
                    TrapFloor = hitPosY/18;
                    photonView.RPC("OpenTrapDoors", RpcTarget.All, trapDoorToOpen);
                    return true;
                }

            }
            return false;
            
        }

        [PunRPC]
        public void AskTrapDoorOpening(int hitPosX, int hitPosZ, int hitPosY)
        {
            CheckTrapButton(hitPosX, hitPosZ, hitPosY);
        }

        [PunRPC]
        public void OpenTrapDoors(string toOpen)
        {
            trapDoorToOpen = toOpen;
        }

        [PunRPC]
        public void AskBrokenPipe(int hitPosX, int hitPosZ, int hitPosY)
        {
            photonView.RPC("brokePipe", RpcTarget.All, hitPosX, hitPosZ, hitPosY);
        }

        [PunRPC]
        public void brokePipe(int hitPosX, int hitPosZ, int hitPosY)
        {
            xTrap = hitPosX;
            yTrap = hitPosY;
            zTrap = hitPosZ;
        }
    }
}