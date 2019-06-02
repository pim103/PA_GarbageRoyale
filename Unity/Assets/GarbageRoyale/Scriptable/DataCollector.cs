using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scriptable
{
    public class DataCollector : MonoBehaviour
    {
        [SerializeField]
        private NewScriptableObject Scriptable;

        public static DataCollector instance;

        // Start is called before the first frame update
        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }

        public void AddKillPoint(GameObject go, int id, float time)
        {
            Scriptable.RegisterKillPoint(go, id, time);
        }

        public void InitMap(Dictionary<string, string>[] rll, Dictionary<string, int>[] rt, Dictionary<string, int>[] ir,
            GameObject[] pref,
            GameObject trans,
            int[][,] f,
            int[][,] fr
        )
        {
            Scriptable.initMap(rll, rt, ir, pref, trans, f, fr);
        }
    }
}
