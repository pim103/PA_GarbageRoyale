using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class TrapManifScript : MonoBehaviour, TrapInterface
    {
        private ItemController ic;

        private int nbProj;

        private void Start()
        {
            nbProj = 5;
            ic = GameObject.Find("Controller").GetComponent<ItemController>();
        }
        
        public void TriggerTrap(int idPlayer)
        {
            if(nbProj <= 0)
            {
                return;
            }

            ic.LaunchProjectile(transform.GetComponent<Item>().getId());
            nbProj--;
        }
    }
}