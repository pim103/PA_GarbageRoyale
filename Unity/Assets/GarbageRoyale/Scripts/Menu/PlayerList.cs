using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class PlayerList : MonoBehaviour
    {
        [SerializeField] 
        private Text textInfo;

        public void SetPlayerInfo(string name)
        {
            textInfo.text = name;
        }
    }
}