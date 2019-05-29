using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace GarbageRoyale.Scripts
{
    public class SkillSpawner : MonoBehaviourPunCallbacks
    {
        private int spawnerId;
        private GameObject skillGob;
        private Skill skillSelf;

        private bool isSpawned = false;
        
        [SerializeField] 
        private GameObject skillPrefab;
        [SerializeField] 
        private GameObject _spawnerSkills;
        [SerializeField] 
        private int skillType;
        public RoomInfo gameInfo;

        private void Start()
        {
            initSkills();
        }
        
        public void initSkills()
        {

            /*skillGob = Instantiate(skillPrefab, new Vector3(_spawnerSkills.transform.position.x, _spawnerSkills.transform.position.y + 0.7f, _spawnerSkills.transform.position.z), Quaternion.identity);
            skillGob.name = "Staff_" + _spawnerSkills.transform.position.x + "_" + ((int)_spawnerSkills.transform.position.y + 1) + "_" + _spawnerSkills.transform.position.z;

            skillSelf = skillGob.GetComponent<Skill>();
            skillSelf.initSkill(skillType);*/

        }
    }
}