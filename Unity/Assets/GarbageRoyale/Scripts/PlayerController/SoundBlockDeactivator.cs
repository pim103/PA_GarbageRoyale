using System.Collections;
using UnityEngine;

namespace GarbageRoyale.Scripts.PlayerController
{
    public class SoundBlockDeactivator : MonoBehaviour
    {
        // Start is called before the first frame update
        private void OnEnable()
        {
            StartCoroutine(DeactivateMyself());
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
