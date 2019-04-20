﻿using System.Collections;
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

        [SerializeField]
        private AudioClip caveAmbientSound;
        [SerializeField]
        private AudioClip underwaterAmbientSound;
        [SerializeField]
        private GameObject ambiant;

        [SerializeField]
        private AudioClip pipeSound;

        private GameController gc;
        private Dictionary<int, GameObject> characterList = new Dictionary<int, GameObject>();
        private Dictionary<int, GameObject> characterSound = new Dictionary<int, GameObject>();
        private Dictionary<int, Sound> characterPlaying = new Dictionary<int, Sound>();

        private GameObject cloneSource;
        private AudioSource sourceOrigin;
        private string lastSoundPlayed;

        private GameObject ambiantSource;
        private string lastAmbientSoundPlayed;

        public enum Sound
        {
            None,
            Pipe,
            Walk,
            Swim,
            FeetOnWater
        }

        // Start is called before the first frame update
        void Start()
        {
            gc = GetComponent<GameController>();
            characterList = gc.characterList;
            characterSound = gc.characterSound;

            ambiantSource = Instantiate(ambiant, new Vector3(162f, 50f, 162f), Quaternion.identity);
            lastAmbientSoundPlayed = "cave";
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            characterPlaying.Add(newPlayer.ActorNumber, Sound.None);
            photonView.RPC("initListSound", newPlayer, newPlayer.ActorNumber);

            if(!PhotonNetwork.IsMasterClient)
            {
                foreach (KeyValuePair<int, GameObject> eachPlayer in characterSound)
                {
                    photonView.RPC("initListSound", newPlayer, eachPlayer.Key);
                }
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (gc.getCanMove())
            {
                photonView.RPC("getNeededSong", RpcTarget.MasterClient, null);
            }
        }

        [PunRPC]
        void initListSound(int id,  PhotonMessageInfo info)
        {
            characterPlaying.Add(id, Sound.None);
        }

        public void playSound(Sound sound)
        {
            photonView.RPC("verifySongPlay", RpcTarget.MasterClient, sound);
        }

        [PunRPC]
        void verifySongPlay(Sound sound, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            photonView.RPC("playSoundForAll", RpcTarget.AllBuffered, info.Sender.ActorNumber, sound);
        }

        [PunRPC]
        private void playSoundForAll(int idPlayer, Sound sound, PhotonMessageInfo info)
        {
            if (gc.getCanMove())
            {
                AudioSource audio = characterSound[idPlayer].GetComponent<AudioSource>();

                switch (sound)
                {
                    case Sound.Pipe:
                        characterPlaying[idPlayer] = Sound.Pipe;
                        audio.PlayOneShot(pipeSound);
                        break;
                    default:
                        break;
                }
            }
        }

        [PunRPC]
        void getNeededSong(PhotonMessageInfo info)
        {
            PlayerMovement playerMov;
            playerMov = characterList[info.Sender.ActorNumber].GetComponent<PlayerMovement>();
            photonView.RPC("playWalkSound", RpcTarget.AllBuffered, playerMov.needToPlaySong, info.Sender.ActorNumber, playerMov.soundNeeded);
            photonView.RPC("playAmbientSound", info.Sender, playerMov.getHeadIsOnWater());
        }

        [PunRPC]
        private void playWalkSound(bool playSong, int idPlayer, string soundPlayed)
        {
            if(gc.getCanMove())
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
                else if (!playSong && characterPlaying[idPlayer] != Sound.Pipe)
                {
                    audio.Stop();
                } else if(!audio.isPlaying)
                {
                    characterPlaying[idPlayer] = Sound.None;
                }
            }
        }

        [PunRPC]
        private void playAmbientSound(bool isHeadInWater)
        {
            AudioSource ambiant = ambiantSource.GetComponent<AudioSource>();

            if(isHeadInWater && lastAmbientSoundPlayed == "cave")
            {
                ambiant.Stop();
            } else if(!isHeadInWater && lastAmbientSoundPlayed == "water")
            {
                ambiant.Stop();
            }

            if (isHeadInWater && !ambiant.isPlaying)
            {
                ambiant.PlayOneShot(underwaterAmbientSound);
                lastAmbientSoundPlayed = "water";
            }
            else if(!ambiant.isPlaying){
                ambiant.PlayOneShot(caveAmbientSound);
                lastAmbientSoundPlayed = "cave";
            }
        }
    }
}
