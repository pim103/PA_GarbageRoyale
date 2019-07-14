using System.Collections;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.Menu
{
    public class RoomListing : MonoBehaviour
    {
        [SerializeField] 
        private Text textInfo;

        public RoomListing(RoomInfo roomInfo)
        {
            RoomInfo = roomInfo;
        }

        public RoomInfo RoomInfo { get; set; }

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            textInfo.text = roomInfo.Name + " : " + roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers + " joueurs";
        }
        
        public void SelectRoom()
        {
            foreach (GameObject room in GameObject.FindGameObjectsWithTag("RoomList"))
            {
                room.gameObject.GetComponent<RawImage>().color = Color.white;
            }
            this.gameObject.GetComponent<RawImage>().color = Color.gray;
        } 
    }
}