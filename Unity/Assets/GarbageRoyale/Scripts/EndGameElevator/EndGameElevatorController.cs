using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.EndGameElevator
{
    public class EndGameElevatorController : MonoBehaviour
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
