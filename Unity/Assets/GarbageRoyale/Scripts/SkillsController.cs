using System;
using System.Collections.Generic;
using GarbageRoyale.Scripts.Environment;
using GarbageRoyale.Scripts.HUD;
using GarbageRoyale.Scripts.InventoryScripts;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class SkillsController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameController gc;

        [SerializeField]
        private EnvironmentController ec;

        private int skillID;
        private int skillType = 0;

        private List<ActiveSkillManager> CurrentSkills = new List<ActiveSkillManager>();
        private ActiveSkillManager foundSkill;
        private bool isDouble = false;

        [SerializeField] 
        private GameObject SkillCD_0;
        [SerializeField] 
        private GameObject SkillCD_1;
        [SerializeField] 
        private GameObject SkillBuff_0;
        [SerializeField] 
        private GameObject SkillBuff_1;
        [SerializeField] 
        private GameObject SkillBuffTime_0;
        [SerializeField] 
        private GameObject SkillBuffTime_1;
        [SerializeField] 
        private RawImage[] buffTextures;

        [SerializeField] 
        private GameObject Water;

        public enum SkillType
        {
            QuietSound,
            StaffMaster,
            Invisibility,
            Tazer,
            AquaticBreath,
            Dash,
            IceWall,
            All
        }

        // Update is called once per frame
        void Update()
        {
            isDouble = false;
            foreach (var skillManager in CurrentSkills)
            {
                if (skillManager.isActive == true)
                {
                    if (skillManager.bufftime <= 0)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            foreach (var otherSkillManager in CurrentSkills)
                            {
                                if (otherSkillManager.skillType == skillManager.skillType &&
                                    otherSkillManager.skillID != skillManager.skillID)
                                {
                                    isDouble = true;
                                    if (otherSkillManager.bufftime <= 0)
                                    {
                                        photonView.RPC("DeactivateSkill", RpcTarget.All,skillManager.playerID, skillManager.skillType);
                                        skillManager.isActive = false;
                                        break;
                                    }
                                }
                            }

                            if (!isDouble)
                            {
                                photonView.RPC("DeactivateSkill", RpcTarget.All, skillManager.playerID, skillManager.skillType);
                                skillManager.isActive = false;
                            }
                        }
                        /*Debug.Log(skillManager.playerID + " " + Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));*/
                        //CurrentSkills.Remove(skillManager);
                        
                        
                    }
                }
                if (skillManager.playerID == Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
                {
                    //Debug.Log(skillManager.coolDown);
                    if (skillManager.skillPlace == 0)
                    {
                        if (skillManager.coolDown > 0)
                        {
                            SkillCD_0.GetComponent<Text>().text = skillManager.coolDown.ToString();
                        }
                        else
                        {
                            SkillCD_0.GetComponent<Text>().text = "";
                        }
                        if (skillManager.bufftime > 0 && skillManager.skillType!=3 && skillManager.skillType !=5)
                        {
                            SkillBuffTime_0.GetComponent<Text>().text = skillManager.bufftime.ToString();
                        }
                        else
                        {
                            SkillBuffTime_0.GetComponent<Text>().text = "";
                            SkillBuff_0.GetComponent<RawImage>().texture = null;
                        }
                    }
                    if (skillManager.skillPlace == 1)
                    {
                        if(skillManager.coolDown > 0)
                        {
                            SkillCD_1.GetComponent<Text>().text = skillManager.coolDown.ToString();
                        }
                        else
                        {
                            SkillCD_1.GetComponent<Text>().text = "";
                        }
                        if(skillManager.bufftime > 0)
                        {
                            SkillBuffTime_1.GetComponent<Text>().text = skillManager.bufftime.ToString();
                        }
                        else
                        {
                            SkillBuffTime_1.GetComponent<Text>().text = "";
                            SkillBuff_1.GetComponent<RawImage>().texture = null;
                        }
                    }
                }
            }
        }

        [PunRPC]
        public void AskSkillActivation(int skillPlace, int playerIndex)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            var ray = gc.players[playerIndex].PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hitInfo;
            bool touch = Physics.Raycast(ray, out hitInfo, 2f);
            int targetID= -1;

            if (touch)
            {
                if (hitInfo.transform.name.StartsWith("Player"))
                {
                    targetID = hitInfo.transform.gameObject.GetComponent<ExposerPlayer>().PlayerIndex;
                }
            }

            ActiveSkillManager newSkill;

            skillID = gc.players[playerIndex].GetComponent<Inventory>().skillInventory[skillPlace];
            if(skillID == -1)
            {
                return;
            }

            Skill skillInfos = gc.items[skillID].GetComponent<Skill>();
            skillType = skillInfos.type;
            
            foreach (var skillManager in CurrentSkills)
            {
                if (skillManager.skillType == skillType && skillManager.skillID == skillID &&
                    skillManager.playerID == playerIndex)
                {
                    if (skillManager.coolDown > 0)
                    {
                        return;
                    }

                    foundSkill = skillManager;
                }
            }

            if (foundSkill)
            {
                newSkill = foundSkill;
                foundSkill = null;
            }
            else
            {
                newSkill = gameObject.AddComponent<ActiveSkillManager>();
            }

            newSkill.playerID = playerIndex;
            newSkill.bufftime = skillInfos.bufftime;
            newSkill.coolDown = skillInfos.cooldown;
            newSkill.skillType = skillInfos.type;
            newSkill.skillID = skillID;
            newSkill.skillPlace = skillPlace;
            newSkill.isActive = true;
            CurrentSkills.Add(newSkill);
            photonView.RPC("ActivateSkill",RpcTarget.All, playerIndex,skillPlace,skillID,targetID);
            //photonView.RPC("AnswerSkillActivation",RpcTarget.All,skillType, playerIndex);
        }
        
        [PunRPC]
        public void ActivateSkill(int playerIndex,int skillPlace, int skillID, int targetID)
        {
            Skill skillInfos = gc.items[skillID].GetComponent<Skill>();
            skillType = skillInfos.type;
            switch ((SkillType)skillType)
            {
                case SkillType.QuietSound:
                    gc.playersActions[playerIndex].isQuiet = true;
                    break;
                case SkillType.StaffMaster:
                    gc.playersActions[playerIndex].isDamageBoosted = true;
                    break;
                case SkillType.Invisibility:
                    gc.players[playerIndex].PlayerRenderer.enabled = false;
                    break;
                case SkillType.Tazer:
                    if (targetID != -1)
                    {
                        gc.playersActions[targetID].isFallen = true;
                        gc.playersActions[targetID].timeLeftFallen = 2.0f;
                        if (gc.playersActions[playerIndex].feetIsInWater || gc.playersActions[playerIndex].headIsInWater)
                        {
                            GameObject electricity = ObjectPooler.SharedInstance.GetPooledObject(12);
                            electricity.transform.position = gc.players[targetID].PlayerFeet.transform.position;
                            electricity.SetActive(true);
                            electricity.transform.parent = Water.transform;
                            electricity.transform.localPosition =  new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
                        }
                    }
                    break;
                case SkillType.AquaticBreath:
                    gc.playersActions[playerIndex].isAmphibian = true;
                    break;
                case SkillType.Dash:
                    var forward = gc.players[playerIndex].PlayerCamera.transform.forward;
 
                    forward.y = 0f;
                    forward.Normalize();
                    
                    gc.moveDirection[playerIndex] = forward;
                    gc.moveDirection[playerIndex] *= 5;

                    //gc.players[playerIndex].PlayerChar.Move(gc.moveDirection[playerIndex]);
                    if (gc.playersActions[playerIndex].isInWater || gc.playersActions[playerIndex].headIsInWater || gc.playersActions[playerIndex].isInWater)
                    {
                        GameObject electricity = ObjectPooler.SharedInstance.GetPooledObject(12);
                        electricity.transform.position = gc.players[playerIndex].PlayerFeet.transform.position;
                        electricity.SetActive(true);
                        electricity.transform.parent = Water.transform;
                        //electricity.transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
                    }
                    break;
                case SkillType.IceWall:
                    Vector3 pos = Vector3.forward;
                    RaycastHit info;
                    if (Physics.Raycast(gc.players[playerIndex].PlayerFeet.transform.position, Vector3.down, out info))
                    {
                        pos = info.point + (Vector3.up) + (gc.players[playerIndex].PlayerCamera.transform.forward * 2);
                    }

                    GameObject iceWall = ObjectPooler.SharedInstance.GetPooledObject(22);
                    iceWall.transform.position = pos;
                    Vector3 rot = iceWall.transform.localEulerAngles;
                    rot.y += gc.players[playerIndex].PlayerGameObject.transform.localEulerAngles.y;
                    iceWall.transform.localEulerAngles = rot;
                    iceWall.SetActive(true);

                    IceWallScript iws = iceWall.GetComponent<IceWallScript>();
                    iws.ActiveIceWall();
                    iws.id = ec.iceWalls.Count;
                    ec.iceWalls.Add(ec.iceWalls.Count, iceWall);
                    break;
                default:
                    break;
            }
            
            if (playerIndex == Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    ActiveSkillManager newSkill;
                    foreach (var skillManager in CurrentSkills)
                    {
                        if (skillManager.skillType == skillType && skillManager.skillID == skillID &&
                            skillManager.playerID == playerIndex)
                        {
                            if (skillManager.coolDown > 0)
                            {
                                return;
                            }

                            foundSkill = skillManager;
                        }
                    }
                    if (foundSkill)
                    {
                        newSkill = foundSkill;
                        foundSkill = null;
                    }
                    else
                    {
                        newSkill = gameObject.AddComponent<ActiveSkillManager>();
                    }
                    //ActiveSkillManager newSkill = gameObject.AddComponent<ActiveSkillManager>();
                    newSkill.playerID = playerIndex;
                    newSkill.bufftime = skillInfos.bufftime;
                    newSkill.coolDown = skillInfos.cooldown;
                    newSkill.skillType = skillInfos.type;
                    newSkill.skillID = skillID;
                    newSkill.skillPlace = skillPlace;
                    newSkill.isActive = true;
                    CurrentSkills.Add(newSkill);
                }
                if (skillPlace == 0)
                {
                    if (skillInfos.type != 3 && skillInfos.type != 5)
                    {
                        SkillBuff_0.GetComponent<RawImage>().texture = buffTextures[skillInfos.type].texture;
                        SkillBuffTime_0.GetComponent<Text>().text =skillInfos.bufftime.ToString();
                    }
                }
                else
                {
                    SkillBuff_1.GetComponent<RawImage>().texture = buffTextures[skillInfos.type].texture;
                    SkillBuffTime_1.GetComponent<Text>().text =skillInfos.bufftime.ToString();
                }
            }
        }

        [PunRPC]
        public void DeactivateSkill(int playerIndex, int type)
        {
            switch ((SkillType)type)
            {
                case SkillType.QuietSound:
                    gc.playersActions[playerIndex].isQuiet = false;
                    break;
                case SkillType.StaffMaster:
                    gc.playersActions[playerIndex].isDamageBoosted = false;
                    break;
                case SkillType.Invisibility:
                    gc.players[playerIndex].PlayerRenderer.enabled = true;
                    break;
                case SkillType.AquaticBreath:
                    gc.playersActions[playerIndex].isAmphibian = false;
                    break;
            }
        }

        public bool ReduceCooldown(int playerId)
        {
            bool findSkillInCd = false;

            foreach (var skillManager in CurrentSkills)
            {
                if (skillManager.playerID == playerId)
                {
                    if (skillManager.coolDown > 0)
                    {
                        findSkillInCd = true;
                        skillManager.coolDown = skillManager.coolDown > 10 ? skillManager.coolDown - 10 : 0;
                    }
                }
            }

            return findSkillInCd;
        }
    }
}
