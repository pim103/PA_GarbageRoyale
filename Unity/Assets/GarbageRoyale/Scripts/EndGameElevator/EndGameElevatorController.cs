using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts.EndGameElevator
{
    public class EndGameElevatorController : MonoBehaviourPunCallbacks
    {
        public List<GameObject> endroomList = new List<GameObject>();
        private GameController gc;
        private bool hasBeenInit = false;
        private void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
        }

        public void InitEndPhase()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("InitAll",RpcTarget.All);
            }
        }
        
        [PunRPC]
        public void InitAll()
        {
            if (!hasBeenInit)
            {
                foreach (var room in endroomList)
                {
                    room.GetComponent<EndRoomScript>().light1.gameObject.SetActive(true);
                    room.GetComponent<EndRoomScript>().light2.gameObject.SetActive(true);
                }

                foreach (var player in gc.players)
                {
                    player.EndElevatorArrow.SetActive(true);
                }

                hasBeenInit = true;
            }
        }
    }
}
