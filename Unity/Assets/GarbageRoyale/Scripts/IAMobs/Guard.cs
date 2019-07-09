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
        private int blockIDHash;
        private int hasHeardNoiseHash;

        [SerializeField] 
        private Animator ratAnimator;
        
        public Vector3 GuardPosition { get { return guardPosition; } }
        public int ReachedGuardPointHash { get { return reachedGuardPointHash; } }
        
        private GameController gc;

        [SerializeField] 
        private SoundDetector QuietSoundDetector;
        [SerializeField] 
        private SoundDetector MidLevelSoundDetector;
        [SerializeField] 
        private SoundDetector LoudSoundDetector;
        
        private void Start()
        {
            guardPosition = transform.position;
            ratState = GetComponent(typeof(Animator)) as Animator;
            playerOnSightHash = Animator.StringToHash("PlayerOnSight");
            reachedGuardPointHash = Animator.StringToHash("ReachedGuardPoint");
            isMovingHash = Animator.StringToHash("isMoving");
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
                ratAnimator.SetBool(isMovingHash,true);
                Debug.Log("player");
            } 
            /*else if(LoudSoundDetector.hasDetectedSound && !ratState.GetBool(playerOnSightHash))
            {
                ratState.SetInteger(blockIDHash,LoudSoundDetector.detectedBlockId);
                ratState.SetInteger(playerIDHash,LoudSoundDetector.detectedBlockPlayerId);
                ratState.SetBool(hasHeardNoiseHash,true);
                Debug.Log("loudsound");
            } 
            else if (MidLevelSoundDetector.hasDetectedSound && !ratState.GetBool(playerOnSightHash))
            {
                ratState.SetInteger(blockIDHash,LoudSoundDetector.detectedBlockId);
                ratState.SetInteger(playerIDHash,LoudSoundDetector.detectedBlockPlayerId);
                ratState.SetBool(hasHeardNoiseHash,true);
                Debug.Log("midsound");
            }
            else if (QuietSoundDetector.hasDetectedSound && !ratState.GetBool(playerOnSightHash))
            {
                ratState.SetInteger(blockIDHash,LoudSoundDetector.detectedBlockId);
                ratState.SetInteger(playerIDHash,LoudSoundDetector.detectedBlockPlayerId);
                ratState.SetBool(hasHeardNoiseHash,true);
                Debug.Log("quietsound");
            }*/
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
                Debug.Log("midsound");
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
