using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.PrefabPlayer
{
    public class ExposerPlayer : MonoBehaviour
    {
        public int PlayerIndex;

        public CharacterController PlayerChar;
        public PhotonTransformView PlayerPhotonTransform;
        public Transform PlayerTransform;
        public Light SpotLight;
        public GameObject PlayerGameObject;
        public Camera PlayerCamera;
        public AudioListener PlayerAudioListener;
        public GameObject PlayerFeet;

        public GameObject PlayerTorch;
        public GameObject PlayerStaff;
        public GameObject PlayerToiletPaper;
        public GameObject PlayerJerrican;
        public GameObject PlayerBottle;
        public GameObject PlayerBrokenBottle;
        public GameObject PlayerBottleOil;
        public GameObject PlayerMolotov;
        public GameObject PlayerRope;
        public GameObject PlayerMetalSheet;
        public GameObject PlayerBoxNail;
        public GameObject PlayerBattery;

        public GameObject PlayerWolfTrap;
        public GameObject PlayerTrapManif;

        public AudioSource PlayerMovement;
        public AudioSource PlayerAction;

        public PlayerStats PlayerStats;
        public Inventory PlayerInventory;
    }
}
