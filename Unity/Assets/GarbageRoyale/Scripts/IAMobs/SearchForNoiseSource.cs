using System;
using GarbageRoyale.Scripts.PlayerController;
using UnityEngine;
using UnityEngine.AI;

namespace GarbageRoyale.Scripts.IAMobs
{
    public class SearchForNoiseSource : StateMachineBehaviour
    {
        private const float PathUpdateInterval = 0.3f;
        private GameObject soundBlock;
        private NavMeshAgent agent;
        private float lastUpdateTime = 0f;
        private GameController gc;

        private void Awake()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(soundBlock == null)
            {
                soundBlock = ObjectPoolerSoundBlocks.SharedInstance.GetPooledObjectWithID(animator.GetInteger("PlayerID"),animator.GetInteger("BlockID"));
                agent = animator.gameObject.GetComponent(typeof(NavMeshAgent)) as NavMeshAgent;
            }
        }
    
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log(agent.remainingDistance);
            if(Time.time > lastUpdateTime + PathUpdateInterval && (agent.remainingDistance > 0.2f || Math.Abs(agent.remainingDistance) < 0.00001f))
            {
                lastUpdateTime = Time.time;
                agent.SetDestination(soundBlock.transform.position);
            } else {
                soundBlock.GetComponent<SoundBlockController>().hasBeenSeenByMob = true;
                animator.SetBool("HasHeardNoise",false);
            }
        }
    }
}
