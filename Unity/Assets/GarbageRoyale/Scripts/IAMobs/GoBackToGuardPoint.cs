using UnityEngine;
using UnityEngine.AI;

namespace GarbageRoyale.Scripts.IAMobs
{
    public class GoBackToGuardPoint : StateMachineBehaviour {
        private NavMeshAgent _agent;
        private Guard _manager;
        [SerializeField] 
        private Animator ratAnimator;
        private int isMovingHash;
    
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(_agent == null)
            {
                _agent = animator.gameObject.GetComponent(typeof(NavMeshAgent)) as NavMeshAgent;
                _manager = animator.gameObject.GetComponent(typeof(Guard)) as Guard;
                isMovingHash = Animator.StringToHash("isMoving");
            }
            _agent.SetDestination(_manager.GuardPosition);
        }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_agent.remainingDistance < 0.2f)
            {
                animator.SetTrigger(_manager.ReachedGuardPointHash);
            }
        }
    }
}
