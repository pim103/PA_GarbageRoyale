using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class Item : MonoBehaviour
    {
        private int id;
        private string name;
        private float damage;
        public int type; // Weapon, Utils, Trap
        public RawImage itemText;
        
        public Item()
        {
            id = -1;
            name = "Null";
            damage = 0f;
            type = 0;
        }

        public void initItem(int type)
        {
            setType(type);
            switch (type)
            {
                case 1:
                    //Debug.Log("Init Wooden Staff");
                    setId(1);
                    setName("Wooden Staff");
                    setDamage(15f);
                    //setTexture();
                    break;
                case 2:
                    //Debug.Log("Init Steel Staff");
                    setId(2);
                    setName("Steel Staff");
                    setDamage(20f);
                    break;
                case 3:
                    //Debug.Log("Init Steel Staff");
                    setId(3);
                    setName("Extinct Torch");
                    setDamage(7f);
                    break;
                case 4:
                    //Debug.Log("Init Steel Staff");
                    setId(4);
                    setName("Lit Torch");
                    setDamage(7f);
                    break;
                default:
                    setId(0);
                    setName("Void");
                    setDamage(0f);
                    break;
            }
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
        
        public RawImage getTexture()
        {
            return this.itemText;
        }
        public void setTexture(RawImage it)
        {
            this.itemText = it;
        }
    }
}