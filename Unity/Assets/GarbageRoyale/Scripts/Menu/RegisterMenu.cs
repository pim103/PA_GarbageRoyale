using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class RegisterMenu : MonoBehaviour
    {
        
        [SerializeField]
        private InputField accountMail;
        [SerializeField] 
        private InputField accountPassword;
        [SerializeField] 
        private InputField accountPasswordConfirmation;
        [SerializeField]
        private Button submitButton;
        [SerializeField]
        private Button exitButton;

        [SerializeField]
        private StartGame controller;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}

