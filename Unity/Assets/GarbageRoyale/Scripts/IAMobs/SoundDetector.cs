using GarbageRoyale.Scripts.PlayerController;
using UnityEngine;

namespace GarbageRoyale.Scripts.IAMobs
{
    public class SoundDetector : MonoBehaviour
    {
        
        private SoundBlockController sbc;
        
        [SerializeField]
        private string blockTag;

        [SerializeField] 
        private Guard guard;
        
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag(blockTag))
            {
                sbc = collider.gameObject.GetComponent<SoundBlockController>();
                if (!sbc.hasBeenSeenByMob)
                {
                    guard.SoundHeard(sbc.id,sbc.playerId);
                    Debug.Log("allo");
                }
            }
        }
    }
}
