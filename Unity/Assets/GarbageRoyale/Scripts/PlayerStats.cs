using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace GarbageRoyale.Scripts
{
    public class PlayerStats : MonoBehaviour
    {
        private int id;

        private float hp;
        private float stamina;
        private float breath;
        private float basicAttack;

        private PlayerMovement pm; //Like Pierre-Marie :thumbsup: 
        private bool isDead;
        private bool isRotatePlayer;

        // Start is called before the first frame update
        void Start()
        {
            hp = 100f;
            stamina = 100f;
            breath = 100f;
            basicAttack = 20f;

            isDead = false;
            isRotatePlayer = false;

            pm = GetComponent<PlayerMovement>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(pm.getHeadIsOnWater())
            {
                if(breath > 0)
                {
                    breath -= 0.1f;
                } else if(hp > 0)
                {
                    takeDamage(0.2f);
                }
            } else
            {
                if (breath < 100)
                {
                    breath += 1;
                }
            }
        }

        private void rotateDeadPlayer()
        {
            pm.transform.Rotate(90, 0, 0);
            isRotatePlayer = true;
        }

        public void takeDamage(float damage)
        {
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

        public float getHp()
        {
            return hp;
        }

        public float getBreath()
        {
            return breath;
        }

        public void setBreath(float b)
        {
            breath = b;
        }

        public float getStamina()
        {
            return stamina;
        }

        public bool getIsDead()
        {
            return isDead;
        }

        public void setId(int idPlayer)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                id = idPlayer;
            }
        }

        public int getId()
        {
            return id;
        }

        public float getBasickAttack()
        {
            return basicAttack;
        }
    }
}
