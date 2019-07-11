using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class SpawnMob : MonoBehaviour
    {
        
        private GameController gc;

        private GameObject Mob;
        // Start is called before the first frame update
        void Start()
        {
            /*
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            Mob = Instantiate(gc.Mob, transform.position, Quaternion.identity);
            Mob.transform.GetChild(0).GetComponent<MobStats>().id = gc.mobList.Count;
            gc.mobList.Add(gc.mobList.Count,Mob);
            */
        }
    }
}
