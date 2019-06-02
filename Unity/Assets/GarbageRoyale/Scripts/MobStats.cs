using System.Collections;
using System.Collections.Generic;
using GarbageRoyale.Scripts;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using UnityEngine;

public class MobStats : MonoBehaviour
{
    public int id;

    public float hp;
    public float stamina;
    public float breath;
    public float basicAttack;
    
    public bool isDead;
    public bool isRotateMob;
    
    private InventoryActionsController iac;
    private GameController gc;
    private CameraRaycast cr;

    // Start is called before the first frame update
    void Start()
    {
        iac = GameObject.Find("Controller").GetComponent<InventoryActionsController>();
        gc = GameObject.Find("Controller").GetComponent<GameController>();
        cr = GameObject.Find("PlayerListScripts").GetComponent<CameraRaycast>();
        //id = (int) transform.position.x + (int) transform.position.y + (int) transform.position.z;
        hp = 100f;
        stamina = 100f;
        breath = 100f;
        basicAttack = 20f;

        isDead = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
    
    public void rotateDeadMob()
    {
        transform.Rotate(90, 0, 0);
        isRotateMob = true;
    }

    public void takeDamage(int playerIndex)
    {
        PlayerStats ps = gc.players[playerIndex].PlayerStats;
        float damage = ps.getBasickAttack();
        int indexItem = gc.players[playerIndex].PlayerInventory.itemInventory[iac.placeInHand];
        
        if (indexItem != -1)
        {
            Item item = gc.items[gc.players[playerIndex].PlayerInventory.itemInventory[iac.placeInHand]].GetComponent<Item>();
            damage += item.getDamage();
        }

        if (ps.getStamina() >= ps.getAttackCostStamina())
        {
            hp -= damage;
        }

        if(hp <= 0)
        {
            /*isDead = true;
            if (!isRotateMob)
            {
                rotateDeadMob();
                lootSkill();
                gameObject.SetActive(false);
            }*/
            cr.photonView.RPC("MobDeath",RpcTarget.All,id);
        }
    }

    public void lootSkill()
    {
        
        GameObject lootedSkill;
        lootedSkill = ObjectPooler.SharedInstance.GetPooledObject(6);
        lootedSkill.GetComponent<Item>().setId(gc.items.Count);
        gc.items.Add(gc.items.Count,lootedSkill);
        lootedSkill.SetActive(true);
        lootedSkill.transform.position = transform.position;
    }
}
