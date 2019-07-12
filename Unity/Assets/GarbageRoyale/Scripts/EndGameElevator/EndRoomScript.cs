using GarbageRoyale.Scripts.Menu;
using GarbageRoyale.Scripts.PlayerController;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts.EndGameElevator
{
    public class EndRoomScript : MonoBehaviour
    {
        [SerializeField] public GameObject light1;
        [SerializeField] public GameObject light2;
        private EndGameElevatorController eec;
        private PlayerControllerMaster pcm;

        private void Start()
        {
            eec = GameObject.Find("Controller").GetComponent<EndGameElevatorController>();
            pcm = GameObject.Find("PlayerListScripts").GetComponent<PlayerControllerMaster>();
            eec.endroomList.Add(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    pcm.photonView.RPC("EndGameRPC", RpcTarget.All, EndGameMenu.StateEndGame.One_Alive,
                        other.GetComponent<ExposerPlayer>().PlayerIndex);
                }
            }
        }
    }
}
