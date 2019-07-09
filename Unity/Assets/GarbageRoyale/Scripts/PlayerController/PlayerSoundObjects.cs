using System.Collections;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GarbageRoyale.Scripts.PlayerController
{
    public class PlayerSoundObjects : MonoBehaviourPunCallbacks
    {
        private GameController gc;

        private GameObject block;
        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            foreach (var currentPlayer in gc.players)
            {
                StartCoroutine(PutBlockWhileWalking(currentPlayer.PlayerIndex));
            }
        }

        private IEnumerator PutBlockWhileWalking(int playerIndex)
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f);
                if (gc.players[playerIndex].PlayerGameObject.activeInHierarchy)
                {
                    block = ObjectPoolerSoundBlocks.SharedInstance.GetPooledObject(playerIndex);
                    block.SetActive(true);
                    block.transform.position = gc.players[playerIndex].transform.position;
                }
            }
        }
    }
}
