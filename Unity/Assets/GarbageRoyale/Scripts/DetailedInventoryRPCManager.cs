using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class DetailedInventoryRPCManager : MonoBehaviourPunCallbacks
    {
        private GameController gc;
        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        [PunRPC]
        public void AskSwapInventoryItems(int playerIndex, int oldPlace, int newPlace)
        {
            Debug.Log("yes");
            Debug.Log(" sent playerindex "+playerIndex+" oldplace "+oldPlace+" newplace "+newPlace);
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            photonView.RPC("AnswerSwapInventoryItems", RpcTarget.All,playerIndex, oldPlace, newPlace);
        }
        
        [PunRPC]
        public void testrpc()
        {
            Debug.Log("ALLO");
        }

        [PunRPC]
        public void AnswerSwapInventoryItems(int playerIndex, int oldPlace, int newPlace)
        {
            Debug.Log(" received playerindex "+playerIndex+" oldplace "+oldPlace+" newplace "+newPlace);
            Inventory playerInventory = gc.players[playerIndex].PlayerGameObject.GetComponent<Inventory>();
            int idMem = playerInventory.itemInventory[newPlace];
            playerInventory.itemInventory[newPlace] = playerInventory.itemInventory[oldPlace];
            playerInventory.itemInventory[oldPlace] = idMem;
        }

        [PunRPC]
        public void AskCraftItem(int itemtype, int playerIndex)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            photonView.RPC("AnswerCraftItem", RpcTarget.All, itemtype,playerIndex);
        }
        
        [PunRPC]
        public void AnswerCraftItem(int itemtype, int playerIndex)
        {
            Inventory playerInventory = gc.players[playerIndex].PlayerGameObject.GetComponent<Inventory>();
            playerInventory.itemInventory[25] = itemtype;
        }
        
        [PunRPC]
        public void AskDeleteCraftItem(int playerIndex)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            photonView.RPC("AnswerDeleteCraftItem", RpcTarget.All, playerIndex);
        }
        
        [PunRPC]
        public void AnswerDeleteCraftItem(int playerIndex)
        {
            Inventory playerInventory = gc.players[playerIndex].PlayerGameObject.GetComponent<Inventory>();
            playerInventory.itemInventory[25] = -1;
        }
    }
}
