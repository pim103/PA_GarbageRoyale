using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class Skill : MonoBehaviour
    {
        private int id;
        private string name;
        private float cooldown;
        private float effectCount;

        public Skill()
        {
            id = -1;
            name = "Null";
            cooldown = 0f;
            effectCount = 0f;

        }
        
        public Skill(int i, string na, float cool, float ec)
        {
            id = i;
            name = na;
            cooldown = cool;
            effectCount = ec;
        }
        
        private void Start()
        {
            
        }

        public void initSkill(int type)
        {
            switch (type)
            {
                case 1:
                    Id = 1;
                    Name = "Soin +10hp";
                    Cooldown = 5f;
                    EffectCount = 10;
                    break;
                default:
                    Id = 0;
                    Name = "Void Skill";
                    Cooldown = 0f;
                    EffectCount = 0f;
                    break;
            }
        }

        public int Id
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public float Cooldown
        {
            get => cooldown;
            set => cooldown = value;
        }

        public float EffectCount
        {
            get => effectCount;
            set => effectCount = value;
        }
    }
}