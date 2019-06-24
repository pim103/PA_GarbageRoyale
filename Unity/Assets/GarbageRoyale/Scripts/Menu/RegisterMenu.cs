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
        
        [SerializeField]
        private GameObject nameError;
        [SerializeField]
        private GameObject emailError;
        [SerializeField]
        private GameObject passError;
        [SerializeField]
        private GameObject confError;

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
                Debug.Log(www.downloadHandler.text);
                DisplayErrors(www.downloadHandler.text);
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
        public void DisplayErrors(string errors)
        {
            nameError.SetActive(false);
            emailError.SetActive(false);
            passError.SetActive(false);
            confError.SetActive(false);
            if (errors.IndexOf("1") != -1)
            {
                nameError.SetActive(true);
            }
            if (errors.IndexOf("2") != -1)
            {
                emailError.SetActive(true);
            }
            if (errors.IndexOf("3") != -1)
            {
                passError.SetActive(true);
            }
            if (errors.IndexOf("4") != -1)
            {
                confError.SetActive(true);
            }
        }
    }
}

