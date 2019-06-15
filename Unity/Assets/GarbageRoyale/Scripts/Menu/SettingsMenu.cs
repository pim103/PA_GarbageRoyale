using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class SettingsMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Button settingsButton;
        [SerializeField] 
        private Button disconnectButton;
        [SerializeField] 
        private Button exitButton;

        [SerializeField]
        private StartGame controller;
    
        // Start is called before the first frame update
        void Start()
        {
            disconnectButton.interactable = false;
            if(!PhotonNetwork.OfflineMode)
            {
                Debug.Log(PhotonNetwork.AuthValues);
                disconnectButton.interactable = true;
            }
        }

        public void AskForExit()
        {
            Application.Quit();
        }
    }
}