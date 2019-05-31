using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class ItemController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameController gc;

        public void brokeBottle(int id)
        {
            photonView.RPC("brokeBottleRpc", RpcTarget.MasterClient, id);
        }

        [PunRPC]
        private void brokeBottleRpc(int id)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            photonView.RPC("addSpecificBrokenBottle", RpcTarget.All, id, gc.items.Count);
        }

        [PunRPC]
        private void addSpecificBrokenBottle(int id, int idItem)
        {
            GameObject bottle = gc.items[id];
            bottle.SetActive(false);

            GameObject brokenBottle = ObjectPooler.SharedInstance.GetPooledObject(4);
            brokenBottle.transform.position = bottle.transform.position;
            brokenBottle.transform.rotation = bottle.transform.rotation;
            brokenBottle.SetActive(true);
            brokenBottle.GetComponent<Item>().id = idItem;

            gc.items.Add(idItem, brokenBottle);
        }
    }
}
