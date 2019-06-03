using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.PlayerController
{
    public class ListPlayerIntents : MonoBehaviour
    {
        public float horizontalAxe { get; set; }
        public float verticalAxe { get; set; }

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

        public bool isInWater { get; set; }
        public bool headIsInWater { get; set; }
        public bool feetIsInWater { get; set; }
        public bool isInTransition { get; set; }

        //STATUS
        public bool isOiled { get; set; }
        public float timeLeftOiled;
        public bool isBurning { get; set; }
        public bool isQuiet { get; set; }
        public float timeLeftBurn;

        public bool isFallen { get; set; }
        public float timeLeftFallen;

        public bool isSlow { get; set; }
    }
}