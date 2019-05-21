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

        public GameObject PlayerTorch;
        public GameObject PlayerStaff;
        public GameObject PlayerToiletPaper;

        public AudioSource PlayerMovement;
        public AudioSource PlayerAction;
    }
}
