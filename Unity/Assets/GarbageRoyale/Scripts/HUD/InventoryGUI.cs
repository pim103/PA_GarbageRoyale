using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.HUD
{
    public class InventoryGUI : MonoBehaviour
    {
        [SerializeField]
        public RawImage [] rawSprites;

        private GameObject inventoryGui;

        [SerializeField]
        private Slider healthBar;

        [SerializeField]
        private Slider staminaBar;

        [SerializeField]
        private Slider breathBar;

        [SerializeField] 
        public RawImage[] skillTextures;

        public void initInventoryGUI(GameObject invPr, RawImage [] rwSpr)
        {
            rawSprites = rwSpr;
            //inventoryPrefab = invPr;
            //inventoryGui = Instantiate(inventoryPrefab, new Vector2(0, 0), Quaternion.identity);
            //Debug.Log("Oui " + rawSprites[0] + " " + inventoryPrefab.name);
        }
        
        public void printSprite(int idx, RawImage ri)
        {
            string name = "ItemImg_";
            RawImage rawImg = GameObject.Find(name + idx).GetComponent<RawImage>();
            rawImg.texture = ri.texture;
            
            //rawImg.color = Color.white;
        }
        public void printSkillSprite(int idx, int id)
        {
            string name = "SkillImg_";
            RawImage rawImg = GameObject.Find(name + idx).GetComponent<RawImage>();
            rawImg.texture = skillTextures[id].texture;
            //rawImg.color = Color.white;
        }

        public void deleteSprite(int idx)
        {
            RawImage rawImg = GameObject.Find("ItemImg_" + idx).GetComponent<RawImage>();
            rawImg.texture = null;
        }
        
        public void deleteSkillSprite(int idx)
        {
            RawImage rawImg = GameObject.Find("SkillImg_" + idx).GetComponent<RawImage>();
            rawImg.texture = null;
        }
        /*
        public void printSkillSprite(int idx, int id)
        {
            RawImage rawImg = GameObject.Find("SkillImg_" + idx).GetComponent<RawImage>();
            rawImg.texture = rawSprites[id-1].texture;
            rawImg.color = Color.white;
        }
        */

        public void updateBar(float hp, float stamina, float breath)
        {
            healthBar.value = hp;
            staminaBar.value = stamina;
            breathBar.value = breath;
        }
    }
    
}