using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GarbageRoyale.Scripts.IAMobs
{
    public class AttackPlayer : StateMachineBehaviour
    {
        private const float PathUpdateInterval = 1f;
        private GameObject player;
        private NavMeshAgent agent;
        private float lastUpdateTime = 0f;
        private GameController gc;
        private Guard manager;
        
        private void Awake()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(player == null)
            {
                player = gc.players[animator.GetInteger("PlayerID")].PlayerGameObject;
                agent = animator.gameObject.GetComponent<NavMeshAgent>();
                manager = animator.gameObject.GetComponent<Guard>();
            }
        }
    
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            if (Time.time > lastUpdateTime + PathUpdateInterval)
            {
                agent.SetDestination(player.transform.position);
                lastUpdateTime = Time.time;
                if (agent.remainingDistance < 2f || Math.Abs(agent.remainingDistance) < 0.00001f)
                {
                    agent.isStopped = true;
                    manager.attackZone.gameObject.SetActive(true);
                    manager.startAttack();
                    
                    //gc.players[animator.GetInteger("PlayerID")].PlayerStats.takeDamage(1f);
                    //Debug.Log("attack : " + agent.remainingDistance);
                }
                else
                {
                    //Debug.Log("not attack : " + agent.remainingDistance);
                    agent.isStopped = false;
                    animator.SetBool("isCloseToPlayer", false);
                }
            }
        }
    }
}
