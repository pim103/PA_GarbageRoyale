using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
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
        private GameController gc;

        [SerializeField]
        private Item item;

        [SerializeField]
        public bool isOiled;

        [SerializeField]
        public bool isBurn;

        [SerializeField]
        public bool isElec;

        // Start is called before the first frame update
        void Start()
        {
            countChoc = 0;
            isBroken = false;
            GameObject controller = GameObject.Find("Controller");
            ic = controller.GetComponent<ItemController>();
            gc = controller.GetComponent<GameController>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if(item.isThrow)
            {
                bool isPlayer = collision.transform.name.StartsWith("Player");

                if (isPlayer && isOiled)
                {
                    ic.OiledPlayer(item.getId(), collision.transform.GetComponent<ExposerPlayer>().PlayerIndex);
                    isBroken = true;
                }
                else if(isBurn)
                {
                    ic.BurnSurface(item.getId());
                    isBroken = true;
                }
                else if (isElec)
                {
                    bool inWater = (collision.transform.position.y <= gc.water.waterObject.transform.position.y + 0.2f);
                    int idPlayer = -1;

                    if(isPlayer)
                    {
                        idPlayer = collision.transform.GetComponent<ExposerPlayer>().PlayerIndex;
                        Debug.Log(idPlayer);
                    }

                    if(idPlayer != -1 || inWater)
                    {
                        ic.BrokeElectof(item.getId(), idPlayer, inWater);
                    }
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