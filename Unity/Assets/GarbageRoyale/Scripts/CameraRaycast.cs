using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class CameraRaycast : MonoBehaviour
    {
        private CameraRaycastHitActions actionScript;
        private PlayerAttack attackScript;

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
                bool touch = Physics.Raycast(ray, out hitInfo, 2f);
                
                if (touch)
                {
                    if (hitInfo.transform.name == "Player(Clone)")
                    {
                        attackScript = GameObject.Find("Controller").GetComponent<PlayerAttack>();
                        attackScript.hitPlayer(hitInfo);
                    } else
                    {
                        actionScript = GameObject.Find("Controller").GetComponent<CameraRaycastHitActions>();
                        actionScript.hitInfo = hitInfo;
                        actionScript.Send = true;
                    }
                }
            }
        }
    }
}    
