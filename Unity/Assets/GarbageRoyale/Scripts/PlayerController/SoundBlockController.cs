using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

namespace GarbageRoyale.Scripts.PlayerController
{
    public class SoundBlockController : MonoBehaviour
    {
        [SerializeField]
        private GameObject textContainer;

        [SerializeField]
        private TextMesh textIdPlayer;

        private GameController gc;

        public int id;
        public int playerId;
        public int actualIdPlayer;

        public bool hasBeenSeenByMob = false;

        private void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            actualIdPlayer = Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId);
        }

        private void OnEnable()
        {
            StartCoroutine(DeactivateMyself());
            StartCoroutine(LookAtPlayer());
            textIdPlayer.text = playerId.ToString();
        }

        IEnumerator DeactivateMyself()
        {
            while (true)
            {
                yield return new WaitForSeconds(29.0f);
                gameObject.SetActive(false);
            }
        }

        IEnumerator LookAtPlayer()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                textContainer.transform.LookAt(gc.players[actualIdPlayer].PlayerCamera.transform);
            }
        }
    }
}
