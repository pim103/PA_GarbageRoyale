using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using GarbageRoyale.Scripts.HUD;

namespace GarbageRoyale.Scripts.PrefabPlayer
{
    public class PlayerStats : MonoBehaviour
    {
        public float defaultHp;
        public float defaultStamina;
        public float defaultBreath;

        public float currentHp;
        public float currentStamina;
        public float currentBreath;
        public float basicAttack;
        public float attackCostStamina;

        public bool isDead;
        public string playerName;
        private int playerRole;
        private bool isRotatePlayer;

        public bool isAlreadyTrigger;
        public bool isInvincible;

        // Start is called before the first frame update
        void Start()
        {
            defaultHp = 100f;
            defaultStamina = 100f;
            defaultBreath = 100f;

            currentHp = defaultHp;
            currentStamina = defaultStamina;
            currentBreath = 1.0f;
            basicAttack = 3f;
            attackCostStamina = 20f;
            playerName = "null";
            playerRole = 0;

            isDead = false;
            isRotatePlayer = false;
            isAlreadyTrigger = false;
            isInvincible = false;
        }

        private void rotateDeadPlayer()
        {
            //pm.transform.Rotate(90, 0, 0);
            isRotatePlayer = true;
        }

        public void takeDamage(float damage)
        {
            if (!isInvincible)
            {
                currentHp -= damage;
            }
            
            if(currentHp <= 0 )
            {
                isDead = true;
                if (!isRotatePlayer)
                {
                    rotateDeadPlayer();
                }
            }
        }

        public void healPlayer(float hp)
        {
            if(currentHp + hp < defaultHp)
            {
                currentHp += hp;
            }
            else
            {
                currentHp = defaultHp;
            }
        }

        public float getHp()
        {
            return currentHp;
        }

        public float getBreath()
        {
            return currentBreath;
        }

        public void setBreath(float b)
        {
            currentBreath = b;
        }

        public void setStamina(float s)
        {
            currentStamina = s;
        }

        public int PlayerRole
        {
            get => playerRole;
            set => playerRole = value;
        }

        public string PlayerName
        {
            get => playerName;
            set => playerName = value;
        }

        public void useStamina()
        {
            currentStamina -= attackCostStamina;
            if(currentStamina < 0)
            {
                currentStamina = 0;
            }
        }

        public float getAttackCostStamina()
        {
            return attackCostStamina;
        }

        public float getStamina()
        {
            return currentStamina;
        }

        public bool getIsDead()
        {
            return isDead;
        }

        public float getBasickAttack()
        {
            return basicAttack;
        }
    }
}
