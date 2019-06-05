using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{

    public class MetalSheetScript : MonoBehaviour
    {
        [SerializeField]
        private AudioSource sound;

        [SerializeField]
        private PreviewItemScript scriptPreview;

        private GameController gc;

        private void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (other.name.StartsWith("Player"))
            {
                int idPlayer = other.GetComponent<ExposerPlayer>().PlayerIndex;
                gc.playersActions[idPlayer].isOnMetalSheet = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (other.name.StartsWith("Player"))
            {
                int idPlayer = other.GetComponent<ExposerPlayer>().PlayerIndex;
                gc.playersActions[idPlayer].isOnMetalSheet = false;
            }
        }
    }
}