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
    public class LoginMenu : MonoBehaviourPunCallbacks
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
        private GameObject dialogWindow;
        [SerializeField]
        private Text dialogText;
        [SerializeField]
        private GameObject dialogButton;
        [SerializeField]
        private Button dialogButtonBtn;
        
        [SerializeField]
        private Button offlineRoomButton;

        [SerializeField]
        private StartGame controller;

        public string[] httpResponse;

        // Start is called before the first frame update
        void Start()
        {
            accountPassword.inputType = InputField.InputType.Password;
            submitButton.onClick.AddListener(CallLogin);
            registerButton.onClick.AddListener(GoToRegistrationScreen);
            PhotonNetwork.AuthValues = new AuthenticationValues("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            exitButton.onClick.AddListener(AskForExit);
            dialogButtonBtn.onClick.AddListener(ConfirmationDialogBox);
            offlineRoomButton.onClick.AddListener(AskForOffline);
        }

        public void CallLogin()
        {
            dialogWindow.SetActive(true);
            //dialogText.enabled = true;
            dialogText.text = "Connexion en cours";
            StartCoroutine(Login());
        }

        IEnumerator Login()
        {
            WWWForm form = new WWWForm();
            form.AddField("accountEmail", accountMail.text);
            form.AddField("accountPassword", accountPassword.text);
            var www = UnityWebRequest.Post("https://garbage-royale.heolia.eu/services/account/logging.php", form);
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return www.SendWebRequest();
            dialogText.text = "Authentification en cours";
            yield return new WaitForSeconds(0.5f);
            if (www.responseCode == 202)
            {
                dialogText.text = "Connexion réussie";
                yield return new WaitForSeconds(0.5f);
                PhotonNetwork.ConnectUsingSettings();
                httpResponse = www.downloadHandler.text.Split('#');
                PhotonNetwork.AuthValues = new AuthenticationValues(httpResponse[1]);
                PhotonNetwork.NickName = httpResponse[0];
                //Debug.Log(httpResponse[0]);
                dialogWindow.SetActive(false);
                controller.launchMainMenu();
            }
            else if (www.responseCode == 406)
            {
                dialogButton.SetActive(true);
                dialogText.text = "Votre email ou/et votre mot de passe n'est pas valide.";
                //Debug.Log("Serveur d'authentification indisponible.");
            }
            else
            {
                dialogButton.SetActive(true);
                dialogText.text = "Une erreur est survenue. Veuillez réessayer à nouveau. Si cela ne fonctionnne toujours pas, veuillez contacter le support.";
                Debug.Log(www.downloadHandler.data);
            }
        }

        public void GoToRegistrationScreen()
        {
            controller.launchRegisterMenu();
        }

        public void ConfirmationDialogBox()
        {
            dialogWindow.SetActive(false);
            dialogButton.SetActive(false);
        }
        
        public void AskForOffline()
        {
            if(PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.CreateRoom("offlineRoom");
        }
        
        public override void OnJoinedRoom()
        {
            controller.gameController.SetActive(true);
            controller.mainCamera.enabled = false;
            controller.mainMenu.SetActive(false);
            controller.subMenu.SetActive(false);
            controller.loginMenu.SetActive(false);
        
            offlineRoomButton.interactable = false;
        }
        public void AskForExit()
        {
            Application.Quit();
        }
    }
}