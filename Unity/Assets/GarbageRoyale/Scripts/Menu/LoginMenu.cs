using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class LoginMenu : MonoBehaviour
    {
        
        [SerializeField]
        private InputField accountMail;
        [SerializeField] 
        private InputField accountPassword;
        [SerializeField]
        private Button submitButton;
        [SerializeField]
        private Button exitButton;
        [SerializeField]
        private Button registerButton;

        [SerializeField]
        private StartGame controller;

        // Start is called before the first frame update
        void Start()
        {
            registerButton.onClick.AddListener(AskForRegistration);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void AskForRegistration()
        {
            controller.launchRegisterMenu();
        }
    }
}