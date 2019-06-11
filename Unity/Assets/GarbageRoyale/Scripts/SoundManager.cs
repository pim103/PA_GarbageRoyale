﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

namespace GarbageRoyale.Scripts
{
    public class SoundManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameController gc;

        [SerializeField]
        private AudioClip walkSound;
        [SerializeField]
        private AudioClip walkWaterSound;
        [SerializeField]
        private AudioClip swimmingSound;
        [SerializeField]
        private AudioClip walkMetalSheetSound;

        [SerializeField]
        private AudioClip caveAmbientSound;
        [SerializeField]
        private AudioClip underwaterAmbientSound;
        [SerializeField]
        private GameObject ambiant;

        [SerializeField]
        private AudioClip pipeSound;
        [SerializeField]
        private AudioClip buttonSound;
        [SerializeField]
        private AudioClip openingDoorSound;
        [SerializeField]
        private AudioClip endOpeningDoorSound;
        [SerializeField]
        private AudioClip gazSound;
        [SerializeField]
        private AudioClip explosionSound;
        [SerializeField]
        private AudioClip torchLightSound;

        private Dictionary<int, Sound> characterPlaying = new Dictionary<int, Sound>();

        private GameObject cloneSource;
        private AudioSource sourceOrigin;
        private Sound[] lastSoundPlayed;

        private GameObject ambiantSource;
        private Sound lastAmbientSoundPlayed;

        public enum Sound
        {
            None,
            Pipe,
            Walk,
            Swim,
            FeetOnWater,
            Button,
            OpeningDoor,
            EndOpeningDoor,
            Cave,
            Water,
            Menu,
            MetalSheet
        }

        // Start is called before the first frame update
        void Awake()
        {
            lastAmbientSoundPlayed = Sound.Menu;
            lastSoundPlayed = Enumerable.Repeat(Sound.Walk, 10).ToArray();
        }

        public void initAmbientSound()
        {
            gc.menuSound.Stop();
            lastAmbientSoundPlayed = Sound.Cave;
            gc.menuSound.clip = caveAmbientSound;
            gc.menuSound.loop = true;
            gc.menuSound.Play();
        }

        public void initWaterSound()
        {
            gc.menuSound.Stop();
            lastAmbientSoundPlayed = Sound.Water;
            gc.menuSound.clip = underwaterAmbientSound;
            gc.menuSound.loop = true;
            gc.menuSound.Play();
        }

        public AudioClip getTrapSound()
        {
            return endOpeningDoorSound;
        }

        public AudioClip getGazSound()
        {
            return gazSound;
        }

        public AudioClip getExplosionSound()
        {
            return explosionSound;
        }

        public AudioClip getTorchLightSound()
        {
            return torchLightSound;
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
        /*
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
                    case Sound.Button:
                        characterPlaying[idPlayer] = Sound.Button;
                        audio.PlayOneShot(buttonSound);
                        break;
                    case Sound.OpeningDoor:
                        characterPlaying[idPlayer] = Sound.Button;
                        audio.PlayOneShot(openingDoorSound);
                        break;
                    case Sound.EndOpeningDoor:
                        characterPlaying[idPlayer] = Sound.Button;
                        audio.PlayOneShot(endOpeningDoorSound);
                        break;
                    case Sound.None:
                    default:
                        audio.Stop();
                        break;
                }
            }
        }
        */
        public void stopSound()
        {
            photonView.RPC("verifySongPlay", RpcTarget.MasterClient, Sound.None);
        }

        [PunRPC]
        void getNeededSong(PhotonMessageInfo info)
        {
            /*
            PlayerMovement playerMov;
            playerMov = characterList[info.Sender.ActorNumber].GetComponent<PlayerMovement>();*/
            //photonView.RPC("playWalkSound", RpcTarget.AllBuffered, playerMov.needToPlaySong, info.Sender.ActorNumber, playerMov.soundNeeded);
            //photonView.RPC("playAmbientSound", info.Sender, playerMov.getHeadIsOnWater());
        }

        public void playWalkSound(int idPlayer, bool playSong, Sound soundNeeded, bool isQuiet)
        {
            if (!isQuiet)
            {
                AudioSource audio = gc.players[idPlayer].PlayerMovement;

                if (gc.AvatarToUserId[idPlayer] == PhotonNetwork.AuthValues.UserId)
                {
                    if (soundNeeded == Sound.Swim && lastAmbientSoundPlayed == Sound.Cave)
                    {
                        initWaterSound();
                    }
                    else if (soundNeeded != Sound.Swim && lastAmbientSoundPlayed == Sound.Water)
                    {
                        initAmbientSound();
                    }
                }

                if (lastSoundPlayed[idPlayer] != soundNeeded)
                {
                    audio.Stop();
                    lastSoundPlayed[idPlayer] = soundNeeded;
                }

                if (!playSong)
                {
                    audio.Stop();
                }
                else
                {
                    if (!audio.isPlaying)
                    {
                        switch (soundNeeded)
                        {
                            case Sound.Walk:
                                audio.PlayOneShot(walkSound);
                                if (gc.playersActions[idPlayer].isRunning)
                                {
                                    audio.volume = audio.volume * 1.5f;
                                }
                                if (gc.playersActions[idPlayer].isCrouched)
                                {
                                    audio.volume = audio.volume * 0.5f;
                                }
                                break;
                            case Sound.FeetOnWater:
                                audio.PlayOneShot(walkWaterSound);
                                break;
                            case Sound.Swim:
                                audio.PlayOneShot(swimmingSound);
                                break;
                            case Sound.MetalSheet:
                                audio.PlayOneShot(walkMetalSheetSound);
                                break;
                        }
                    }
                }
            }
        }
    }
}
