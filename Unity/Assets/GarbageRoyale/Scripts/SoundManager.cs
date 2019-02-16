using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace GarbageRoyale.Scripts
{
    public class SoundManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private AudioClip walkSound;
        [SerializeField]
        private AudioClip walkWaterSound;
        [SerializeField]
        private AudioClip swimmingSound;

        private GameController gc;
        private Dictionary<int, GameObject> characterList = new Dictionary<int, GameObject>();
        private Dictionary<int, GameObject> characterSound = new Dictionary<int, GameObject>();

        private GameObject cloneSource;
        private AudioSource sourceOrigin;

        // Start is called before the first frame update
        void Start()
        {
            gc = GetComponent<GameController>();
            characterList = gc.characterList;
            characterSound = gc.characterSound;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            photonView.RPC("getNeededSong", RpcTarget.MasterClient, null);
        }

        [PunRPC]
        void getNeededSong(PhotonMessageInfo info)
        {
            PlayerMovement playerMov;
            playerMov = characterList[info.Sender.ActorNumber].GetComponent<PlayerMovement>();
            photonView.RPC("playWalkSound", RpcTarget.AllBuffered, playerMov.needToPlaySong, info.Sender.ActorNumber, playerMov.soundNeeded);
        }

        [PunRPC]
        private void playWalkSound(bool playSong, int idPlayer, string soundPlayed)
        {
            AudioSource audio = characterSound[idPlayer].GetComponent<AudioSource>();

            if (playSong && !audio.isPlaying)
            {
                switch (soundPlayed)
                {
                    case "walk":
                        audio.PlayOneShot(walkSound);
                        break;
                    case "walkInWater":
                        audio.PlayOneShot(walkWaterSound);
                        break;
                    case "swimming":
                        audio.PlayOneShot(swimmingSound);
                        break;
                }
            }
            else if (!playSong)
            {
                audio.Stop();
            }
        }
    }
}
