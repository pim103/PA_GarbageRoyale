using System;
using System.Collections;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Networking;
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
            accountPassword.inputType = InputField.InputType.Password;
            submitButton.onClick.AddListener(CallLogin);
            registerButton.onClick.AddListener(GoToRegistrationScreen);
        }

        public void CallLogin()
        {
            StartCoroutine(Login());
        }

        IEnumerator Login()
        {
            WWWForm form = new WWWForm();
            form.AddField("accountEmail", accountMail.text);
            form.AddField("accountPassword", accountPassword.text);
            var www = UnityWebRequest.Post("http://garbage-royale.heolia.eu/services/account/logging.php", form);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.responseCode);
                Debug.Log(www.downloadHandler.text);
                if (www.responseCode == 202)
                {
                    PhotonNetwork.ConnectUsingSettings();
                    PhotonNetwork.AuthValues = new AuthenticationValues(www.downloadHandler.text);
                    controller.launchMainMenu();
                }
            }
        }

        public void GoToRegistrationScreen()
        {
            controller.launchRegisterMenu();
        }
    }
}