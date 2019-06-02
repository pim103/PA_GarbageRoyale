using GarbageRoyale.Scripts.PrefabPlayer;
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

        [SerializeField]
        private Item item;

        [SerializeField]
        public bool isOiled;

        [SerializeField]
        public bool isBurn;

        // Start is called before the first frame update
        void Start()
        {
            countChoc = 0;
            isBroken = false;
            ic = GameObject.Find("Controller").GetComponent<ItemController>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(item.isThrow)
            {
                if (collision.transform.name.StartsWith("Player") && isOiled)
                {
                    ic.OiledPlayer(item.getId(), collision.transform.GetComponent<ExposerPlayer>().PlayerIndex);
                    isBroken = true;
                }
                else if(isBurn)
                {
                    ic.BurnSurface(item.getId());
                    isBroken = true;
                }
                else if (!isBroken && countChoc > 3)
                {
                    ic.brokeBottle(item.getId(), false, 0, 0);
                    isBroken = true;
                }
                else
                {
                    countChoc++;
                }
            }
        }
    }
}