using System;
using System.Collections;
using UnityEngine;

namespace GarbageRoyale.Scripts.PlayerController
{
    public class SoundBlockController : MonoBehaviour
    {
        public int id;
        public int playerId;

        public bool hasBeenSeenByMob = false;
        
        private void OnEnable()
        {
            StartCoroutine(DeactivateMyself());
            transform.GetChild(0).GetComponent<TextMesh>().text = playerId.ToString();
        }

        IEnumerator DeactivateMyself()
        {
            while (true)
            {
                yield return new WaitForSeconds(29.0f);
                gameObject.SetActive(false);
            }
        }
    }
}
