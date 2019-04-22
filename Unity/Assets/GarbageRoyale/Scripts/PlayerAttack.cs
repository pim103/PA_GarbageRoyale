using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
namespace GarbageRoyale.Scripts
{
    public class PlayerAttack : MonoBehaviourPunCallbacks
    {
        private GameController gc;
        public Dictionary<int, GameObject> characterList = new Dictionary<int, GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            characterList = gc.characterList;
        }

        public void hitPlayer(RaycastHit info)
        {
            photonView.RPC("findPlayer", RpcTarget.MasterClient, info.transform.position.x, info.transform.position.y, info.transform.position.z);
        }

        [PunRPC]
        private void findPlayer(float x, float y, float z, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            GameObject sourceDamage = characterList[info.Sender.ActorNumber];
            float damage = sourceDamage.GetComponent<PlayerStats>().getBasickAttack();
            int objectInHand = sourceDamage.GetComponent<InventoryController>().itemInHand;

            Item item = new Item();
            item.initItem(objectInHand);

            damage += item.getDamage();

            foreach (var player in characterList)
            {
                if(player.Value == sourceDamage)
                {
                    continue;
                }
                if(player.Value.transform.position.x == x && player.Value.transform.position.y == y && player.Value.transform.position.z == z)
                {
                    PlayerStats ps = player.Value.GetComponent<PlayerStats>();
                    ps.takeDamage(damage);
                    break;
                }
            }
        }
    }
}
