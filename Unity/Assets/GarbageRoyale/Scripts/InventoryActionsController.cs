using System.Collections.Generic;
using GarbageRoyale.Scripts.HUD;
using GarbageRoyale.Scripts.Items;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class InventoryActionsController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameController gc;

        public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
        private Inventory playerInventory;
        private GameObject gtest;
        public string itemInHand;
        public int placeInHand = -1;
        public List<GameObject> thrownItems;
        public List<int> thrownItemsCount;
        
        AudioClip torchLightSound;

        public Dictionary<int, GameObject[]> listTorchLigt = new Dictionary<int, GameObject[]>();

        public bool Send;
        // Start is called before the first frame update
        void Start()
        {
            torchLightSound = GameObject.Find("Controller").GetComponent<SoundManager>().getTorchLightSound();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 0, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                placeInHand = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 1, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                placeInHand = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 2 ,System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                placeInHand = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 3, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                placeInHand = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 4, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                placeInHand = 4;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if(placeInHand!=-1) photonView.RPC("AskDropItem", RpcTarget.MasterClient, placeInHand, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),false);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                if(placeInHand!=-1) photonView.RPC("AskDropItem", RpcTarget.MasterClient, placeInHand, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId),true);
            }

            if(Input.GetKeyDown(KeyCode.Mouse1))
            {
                if(itemInHand == "Torch") photonView.RPC("LightOnTorchRPC", RpcTarget.MasterClient, placeInHand, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                else if(itemInHand == "Jerrican") photonView.RPC("DisperseOil", RpcTarget.MasterClient, placeInHand, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                else if (itemInHand == "Rope")
                {
                    int idPlayer = System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId);
                    int idItem = gc.players[idPlayer].GetComponent<Inventory>().getItemInventory()[placeInHand];
                    RopeScript rs = gc.players[idPlayer].PlayerRope.GetComponent<RopeScript>();
                    rs.inEditMode = !rs.inEditMode;
                    rs.idItem = idItem;
                }
                else if (itemInHand == "Nail Box")
                {
                    photonView.RPC("ThrowNail", RpcTarget.MasterClient, placeInHand, System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));
                }
                else if (itemInHand == "Metal Sheet" || itemInHand == "Wolf Trap" || itemInHand == "Trap Manif")
                {
                    photonView.RPC("WantToPlaceObject", RpcTarget.MasterClient, PhotonNetwork.AuthValues.UserId, placeInHand);
                }
            }
            
            /*if (Input.GetKeyDown(KeyCode.A))
            {
                gc.playersActions[System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId)].isQuiet = true;
            }*/

            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var item in thrownItems)
                {
                    if(thrownItemsCount[thrownItems.IndexOf(item)]<30 && thrownItemsCount[thrownItems.IndexOf(item)] >= 0){
                        item.transform.Translate(new Vector3(0,0,4)*Time.deltaTime);
                        item.transform.GetChild(0).Rotate(new Vector3(360,0,0)*Time.deltaTime,Space.Self);
                        thrownItemsCount[thrownItems.IndexOf(item)]+=1;
                    }
                    else if(thrownItemsCount[thrownItems.IndexOf(item)]>0)
                    {
                        item.transform.Translate(0,-1,0);
                        item.transform.GetChild(0).rotation = Quaternion.identity;
                        thrownItemsCount[thrownItems.IndexOf(item)] = -1;
                    }
                }
            }
        }

        [PunRPC]
        private void WantToPlaceObject(string userId, int inventoryPlace, PhotonMessageInfo info)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            int idPlayer = System.Array.IndexOf(gc.AvatarToUserId, userId);
            int idItem = gc.players[idPlayer].GetComponent<Inventory>().getItemInventory()[inventoryPlace];

            photonView.RPC("ActiveSpecificPreview", info.Sender, gc.items[idItem].GetComponent<Item>().type, idPlayer);
        }

        [PunRPC]
        private void ActiveSpecificPreview(int type, int idPlayer)
        {
            PreviewItemScript pis = null;

            switch(type)
            {
                case 13:
                    pis = pis = gc.players[idPlayer].PlayerMetalSheet.GetComponent<PreviewItemScript>();
                    break;
                case 15:
                    pis = gc.players[idPlayer].PlayerWolfTrap.GetComponent<PreviewItemScript>();
                    break;
                case 17:
                    pis = gc.players[idPlayer].PlayerTrapManif.GetComponent<PreviewItemScript>();
                    break;
            }
            
            if(pis != null)
            {
                pis.inEditMode = !pis.inEditMode;
            }
        }

        public void ExtinctTorch(int id)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("LightOnSpecificTorch", RpcTarget.All, id, false);
            }
        }
        
        [PunRPC]
        public void AskChangePlayerHandItem(int inventoryPlace,int playerIndex,PhotonMessageInfo info)
        {
            Inventory inventoryData = gc.players[playerIndex].GetComponent<Inventory>();
            var idItem = inventoryData.getItemInventory()[inventoryPlace];
            var itemName = "";

            if(idItem > -1)
            {
                itemName = gc.items[idItem].GetComponent<Item>().name;
            }

            photonView.RPC("AnswerChangePlayerHandItem", RpcTarget.All, itemName, playerIndex);
            photonView.RPC("setItemInHand", info.Sender, itemName);
        }
    
        [PunRPC]
        public void AnswerChangePlayerHandItem(string item, int playerIndex)
        {
            int handChild = -1;
            //Desactivate unused items
            gc.players[playerIndex].PlayerStaff.SetActive(false);
            gc.players[playerIndex].PlayerTorch.SetActive(false);
            gc.players[playerIndex].PlayerToiletPaper.SetActive(false);
            gc.players[playerIndex].PlayerJerrican.SetActive(false);
            gc.players[playerIndex].PlayerBottle.SetActive(false);
            gc.players[playerIndex].PlayerBrokenBottle.SetActive(false);
            gc.players[playerIndex].PlayerBottleOil.SetActive(false);
            gc.players[playerIndex].PlayerMolotov.SetActive(false);
            gc.players[playerIndex].PlayerRope.SetActive(false);
            gc.players[playerIndex].PlayerMetalSheet.SetActive(false);
            gc.players[playerIndex].PlayerBoxNail.SetActive(false);
            gc.players[playerIndex].PlayerWolfTrap.SetActive(false);
            gc.players[playerIndex].PlayerBattery.SetActive(false);
            gc.players[playerIndex].PlayerTrapManif.SetActive(false);

            switch (item)
            {
                case "Wooden Staff":
                    handChild = 1;
                    gc.players[playerIndex].PlayerStaff.SetActive(true);
                    break;
                case "Steel Staff":
                    handChild = 1;
                    gc.players[playerIndex].PlayerStaff.SetActive(true);
                    break;
                case "Torch":
                    handChild = 0;
                    gc.players[playerIndex].PlayerTorch.SetActive(true);
                    break;
                case "Toilet Paper":
                    handChild = 2;
                    gc.players[playerIndex].PlayerToiletPaper.SetActive(true);
                    break;
                case "Jerrican":
                    gc.players[playerIndex].PlayerJerrican.SetActive(true);
                    break;
                case "Bottle":
                    gc.players[playerIndex].PlayerBottle.SetActive(true);
                    break;
                case "Broken Bottle":
                    gc.players[playerIndex].PlayerBrokenBottle.SetActive(true);
                    break;
                case "Bottle Oil":
                    gc.players[playerIndex].PlayerBottleOil.SetActive(true);
                    break;
                case "Molotov":
                    gc.players[playerIndex].PlayerMolotov.SetActive(true);
                    break;
                case "Rope":
                    gc.players[playerIndex].PlayerRope.SetActive(true);
                    break;
                case "Metal Sheet":
                    gc.players[playerIndex].PlayerMetalSheet.SetActive(true);
                    break;
                case "Nail Box":
                    gc.players[playerIndex].PlayerBoxNail.SetActive(true);
                    break;
                case "Wolf Trap":
                    gc.players[playerIndex].PlayerWolfTrap.SetActive(true);
                    break;
                case "Battery":
                    gc.players[playerIndex].PlayerBattery.SetActive(true);
                    break;
                case "Trap Manif":
                    gc.players[playerIndex].PlayerTrapManif.SetActive(true);
                    break;
                default:
                    Debug.Log("Error, wrong item");
                    break;
            }
        }

        [PunRPC]
        public void setItemInHand(string newItemInHand)
        {
            itemInHand = newItemInHand;
        }
        
        [PunRPC]
        public void AskDropItem(int inventoryPlace,int playerIndex, bool throwItem,PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            int idItem = gc.players[playerIndex].GetComponent<Inventory>().getItemInventory()[inventoryPlace];
            if (idItem == -1)
            {
                return;
            }
            
            int typeItem = gc.items[idItem].GetComponent<Item>().type;
            string typeName = gc.items[idItem].GetComponent<Item>().name;

            photonView.RPC("AnswerDropItem", RpcTarget.All, typeItem, idItem, playerIndex, throwItem, inventoryPlace, typeName);
        }
    
        [PunRPC]
        public void AnswerDropItem(int typeItem, int idItem, int playerIndex, bool throwItem, int inventoryPlace, string typeName)
        {
            gc.players[playerIndex].GetComponent<Inventory>().itemInventory[inventoryPlace] = -1;
            int handChild = -1;

            gc.items[idItem].transform.parent = null;
            gc.items[idItem].SetActive(true);
            gc.items[idItem].GetComponent<Item>().resetScale();
            gc.items[idItem].GetComponent<Item>().transform.position = gc.players[playerIndex].PlayerTorch.transform.position + Vector3.forward;

            gc.players[playerIndex].PlayerStaff.SetActive(false);
            gc.players[playerIndex].PlayerTorch.SetActive(false);
            gc.players[playerIndex].PlayerToiletPaper.SetActive(false);
            gc.players[playerIndex].PlayerJerrican.SetActive(false);
            gc.players[playerIndex].PlayerBottle.SetActive(false);
            gc.players[playerIndex].PlayerBrokenBottle.SetActive(false);
            gc.players[playerIndex].PlayerBottleOil.SetActive(false);
            gc.players[playerIndex].PlayerMolotov.SetActive(false);
            gc.players[playerIndex].PlayerRope.SetActive(false);
            gc.players[playerIndex].PlayerMetalSheet.SetActive(false);
            gc.players[playerIndex].PlayerBoxNail.SetActive(false);
            gc.players[playerIndex].PlayerWolfTrap.SetActive(false);
            gc.players[playerIndex].PlayerBattery.SetActive(false);
            gc.players[playerIndex].PlayerTrapManif.SetActive(false);

            if (throwItem)
            {
                gc.items[idItem].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 2, 10), ForceMode.Impulse);
                gc.items[idItem].GetComponent<Item>().isThrow = true;
            }

            switch (typeName)
            {
                case "Torch":
                    handChild = 0;
                    gc.players[playerIndex].PlayerTorch.SetActive(false);
                    if(gc.players[playerIndex].PlayerTorch.transform.GetChild(0).gameObject.activeSelf)
                    {
                        gc.items[idItem].transform.GetChild(0).gameObject.SetActive(true);
                    } else
                    {
                        gc.items[idItem].transform.GetChild(0).gameObject.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
            
            if (playerIndex == System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                gc.GetComponent<InventoryGUI>().deleteSprite(placeInHand);
                itemInHand = "";
                placeInHand = -1;
            }
        }
        
        [PunRPC]
        private void LightOnTorchRPC(int placeInHand, int playerIndex)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            Inventory inventoryData = gc.players[playerIndex].GetComponent<Inventory>();
            int itemId = inventoryData.getItemInventory()[placeInHand];

            if(gc.playersActions[playerIndex].headIsInWater)
            {
                return;
            }

            if (gc.players[playerIndex].PlayerTorch.activeSelf && gc.items[itemId].GetComponent<Item>().name == "Torch")
            {
                bool toggle = !gc.players[playerIndex].PlayerTorch.transform.GetChild(0).gameObject.activeSelf;
                photonView.RPC("LightOnSpecificTorch", RpcTarget.All, playerIndex, toggle);
            }
        }

        [PunRPC]
        private void LightOnSpecificTorch(int playerIndex, bool toggle)
        {
            gc.players[playerIndex].PlayerTorch.transform.GetChild(0).gameObject.SetActive(toggle);
        }

        [PunRPC]
        private void DisperseOil(int placeInHand, int playerIndex)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            Inventory inventoryData = gc.players[playerIndex].GetComponent<Inventory>();
            int itemId = inventoryData.getItemInventory()[placeInHand];

            Vector3 pos = Vector3.forward;
            RaycastHit info;
            if (Physics.Raycast(gc.players[playerIndex].PlayerFeet.transform.position, transform.TransformDirection(Vector3.down), out info))
            {
                pos = new Vector3(info.point.x, info.point.y + 0.1f, info.point.z);
            }

            if (gc.items[itemId].GetComponent<Item>().name == "Jerrican" && gc.items[itemId].GetComponent<OilScript>().nbOil > 0)
            {
                gc.items[itemId].GetComponent<OilScript>().nbOil--;
                photonView.RPC("DisperseSpecificOil", RpcTarget.All, playerIndex, pos);
            }
        }

        [PunRPC]
        private void DisperseSpecificOil(int playerIndex, Vector3 position)
        {
            ParticleSystem ps = gc.players[playerIndex].PlayerJerrican.transform.GetChild(0).GetComponent<ParticleSystem>();

            if(!ps.isPlaying)
            {
                ps.Play();

                GameObject oil = ObjectPooler.SharedInstance.GetPooledObject(0);
                oil.transform.position = position;
                oil.SetActive(true);
            }
        }

        [PunRPC]
        private void ThrowNail(int placeInHand, int playerIndex)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            Inventory inventoryData = gc.players[playerIndex].GetComponent<Inventory>();
            int itemId = inventoryData.getItemInventory()[placeInHand];

            Vector3 pos = Vector3.forward;
            RaycastHit info;
            if (Physics.Raycast(gc.players[playerIndex].PlayerFeet.transform.position, transform.TransformDirection(Vector3.down), out info))
            {
                pos = new Vector3(info.point.x, info.point.y + 0.1f, info.point.z + 2.0f);
            }

            if (gc.items[itemId].GetComponent<Item>().name == "Nail Box" && !gc.items[itemId].GetComponent<NailAreaScript>().isEmpty)
            {
                gc.items[itemId].GetComponent<NailAreaScript>().isEmpty = true;
                photonView.RPC("DisperseSpecificNail", RpcTarget.All, pos);
            }
        }

        [PunRPC]
        private void DisperseSpecificNail(Vector3 position)
        {
            GameObject nailArea = ObjectPooler.SharedInstance.GetPooledObject(7);
            nailArea.transform.position = position;
            nailArea.SetActive(true);
        }
    }
}