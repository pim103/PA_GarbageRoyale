using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class TrapManifScript : MonoBehaviour, TrapInterface
    {
        private GameController gc;

        private int nbProj;

        private void Start()
        {
            nbProj = 5;
            gc = GameObject.Find("Controller").GetComponent<GameController>();
        }
        
        public void TriggerTrap(int idPlayer)
        {
            if(nbProj <= 0)
            {
                return;
            }

            GameObject toiletPaper = ObjectPooler.SharedInstance.GetPooledObject(11);
            toiletPaper.SetActive(true);
            toiletPaper.transform.GetChild(0).gameObject.SetActive(true);
            toiletPaper.transform.position = transform.position + Vector3.up;
            toiletPaper.transform.localScale = toiletPaper.transform.localScale / 2;
            toiletPaper.GetComponent<Item>().enabled = false;
            toiletPaper.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, -10, 2), ForceMode.Impulse);
            nbProj--;
        }
    }
}