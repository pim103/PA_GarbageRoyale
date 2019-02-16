using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class Item : MonoBehaviour
    {
        private string name;
        private float damage;
        [SerializeField] 
        private int type; // Weapon, Utils, Trap
        
        void Start()
        {
            initItem(type);
        }

        public Item(string name, float damage, int type)
        {
            this.name = name;
            this.damage = damage;
            this.type = type;
        }

        public Item initItem(int type)
        {
            switch (type)
            {
                case 1:
                    //Debug.Log("Init Wooden Staff");
                    return new Item("Wooden Staff", 15f, type);
                case 2:
                    //Debug.Log("Init Steel Staff");
                    return new Item("Steel Staff", 20f, type);
                case 3:
                    //Debug.Log("Init Steel Staff");
                    return new Item("Extinct Torch", 7f, type);
                case 4:
                    //Debug.Log("Init Steel Staff");
                    return new Item("Lit Torch", 7f, type);
            }
            return new Item("Void", 0f, type);
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