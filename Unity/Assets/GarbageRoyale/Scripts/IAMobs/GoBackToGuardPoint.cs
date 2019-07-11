using UnityEngine;
using UnityEngine.AI;

namespace GarbageRoyale.Scripts.IAMobs
{
    public class GoBackToGuardPoint : StateMachineBehaviour {
        private NavMeshAgent agent;
        private Guard manager;
    
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(agent == null)
            {
                agent = animator.gameObject.GetComponent(typeof(NavMeshAgent)) as NavMeshAgent;
                manager = animator.gameObject.GetComponent(typeof(Guard)) as Guard;
            }
            agent.SetDestination(manager.GuardPosition);
        }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (agent.remainingDistance < 0.2f)
            {
                animator.SetTrigger(manager.ReachedGuardPointHash);
            }
        }
    }
}
