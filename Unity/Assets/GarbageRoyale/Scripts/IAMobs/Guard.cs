using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

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
        private int isAttackingHash;

        [SerializeField] 
        public Animator ratAnimator;

        [SerializeField] 
        public MobStats mstats;
        
        public Vector3 GuardPosition { get { return guardPosition; } }
        public int ReachedGuardPointHash { get { return reachedGuardPointHash; } }
        
        private MobController mc;

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
            mc = GameObject.Find("Controller").GetComponent<MobController>();
            if (PhotonNetwork.IsMasterClient)
            {
                guardPosition = transform.position;
                ratState = GetComponent<Animator>();
                playerOnSightHash = Animator.StringToHash("PlayerOnSight");
                reachedGuardPointHash = Animator.StringToHash("ReachedGuardPoint");
                isMovingHash = Animator.StringToHash("isMoving");
                isRunningHash = Animator.StringToHash("isRunning");
                playerIDHash = Animator.StringToHash("PlayerID");
                blockIDHash = Animator.StringToHash("BlockID");
                hasHeardNoiseHash = Animator.StringToHash("HasHeardNoise");
                isAttackingHash = Animator.StringToHash("isAttacking");
            }
            else
            {
                GetComponent<NavMeshAgent>().enabled = false;
            }
        }
        private void OnTriggerEnter(Collider collider)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (collider.CompareTag("Player"))
                {
                    ratState.SetBool(playerOnSightHash, true);
                    ratState.SetInteger(playerIDHash, collider.gameObject.GetComponent<ExposerPlayer>().PlayerIndex);
                    ratAnimator.SetBool(isRunningHash, true);
                    ratAnimator.SetBool(isMovingHash, true);
                    //Debug.Log("player");
                }
            }
        }
        private void OnTriggerExit(Collider collider)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (collider.CompareTag("Player"))
                {
                    ratState.SetBool(playerOnSightHash, false);
                }
            }
        }

        public void SoundHeard(int detectedBlockId, int detectedBlockPlayerId)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (!ratState.GetBool(playerOnSightHash))
                {
                    ratState.SetInteger(blockIDHash, detectedBlockId);
                    ratState.SetInteger(playerIDHash, detectedBlockPlayerId);
                    ratState.SetBool(hasHeardNoiseHash, true);
                    ratAnimator.SetBool(isMovingHash, true);
                }
            }
        }
        
        private void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (ratState.GetBool(reachedGuardPointHash))
                {
                    ratAnimator.SetBool(isRunningHash, false);
                    ratAnimator.SetBool(isMovingHash, false);
                }
                /*Debug.Log(mstats.id);
                Debug.Log(mc.mobsPosX[mstats.id]);*/
                mc.mobsPosX[mstats.id] = transform.position.x;
                mc.mobsPosY[mstats.id] = transform.position.y;
                mc.mobsPosZ[mstats.id] = transform.position.z;
                mc.mobsRotY[mstats.id] = transform.eulerAngles.y;
                mc.mobsAnimState[mstats.id] = 0;
                
                
            }
            else
            {
                switch (mc.mobsAnimState[mstats.id])
                {
                    case 0:
                        ratAnimator.Play("idle");
                        break;
                    case 1:
                        ratAnimator.Play("walk");
                        break;
                    case 2:
                        ratAnimator.Play("run");
                        break;
                    case 3:
                        ratAnimator.Play("jumpBite");
                        break;
                }
                transform.position = new Vector3(mc.mobsPosX[mstats.id],mc.mobsPosY[mstats.id],mc.mobsPosZ[mstats.id]);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x,mc.mobsRotY[mstats.id],transform.eulerAngles.z);
            }
        }

        public void startAttack()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(ActivateAttackZone());
            }
        }
        IEnumerator ActivateAttackZone()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                ratAnimator.SetBool(isAttackingHash, true);
                yield return new WaitForSeconds(1f);
                attackZone.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                attackZone.gameObject.SetActive(false);
                ratAnimator.SetBool(isAttackingHash, false);
            }
        }
    }
}
