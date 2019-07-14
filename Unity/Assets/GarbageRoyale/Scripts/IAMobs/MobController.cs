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
        public float[] mobsRotY = new float[300];
        public float[] mobsHP = new float[300];

        public int[] mobsAnimState = new int[300];
        public bool[] hasSeenAnything = new bool[300];
        
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(UpdateAllMobs());
            mobsPosX = Enumerable.Repeat(0f, 300).ToArray();
            mobsPosY = Enumerable.Repeat(0f, 300).ToArray();
            mobsPosY = Enumerable.Repeat(0f, 300).ToArray();
            mobsRotY = Enumerable.Repeat(0f, 300).ToArray();
            mobsAnimState = Enumerable.Repeat(0, 300).ToArray();
            mobsHP = Enumerable.Repeat(100f, 300).ToArray();
            hasSeenAnything = Enumerable.Repeat(false, 300).ToArray();
        }

        IEnumerator UpdateAllMobs()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.025f);
                if (PhotonNetwork.IsMasterClient)
                {
                    //photonView.RPC("UpdateArrays",RpcTarget.Others,mobsAnimState,mobsPosX,mobsPosY,mobsPosZ,mobsRotY,mobsHP);
                    for (int i = 0;i<300;i++)
                    {
                        if (hasSeenAnything[i])
                        {
                            photonView.RPC("UpdateCurrentMob",RpcTarget.Others,i, mobsAnimState[i],mobsPosX[i],mobsPosY[i],mobsPosZ[i],mobsRotY[i],mobsHP[i], true);
                        }
                    }
                }
            }
        }

        [PunRPC]
        void UpdateArrays(int[] animStates, float[] posX, float[] posY, float[] posZ, float[] rotY, float[] HPs)
        {
            mobsAnimState = animStates;
            mobsPosX = posX;
            mobsPosY = posY;
            mobsPosZ = posZ;
            mobsRotY = rotY;
            mobsHP = HPs;
        }
        
        [PunRPC]
        void UpdateCurrentMob(int mobId,int animStates, float posX, float posY, float posZ, float rotY, float HPs, bool Seen)
        {
            mobsAnimState[mobId] = animStates;
            mobsPosX[mobId] = posX;
            mobsPosY[mobId] = posY;
            mobsPosZ[mobId] = posZ;
            mobsRotY[mobId] = rotY;
            mobsHP[mobId] = HPs;
            hasSeenAnything[mobId] = Seen;
        }
    }
}
