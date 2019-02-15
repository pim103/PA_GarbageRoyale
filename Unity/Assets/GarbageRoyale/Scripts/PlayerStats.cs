using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class PlayerStats : MonoBehaviour
    {
        private float hp;
        private float stamina;
        private float breath;

        private PlayerMovement pm; //Like Pierre-Marie :thumbsup: 
        private bool isDead;
        private bool isRotatePlayer;

        // Start is called before the first frame update
        void Start()
        {
            hp = 100f;
            stamina = 100f;
            breath = 100f;

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
                    hp -= 0.2f;
                }
            } else
            {
                if (breath < 100)
                {
                    breath += 1;
                }
            }

            if(hp <= 0)
            {
                isDead = true;
                if(!isRotatePlayer)
                {
                    rotateDeadPlayer();
                }
            }
        }

        private void rotateDeadPlayer()
        {
            pm.transform.Rotate(90, 0, 0);
            isRotatePlayer = true;
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
    }
}
