using System;
using System.Collections.Generic;
using GarbageRoyale.Scripts.HUD;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class SkillsController : MonoBehaviourPunCallbacks
    {
        private GameController gc;
        //private GameObject InvGUI;

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
        
        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            //InvGUI = GameObject.Find("InventoryGUI");
        }
        

        // Update is called once per frame
        void Update()
        {
            /*if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }*/

            isDouble = false;
            foreach (var skillManager in CurrentSkills)
            {
                if (skillManager.isActive == true)
                {
                    if (skillManager.bufftime <= 0)
                    {
                        //Debug.Log("hai " + skillManager.bufftime);
                        switch (skillManager.skillType)
                        {
                            case 0:
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
                                                Debug.Log("deact");
                                                photonView.RPC("DeactivateQuietSkill", RpcTarget.All,skillManager.playerID);
                                                skillManager.isActive = false;
                                                break;
                                            }
                                        }
                                    }

                                    if (!isDouble)
                                    {
                                        Debug.Log("deact2");
                                        photonView.RPC("DeactivateQuietSkill", RpcTarget.All, skillManager.playerID);
                                        skillManager.isActive = false;
                                    }
                                }
                                /*Debug.Log(skillManager.playerID + " " + Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId));*/
                                //CurrentSkills.Remove(skillManager);
                                break;
                        }
                    }
                }
                if (skillManager.playerID ==
                    Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
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
                        if (skillManager.bufftime > 0)
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
            Debug.Log("hoi");
            ActiveSkillManager newSkill;
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            skillID = gc.players[playerIndex].GetComponent<Inventory>().skillInventory[skillPlace];
            Skill skillInfos = gc.items[skillID].GetComponent<Skill>();
            skillType = skillInfos.type;
            foreach (var skillManager in CurrentSkills)
            {
                if (skillManager.skillType ==  skillType && skillManager.skillID == skillID &&
                    skillManager.playerID == playerIndex)
                {
                    if (skillManager.coolDown > 0)
                    {
                        return;
                    }

                    foundSkill = skillManager;
                }
            }
            Debug.Log(skillID);
            switch (skillType)
            {
                case 0:
                    if (foundSkill)
                    {
                        newSkill = foundSkill;
                        foundSkill = null;
                    }
                    else
                    {
                        newSkill = gameObject.AddComponent<ActiveSkillManager>();
                    }
                    Debug.Log("yees");
                    newSkill.playerID = playerIndex;
                    newSkill.bufftime = skillInfos.bufftime;
                    newSkill.coolDown = skillInfos.cooldown;
                    newSkill.skillType = skillInfos.type;
                    newSkill.skillID = skillID;
                    newSkill.skillPlace = skillPlace;
                    newSkill.isActive = true;
                    CurrentSkills.Add(newSkill);
                    photonView.RPC("ActivateQuietSkill",RpcTarget.All, playerIndex,skillPlace);
                    break;
            }
            //photonView.RPC("AnswerSkillActivation",RpcTarget.All,skillType, playerIndex);
        }
        
        [PunRPC]
        public void ActivateQuietSkill(int playerIndex,int skillPlace)
        {
            Debug.Log("wut1");
            skillID = gc.players[playerIndex].GetComponent<Inventory>().skillInventory[skillPlace];
            Skill skillInfos = gc.items[skillID].GetComponent<Skill>();
            gc.playersActions[playerIndex].isQuiet = true;
            Debug.Log("wut2");
            
            if (playerIndex == Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId))
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    ActiveSkillManager newSkill;
                    skillType = gc.players[playerIndex].GetComponent<Inventory>().skillInventory[skillPlace];
                    foreach (var skillManager in CurrentSkills)
                    {
                        if (skillManager.skillType == skillType && skillManager.skillID == skillPlace &&
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
                    SkillBuff_0.GetComponent<RawImage>().texture = buffTextures[skillInfos.type].texture;
                    SkillBuffTime_0.GetComponent<Text>().text =skillInfos.bufftime.ToString();
                }
                else
                {
                    SkillBuff_1.GetComponent<RawImage>().texture = buffTextures[skillInfos.type].texture;
                    SkillBuffTime_1.GetComponent<Text>().text =skillInfos.bufftime.ToString();
                }
            }
            Debug.Log("wut");
            
        }

        [PunRPC]
        public void DeactivateQuietSkill(int playerIndex)
        {
            gc.playersActions[playerIndex].isQuiet = false;
        }
    }
}
