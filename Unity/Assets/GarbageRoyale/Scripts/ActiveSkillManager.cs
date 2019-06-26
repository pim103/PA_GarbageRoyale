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
        public int skillPlace = 0;
        public bool isActive = true;

        private void Start()
        {
            StartCoroutine(DecreaseTimers());
        }

        private IEnumerator DecreaseTimers()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (bufftime > 0)
                {
                    bufftime--;
                }

                if (coolDown>0)
                {
                    coolDown--;
                }
            }
        }
    }
}
