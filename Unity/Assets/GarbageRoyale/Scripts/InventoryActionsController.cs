using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class InventoryActionsController : MonoBehaviourPunCallbacks
    {
        public Dictionary <int, GameObject> characterList = new Dictionary<int, GameObject>();
        private Inventory playerInventory;
        private GameObject gtest;
        public int itemInHand;
        public int placeInHand;
        public List<GameObject> thrownItems;
        public List<int> thrownItemsCount;

        AudioClip torchLightSound;

        public Dictionary<int, GameObject[]> listTorchLigt = new Dictionary<int, GameObject[]>();

        public bool Send;
        // Start is called before the first frame update
        void Start()
        {
            characterList = GameObject.Find("Controller").GetComponent<GameController>().characterList;
            torchLightSound = GameObject.Find("Controller").GetComponent<SoundManager>().getTorchLightSound();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //itemInHand = playerInventory.getItemInventory()[0];
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 0);
                placeInHand = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 1);
                placeInHand = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 2);
                placeInHand = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 3);
                placeInHand = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                photonView.RPC("AskChangePlayerHandItem", RpcTarget.MasterClient, 4);
                placeInHand = 4;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if(itemInHand!=0) photonView.RPC("AskDropItem", RpcTarget.MasterClient, placeInHand);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if(itemInHand!=0) photonView.RPC("AskThrowItem", RpcTarget.MasterClient, placeInHand);
            }

            if (Send)
            {
                photonView.RPC("ChangeItemInHandOthers", RpcTarget.MasterClient, itemInHand);
                Send = false;
            }

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

                foreach(var torch in listTorchLigt)
                {
                    GameObject flame = torch.Value[0];
                    GameObject sound = torch.Value[1];
                    GameObject player = characterList[torch.Key];
                    
                    if(flame.activeSelf)
                    {
                        photonView.RPC("moveTorch", RpcTarget.All, torch.Key, player.transform.position.x, player.transform.position.y, player.transform.position.z);
                    }
                }
            }
        }

        public void triggerTorch(bool isOn)
        {
            photonView.RPC("askTriggerTorch", RpcTarget.MasterClient, isOn);
        }

        [PunRPC]
        private void askTriggerTorch(bool isOn, PhotonMessageInfo info)
        {
            Transform hand = characterList[info.Sender.ActorNumber].transform.GetChild(8).transform;
            photonView.RPC("triggerTorchForAll", info.Sender, isOn, info.Sender.ActorNumber, hand.position.x, hand.position.y, hand.position.z);
        }

        [PunRPC]
        private void triggerTorchForAll(bool isOn, int id, float posX, float posY, float posZ)
        {
            photonView.RPC("sendTriggerTorch", RpcTarget.Others, isOn, id, posX, posY, posZ);
        }

        [PunRPC]
        private void sendTriggerTorch(bool isOn, int id, float posX, float posY, float posZ)
        {
            Vector3 positionTorch = new Vector3(posX, posY, posZ);

            if(isOn && !listTorchLigt.ContainsKey(id))
            {
                GameObject flame = ObjectPooler.SharedInstance.GetPooledObject(7);
                flame.SetActive(true);
                flame.transform.position = positionTorch;

                GameObject crateSound = ObjectPooler.SharedInstance.GetPooledObject(2);
                crateSound.SetActive(true);
                crateSound.transform.position = positionTorch;

                AudioSource audio = crateSound.GetComponent<AudioSource>();
                audio.clip = torchLightSound;
                audio.loop = true;
                audio.Play();

                GameObject[] go = new GameObject[2];
                go[0] = flame;
                go[1] = crateSound;

                listTorchLigt.Add(id, go);

            } else if(isOn)
            {
                GameObject[] go = listTorchLigt[id];
                go[0].SetActive(true);
                go[1].SetActive(true);

                go[0].transform.position = positionTorch;
                go[1].transform.position = positionTorch;
            }
            else
            {
                GameObject[] go = listTorchLigt[id];
                go[0].SetActive(false);
                go[1].SetActive(false);
            }
        }

        [PunRPC]
        private void moveTorch(int id, float x, float y, float z)
        {
            if(id != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                Vector3 newPos = new Vector3(x, y, z);
                Debug.Log(newPos + " id = " + id);
                listTorchLigt[id][0].transform.position = newPos;
                listTorchLigt[id][1].transform.position = newPos;
            }
        }
    
        [PunRPC]
        public void AskChangePlayerHandItem(int inventoryPlace,PhotonMessageInfo info)
        {
            Inventory inventoryData = characterList[info.Sender.ActorNumber].GetComponent<Inventory>();
            photonView.RPC("AnswerChangePlayerHandItem", info.Sender, inventoryData.getItemInventory()[inventoryPlace]);
            characterList[info.Sender.ActorNumber].GetComponent<InventoryController>().itemInHand = inventoryData.getItemInventory()[inventoryPlace];
        }
    
        [PunRPC]
        public void AnswerChangePlayerHandItem(int item,PhotonMessageInfo info)
        {
            itemInHand = item;
        }
        [PunRPC]
        public void ChangeItemInHandOthers(int item,PhotonMessageInfo info)
        {
            int poolID = 0;
            GameObject putitem;
            switch (item)
            {
                case 1:
                    poolID = 0;
                    break;
                case 4:
                    poolID = 1;
                    break;
                case 5:
                    poolID = 3;
                    break;
            }
        
            putitem = ObjectPoolerPhoton.SharedInstance.GetPooledObject(poolID);
            putitem.SetActive(true);
            putitem.transform.SetParent(characterList[info.Sender.ActorNumber].transform.GetChild(8).transform);
            putitem.transform.localPosition = new Vector3(0,0,0);
            putitem.transform.localEulerAngles = new Vector3(0,0,0);
        }
    
        [PunRPC]
        public void AskDropItem(int inventoryPlace,PhotonMessageInfo info)
        {
            Inventory inventoryData = characterList[info.Sender.ActorNumber].GetComponent<Inventory>();
            characterList[info.Sender.ActorNumber].transform.GetChild(8).GetChild(0).parent = null;
            inventoryData.getItemInventory()[inventoryPlace] = 0;
            photonView.RPC("AnswerDropItem", info.Sender);
        }
    
        [PunRPC]
        public void AskThrowItem(int inventoryPlace,PhotonMessageInfo info)
        {
            Transform child;
            GameObject newParent = new GameObject("thrownObjectParent");
            Inventory inventoryData = characterList[info.Sender.ActorNumber].GetComponent<Inventory>();
            child = characterList[info.Sender.ActorNumber].transform.GetChild(8).GetChild(0);
            newParent.transform.position = child.transform.position;
            newParent.transform.rotation = child.transform.rotation;
            child.parent = newParent.transform;
            newParent.transform.Translate(0,1,0);
            inventoryData.getItemInventory()[inventoryPlace] = 0;
            photonView.RPC("AnswerDropItem", info.Sender);
            /*for (int i = 0; i < 100; i++)
        {
            child.Translate(Vector3.forward*Time.deltaTime,Space.Self);
            
        }*/
            thrownItems.Add(newParent);
            thrownItemsCount.Add(0);
        }
    
        [PunRPC]
        public void AnswerDropItem()
        {
            itemInHand = 0;
        }
    
    
    }
}