using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class BottleScript : MonoBehaviour
    {
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
            GameController gc = GameObject.Find("Controller").GetComponent<GameController>();
            isBroken = true;
            gameObject.SetActive(false);
            GameObject brokenBottle = ObjectPooler.SharedInstance.GetPooledObject(4);
            brokenBottle.transform.position = transform.position;
            brokenBottle.transform.rotation = transform.rotation;
            brokenBottle.SetActive(true);
            brokenBottle.GetComponent<Item>().id = gc.items.Count;

            gc.items.Add(gc.items.Count, brokenBottle);
        }
    }
}