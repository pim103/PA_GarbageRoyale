using System;
using System.Runtime.CompilerServices;
using GarbageRoyale.Scripts.PrefabPlayer;
using UnityEngine;

namespace GarbageRoyale.Scripts.IAMobs
{
    public class Guard : MonoBehaviour
    {
        private Vector3 guardPosition;
        private Animator ratState;
        private int playerOnSightHash;
        private int reachedGuardPointHash;
        private int isMovingHash;
        private int playerIDHash;

        [SerializeField] 
        private Animator ratAnimator;
        public Vector3 GuardPosition { get { return guardPosition; } }
        public int ReachedGuardPointHash { get { return reachedGuardPointHash; } }
        public GameObject detectedPlayer;

        private GameController gc;
        
        private void Start()
        {
            guardPosition = transform.position;
            ratState = GetComponent(typeof(Animator)) as Animator;
            playerOnSightHash = Animator.StringToHash("PlayerOnSight");
            reachedGuardPointHash = Animator.StringToHash("ReachedGuardPoint");
            isMovingHash = Animator.StringToHash("isMoving");
            playerIDHash = Animator.StringToHash("PlayerID");
        }
        private void OnTriggerEnter(Collider collider)
        {
            if(collider.CompareTag("Player"))
            {
                ratState.SetBool(playerOnSightHash, true);
                ratState.SetInteger(playerIDHash,collider.gameObject.GetComponent<ExposerPlayer>().PlayerIndex);
                ratAnimator.SetBool(isMovingHash,true);
                detectedPlayer = collider.gameObject;
            } 
            else if(collider.CompareTag("LoudSoundBlock"))
            {
                
            } 
            else if (collider.CompareTag("MidLevelSoundBlock"))
            {
                
            }
            else if (collider.CompareTag("QuietSoundBlock"))
            {
                
            }
        }
        private void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                ratState.SetBool(playerOnSightHash, false);
            }
        }

        private void Update()
        {
            if (ratState.GetBool(reachedGuardPointHash))
            {
                ratAnimator.SetBool(isMovingHash,false);
            }
        }
    }
}
