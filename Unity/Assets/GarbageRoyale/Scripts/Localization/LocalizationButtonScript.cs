using System;
using GarbageRoyale.Scripts.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Localization
{
    public class LocalizationButtonScript : MonoBehaviour
    {
        [SerializeField] 
        private string fileName;
        [SerializeField] 
        private Button btn;
        [SerializeField] 
        private LocalizationController lc;
        
        private void Start()
        {
            btn.onClick.AddListener(ReloadScene);
        }

        private void ReloadScene()
        {
            Debug.Log(fileName);
            LocalizationSave.NewLocalizationSave(fileName);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}