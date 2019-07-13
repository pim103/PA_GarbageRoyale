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
        private GameController gc;

        [SerializeField] 
        private SoundDetector QuietSoundDetector;
        [SerializeField] 
        private SoundDetector MidLevelSoundDetector;
        [SerializeField] 
        private SoundDetector LoudSoundDetector;

        [SerializeField]
        public GameObject attackZone;

        private int oldValue;
        
        private void Start()
        {
            mc = GameObject.Find("Controller").GetComponent<MobController>();
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            if (PhotonNetwork.IsMasterClient)
            {
                guardPosition = transform.position;
                ratState = GetComponent<Animator>();
                playerOnSightHash = Animator.StringToHash("PlayerOnSight");
                reachedGuardPointHash = Animator.StringToHash("ReachedGuardPoint");
                playerIDHash = Animator.StringToHash("PlayerID");
                blockIDHash = Animator.StringToHash("BlockID");
                hasHeardNoiseHash = Animator.StringToHash("HasHeardNoise");
            }
            else
            {
                GetComponent<NavMeshAgent>().enabled = false;
            }
            isMovingHash = Animator.StringToHash("isMoving");
            isRunningHash = Animator.StringToHash("isRunning");
            isAttackingHash = Animator.StringToHash("isAttacking");
        }
        private void OnTriggerEnter(Collider collider)
        {
            if (PhotonNetwork.IsMasterClient && gc.doorIsOpen)
            {
                if (collider.CompareTag("Player"))
                {
                    ratState.SetBool(playerOnSightHash, true);
                    ratState.SetInteger(playerIDHash, collider.gameObject.GetComponent<ExposerPlayer>().PlayerIndex);
                    ratAnimator.SetBool(isRunningHash, true);
                    ratAnimator.SetBool(isMovingHash, true);
                    mc.mobsAnimState[mstats.id] = 2;
                    //Debug.Log("player");
                }
            }
        }
        private void OnTriggerExit(Collider collider)
        {
            if (PhotonNetwork.IsMasterClient && gc.doorIsOpen)
            {
                if (collider.CompareTag("Player"))
                {
                    ratState.SetBool(playerOnSightHash, false);
                }
            }
        }

        public void SoundHeard(int detectedBlockId, int detectedBlockPlayerId)
        {
            if (PhotonNetwork.IsMasterClient && gc.doorIsOpen)
            {
                if (!ratState.GetBool(playerOnSightHash))
                {
                    ratState.SetInteger(blockIDHash, detectedBlockId);
                    ratState.SetInteger(playerIDHash, detectedBlockPlayerId);
                    ratState.SetBool(hasHeardNoiseHash, true);
                    ratAnimator.SetBool(isMovingHash, true);
                    mc.mobsAnimState[mstats.id] = 1;
                }
            }
        }
        
        private void Update()
        {
            if (PhotonNetwork.IsMasterClient && gc.doorIsOpen)
            {
                if (ratState.GetBool(reachedGuardPointHash))
                {
                    ratAnimator.SetBool(isRunningHash, false);
                    ratAnimator.SetBool(isMovingHash, false);
                    mc.mobsAnimState[mstats.id] = 0;
                }
                /*Debug.Log(mstats.id);
                Debug.Log(mc.mobsPosX[mstats.id]);*/
                mc.mobsPosX[mstats.id] = transform.position.x;
                mc.mobsPosY[mstats.id] = transform.position.y;
                mc.mobsPosZ[mstats.id] = transform.position.z;
                mc.mobsRotY[mstats.id] = transform.eulerAngles.y;
                //mc.mobsAnimState[mstats.id] = 0;
                
                
            }
            else
            {
                switch (mc.mobsAnimState[mstats.id])
                {
                    case 0:
                        //ratAnimator.Play("idle");
                        ratAnimator.SetBool(isMovingHash,false);
                        ratAnimator.SetBool(isRunningHash,false);
                        ratAnimator.SetBool(isAttackingHash,false);
                        break;
                    case 1:
                        //ratAnimator.Play("walk");
                        ratAnimator.SetBool(isMovingHash,true);
                        ratAnimator.SetBool(isRunningHash,false);
                        ratAnimator.SetBool(isAttackingHash,false);
                        break;
                    case 2:
                        //ratAnimator.Play("run");
                        ratAnimator.SetBool(isMovingHash,true);
                        ratAnimator.SetBool(isRunningHash,true);
                        ratAnimator.SetBool(isAttackingHash,false);
                        break;
                    case 3:
                        //ratAnimator.Play("jumpBite");
                        ratAnimator.SetBool(isAttackingHash,true);
                        break;
                    case 4:
                        //ratAnimator.Play("jumpBite");
                        ratAnimator.SetBool(isAttackingHash,true);
                        break;
                    default:
                        break;
                }
                transform.position = new Vector3(mc.mobsPosX[mstats.id],mc.mobsPosY[mstats.id],mc.mobsPosZ[mstats.id]);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x,mc.mobsRotY[mstats.id],transform.eulerAngles.z);
            }
        }

        public void StartAttack()
        {
            if (PhotonNetwork.IsMasterClient && gc.doorIsOpen)
            {
                StartCoroutine(ActivateAttackZone());
            }
        }
        IEnumerator ActivateAttackZone()
        {
            if (PhotonNetwork.IsMasterClient && gc.doorIsOpen)
            {
                ratAnimator.SetBool(isAttackingHash, true);
                mc.mobsAnimState[mstats.id] = 3;
                yield return new WaitForSeconds(1f);
                attackZone.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                attackZone.gameObject.SetActive(false);
                ratAnimator.SetBool(isAttackingHash, false);
                mc.mobsAnimState[mstats.id] = 4;
            }
        }
    }
}
