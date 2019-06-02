using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class SkillsController : MonoBehaviourPunCallbacks
    {
        private GameController gc;

        private int skillType;

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
                if (skillManager.bufftime <= 0)
                {
                    Debug.Log("hai "+skillManager.bufftime);
                    switch (skillManager.skillType)
                    {
                        case 0:
                            foreach (var otherSkillManager in CurrentSkills)
                            {
                                if (otherSkillManager.skillType == skillManager.skillType &&
                                    otherSkillManager != skillManager)
                                {
                                    isDouble = true;
                                    if (otherSkillManager.bufftime <= 0)
                                    {
                                        Debug.Log("deact2");
                                        photonView.RPC("DeactivateQuietSkill",RpcTarget.All, skillManager.playerID);
                                        break;
                                    } 
                                }
                            }

                            if (!isDouble)
                            {
                                Debug.Log("deact2");
                                photonView.RPC("DeactivateQuietSkill",RpcTarget.All, skillManager.playerID);
                            }
                            //CurrentSkills.Remove(skillManager);
                            break;
                    }
                }
            }
        }

        [PunRPC]
        public void AskSkillActivation(int skillPlace, int playerIndex)
        {
            ActiveSkillManager newSkill;
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

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
                        newSkill = new ActiveSkillManager();
                    }
                    Debug.Log("yees");
                    newSkill.playerID = playerIndex;
                    newSkill.bufftime = 10;
                    newSkill.coolDown = 5;
                    newSkill.skillType = 0;
                    newSkill.skillID = skillPlace;
                    CurrentSkills.Add(newSkill);
                    photonView.RPC("ActivateQuietSkill",RpcTarget.All, playerIndex);
                    break;
            }
            //photonView.RPC("AnswerSkillActivation",RpcTarget.All,skillType, playerIndex);
        }
        
        [PunRPC]
        public void ActivateQuietSkill(int playerIndex)
        {
            gc.playersActions[playerIndex].isQuiet = true;
            if (playerIndex == Array.IndexOf(gc.AvatarToUserId,PhotonNetwork.AuthValues.UserId))
            {
                ActiveSkillManager newSkill = new ActiveSkillManager();
                newSkill.playerID = playerIndex;
                newSkill.bufftime = 10;
                newSkill.coolDown = 5;
                newSkill.skillType = 0;
                CurrentSkills.Add(newSkill);
            }
        }

        [PunRPC]
        public void DeactivateQuietSkill(int playerIndex)
        {
            gc.playersActions[playerIndex].isQuiet = false;
        }
    }
}
