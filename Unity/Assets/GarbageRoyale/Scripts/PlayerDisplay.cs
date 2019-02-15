
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace GarbageRoyale.Scripts
{
    public class PlayerDisplay : MonoBehaviourPunCallbacks
    {
        private GameController gc;
        public Dictionary<int, GameObject> characterList = new Dictionary<int, GameObject>();

        private float hp;
        private float breath;
        private float stamina;
        private bool isOnWater;

        private PlayerStats playerStats;
        private PlayerMovement playerMov;

        private Texture2D breathTexture;
        private Texture2D hpTexture;
        private Texture2D borderTexture;
        private Texture2D filterWaterTexture;
        private Texture2D deadTexture;

        private Text deadMessage;

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            characterList = gc.characterList;
            initTexture();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            photonView.RPC("getPlayerStats", RpcTarget.MasterClient);
        }

        [PunRPC]
        private void getPlayerStats(PhotonMessageInfo info)
        {
            playerStats = characterList[info.Sender.ActorNumber].GetComponent<PlayerStats>();
            playerMov = characterList[info.Sender.ActorNumber].GetComponent<PlayerMovement>();
            photonView.RPC("initPlayerStats", info.Sender, playerStats.getHp(), playerStats.getBreath(), playerStats.getStamina(), playerMov.getHeadIsOnWater());
        }

        [PunRPC]
        private void initPlayerStats(float hp, float breath, float stamina, bool isOnWater)
        {
            this.hp = hp;
            this.breath = breath;
            this.stamina = stamina;
            this.isOnWater = isOnWater;
        }

        private void OnGUI()
        { 
            if (PhotonNetwork.IsMasterClient)
            {
                playerStats = characterList[PhotonNetwork.LocalPlayer.ActorNumber].GetComponent<PlayerStats>();
                playerMov = characterList[PhotonNetwork.LocalPlayer.ActorNumber].GetComponent<PlayerMovement>();
                hp = playerStats.getHp();
                breath = playerStats.getBreath();
                isOnWater = playerMov.getHeadIsOnWater();
            }

            // BREATH
            //top
            GUI.DrawTexture(new Rect(Screen.width - 153, Screen.height - 53, 106, 3), borderTexture);
            //bot
            GUI.DrawTexture(new Rect(Screen.width - 153, Screen.height - 20, 106, 3), borderTexture);
            //left
            GUI.DrawTexture(new Rect(Screen.width - 153, Screen.height - 53, 3, 36), borderTexture);
            //right
            GUI.DrawTexture(new Rect(Screen.width - 50, Screen.height - 53, 3, 36), borderTexture);
            //respi
            GUI.DrawTexture(new Rect(Screen.width - 150, Screen.height - 50, breath, 30), breathTexture);
        
            // HP
            //top
            GUI.DrawTexture(new Rect(47, Screen.height - 53, 106, 3), borderTexture);
            //bot
            GUI.DrawTexture(new Rect(47, Screen.height - 20, 106, 3), borderTexture);
            //left
            GUI.DrawTexture(new Rect(47, Screen.height - 53, 3, 36), borderTexture);
            //right
            GUI.DrawTexture(new Rect(150, Screen.height - 53, 3, 36), borderTexture);
            //respi
            GUI.DrawTexture(new Rect(50, Screen.height - 50, hp, 30), hpTexture);
            
            if(isOnWater && hp > 0)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), filterWaterTexture);
            } else if (hp <= 0)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), deadTexture);
            }
        }

        private void initTexture()
        {
            filterWaterTexture = gc.MakeTex(Screen.width, Screen.height, new Color(0, 0.5f, 1, 0.5f));
            hpTexture = gc.MakeTex(100, 30, Color.red);
            breathTexture = gc.MakeTex(100, 30, Color.blue);
            borderTexture = gc.MakeTex(110, 40, new Color(0.8f, 0.8f, 0.8f, 1f));
            deadTexture = gc.MakeTex(Screen.width, Screen.height, new Color(0.7f, 0.7f, 0.7f, 0.7f));
            // deadMessage.text = "Vous êtes mort !";
        }
    }
}
