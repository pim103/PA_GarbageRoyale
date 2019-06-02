using GarbageRoyale.Scripts.HUD;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class Inventory : MonoBehaviour
    {
        private static int itemSlots = 26;
        public int[] itemInventory; //= new int[itemSlots];
        private int lastCounterItem;
    
        private static int skillSlots = 2;
        public int[] skillInventory; // = new int[skillSlots];
        private int lastCounterSkill;

        public Inventory()
        {
            // Item Inventory Init
            itemInventory = new int[itemSlots];
            lastCounterItem = 0;
        
            // Skill Inventory Init
            skillInventory = new int[skillSlots];
            lastCounterSkill = 0;
        }

        public void testtouch()
        {
            Debug.Log("touched");
        }

        public void Start()
        {
            for (int i = 0; i < itemSlots; i++)
            {
                itemInventory[i] = -1;
            }
            for (int i = 0; i < skillSlots; i++)
            {
                skillInventory[i] = -1;
            }
            
        }

        public int findPlaceInventory()
        {
            for (int i = 0; i < itemSlots; i++)
            {
                if (itemInventory[i] == -1)
                {
                    return i;
                }
            }
            return -1;
        }
        
        public int findPlaceSkills()
        {
            for (int i = 0; i < itemSlots; i++)
            {
                if (skillInventory[i] == -1)
                {
                    return i;
                }
            }
            return -1;
        }
    
        public int[] getItemInventory()
        {
            return this.itemInventory;
        }
    
        public bool setItemInventory(int itemId)
        {
            int voidIndex = findPlaceInventory();
            if (voidIndex != -1)
            {
                this.itemInventory[voidIndex] = itemId;
            
                return true;
            }
            else
            {
                Debug.Log("No place found");
                return false;
            }
        }
        public bool setSkillInventory(int itemId)
        {
            int voidIndex = findPlaceSkills();
            if (voidIndex != -1)
            {
                skillInventory[voidIndex] = itemId;
            
                return true;
            }
            else
            {
                Debug.Log("No place found");
                return false;
            }
        }
        public int getLastCounterItem()
        {
            return lastCounterItem;
        }
    
        public void setLastCounterItem(int lastCounterItem)
        {
            this.lastCounterItem = lastCounterItem;
        }
    
        public int getLastCounterSkill()
        {
            return lastCounterSkill;
        }
    
        public void setLastCounterSkill(int lastCounterSkill)
        {
            this.lastCounterSkill = lastCounterSkill;
        }
    }
}
