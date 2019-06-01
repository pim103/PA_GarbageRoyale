using GarbageRoyale.Scripts.PrefabPlayer;
using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class Item : MonoBehaviour
    {
        private int id;
        public string name;
        public float damage;
        public int type; // Weapon, Utils, Trap

        [SerializeField]
        public RawImage itemImg;

        public bool initOnStart;
        public bool isOnline;

        public Vector3 scale;

        public bool isThrow;

        private void OnCollisionEnter(Collision collision)
        {
            if(isThrow && collision.transform.name.StartsWith("Player"))
            {
                PlayerAttack pa = GameObject.Find("Controller").GetComponent<PlayerAttack>();
                pa.HitByThrowItem(id, collision.transform.GetComponent<ExposerPlayer>().PlayerIndex);
            }
            isThrow = false;
        }

        public Item()
        {
            id = -1;
            isThrow = false;
            name = "Null";
            damage = 0f;
            type = 0;
        }

        public void resetScale()
        {
            gameObject.transform.localScale = scale;
        }
        
        public int getId()
        {
            return this.id;
        }
        public void setId(int id)
        {
            this.id = id;
        }
        
        public string getName()
        {
            return this.name;
        }
        public void setName(string na)
        {
            this.name = na;
        }
        
        public float getDamage()
        {
            return this.damage;
        }
        public void setDamage(float da)
        {
            this.damage = da;
        }
        
        public int getType()
        {
            return this.type;
        }
        public void setType(int ty)
        {
            this.type = ty;
        }
    }
}