using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class SpawnMob : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameObject.Find("Controller").GetComponent<GameController>().mobList.Add((int)transform.position.x+(int)transform.position.y+(int)transform.position.z,PhotonNetwork.Instantiate("Mob", transform.position, Quaternion.identity));
            }
        }
    }
}
