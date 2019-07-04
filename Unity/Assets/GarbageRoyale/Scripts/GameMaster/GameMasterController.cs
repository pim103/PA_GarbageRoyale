using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts.GameMaster
{
    public class GameMasterController : MonoBehaviour
    {
        [SerializeField]
        public GameObject gmGUI;

        public bool isInGMGUI = false;

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Quote))
            {
                if (!isInGMGUI)
                {
                    gmGUI.SetActive(true);
                    isInGMGUI = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    gmGUI.SetActive(false);
                    isInGMGUI = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }
}