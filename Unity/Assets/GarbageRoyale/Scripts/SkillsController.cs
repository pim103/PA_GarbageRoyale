using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class SkillsController : MonoBehaviourPunCallbacks
    {
        private GameController gc;

        private int skillID;
        private int skillType = 0;
        

        private List<ActiveSkillManager> CurrentSkills = new List<ActiveSkillManager>();
        private ActiveSkillManager foundSkill;
        private bool isDouble = false;
        
        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
        }
        

        // Update is called once per frame
        void Update()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            isDouble = false;
            foreach (var skillManager in CurrentSkills)
            {
                if (skillManager.isActive == true)
                {
                    if (skillManager.bufftime <= 0)
                    {
                        Debug.Log("hai " + skillManager.bufftime);
                        switch (skillManager.skillType)
                        {
                            case 0:
                                foreach (var otherSkillManager in CurrentSkills)
                                {
                                    if (otherSkillManager.skillType == skillManager.skillType &&
                                        otherSkillManager.skillID != skillManager.skillID)
                                    {
                                        isDouble = true;
                                        if (otherSkillManager.bufftime <= 0)
                                        {
                                            Debug.Log("deact");
                                            photonView.RPC("DeactivateQuietSkill", RpcTarget.All,
                                                skillManager.playerID);
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

                                //CurrentSkills.Remove(skillManager);
                                break;
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
            if (playerIndex == Array.IndexOf(gc.AvatarToUserId,PhotonNetwork.AuthValues.UserId) && !PhotonNetwork.IsMasterClient)
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
                newSkill.isActive = true;
                CurrentSkills.Add(newSkill);
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
