using System.Collections;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts.IAMobs
{
    public class MobController : MonoBehaviourPunCallbacks
    {
        public float[] mobsPosX = new float[300];
        public float[] mobsPosY = new float[300];
        public float[] mobsPosZ = new float[300];

        public int[] mobsAnimState = new int[300];
        
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(UpdateAllMobs());
            mobsPosX = Enumerable.Repeat(0f, 300).ToArray();
            mobsPosY = Enumerable.Repeat(0f, 300).ToArray();
            mobsPosY = Enumerable.Repeat(0f, 300).ToArray();
            mobsAnimState = Enumerable.Repeat(0, 300).ToArray();
        }

        IEnumerator UpdateAllMobs()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("UpdateArrays",RpcTarget.Others,mobsAnimState,mobsPosX,mobsPosY,mobsPosZ);
                }
            }
        }

        [PunRPC]
        void UpdateArrays(int[] animStates, float[] posX, float[] posY, float[] posZ)
        {
            mobsAnimState = animStates;
            mobsPosX = posX;
            mobsPosY = posY;
            mobsPosZ = posZ;
        }
    }
}
