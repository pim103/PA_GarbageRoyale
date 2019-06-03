using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class PreviewItemScript : MonoBehaviour
    {
        public int nbPreview;

        public int idPlayer;

        [SerializeField]
        public GameObject previewCube;

        [SerializeField]
        public Vector3 rotation;

        private GameController gc;

        private bool toggleCube;

        public bool canPose;
        private Vector3 scalePreview;

        public Vector3 savePos;

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            int idPlayer = System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId);

            canPose = true;
            toggleCube = false;
            scalePreview = previewCube.transform.localScale;
        }

        // Update is called once per frame
        public bool SeePreview()
        {
            var ray = gc.players[idPlayer].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hitInfo;
            bool touch = Physics.Raycast(ray, out hitInfo, 3f);

            if(nbPreview == 1)
            {
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
            }

            return canPose;
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