using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class PreviewItemScript : MonoBehaviour
    {
        public int idPlayer;
        
        [SerializeField]
        public BoxCollider bc;

        [SerializeField]
        public GameObject previewCube;

        [SerializeField]
        public Vector3 rotation;
        
        [SerializeField]
        public Vector3 bonusColliderSize;

        [SerializeField]
        public Item item;

        private ItemController ic;
        private GameController gc;

        private bool toggleCube;

        public bool canPose;
        public Vector3 scalePreview;

        public Vector3 savePos;
        
        public bool inEditMode;
        private bool toggleEditMode;

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            ic = GameObject.Find("Controller").GetComponent<ItemController>();
            int idPlayer = System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId);

            inEditMode = false;
            canPose = false;
            toggleCube = false;
            toggleEditMode = false;
            scalePreview = previewCube.transform.localScale;
        }

        private void Update()
        {
            var ray = gc.players[idPlayer].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hitInfo;
            bool touch = Physics.Raycast(ray, out hitInfo, 3f);
            canPose = false;

            if (inEditMode)
            {
                toggleEditMode = true;

                if (touch)
                {
                    canPose = true;
                    if (!toggleCube)
                    {
                        previewCube.transform.parent = null;
                        previewCube.transform.localEulerAngles = rotation;
                        previewCube.transform.localScale = scalePreview;
                        previewCube.SetActive(true);
                        toggleCube = true;
                    }

                    if (hitInfo.transform.name != "preview")
                    {
                        savePos = hitInfo.point;
                        previewCube.transform.position = hitInfo.point;
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

                if (canPose && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    ic.PlaceObject(savePos, item.type);
                    inEditMode = false;
                    DesactivePreview();
                    Debug.Log("On le pose");
                }
            }
        }
        
        private void FixedUpdate()
        {
            if (toggleEditMode != inEditMode)
            {
                DesactivePreview();
                toggleEditMode = false;
            }
        }

        public void DesactivePreview()
        {
            previewCube.transform.parent = transform;
            previewCube.transform.localScale = scalePreview;
            previewCube.transform.localEulerAngles = Vector3.zero;
            previewCube.SetActive(false);
        }
    }
}