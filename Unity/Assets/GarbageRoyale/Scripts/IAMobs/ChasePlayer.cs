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
        private int oldPlayerId;
        
        private void Awake()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(player == null)
            {
                oldPlayerId = animator.GetInteger("PlayerID");
                player = gc.players[oldPlayerId].PlayerGameObject;
                agent = animator.gameObject.GetComponent<NavMeshAgent>();
            }
        }
    
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(Time.time > lastUpdateTime + (PathUpdateInterval)){ 
                lastUpdateTime = Time.time;
                if (animator.GetInteger("PlayerID") != oldPlayerId)
                {
                    oldPlayerId = animator.GetInteger("PlayerID");
                    player = gc.players[oldPlayerId].PlayerGameObject;
                }
                agent.SetDestination(player.transform.position);
                if(agent.remainingDistance < 2.5f && Math.Abs(agent.remainingDistance) > 0.00001f)
                {
                    animator.SetBool("isCloseToPlayer", true);
                }
            }
        }
    }
}