using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Localization
{
    public class LocalizationText : MonoBehaviour
    {
        public string key;

        private void Start()
        {
            LocalizationController lc = GameObject.Find("LocalizationController").GetComponent<LocalizationController>();
            //lc.LoadLocalizeText("en_EN.json");
            Text text = GetComponent<Text>();
            text.text = lc.GetLocalizedValue(key);
        }
    }
}