using GarbageRoyale.Scripts.InventoryScripts;
using GarbageRoyale.Scripts.PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class ScriptExposer : MonoBehaviour
    {
        public PlayerControllerMaster pcm;
        public GameController gc;
        public CameraRaycast cr;
        public InventoryActionsController iac;
        public SpectateMode sm;
        public PlayerAttack pa;
    }
}