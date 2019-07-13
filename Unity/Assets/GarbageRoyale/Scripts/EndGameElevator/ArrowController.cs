using UnityEngine;

namespace GarbageRoyale.Scripts.EndGameElevator
{
    public class ArrowController : MonoBehaviour
    {
        private EndGameElevatorController eec;

        private bool hasBeenFound = false;
        private void Start()
        {
            eec = GameObject.Find("Controller").GetComponent<EndGameElevatorController>();
        }

        void Update()
        {
            foreach (var room in eec.endroomList)
            {
                if (room.transform.position.y-transform.position.y < 4 && room.transform.position.y-transform.position.y > -4)
                {
                    transform.LookAt(room.transform);
                }
                else
                {
                    if (transform.position.y > 7 * 16)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
