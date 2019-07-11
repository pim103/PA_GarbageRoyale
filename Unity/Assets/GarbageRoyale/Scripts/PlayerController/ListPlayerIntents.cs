using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.PlayerController
{
    public class ListPlayerIntents : MonoBehaviour
    {
        public float horizontalAxe { get; set; }
        public float verticalAxe { get; set; }

        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }
        public float rotX { get; set; }

        public bool wantToGoForward { get; set; }
        public bool wantToGoBackward { get; set; }
        public bool wantToGoRight { get; set; }
        public bool wantToGoLeft { get; set; }

        public float rotationX { get; set; }
        public float rotationY { get; set; }
        public bool wantToJump { get; set; }
        public bool wantToLightUp { get; set; }
        public bool wantToTurnOnTorch { get; set; }
        public bool wantToGoDown { get; set; }

        public bool wantToPunch { get; set; }

        public bool isInWater { get; set; }
        public bool headIsInWater { get; set; }
        public bool feetIsInWater { get; set; }
        public bool isInTransition { get; set; }
        public bool isOnMetalSheet { get; set; }

        //STATUS
        public bool isOiled { get; set; }
        public float timeLeftOiled;
        public bool isBurning { get; set; }
        public bool isQuiet { get; set; }
        public bool isRunning { get; set; }
        public bool isCrouched { get; set; }
        public float timeLeftBurn;
        public bool isDamageBoosted { get; set; }
        
        public bool isFallen { get; set; }
        public float timeLeftFallen;

        public bool isSlow { get; set; }
        public bool isTrap { get; set; }
        
        public bool isAmphibian { get; set; }

        public bool isInInventory { get; set; }
        public bool isInEscapeMenu { get; set; }
        public bool isInGMGUI { get; set; }
        public bool isInvincible { get; set; }
    }
}