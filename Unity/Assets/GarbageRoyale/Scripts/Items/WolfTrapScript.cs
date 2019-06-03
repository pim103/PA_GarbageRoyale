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
        private PreviewItemScript scriptPreview;

        private GameController gc;
        private ItemController ic;

        public bool inEditMode;
        private bool toggleEditMode;

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            ic = GameObject.Find("Controller").GetComponent<ItemController>();
            inEditMode = false;
            toggleEditMode = false;
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
                    //ic.PlaceMetalSheet(scriptPreview.savePos);
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
    }
}