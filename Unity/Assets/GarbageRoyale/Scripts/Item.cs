using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class Item : MonoBehaviour
    {
        public int id;
        public string name;
        public float damage;
        public int type; // Weapon, Utils, Trap

        [SerializeField]
        public RawImage itemImg;

        public bool initOnStart;
        public bool isOnline;

        public Vector3 scale;
        
        private void Start()
        {
        }

        public Item()
        {
            id = -1;
            name = "Null";
            damage = 0f;
            type = 0;
        }

        public void initItem(int type)
        {
            /*
            setType(type);
            switch (type)
            {
                case 1:
                    //Debug.Log("Init Wooden Staff");
                    //setId(1);
                    setName("Wooden Staff");
                    setDamage(15f);
                    //setTexture();
                    break;
                case 2:
                    //Debug.Log("Init Steel Staff");
                    //setId(2);
                    setName("Steel Staff");
                    setDamage(20f);
                    break;
                case 3:
                    //Debug.Log("Init Steel Staff");
                    // setId(3);
                    setName("Extinct Torch");
                    setDamage(7f);
                    break;
                case 4:
                    //Debug.Log("Init Steel Staff");
                    //setId(4);
                    setName("Torch");
                    setDamage(7f);
                    break;
                case 5:
                    //Debug.Log("Init Steel Staff");
                    //setId(5);
                    setName("Toilet Paper");
                    setDamage(-10f);
                    break;
                case 6:
                    setName("Jerrican");
                    setDamage(0f);
                    break;
                case 7:
                    setName("Bottle");
                    setDamage(7f);
                    break;
                default:
                    //setId(0);
                    setName("Void");
                    setDamage(0f);
                    break;
            }
            */
        }

        public void resetScale()
        {
            gameObject.transform.localScale = scale;
            /*
            switch (type)
            {
                case 1:
                    gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    break;
                case 2:
                    gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    break;
                case 3:
                    break;
                case 4:
                    gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.5f);
                    break;
                case 5:
                    gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case 6:
                    gameObject.transform.localScale = new Vector3(0.075f, 0.075f, 0.6f);
                    break;
                case 7:
                    gameObject.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                    break;
                default:
                    break;
            }*/
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