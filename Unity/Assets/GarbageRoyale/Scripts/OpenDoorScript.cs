using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class OpenDoorScript : MonoBehaviour
    {
        public int doorId;
        public bool isOpen = false;
        public int doorLoading = 0;

        [SerializeField]
        public AudioSource DoorSound;

        [SerializeField]
        public AudioSource ButtonSound;

        [SerializeField]
        public AudioClip EndOpeningClip;

        public void openDoor()
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                transform.GetChild(2).transform.Rotate(new Vector3(0,-90,0));
                transform.GetChild(3).transform.Rotate(new Vector3(0,90,0));
            }
            else
            {
                transform.GetChild(2).transform.Rotate(new Vector3(0,90,0));
                transform.GetChild(3).transform.Rotate(new Vector3(0,-90,0));
            }
        }

        public void PlayOpenSound()
        {
            if(!DoorSound.isPlaying)
            {
                DoorSound.Play();
            }
        }

        public void StopOpenSound()
        {
            DoorSound.Stop();
        }

        public void PlayEndOpeningSound()
        {
            DoorSound.Stop();
            DoorSound.PlayOneShot(EndOpeningClip);
        }

        public void PlayButtonSound()
        {
            ButtonSound.Play();
        }
    }
    
    
}
