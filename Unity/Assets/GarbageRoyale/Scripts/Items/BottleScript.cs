using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class BottleScript : MonoBehaviour
    {
        private int countChoc;
        public bool isBroken;
        private ItemController ic;

        // Start is called before the first frame update
        void Start()
        {
            countChoc = 0;
            isBroken = false;
            ic = GameObject.Find("Controller").GetComponent<ItemController>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(!isBroken && countChoc > 3)
            {
                ic.brokeBottle(transform.GetComponent<Item>().getId());
                isBroken = true;
            }
            else
            {
                countChoc++;
            }
        }
    }
}