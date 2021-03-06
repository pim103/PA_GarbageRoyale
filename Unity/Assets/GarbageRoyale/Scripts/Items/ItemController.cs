﻿using GarbageRoyale.Scripts.HUD;
using GarbageRoyale.Scripts.InventoryScripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class ItemController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameController gc;

        [SerializeField]
        private InventoryActionsController iac;
        
        public enum TypeItem
        {
            None,
            WoodenStaff,
            SteelStaff,
            Unknown,
            Torch,
            ToiletPaper,
            Jerrican,
            Bottle,
            BrokenBottle,
            OilBottle,
            Molotov,
            Rope,
            Implant,
            MetalSheet,
            NailBox,
            WolfTrap,
            Battery,
            ManifTrap,
            WaterBottle,
            ElecTrap,
            Electof
        }

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

        public void BrokeElectof(int idItem, int idPlayer, bool inWater)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if(idPlayer != -1)
            {
                gc.playersActions[idPlayer].isFallen = true;
                gc.playersActions[idPlayer].timeLeftFallen = 5.0f;
                gc.players[idPlayer].PlayerStats.takeDamage(8.0f);
            }

            if(inWater)
            {
                photonView.RPC("TriggerElectricityRPC", RpcTarget.All, gc.items[idItem].transform.position, -1);
            }
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
            gc.players[idPlayer].PlayerInventory.itemInventory[placeInHand] = idItem;
        }

        [PunRPC]
        private void BurnSurfaceRPC(Vector3 position)
        {
            GameObject burningSurface = ObjectPooler.SharedInstance.GetPooledObject(5);
            burningSurface.transform.position = position;
            burningSurface.SetActive(true);
        }

        public void PlaceRope(Vector3 pos1, Vector3 pos2, bool initNewRope)
        {
            photonView.RPC("AskPlaceRope", RpcTarget.MasterClient, pos1, pos2, PhotonNetwork.AuthValues.UserId, iac.placeInHand, initNewRope);
        }

        [PunRPC]
        private void AskPlaceRope(Vector3 pos1, Vector3 pos2, string userId, int placeInHand, bool initNewRope)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            int idPlayer = System.Array.IndexOf(gc.AvatarToUserId, userId);
            int idItem = gc.players[idPlayer].PlayerInventory.itemInventory[placeInHand];
            
            if(!initNewRope)
            {
                gc.players[idPlayer].PlayerInventory.itemInventory[placeInHand] = -1;
            }

            //TODO verify coord rope with player
            if (Vector3.Distance(pos1, pos2) <= 8.0f)
            {
                photonView.RPC("PlaceRopeRPC", RpcTarget.All, pos1, pos2, idPlayer, idItem, initNewRope, gc.items.Count);
            }
        }

        [PunRPC]
        private void PlaceRopeRPC(Vector3 pos1, Vector3 pos2, int idPlayer, int idItem, bool initNewRope, int idNewRope)
        {
            GameObject rope;

            if (initNewRope)
            {
                rope = ObjectPooler.SharedInstance.GetPooledObject(10);
                gc.items.Add(idNewRope, rope);
            }
            else
            {
                rope = gc.items[idItem];
            }

            Item ropeItem = rope.GetComponent<Item>();

            rope.transform.parent = null;
            ropeItem.resetScale();
            ropeItem.isPickable = false;
            rope.GetComponent<Rigidbody>().isKinematic = true;
            rope.SetActive(true);

            Vector3 temp = rope.transform.localScale;
            temp.z = Vector3.Distance(pos1, pos2)/2;
            rope.transform.localScale = temp;
            rope.transform.position = (pos1 + pos2)/2;
            rope.transform.LookAt(pos2);
            RopeScript rs = rope.GetComponent<RopeScript>();
            rs.mc.isTrigger = true;

            if (initNewRope)
            {
                rs.idTrap = idItem;
            }

            gc.players[idPlayer].PlayerRope.SetActive(false);

            if (!initNewRope && idPlayer == System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                gc.players[idPlayer].PlayerInventory.itemInventory[iac.placeInHand] = -1;
                gc.GetComponent<InventoryGUI>().deleteSprite(iac.placeInHand);
            }
        }

        public void PlaceObject(Vector3 pos1, Vector3 rot, int typeItem)
        {
            photonView.RPC("AskPlaceObject", RpcTarget.MasterClient, pos1, rot, PhotonNetwork.AuthValues.UserId, iac.placeInHand, typeItem);
        }

        [PunRPC]
        private void AskPlaceObject(Vector3 pos1, Vector3 rot, string userId, int placeInHand, int typeItem)
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
                photonView.RPC("PlaceObjectRPC", RpcTarget.All, pos1, rot, idPlayer, idItem, typeItem);
            }

            gc.players[idPlayer].PlayerInventory.itemInventory[placeInHand] = -1;
        }

        [PunRPC]
        private void PlaceObjectRPC(Vector3 pos1, Vector3 rot, int idPlayer, int idItem, int type)
        {
            GameObject newObject = gc.items[idItem];
            PreviewItemScript pis = newObject.GetComponent<PreviewItemScript>();

            newObject.transform.parent = null;
            newObject.GetComponent<Rigidbody>().isKinematic = true;
            newObject.SetActive(true);

            newObject.GetComponent<Item>().isPickable = false;
            
            newObject.transform.localScale = pis.scalePreview;
            newObject.transform.localEulerAngles = rot;
            newObject.transform.position = pos1;

            pis.bc.isTrigger = true;
            pis.bc.size += pis.bonusColliderSize;
            
            switch((TypeItem)type)
            {
                case TypeItem.MetalSheet:
                    gc.players[idPlayer].PlayerMetalSheet.SetActive(false);
                    break;
                case TypeItem.WolfTrap:
                    gc.players[idPlayer].PlayerWolfTrap.SetActive(false);
                    break;
                case TypeItem.ManifTrap:
                    gc.players[idPlayer].PlayerTrapManif.SetActive(false);
                    break;
                case TypeItem.ElecTrap:
                    gc.players[idPlayer].PlayerElecTrap.SetActive(false);
                    break;
            }

            if (idPlayer == System.Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                gc.players[idPlayer].PlayerInventory.itemInventory[iac.placeInHand] = -1;
                gc.GetComponent<InventoryGUI>().deleteSprite(iac.placeInHand);
            }
        }

        public void LaunchProjectile(int idItem)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            photonView.RPC("LaunchProjectileRPC", RpcTarget.All, idItem);
        }

        [PunRPC]
        private void LaunchProjectileRPC(int idItem)
        {
            GameObject toiletPaper = ObjectPooler.SharedInstance.GetPooledObject(11);
            GameObject trapOrig = gc.items[idItem];

            toiletPaper.SetActive(true);
            toiletPaper.transform.GetChild(0).gameObject.SetActive(true);

            toiletPaper.transform.parent = trapOrig.transform;
            toiletPaper.transform.position = trapOrig.transform.position + Vector3.up;
            toiletPaper.transform.localScale = toiletPaper.transform.localScale / 2;
            toiletPaper.transform.localRotation = Quaternion.identity;

            Item itemToiletPaper = toiletPaper.GetComponent<Item>();
            itemToiletPaper.isPickable = false;
            itemToiletPaper.isThrow = true;
            itemToiletPaper.type = (int)TypeItem.ToiletPaper;
            itemToiletPaper.isBurn = true;

            toiletPaper.GetComponent<Rigidbody>().AddForce((-trapOrig.transform.right * 10) + (Vector3.up * 2), ForceMode.Impulse);
        }

        public void ActiveWolfTrap(int idItem, bool isTrigger)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            photonView.RPC("ActiveWolfTrapRPC", RpcTarget.All, idItem, isTrigger);      
        }

        [PunRPC]
        private void ActiveWolfTrapRPC(int idItem, bool isTrigger)
        {
            WolfTrapScript wts = gc.items[idItem].GetComponent<WolfTrapScript>();

            if(isTrigger)
            {
                wts.leftPanel.transform.localEulerAngles = new Vector3(-15, 90, -90);
                wts.rightPanel.transform.localEulerAngles = new Vector3(-165, 90, -90);
            }
            else
            {
                wts.leftPanel.transform.localEulerAngles = new Vector3(-90, 90, -90);
                wts.rightPanel.transform.localEulerAngles = new Vector3(-90, 90, -90);
            }
        }

        public void TriggerElectricity(Vector3 posTrap, int idTrap)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            photonView.RPC("TriggerElectricityRPC", RpcTarget.All, posTrap, idTrap);
        }

        [PunRPC]
        private void TriggerElectricityRPC(Vector3 posTrap, int idItem)
        {
            if(idItem != -1)
            {
                gc.items[idItem].SetActive(false);
            }

            GameObject electricity = ObjectPooler.SharedInstance.GetPooledObject(12);
            electricity.transform.position = posTrap;
            electricity.SetActive(true);
            electricity.transform.parent = gc.water.waterObject.transform;
        }
    }
}
