using System.Collections;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GarbageRoyale.Scripts.PlayerController
{
    public class PlayerSoundObjects : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private GameController gc;
        [SerializeField] 
        private PlayerControllerMaster pcm;
        
        private GameObject block;
        // Start is called before the first frame update
        void Start()
        {
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
                Debug.Log(playerIndex);
                if (gc.players[playerIndex].PlayerGameObject.activeInHierarchy && pcm.playersWalking[playerIndex])
                {
                    if (!gc.playersActions[playerIndex].isQuiet && !gc.playersActions[playerIndex].isRunning &&
                        !gc.playersActions[playerIndex].isCrouched)
                    {
                        block = ObjectPoolerSoundBlocks.SharedInstance.GetPooledObject(playerIndex);
                        block.SetActive(true);
                        block.tag = "MidLevelSoundBlock";
                        block.transform.position = gc.players[playerIndex].transform.position;
                    }
                    else if (!gc.playersActions[playerIndex].isQuiet && !gc.playersActions[playerIndex].isRunning && gc.playersActions[playerIndex].isCrouched)
                    {
                        block = ObjectPoolerSoundBlocks.SharedInstance.GetPooledObject(playerIndex);
                        block.SetActive(true);
                        block.tag = "QuietSoundBlock";
                        block.transform.position = gc.players[playerIndex].transform.position;
                    }
                    else if (!gc.playersActions[playerIndex].isQuiet && gc.playersActions[playerIndex].isRunning && !gc.playersActions[playerIndex].isCrouched)
                    {
                        block = ObjectPoolerSoundBlocks.SharedInstance.GetPooledObject(playerIndex);
                        block.SetActive(true);
                        block.tag = "LoudSoundBlock";
                        block.transform.position = gc.players[playerIndex].transform.position;
                    }
                }
            }
        }
    }
}
