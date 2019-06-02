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

        public void brokeBottle(int id, bool keepInHand, int idPlayer, int placeInHand)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if(!keepInHand)
            {
                photonView.RPC("addSpecificBrokenBottle", RpcTarget.All, id, gc.items.Count);
            }
            else
            {
                photonView.RPC("addSpecificBrokenBottleInHand", RpcTarget.All, id, gc.items.Count, idPlayer, placeInHand);
            }
        }

        public void OiledPlayer(int idItem, int idPlayer)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            gc.playersActions[idPlayer].isOiled = true;
            gc.playersActions[idPlayer].timeLeftOiled = 10.0f;
            photonView.RPC("addSpecificBrokenBottle", RpcTarget.All, idItem, gc.items.Count);
        }

        public void BurnSurface(int idItem)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            photonView.RPC("BurnSurfaceRPC", RpcTarget.All, gc.items[idItem].transform.position);
            photonView.RPC("addSpecificBrokenBottle", RpcTarget.All, idItem, gc.items.Count);
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
            brokenBottle.GetComponent<Item>().setId(idItem);

            gc.items.Add(idItem, brokenBottle);
        }

        [PunRPC]
        private void addSpecificBrokenBottleInHand(int id, int idItem, int idPlayer, int placeInHand)
        {
            GameObject bottle = gc.items[id];
            bottle.SetActive(false);

            GameObject brokenBottle = ObjectPooler.SharedInstance.GetPooledObject(4);

            gc.players[idPlayer].PlayerBottle.SetActive(false);
            gc.players[idPlayer].PlayerBottleOil.SetActive(false);
            gc.players[idPlayer].PlayerMolotov.SetActive(false);

            gc.players[idPlayer].PlayerBrokenBottle.SetActive(true);

            brokenBottle.GetComponent<Item>().setId(idItem);

            gc.items.Add(idItem, brokenBottle);
            gc.items[idItem].transform.SetParent(gc.players[idPlayer].PlayerTorch.transform.parent);
            gc.players[idPlayer].GetComponent<Inventory>().itemInventory[placeInHand] = idItem;
        }

        [PunRPC]
        private void BurnSurfaceRPC(Vector3 position)
        {
            GameObject burningSurface = ObjectPooler.SharedInstance.GetPooledObject(5);
            burningSurface.transform.position = position;
            burningSurface.SetActive(true);
        }
    }
}
