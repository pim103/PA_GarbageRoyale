using System;
using UnityEngine;
using UnityEngine.AI;

namespace GarbageRoyale.Scripts.IAMobs
{
    public class ChasePlayer : StateMachineBehaviour
    {
        private const float PathUpdateInterval = 0.3f;
        private GameObject player;
        private NavMeshAgent agent;
        private float lastUpdateTime = 0f;
        private GameController gc;
        
        
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
            }
        }
    
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(Time.time > lastUpdateTime + (PathUpdateInterval)){ 
                lastUpdateTime = Time.time;
                agent.SetDestination(player.transform.position);
                if(agent.remainingDistance < 2f && Math.Abs(agent.remainingDistance) > 0.00001f)
                {
                    animator.SetBool("isCloseToPlayer", true);
                }
            }
        }
    }
}