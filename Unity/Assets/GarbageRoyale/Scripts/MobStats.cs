using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobStats : MonoBehaviour
{
    public int id;

    public float hp;
    public float stamina;
    public float breath;
    public float basicAttack;
    
    private bool isDead;
    private bool isRotatePlayer;
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
    
    private void rotateDeadPlayer()
    {
        transform.Rotate(90, 0, 0);
        isRotatePlayer = true;
    }

    public void takeDamage(float damage)
    {
        Debug.Log("ui2");
        hp -= damage;

        if(hp <= 0)
        {
            isDead = true;
            if (!isRotatePlayer)
            {
                rotateDeadPlayer();
            }
        }
    }
}
