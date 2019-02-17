using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class CameraRaycast : MonoBehaviour
    {
        private CameraRaycastHitActions actionScript;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
                RaycastHit hitInfo;

                bool isHit = Physics.Raycast(ray, out hitInfo, 2f);

                if (isHit && hitInfo.transform.name == "Player(Clone)")
                {
                    PlayerAttack pa = GameObject.Find("Controller").GetComponent<PlayerAttack>();
                    pa.hitPlayer(hitInfo);
                }
                else if (isHit)
                {
                    actionScript = GameObject.Find("Controller").GetComponent<CameraRaycastHitActions>();
                    actionScript.hitInfo = hitInfo;
                    actionScript.Send = true;
                }
            }
        }
    }
}    
