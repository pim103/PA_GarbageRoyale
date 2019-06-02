using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class ActiveSkillManager : MonoBehaviour
    {
        public int skillID = 0;
        public int skillType = 0;
        public int coolDown = 0;
        public int bufftime = 0;
        public int playerID = 0;
        
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(DecreaseTimers());
        }

        
        // Update is called once per frame
        void Update()
        {
        
        }
        
        private IEnumerator DecreaseTimers()
        {
            yield return new WaitForSeconds(1f);
            bufftime--;
            coolDown--;
        }
    }
}
