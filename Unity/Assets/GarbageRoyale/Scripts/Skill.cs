using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class Skill : MonoBehaviour
    {
        public int id;
        public int type;
        public int bufftime;
        public int cooldown;
        // Start is called before the first frame update
        void Start()
        {
            id = 0;
            type = 3;
            bufftime = 10;
            cooldown = 20;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
