using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class OnlinePlayerManager : MonoBehaviourPun, IPunObservable
    {
        [HideInInspector]
        public InputStr Input;
        public struct InputStr
        {
            public float LookX;
            public float LookZ;
            public float RunX;
            public float RunZ;
            public bool Jump;
        }
        private GameController gameControl;
        private bool mine;

        private void Awake()
        {
            mine = false;
            gameControl = GameObject.Find("Controller").GetComponent<GameController>();

            foreach (var pair in gameControl.characterList)
            {
                if (pair.Value.transform == this.transform)
                {
                    mine = true;
                }
            }
            //destroy the controller if the player is not controlled by me
            /*if (!photonView.IsMine && GetComponent<CharacterController>() != null)
            {
                Debug.Log(photonView.Owner);
                Destroy(GetComponent<CharacterController>());
                Destroy(transform.GetChild(0).gameObject);
                Destroy(GetComponent<PlayerMovement>());
                Destroy(GetComponent<CameraControl>());
            }*/
                
        }

        private void Update()
        {
            //gameControl = GameObject.Find("Controller").GetComponent<GameController>();
            //Animator.SetBool("Grounded", Grounded);

            
            //Animator.SetFloat("RunX", localVelocity.x);
            //Animator.SetFloat("RunZ", localVelocity.z);


        }

        void FixedUpdate()
        {

        }

        private void LateUpdate()
        {
           
        }

        public static void RefreshInstance(ref OnlinePlayerManager player, OnlinePlayerManager Prefab)
        {
            var position = new Vector3(150, 0.7f, 150);
            var rotation = Quaternion.identity;
            if (player != null)
            {
                position = player.transform.position;
                rotation = player.transform.rotation;
                PhotonNetwork.Destroy(player.gameObject);
            }

            player = PhotonNetwork.Instantiate(Prefab.gameObject.name, position, rotation).GetComponent<OnlinePlayerManager>();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(Input.RunX);
                stream.SendNext(Input.RunZ);
                stream.SendNext(Input.LookX);
                stream.SendNext(Input.LookZ);
            }
            else
            {
                Input.RunX = (float)stream.ReceiveNext();
                Input.RunZ = (float)stream.ReceiveNext();
                Input.LookX = (float)stream.ReceiveNext();
                Input.LookZ = (float)stream.ReceiveNext();
            }
        }
    }
}
