using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.PlayerController
{
    public class ListPlayerIntents : MonoBehaviour
    {
        public float horizontalAxe { get; set; }
        public float verticalAxe { get; set; }
        public float rotationX { get; set; }
        public float rotationY { get; set; }
        public bool wantToJump { get; set; }
        public bool wantToLightUp { get; set; }
        public bool wantToTurnOnTorch { get; set; }
        public bool wantToGoDown { get; set; }

        public bool isInWater { get; set; }
        public bool isInTransition { get; set; }
    }
}