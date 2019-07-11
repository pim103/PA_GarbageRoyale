using System;
using System.Collections;
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
        private int isRunningHash;
        private int playerIDHash;
        private int blockIDHash;
        private int hasHeardNoiseHash;

        [SerializeField] 
        public Animator ratAnimator;
        
        public Vector3 GuardPosition { get { return guardPosition; } }
        public int ReachedGuardPointHash { get { return reachedGuardPointHash; } }
        
        private GameController gc;

        [SerializeField] 
        private SoundDetector QuietSoundDetector;
        [SerializeField] 
        private SoundDetector MidLevelSoundDetector;
        [SerializeField] 
        private SoundDetector LoudSoundDetector;

        [SerializeField]
        public GameObject attackZone;
        
        private void Start()
        {
            guardPosition = transform.position;
            ratState = GetComponent(typeof(Animator)) as Animator;
            playerOnSightHash = Animator.StringToHash("PlayerOnSight");
            reachedGuardPointHash = Animator.StringToHash("ReachedGuardPoint");
            isMovingHash = Animator.StringToHash("isMoving");
            isRunningHash = Animator.StringToHash("isRunning");
            playerIDHash = Animator.StringToHash("PlayerID");
            blockIDHash = Animator.StringToHash("BlockID");
            hasHeardNoiseHash = Animator.StringToHash("HasHeardNoise");
        }
        private void OnTriggerEnter(Collider collider)
        {
            if(collider.CompareTag("Player"))
            {
                ratState.SetBool(playerOnSightHash, true);
                ratState.SetInteger(playerIDHash,collider.gameObject.GetComponent<ExposerPlayer>().PlayerIndex);
                ratAnimator.SetBool(isRunningHash,true);
                ratAnimator.SetBool(isMovingHash,true);
                //Debug.Log("player");
            } 
        }
        private void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                ratState.SetBool(playerOnSightHash, false);
            }
        }

        public void SoundHeard(int detectedBlockId, int detectedBlockPlayerId)
        {
            if(!ratState.GetBool(playerOnSightHash)){
                ratState.SetInteger(blockIDHash, detectedBlockId);
                ratState.SetInteger(playerIDHash, detectedBlockPlayerId);
                ratState.SetBool(hasHeardNoiseHash, true);
                ratAnimator.SetBool(isMovingHash,true);
            }
        }
        
        private void Update()
        {
            if (ratState.GetBool(reachedGuardPointHash))
            {
                ratAnimator.SetBool(isRunningHash,false);
                ratAnimator.SetBool(isMovingHash,false);
            }
        }

        public void startAttack()
        {
            StartCoroutine(ActivateAttackZone());
        }
        IEnumerator ActivateAttackZone()
        {
            yield return new WaitForSeconds(0.5f);
            attackZone.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            attackZone.gameObject.SetActive(false);
        }
    }
}
