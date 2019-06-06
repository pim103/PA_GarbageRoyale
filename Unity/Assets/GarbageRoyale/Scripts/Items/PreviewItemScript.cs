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
        public Vector3 bonusColliderSize;

        [SerializeField]
        public Item item;

        private ItemController ic;
        private GameController gc;

        private bool toggleCube;
        
        //Global
        public bool inEditMode;
        private bool toggleEditMode;

        //Trap
        [SerializeField]
        public GameObject previewCube;

        [SerializeField]
        public Vector3 rotation;

        [SerializeField]
        private bool keepRotation;

        public Vector3 scalePreview;
        public Vector3 savePos;
        public Vector3 saveRot;

        public bool canPose;

        //Rope
        [SerializeField]
        public bool uniqueRope;
        [SerializeField]
        public bool withRope;
        [SerializeField]
        public bool initNewRope;

        private float ropeDistance;
        private bool canSetRopePos2;
        private bool ropeIsPlaced;

        public Vector3 savePosRope;
        public Vector3 savePosRope2;

        [SerializeField]
        private GameObject previewRope1;

        [SerializeField]
        private GameObject previewRope2;

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            ic = GameObject.Find("Controller").GetComponent<ItemController>();
            int idPlayer = System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId);

            savePos = Vector3.zero;
            savePosRope = Vector3.zero;
            savePosRope2 = Vector3.zero;

            inEditMode = false;
            canPose = false;
            toggleCube = false;
            toggleEditMode = false;

            if(!uniqueRope)
            {
                scalePreview = previewCube.transform.localScale;
            }

            ropeDistance = 8.0f;
            canSetRopePos2 = false;
            ropeIsPlaced = false;
        }

        private void Update()
        {

            if (inEditMode)
            {
                toggleEditMode = true;
                var ray = gc.players[idPlayer].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
                RaycastHit hitInfo;
                bool touch = Physics.Raycast(ray, out hitInfo, 3f);
                canPose = false;

                if (withRope && !ropeIsPlaced)
                {
                    PreviewRope(touch, hitInfo);
                }
                else if((withRope && ropeIsPlaced && !uniqueRope) || !withRope)
                {
                    Debug.Log("Placer objet");
                    PreviewTrap(touch, hitInfo);
                }

                if ((canPose && Input.GetKeyDown(KeyCode.Mouse0)) || (uniqueRope && ropeIsPlaced))
                {
                    if(withRope)
                    {
                        ic.PlaceRope(savePosRope, savePosRope2, initNewRope);
                    }
                    
                    if(!uniqueRope)
                    {
                        ic.PlaceObject(savePos, saveRot, item.type);
                        inEditMode = false;
                        DesactivePreview();
                    }
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
            ropeIsPlaced = false;
            canPose = false;
            canSetRopePos2 = false;
            toggleCube = false;

            if (!uniqueRope)
            {
                previewCube.transform.parent = transform;
                previewCube.transform.localScale = scalePreview;
                previewCube.transform.localEulerAngles = Vector3.zero;
                previewCube.SetActive(false);
            }

            if(withRope)
            {
                savePosRope = Vector3.zero;
                savePosRope2 = Vector3.zero;

                previewRope1.transform.parent = transform;
                previewRope2.transform.parent = transform;

                previewRope1.transform.localScale = Vector3.one / 3;
                previewRope2.transform.localScale = Vector3.one / 3;

                previewRope1.transform.localEulerAngles = Vector3.zero;
                previewRope2.transform.localEulerAngles = Vector3.zero;

                previewRope1.SetActive(false);
                previewRope2.SetActive(false);
            }
        }

        private void PreviewTrap(bool touch, RaycastHit hitInfo)
        {
            if (touch)
            {
                canPose = true;
                if (!toggleCube)
                {
                    Debug.Log("Active toi ?");
                    previewCube.transform.parent = null;
                    previewCube.transform.localScale = scalePreview;
                    previewCube.SetActive(true);
                    toggleCube = true;
                }

                if (hitInfo.transform.name != "preview")
                {
                    if (keepRotation)
                    {
                        previewCube.transform.localEulerAngles = new Vector3(rotation.x, rotation.y + gc.players[idPlayer].PlayerGameObject.transform.localEulerAngles.y, rotation.z);
                    }
                    else
                    {
                        previewCube.transform.localEulerAngles = rotation;
                    }
                    savePos = hitInfo.point;
                    saveRot = previewCube.transform.localEulerAngles;
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
        }

        private void PreviewRope(bool touch, RaycastHit hitInfo)
        {
            if(touch)
            {
                if (!toggleCube)
                {
                    if (savePosRope == Vector3.zero)
                    {
                        previewRope1.transform.parent = null;
                        previewRope1.SetActive(true);
                    }
                    else if (savePosRope2 == Vector3.zero)
                    {
                        previewRope2.transform.parent = null;
                        previewRope2.SetActive(true);
                    }
                    toggleCube = true;
                }

                if (hitInfo.transform.name != "preview")
                {
                    if (savePosRope == Vector3.zero)
                    {
                        previewRope1.transform.position = hitInfo.point;
                    }
                    else if (savePosRope2 == Vector3.zero && Vector3.Distance(savePosRope, hitInfo.point) <= ropeDistance)
                    {
                        canSetRopePos2 = true;
                        previewRope2.transform.position = hitInfo.point;
                    }
                    else
                    {
                        canSetRopePos2 = false;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (savePosRope == Vector3.zero)
                    {
                        savePosRope = hitInfo.point;
                    }
                    else if (savePosRope2 == Vector3.zero && canSetRopePos2)
                    {
                        savePosRope2 = hitInfo.point;
                        ropeIsPlaced = true;
                    }
                }
            }
            else
            {
                if (toggleCube)
                {
                    if (savePosRope == Vector3.zero)
                    {
                        previewRope1.SetActive(false);
                    }
                    else if (savePosRope2 == Vector3.zero)
                    {
                        previewRope2.SetActive(false);
                    }
                    toggleCube = false;
                }
            }
        }
    }
}