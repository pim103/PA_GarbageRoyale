using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class RegisterMenu : MonoBehaviour
    {
        [SerializeField]
        private InputField accountName;
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

        void Start()
        {
            accountPassword.inputType = InputField.InputType.Password;
            accountPasswordConfirmation.inputType = InputField.InputType.Password;
            submitButton.onClick.AddListener(CallRegister);
            exitButton.onClick.AddListener(ReturnToLoginScreen);
        }

        public void CallRegister()
        {
            StartCoroutine(Register());
        }

        public void ReturnToLoginScreen()
        {
            controller.launchLoginMenu();
        }

        IEnumerator Register()
        {
            WWWForm form = new WWWForm();
            form.AddField("accountName", accountName.text);
            form.AddField("accountMail", accountMail.text);
            form.AddField("accountPassword", accountPassword.text);
            form.AddField("accountPasswordConfirmation", accountPasswordConfirmation.text);
            var www = UnityWebRequest.Post("http://garbage-royale.heolia.eu/services/account/insert.php", form);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.responseCode);
                Debug.Log(www.downloadHandler.text);
                if (www.responseCode == 201)
                {
                    ReturnToLoginScreen();
                }
            }
        }
    }
}

