using GarbageRoyale.Scripts.PrefabPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class OilScript : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem particles;

        bool isTrigger;
        private GameController gc;
        public int nbOil;

        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

        private void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            isTrigger = false;
            nbOil = 3;
        }

        private void OnParticleCollision(GameObject other)
        {
            ParticlePhysicsExtensions.GetCollisionEvents(particles, other, collisionEvents);

            /*
            if(!isTrigger)
            {
                if(other.name.StartsWith("Player"))
                {
                    int id = other.transform.GetComponent<ExposerPlayer>().PlayerIndex;
                    if(!gc.playersActions[id].isBurning)
                    {
                        gc.playersActions[id].isOiled = true;
                        gc.playersActions[id].timeLeftOiled = 15.0f;
                    }
                    else
                    {
                        gc.playersActions[id].timeLeftBurn = 5.0f;
                    }
                }
                else if(other.name != "jerrican")
                {
                    GameObject oil = ObjectPooler.SharedInstance.GetPooledObject(0);
                    oil.SetActive(true);
                    oil.transform.position = collisionEvents[0].intersection;
                    isTrigger = true;
                }
            }
            */
        }
    }
}
