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
        private GameObject previewCube;

        private GameController gc;
        private ItemController ic;

        private Vector3 scalePreview;

        public int idItem;

        public bool inEditMode;
        private bool toggleCube;
        private bool toggleEditMode;

        private void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            ic = GameObject.Find("Controller").GetComponent<ItemController>();

            inEditMode = false;
            toggleCube = false;
            toggleEditMode = false;
            scalePreview = previewCube.transform.localScale;
        }

        private void Update()
        {
            if (inEditMode)
            {
                toggleEditMode = true;
                var ray = gc.players[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
                RaycastHit hitInfo;
                bool touch = Physics.Raycast(ray, out hitInfo, 3f);

                if (touch)
                {
                    if (!toggleCube)
                    {
                        previewCube.transform.parent = null;
                        previewCube.transform.localEulerAngles = new Vector3(90.0f, 0f, 0f);
                        previewCube.transform.localScale = new Vector3(3, 3, 0.04f);
                        previewCube.SetActive(true);
                        toggleCube = true;
                    }

                    if (hitInfo.transform.name != "preview")
                    {
                        previewCube.transform.position = hitInfo.point;
                    }

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        ic.PlaceMetalSheet(hitInfo.point);
                        inEditMode = false;
                        previewCube.SetActive(false);
                    }
                }
                else
                {
                    if (toggleCube)
                    {
                        previewCube.SetActive(false);
                        toggleCube = false;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (toggleEditMode != inEditMode)
            {
                previewCube.transform.parent = transform;
                previewCube.transform.localScale = scalePreview;
                previewCube.transform.localEulerAngles = Vector3.zero;
                toggleEditMode = false;
                previewCube.SetActive(false);
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