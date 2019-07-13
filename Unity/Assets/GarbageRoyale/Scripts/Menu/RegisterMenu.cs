using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private bool findFirstSelectable = false;

        void Start()
        {
            accountPassword.inputType = InputField.InputType.Password;
            accountPasswordConfirmation.inputType = InputField.InputType.Password;
            submitButton.onClick.AddListener(CallRegister);
            exitButton.onClick.AddListener(ReturnToLoginScreen);
        }
        
        public void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (EventSystem.current != null)
                {
                    GameObject selected = EventSystem.current.currentSelectedGameObject;

                    //try and find the first selectable if there isn't one currently selected
                    //only do it if the findFirstSelectable is true
                    //you may not always want this feature and thus
                    //it is disabled by default
                    if(selected == null && findFirstSelectable)
                    {
                        Selectable found = (Selectable.allSelectables.Count > 0) ? Selectable.allSelectables[0] : null;

                        if(found != null)
                        {
                            //simple reference so that selected isn't null and will proceed
                            //past the next if statement
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
                CallRegister();
            }
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
            var www = UnityWebRequest.Post("https://garbage-royale.heolia.eu/services/account/insert.php", form);
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
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

