using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using GarbageRoyale.Scripts.PrefabPlayer;
using System;
using GarbageRoyale.Scripts.Items;

namespace GarbageRoyale.Scripts
{
    public class PlayerAttack : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameController gc;

        [SerializeField]
        private InventoryActionsController iac;

        [SerializeField]
        private ItemController ic;

        // Start is called before the first frame update
        private void LateUpdate()
        {
            if(!gc.endOfInit)
            {
                return;
            }

            if(Input.GetMouseButtonDown(0))
            {
                photonView.RPC("PunchRPC", RpcTarget.MasterClient, PhotonNetwork.AuthValues.UserId);
            }
        }

        public void hitPlayer(RaycastHit info, int id)
        {
            photonView.RPC("findPlayer", RpcTarget.MasterClient, id, info.transform.position.x, info.transform.position.y, info.transform.position.z, iac.placeInHand, PhotonNetwork.AuthValues.UserId);
        }

        [PunRPC]
        private void findPlayer(int playerId, float x, float y, float z, int inventorySlot, string userId, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            int playerIdSrc = Array.IndexOf(gc.AvatarToUserId, userId);
            PlayerStats ps = gc.players[playerIdSrc].PlayerStats;
            float damage = ps.getBasickAttack();
            int indexItem = gc.players[playerIdSrc].PlayerInventory.itemInventory[inventorySlot];

            bool canBurn = false;
            bool canOiled = false;
            bool isBottle = false;
            BottleScript bottleScript;

            if (indexItem != -1)
            {
                Item item = gc.items[gc.players[playerIdSrc].PlayerInventory.itemInventory[inventorySlot]].GetComponent<Item>();
                if (gc.playersActions[playerIdSrc].isDamageBoosted)
                {
                    damage += 5;
                }
                damage += item.getDamage();
                
                if(item.name == "Torch")
                {
                    if(gc.players[playerIdSrc].PlayerTorch.transform.GetChild(0).gameObject.activeSelf)
                    {
                        canBurn = true;
                    }
                }
                else if(bottleScript = item.transform.GetComponent<BottleScript>())
                {
                    if(bottleScript.isOiled || bottleScript.isBurn)
                    {
                        canOiled = true;
                    }

                    ic.brokeBottle(item.getId(), true, playerIdSrc, inventorySlot);
                }
            }

            if (ps.getStamina() >= ps.getAttackCostStamina())
            {
                GameObject target = gc.players[playerId].PlayerGameObject;
                if (target.transform.position.x == x && target.transform.position.y == y && target.transform.position.z == z)
                {
                    gc.players[playerId].PlayerStats.takeDamage(damage);
                    if(canBurn && gc.playersActions[playerId].isOiled)
                    {
                        gc.playersActions[playerId].isOiled = false;
                        gc.playersActions[playerId].isBurning = true;
                        gc.playersActions[playerId].timeLeftBurn = 5.0f;
                    }
                    else if(canOiled)
                    {
                        if(gc.playersActions[playerId].isBurning)
                        {
                            gc.playersActions[playerId].timeLeftBurn = 5.0f;
                        }
                        else
                        {
                            gc.playersActions[playerId].isOiled = true;
                            gc.playersActions[playerId].timeLeftOiled = 10.0f;
                        }
                    }
                }
            }
        }

        [PunRPC]
        private void PunchRPC(string userId, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            int playerIdSrc = Array.IndexOf(gc.AvatarToUserId, userId);
            PlayerStats ps = gc.players[playerIdSrc].PlayerStats;

            if (ps.getAttackCostStamina() > ps.getStamina()) return;
            ps.useStamina();
        }

        public void HitByThrowItem(int idPlayer, int idItem)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            Item item = gc.items[idItem].GetComponent<Item>();
            float damage = item.damage;
            gc.players[idPlayer].PlayerStats.takeDamage(damage);

            if(item.type == (int)ItemController.TypeItem.Torch || item.type == (int)ItemController.TypeItem.ToiletPaper)
            {
                if(gc.items[idItem].transform.GetChild(0).gameObject.activeSelf)
                {
                    if(gc.playersActions[idPlayer].isOiled)
                    {
                        Debug.Log("Brule");
                        gc.playersActions[idPlayer].isOiled = false;
                        gc.playersActions[idPlayer].isBurning = true;
                        gc.playersActions[idPlayer].timeLeftBurn = 5.0f;
                    }
                }
            }
        }
    }
}
