using GarbageRoyale.Scripts.HUD;
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

        [SerializeField]
        private InventoryActionsController iac;

        public void brokeBottle(int id, bool keepInHand, int idPlayer, int placeInHand)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (!keepInHand)
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

        public void PlaceRope(Vector3 pos1, Vector3 pos2)
        {
            photonView.RPC("AskPlaceRope", RpcTarget.MasterClient, pos1, pos2, PhotonNetwork.AuthValues.UserId, iac.placeInHand);
        }

        [PunRPC]
        private void AskPlaceRope(Vector3 pos1, Vector3 pos2, string userId, int placeInHand)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            int idPlayer = System.Array.IndexOf(gc.AvatarToUserId, userId);
            int idItem = gc.players[idPlayer].PlayerInventory.itemInventory[placeInHand];

            //TODO verify coord rope with player
            if (Vector3.Distance(pos1, pos2) <= 8.0f && gc.items[idItem].GetComponent<Item>().name == "Rope")
            {
                photonView.RPC("PlaceRopeRPC", RpcTarget.All, pos1, pos2, idPlayer, idItem);
            }

            gc.players[idPlayer].PlayerInventory.itemInventory[placeInHand] = -1;
        }

        [PunRPC]
        private void PlaceRopeRPC(Vector3 pos1, Vector3 pos2, int idPlayer, int idItem)
        {
            GameObject rope = gc.items[idItem];

            rope.transform.parent = null;
            rope.GetComponent<Item>().resetScale();
            rope.GetComponent<Rigidbody>().isKinematic = true;
            rope.SetActive(true);

            Vector3 temp = rope.transform.localScale;
            temp.z = Vector3.Distance(pos1, pos2);
            rope.transform.localScale = temp;
            rope.transform.position = pos1;
            rope.transform.LookAt(pos2);
            rope.GetComponent<RopeScript>().mc.isTrigger = true;

            gc.players[idPlayer].PlayerRope.SetActive(false);

            if (idPlayer == System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                gc.players[idPlayer].PlayerInventory.itemInventory[iac.placeInHand] = -1;
                gc.GetComponent<InventoryGUI>().deleteSprite(iac.placeInHand);
            }
        }

        public void PlaceObject(Vector3 pos1, int typeItem)
        {
            photonView.RPC("AskPlaceObject", RpcTarget.MasterClient, pos1, PhotonNetwork.AuthValues.UserId, iac.placeInHand, typeItem);
        }

        [PunRPC]
        private void AskPlaceObject(Vector3 pos1, string userId, int placeInHand, int typeItem)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            int idPlayer = System.Array.IndexOf(gc.AvatarToUserId, userId);
            int idItem = gc.players[idPlayer].PlayerInventory.itemInventory[placeInHand];

            //TODO verify coord rope with player
            if (gc.items[idItem].GetComponent<Item>().type == typeItem)
            {
                photonView.RPC("PlaceObjectRPC", RpcTarget.All, pos1, idPlayer, idItem, typeItem);
            }

            gc.players[idPlayer].PlayerInventory.itemInventory[placeInHand] = -1;
        }

        [PunRPC]
        private void PlaceObjectRPC(Vector3 pos1, int idPlayer, int idItem, int type)
        {
            GameObject newObject = gc.items[idItem];
            PreviewItemScript pis = newObject.GetComponent<PreviewItemScript>();

            newObject.transform.parent = null;
            newObject.GetComponent<Rigidbody>().isKinematic = true;
            newObject.SetActive(true);
            
            newObject.transform.localScale = pis.scalePreview;
            newObject.transform.localEulerAngles = pis.rotation;
            newObject.transform.position = pos1;

            pis.bc.isTrigger = true;
            pis.bc.size += pis.bonusColliderSize;
            
            switch(type)
            {
                case 13:
                    gc.players[idPlayer].PlayerMetalSheet.SetActive(false);
                    break;
                case 15:
                    gc.players[idPlayer].PlayerWolfTrap.SetActive(false);
                    break;
            }

            if (idPlayer == System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                gc.players[idPlayer].PlayerInventory.itemInventory[iac.placeInHand] = -1;
                gc.GetComponent<InventoryGUI>().deleteSprite(iac.placeInHand);
            }
        }
    }
}
