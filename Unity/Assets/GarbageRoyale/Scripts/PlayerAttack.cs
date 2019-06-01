using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using GarbageRoyale.Scripts.PrefabPlayer;
using System;

namespace GarbageRoyale.Scripts
{
    public class PlayerAttack : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameController gc;

        [SerializeField]
        private InventoryActionsController iac;

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

            if (indexItem != -1)
            {
                Item item = gc.items[gc.players[playerIdSrc].PlayerInventory.itemInventory[inventorySlot]].GetComponent<Item>();
                damage += item.getDamage();
                
                if(item.name == "Torch")
                {
                    if(gc.players[playerIdSrc].PlayerTorch.transform.GetChild(0).gameObject.activeSelf)
                    {
                        canBurn = true;
                    }
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

            float damage = gc.items[idItem].GetComponent<Item>().damage;
            gc.players[idPlayer].PlayerStats.takeDamage(damage);
        }
    }
}
