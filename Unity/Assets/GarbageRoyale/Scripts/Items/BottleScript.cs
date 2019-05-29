using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class BottleScript : MonoBehaviour
    {
        [SerializeField]
        public GameObject bottle;

        [SerializeField]
        public GameObject brokenBottle;

        private int countChoc;
        public bool isBroken;

        // Start is called before the first frame update
        void Start()
        {
            countChoc = 0;
            isBroken = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(!isBroken && countChoc > 3)
            {
                broke();
            }
            else
            {
                countChoc++;
            }
        }

        private void broke()
        {
            isBroken = true;
            bottle.SetActive(false);
            brokenBottle.SetActive(true);
        }
    }
}