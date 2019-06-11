using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GarbageRoyale.Scripts
{
    public class InventoryHUDController : MonoBehaviour
    {
        private bool openInventory = false;

        // Start is called before the first frame update
        void Start()
        {
            transform.localScale = new Vector3(0f, 0f, 0f);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!openInventory)
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    openInventory = true;
                    Cursor.lockState = CursorLockMode.None;
                    
                    //gc.players[Array.IndexOf(gc.AvatarToUserId,PhotonNetwork.AuthValues.UserId)].
                }
                else
                {
                    transform.localScale = new Vector3(0f, 0f, 0f);
                    openInventory = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }
}
