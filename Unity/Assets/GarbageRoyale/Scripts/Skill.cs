﻿using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class Skill : MonoBehaviour
    {
        public int id;
        public int type;
        public int bufftime;
        public int cooldown;

        public string name;
        // Start is called before the first frame update
        void Start()
        {
            bufftime = 10;
            cooldown = 20;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
