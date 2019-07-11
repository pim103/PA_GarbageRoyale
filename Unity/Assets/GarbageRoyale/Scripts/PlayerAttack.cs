using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using GarbageRoyale.Scripts.PrefabPlayer;
using System;
using GarbageRoyale.Scripts.Items;
using GarbageRoyale.Scripts.InventoryScripts;

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
        private void Update()
        {
            if(!gc.endOfInit)
            {
                return;
            }

            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            /*
            int idPlayer = Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId);
            if (!gc.playersActions[idPlayer].isInInventory && !gc.playersActions[idPlayer].isInEscapeMenu && Input.GetMouseButtonDown(0))
            {
                photonView.RPC("PunchRPC", RpcTarget.MasterClient, PhotonNetwork.AuthValues.UserId);
            }
            */
        }

        public void sendRaycast(int idSrc, int inventorySlot)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return; 
            }

            PlayerStats ps = gc.players[idSrc].PlayerStats;

            if (ps.getAttackCostStamina() > ps.getStamina())
            {
                return;
            }
            ps.useStamina();

            var ray = gc.players[idSrc].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hitInfo;
            bool touch = Physics.Raycast(ray, out hitInfo, 2f);

            if(touch)
            {
                Debug.Log(hitInfo.transform.name);
                if (hitInfo.transform.name == "pipe")
                {
                    int pipeId = hitInfo.transform.parent.GetComponent<PipeScript>().pipeIndex;
                    photonView.RPC("brokePipeRPC", RpcTarget.MasterClient, pipeId);
                }
                else if (hitInfo.transform.name == "Mob(Clone)" || hitInfo.transform.name == "GIANT_RAT_LEGACY")
                {
                    int idMob = hitInfo.transform.GetComponent<MobStats>().id;
                    gc.mobList[idMob].transform.GetChild(0).GetComponent<MobStats>().takeDamage(idSrc, inventorySlot);
                }
                else if (hitInfo.transform.name.StartsWith("Player"))
                {
                    int idHit = hitInfo.transform.gameObject.GetComponent<ExposerPlayer>().PlayerIndex;
                    hitPlayer(idSrc, idHit, inventorySlot);
                }
            }
        }

        public void hitPlayer(int idSrc, int idTarget, int inventorySlot)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            PlayerStats ps = gc.players[idSrc].PlayerStats;

            float damage = ps.getBasickAttack();
            int indexItem = gc.players[idSrc].PlayerInventory.itemInventory[inventorySlot];

            bool canBurn = false;
            bool canOiled = false;
            bool isBottle = false;
            BottleScript bottleScript;

            if (indexItem != -1)
            {
                Item item = gc.items[gc.players[idSrc].PlayerInventory.itemInventory[inventorySlot]].GetComponent<Item>();
                if (gc.playersActions[idSrc].isDamageBoosted)
                {
                    damage += 5;
                }
                damage += item.getDamage();
                
                if(item.name == "Torch")
                {
                    if(gc.players[idSrc].PlayerTorch.transform.GetChild(0).gameObject.activeSelf)
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

                    ic.brokeBottle(item.getId(), true, idSrc, inventorySlot);
                }
            }

            gc.players[idTarget].PlayerStats.takeDamage(damage);
            if(canBurn && gc.playersActions[idTarget].isOiled)
            {
                gc.playersActions[idTarget].isOiled = false;
                gc.playersActions[idTarget].isBurning = true;
                gc.playersActions[idTarget].timeLeftBurn = 5.0f;
            }
            else if(canOiled)
            {
                if(gc.playersActions[idTarget].isBurning)
                {
                    gc.playersActions[idTarget].timeLeftBurn = 5.0f;
                }
                else
                {
                    gc.playersActions[idTarget].isOiled = true;
                    gc.playersActions[idTarget].timeLeftOiled = 10.0f;
                }
            }
        }

        public void HitByThrowItem(int idPlayer, int idItem, int type, bool isBurn)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            float damage = 0.0f;

            if (idItem != -1)
            {
                Item item = gc.items[idItem].GetComponent<Item>();
                damage = item.damage;
            }
            else if(type == (int)ItemController.TypeItem.Torch && isBurn)
            {
                damage = 10.0f;
            }
            gc.players[idPlayer].PlayerStats.takeDamage(damage);

            if((type == (int)ItemController.TypeItem.Torch && gc.items[idItem].transform.GetChild(0).gameObject.activeSelf) || (type == (int)ItemController.TypeItem.ToiletPaper && isBurn) )
            {
                if(gc.playersActions[idPlayer].isOiled)
                {
                    gc.playersActions[idPlayer].isOiled = false;
                    gc.playersActions[idPlayer].isBurning = true;
                    gc.playersActions[idPlayer].timeLeftBurn = 8.0f;
                }
                else if((type == (int)ItemController.TypeItem.ToiletPaper && isBurn))
                {
                    gc.playersActions[idPlayer].isBurning = true;
                    gc.playersActions[idPlayer].timeLeftBurn = 3.0f;
                }
            }
        }

        [PunRPC]
        private void brokePipeRPC(int pipeId)
        {
            //TODO verify coord
            photonView.RPC("brokeSpecificPipeRPC", RpcTarget.All, pipeId);
        }

        [PunRPC]
        private void brokeSpecificPipeRPC(int pipeId)
        {
            gc.pipes[pipeId].GetComponent<PipeScript>().brokePipe();
        }

        [PunRPC]
        private void MobDeathAll(int mobID, int type)
        {
            MobStats stats = gc.mobList[mobID].transform.GetChild(0).GetComponent<MobStats>();
            stats.isDead = true;
            if (!stats.isRotateMob)
            {
                stats.rotateDeadMob();
                stats.lootSkill(type);
                stats.gameObject.SetActive(false);
            }
        }
    }
}
