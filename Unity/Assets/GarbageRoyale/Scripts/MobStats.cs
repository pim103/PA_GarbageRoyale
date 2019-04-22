using System.Collections;
using System.Collections.Generic;
using GarbageRoyale.Scripts;
using UnityEngine;

public class MobStats : MonoBehaviour
{
    public int id;

    public float hp;
    public float stamina;
    public float breath;
    public float basicAttack;
    
    private bool isDead;
    private bool isRotateMob;
    // Start is called before the first frame update
    void Start()
    {
        id = (int) transform.position.x + (int) transform.position.y + (int) transform.position.z;
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
    
    private void rotateDeadMob()
    {
        transform.Rotate(90, 0, 0);
        isRotateMob = true;
    }

    public void takeDamage(float damage)
    {
        hp -= damage;

        if(hp <= 0)
        {
            isDead = true;
            if (!isRotateMob)
            {
                rotateDeadMob();
                lootSkill();
                gameObject.SetActive(false);
            }
        }
    }

    private void lootSkill()
    {
        
        GameObject lootedSkill;
        lootedSkill = ObjectPoolerPhoton.SharedInstance.GetPooledObject(2);
        lootedSkill.SetActive(true);
        lootedSkill.transform.position = transform.position;
    }
}
