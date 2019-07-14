using System;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
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

        private GameController gc;

        public string[] httpResponse;
        private bool findFirstSelectable = false;

        // Start is called before the first frame update
        void Start()
        {
            accountPassword.inputType = InputField.InputType.Password;
            accountMail.text = LoginSave.LoadMailLogin();
            submitButton.onClick.AddListener(CallLogin);
            registerButton.onClick.AddListener(GoToRegistrationScreen);
            PhotonNetwork.AuthValues = new AuthenticationValues("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            exitButton.onClick.AddListener(AskForExit);
            dialogButtonBtn.onClick.AddListener(ConfirmationDialogBox);
            offlineRoomButton.onClick.AddListener(AskForOffline);
        }
        
        public void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (EventSystem.current != null)
                {
                    GameObject selected = EventSystem.current.currentSelectedGameObject;
                    
                    if(selected == null && findFirstSelectable)
                    {
                        Selectable found = (Selectable.allSelectables.Count > 0) ? Selectable.allSelectables[0] : null;

                        if(found != null)
                        {
                            selected = found.gameObject;
                        }
                    }

                    if (selected != null)
                    {
                        Selectable current = (Selectable)selected.GetComponent("Selectable");

                        if (current != null)
                        {
                            Selectable nextDown = current.FindSelectableOnDown();
                            Selectable nextUp = current.FindSelectableOnUp();
                            Selectable nextRight = current.FindSelectableOnRight();
                            Selectable nextLeft = current.FindSelectableOnLeft();

                            if(nextDown != null)
                            {
                                nextDown.Select();
                            }
                            else if (nextRight != null)
                            {
                                nextRight.Select();
                            }
                            else if (nextUp != null)
                            {
                                nextUp.Select();
                            }
                            else if (nextLeft != null)
                            {
                                nextLeft.Select();
                            }
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                CallLogin();
            }
        }

        public void CallLogin()
        {
            LoginSave.SaveMailLogin(accountMail.text);
            dialogWindow.SetActive(true);
            //dialogText.enabled = true;
            dialogText.text = controller.lc.GetLocalizedValue("dialog_login");
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
            dialogText.text = controller.lc.GetLocalizedValue("dialog_auth");
            yield return new WaitForSeconds(0.5f);
            if (www.responseCode == 202)
            {
                dialogText.text = controller.lc.GetLocalizedValue("dialog_login_done");
                yield return new WaitForSeconds(0.5f);
                PhotonNetwork.ConnectUsingSettings();
                httpResponse = www.downloadHandler.text.Split('#');
                
                PhotonNetwork.AuthValues = new AuthenticationValues(httpResponse[1]);
                PhotonNetwork.NickName = httpResponse[0];
                PhotonNetwork.SetPlayerCustomProperties(new ExitGames.Client.Photon.Hashtable()
                {
                    {"role", httpResponse[2]}
                });
                
                //Debug.Log(httpResponse[0]);
                dialogWindow.SetActive(false);
                controller.launchMainMenu();
            }
            else if (www.responseCode == 406)
            {
                dialogButton.SetActive(true);
                dialogText.text = controller.lc.GetLocalizedValue("dialog_login_error");
                //Debug.Log("Serveur d'authentification indisponible.");
            }
            else
            {
                dialogButton.SetActive(true);
                dialogText.text = controller.lc.GetLocalizedValue("dialog_login_server_error");
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
            controller.gameController.GetComponent<GameController>().MasterActivateAvatarPlayer();
            //Debug.Log();
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