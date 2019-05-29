using System.Collections;
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
            submitButton.onClick.AddListener(CallRegister);
        }

        public void CallRegister()
        {
            StartCoroutine(Register());
        }

        IEnumerator Register()
        {
            WWWForm form = new WWWForm();
            form.AddField("accountName", accountName.text);
            form.AddField("accountMail", accountMail.text);
            form.AddField("accountPassword", accountPassword.text);
            form.AddField("accountPasswordConfirmation", accountPasswordConfirmation.text);
            WWW www = new WWW("http://garbagebr.lan/services/account/insert.php");
            yield return www;
            if (www.text == "0")
            {
                Debug.Log("Account created!");
            }
            else
            {
                Debug.Log(www);
            }
        }
    }
}

