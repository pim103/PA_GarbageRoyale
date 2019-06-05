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
        public BoxCollider bc;

        [SerializeField]
        private AudioSource sound;

        [SerializeField]
        private PreviewItemScript scriptPreview;

        private GameController gc;
        private ItemController ic;

        public bool inEditMode;
        private bool toggleEditMode;

        private void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            ic = GameObject.Find("Controller").GetComponent<ItemController>();

            inEditMode = false;
            toggleEditMode = false;
        }

        private void Update()
        {
            if (inEditMode)
            {
                toggleEditMode = true;
                bool canPose = scriptPreview.SeePreview();

                if (canPose && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    ic.PlaceMetalSheet(scriptPreview.savePos);
                    inEditMode = false;
                    scriptPreview.DesactivePreview();
                }
            }
        }

        private void FixedUpdate()
        {
            if (toggleEditMode != inEditMode)
            {
                scriptPreview.DesactivePreview();
                toggleEditMode = false;
            }
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