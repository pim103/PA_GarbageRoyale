using System.Collections;
using System.Collections.Generic;
using GarbageRoyale.Scripts;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static int itemSlots = 5;
    private int[] itemInventory; //= new int[itemSlots];
    private int lastCounterItem;
    
    private static int skillSlots = 2;
    private int[] skillInventory; // = new int[skillSlots];
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

    public void initInventory()
    {
        for (int i = 0; i < itemSlots; i++)
        {
            itemInventory[i] = 0;
        }
    }

    public int findPlaceInventory()
    {
        for (int i = 0; i < itemSlots; i++)
        {
            if (itemInventory[i] == 0)
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
    
    public void setItemInventory(int itemId)
    {
        int voidIndex = findPlaceInventory();
        if (voidIndex != -1)
        {
            this.itemInventory[voidIndex] = itemId;
        }
        else
        {
            Debug.Log("No place found");
        }
    }
    public int getLastCounterItem()
    {
        return this.lastCounterItem;
    }
    
    public void setLastCounterItem(int lastCounterItem)
    {
        this.lastCounterItem = lastCounterItem;
    }
    
    public int getLastCounterSkill()
    {
        return this.lastCounterSkill;
    }
    
    public void setLastCounterSkill(int lastCounterSkill)
    {
        this.lastCounterSkill = lastCounterSkill;
    }
}
