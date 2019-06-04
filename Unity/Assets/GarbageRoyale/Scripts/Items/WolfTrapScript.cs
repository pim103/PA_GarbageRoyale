using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class WolfTrapScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject leftPanel;

        [SerializeField]
        private GameObject rightPanel;

        [SerializeField]
        private GameObject rope;

        [SerializeField]
        private GameObject centerRotation;

        [SerializeField]
        private Rigidbody rigid;

        [SerializeField]
        public BoxCollider bc;

        [SerializeField]
        private PreviewItemScript scriptPreview;

        private GameController gc;
        private ItemController ic;

        public bool inEditMode;
        private bool toggleEditMode;

        private bool isTrigger;

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            ic = GameObject.Find("Controller").GetComponent<ItemController>();
            inEditMode = false;
            toggleEditMode = false;
            isTrigger = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (inEditMode)
            {
                toggleEditMode = true;
                bool canPose = scriptPreview.SeePreview();

                if (canPose && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    ic.PlaceWolfTrap(scriptPreview.savePos);
                    inEditMode = false;
                    rigid.useGravity = false;
                    rigid.isKinematic = true;
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

            Debug.Log("Enter : " + other.name);
            if (!isTrigger && other.name.StartsWith("Player"))
            {
                int idPlayer = other.GetComponent<ExposerPlayer>().PlayerIndex;

                isTrigger = true;
                leftPanel.transform.localEulerAngles = new Vector3(-15, 90, -90);
                rightPanel.transform.localEulerAngles = new Vector3(-165, 90, -90);
                StartCoroutine(trapPlayer(idPlayer));
            }
        }

        private IEnumerator trapPlayer(int id)
        {
            gc.playersActions[id].isTrap = true;
            gc.players[id].PlayerStats.takeDamage(10.0f);

            yield return new WaitForSeconds(3.0f);
            gc.playersActions[id].isTrap = false;

            leftPanel.transform.localEulerAngles = new Vector3(-90, 90, -90);
            rightPanel.transform.localEulerAngles = new Vector3(-90, 90, -90);
            isTrigger = false;
        }
    }
}