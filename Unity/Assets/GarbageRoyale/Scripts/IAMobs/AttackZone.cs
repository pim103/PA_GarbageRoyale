using GarbageRoyale.Scripts.PrefabPlayer;
using UnityEngine;

namespace GarbageRoyale.Scripts.IAMobs
{
    public class AttackZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<PlayerStats>().takeDamage(1f);
            }
        }
    }
}
