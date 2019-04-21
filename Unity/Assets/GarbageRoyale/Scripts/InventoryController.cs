using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class InventoryController : MonoBehaviourPunCallbacks
    {
        public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
        private Inventory playerInventory;
        [SerializeField]
        private GameObject playerPrefab;
        private GameObject gtest;
        public int itemInHand;
        

        // Start is called before the first frame update
        void Start()
        {
            characterList = GameObject.Find("Controller").GetComponent<GameController>().characterList;
            playerPrefab = characterList[PhotonNetwork.LocalPlayer.ActorNumber];
            playerInventory = playerPrefab.AddComponent<Inventory>();
            playerInventory.initInventory();
            itemInHand = 0;

            //Debug.Log(string.Format("Inventory : {0}", playerInventory.getItemInventory()[0]));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                itemInHand = playerInventory.getItemInventory()[0];
            }
        }

        // Update is called once per frame
        /*public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        characterList[newPlayer.ActorNumber].AddComponent<Inventory>();
        characterList[newPlayer.ActorNumber].GetComponent<Inventory>().initInventory();
    }*/
        
    
    }
}