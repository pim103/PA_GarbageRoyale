using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class PipeScript : MonoBehaviour
    {
        private CameraRaycastHitActions ray;
        bool isBroken;

        // Start is called before the first frame update
        void Start()
        {
            ray = GameObject.Find("Controller").GetComponent<CameraRaycastHitActions>();
            isBroken = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!isBroken && ray.xTrap == (int)transform.position.x && ray.yTrap == (int)transform.position.y && ray.zTrap == (int)transform.position.z)
            {
                GameObject BrokenPipe;
                isBroken = true;

                gameObject.SetActive(false);

                BrokenPipe = ObjectPooler.SharedInstance.GetPooledObject(1);
                BrokenPipe.SetActive(true);
                BrokenPipe.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                BrokenPipe.transform.localEulerAngles = transform.localEulerAngles;
            }
        }
    }
}