using GarbageRoyale.Scripts.InventoryScripts;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using UnityEngine;

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
    
        private InventoryActionsController iac;
        private GameController gc;
        private PlayerAttack pa;

        // Start is called before the first frame update
        void Start()
        {
            iac = GameObject.Find("Controller").GetComponent<InventoryActionsController>();
            gc = GameObject.Find("Controller").GetComponent<GameController>();
            pa = GameObject.Find("Controller").GetComponent<PlayerAttack>();
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
                pa.photonView.RPC("MobDeathAll",RpcTarget.All,id, Random.Range(0, 6));
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

            switch (type)
            {
                case 0:
                    skill.name = "Marche silencieuse";
                    break;
                case 1:
                    skill.name = "Maîtrise du bâton";
                    break;
                case 2:
                    skill.name = "Invisibilité";
                    break;
                case 3:
                    skill.name = "Taser";
                    break;
                case 4:
                    skill.name = "Respiration aquatique";
                    break;
                case 5:
                    skill.name = "Dash éclair";
                    break;
            }
        }
    }
}
