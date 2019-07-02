using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class TransitionScript : MonoBehaviour
    {
        private GameController gc;

        private void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name.StartsWith("Player"))
            {
                int idPlayer = other.GetComponent<ExposerPlayer>().PlayerIndex;
                gc.playersActions[idPlayer].isInTransition = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.name.StartsWith("Player"))
            {
                int idPlayer = other.GetComponent<ExposerPlayer>().PlayerIndex;
                gc.playersActions[idPlayer].isInTransition = false;
            }
        }
    }
}
