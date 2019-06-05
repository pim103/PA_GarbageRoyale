using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts.Items
{
    public class TrapManifScript : MonoBehaviour
    {
        [SerializeField]
        private RopeScript rs;

        private GameController gc;

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();

            GameObject rope = ObjectPooler.SharedInstance.GetPooledObject(10);
            int id = gc.items.Count;
            gc.items.Add(id, rope);

            rs.idItem = id;
        }
    }
}