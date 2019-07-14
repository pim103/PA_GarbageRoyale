using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GarbageRoyale.Scripts.Localization
{
    public class LocalizationController : MonoBehaviour
    {
        public static LocalizationController instance;
        private Dictionary<string, string> localizedText;
        private bool isReady = false;
        private string missingText = "Localized text not found";

        private void Awake()
        {
            Debug.Log("Localization Awaken!");
        }

        public void LoadLocalizeText(string fileName)
        {
            localizedText = new Dictionary<string, string>();
            Debug.Log(fileName);
            string filePath = Application.streamingAssetsPath + "/Localization/" + fileName;

            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
                for (int i = 0; i < loadedData.items.Length; i++)
                {
                    localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
                }
                Debug.Log("Data loaded ! " + localizedText.Count + "entries");
            }
            else
            {
                Debug.Log("Error can't localize : " + filePath + " on " + Application.streamingAssetsPath);
            }
            isReady = true;
        }

        public string GetLocalizedValue(string key)
        {
            string res = missingText;
            if (localizedText.ContainsKey(key))
            {
                res = localizedText[key];
            }

            return res;
        }

        public bool GetIsReady()
        {
            return isReady;
        }
    }
}