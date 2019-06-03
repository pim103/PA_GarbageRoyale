using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{

    public class RopeScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject previewCube;

        [SerializeField]
        private GameObject previewCube2;

        [SerializeField]
        private Renderer material;

        [SerializeField]
        public MeshCollider mc;

        private GameController gc;
        private ItemController ic;

        public int idItem;

        public bool inEditMode;
        private bool toggleCube;
        private bool toggleEditMode;

        private bool canSetPos2;
        private bool isDeployed;

        private float ropeDistance;

        private Vector3 savePos1;
        private Vector3 savePos2;

        private Vector3 scalePreview;
        private Quaternion rotationPreview;

        private Color redColor;
        private Color greenColor;

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            ic = GameObject.Find("Controller").GetComponent<ItemController>();

            resetValue();
        }

        // Update is called once per frame
        void Update()
        {
            if (inEditMode)
            {
                toggleEditMode = true;
                var ray = gc.players[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
                RaycastHit hitInfo;
                bool touch = Physics.Raycast(ray, out hitInfo, 3f);

                if (savePos1 != Vector3.zero && savePos2 != Vector3.zero && !isDeployed)
                {
                    ic.PlaceRope(savePos1, savePos2);

                    isDeployed = true;
                }
                else if(touch)
                {
                    if (!toggleCube)
                    {
                        if (savePos1 == Vector3.zero)
                        {
                            previewCube.SetActive(true);
                        }
                        else if (savePos2 == Vector3.zero)
                        {
                            previewCube2.SetActive(true);
                        }
                        toggleCube = true;
                    }

                    if (hitInfo.transform.name != "preview")
                    {
                        if(savePos1 == Vector3.zero)
                        {
                            previewCube.transform.position = hitInfo.point;
                        }
                        else if (savePos2 == Vector3.zero && Vector3.Distance(savePos1, hitInfo.point) <= ropeDistance)
                        {
                            material.material.color = greenColor;
                            canSetPos2 = true;
                            previewCube2.transform.position = hitInfo.point;
                        }
                        else
                        {
                            material.material.color = redColor;
                            canSetPos2 = false;
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        if (savePos1 == Vector3.zero)
                        {
                            savePos1 = hitInfo.point;
                            previewCube.transform.parent = null;
                        }
                        else if (savePos2 == Vector3.zero && canSetPos2)
                        {
                            savePos2 = hitInfo.point;
                            previewCube2.transform.parent = null;
                        }
                    }
                }
                else
                {
                    if(toggleCube)
                    {
                        if (savePos1 == Vector3.zero)
                        {
                            previewCube.SetActive(false);
                        }
                        else if (savePos2 == Vector3.zero)
                        {
                            previewCube2.SetActive(false);
                        }
                        toggleCube = false;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (toggleEditMode != inEditMode)
            {
                previewCube.SetActive(false);
                previewCube2.SetActive(false);

                previewCube.transform.parent = transform;
                previewCube2.transform.parent = transform;

                previewCube.transform.localScale = scalePreview;
                previewCube2.transform.localScale = scalePreview;

                resetValue();
            }
        }

        private void resetValue()
        {
            savePos1 = Vector3.zero;
            savePos2 = Vector3.zero;
            inEditMode = false;
            toggleCube = false;
            toggleEditMode = false;

            canSetPos2 = false;
            isDeployed = false;

            ropeDistance = 8.0f;

            scalePreview = previewCube.transform.localScale;
            rotationPreview = previewCube.transform.rotation;

            redColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            greenColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.name.StartsWith("Player"))
            {
                Debug.Log("TU TOMBES");
            }
        }
    }
}