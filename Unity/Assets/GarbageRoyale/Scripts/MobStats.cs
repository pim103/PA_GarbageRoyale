using GarbageRoyale.Scripts.InventoryScripts;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using System;
using System.Collections;
using GarbageRoyale.Scripts.IAMobs;
using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
{
    public class MobStats : MonoBehaviour
    {
        public int id;

        public float hp;
        public float stamina;
        public float breath;
        public float basicAttack;
    
        public bool isDead;
        public bool isRotateMob;
    
        private GameController gc;
        private PlayerAttack pa;

        private bool seeCanvas;
        private int idPlayer;

        [SerializeField]
        private GameObject healthBarCanvas;

        [SerializeField]
        private Slider sliderHealthBar;

        private MobController mc;

        // Start is called before the first frame update
        void Start()
        {
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            pa = GameObject.Find("Controller").GetComponent<PlayerAttack>();
            mc = GameObject.Find("Controller").GetComponent<MobController>();
            //id = (int) transform.position.x + (int) transform.position.y + (int) transform.position.z;
            hp = 100f;
            stamina = 100f;
            breath = 100f;
            basicAttack = 20f;

            seeCanvas = false;
            isDead = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(!seeCanvas && hp != 100)
            {
                seeCanvas = true;
                idPlayer = Array.IndexOf(gc.AvatarToUserId, PhotonNetwork.AuthValues.UserId);
                healthBarCanvas.SetActive(true);
                StartCoroutine(UpdateStats());
            }

            if (PhotonNetwork.IsMasterClient)
            {
                mc.mobsHP[id] = hp;
            }
            else
            {
                hp = mc.mobsHP[id];
            }
        }

        private IEnumerator UpdateStats()
        {
            while(hp > 0)
            {
                sliderHealthBar.value = 100 - hp;
                healthBarCanvas.transform.LookAt(gc.players[idPlayer].PlayerCamera.transform);
                yield return new WaitForSeconds(0.5f);
            }

            healthBarCanvas.SetActive(false);
            seeCanvas = false;
        }
    
        public void rotateDeadMob()
        {
            transform.Rotate(90, 0, 0);
            isRotateMob = true;
        }

        public void takeDamage(int playerIndex, int inventorySlot)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            PlayerStats ps = gc.players[playerIndex].PlayerStats;
            float damage = ps.getBasickAttack();
            int indexItem = gc.players[playerIndex].PlayerInventory.itemInventory[inventorySlot];
        
            if (indexItem != -1)
            {
                Item item = gc.items[gc.players[playerIndex].PlayerInventory.itemInventory[inventorySlot]].GetComponent<Item>();
                damage += item.getDamage();
            }

            hp -= damage;    

            if(hp <= 0)
            {
                pa.photonView.RPC("MobDeathAll",RpcTarget.All,id, UnityEngine.Random.Range(0, (int)SkillsController.SkillType.All));
            }
        }
        
        public void takeDamageFromEnv(float damage)
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            hp -= damage;    

            if(hp <= 0)
            {
                pa.photonView.RPC("MobDeathAll",RpcTarget.All,id, UnityEngine.Random.Range(0, (int)SkillsController.SkillType.All));
            }
        }

        public void lootSkill(int type)
        {
            var lootedSkill = ObjectPooler.SharedInstance.GetPooledObject(6);
            lootedSkill.GetComponent<Item>().setId(gc.items.Count);
            gc.items.Add(gc.items.Count,lootedSkill);
            lootedSkill.SetActive(true);
            lootedSkill.transform.position = transform.position;

            Skill skill = lootedSkill.GetComponent<Skill>();
            skill.type = type;

            switch ((SkillsController.SkillType)type)
            {
                case SkillsController.SkillType.QuietSound:
                    skill.name = "Marche silencieuse";
                    break;
                case SkillsController.SkillType.StaffMaster:
                    skill.name = "Maîtrise du bâton";
                    break;
                case SkillsController.SkillType.Invisibility:
                    skill.name = "Invisibilité";
                    break;
                case SkillsController.SkillType.Tazer:
                    skill.name = "Taser";
                    break;
                case SkillsController.SkillType.AquaticBreath:
                    skill.name = "Respiration aquatique";
                    break;
                case SkillsController.SkillType.Dash:
                    skill.name = "Dash éclair";
                    break;
                case SkillsController.SkillType.IceWall:
                    skill.name = "Mur de Glace";
                    break;
                case SkillsController.SkillType.Hunting:
                    skill.name = "Chasseur";
                    break;
                default:
                    skill.name = "Inconnu";
                    break;
            }
        }
    }
}
