using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.HUD
{
    public class InventoryGUI : MonoBehaviour
    {
        [SerializeField]
        private RawImage [] rawSprites;
        [SerializeField]
        private GameObject inventoryPrefab;

        private GameObject inventoryGui;

        public void initInventoryGUI(GameObject invPr, RawImage [] rwSpr)
        {
            rawSprites = rwSpr;
            inventoryPrefab = invPr;
            inventoryGui = Instantiate(inventoryPrefab, new Vector2(0, 0), Quaternion.identity);
            Debug.Log("Oui " + rawSprites[0] + " " + inventoryPrefab.name);
        }
        
        public void printSprite(int idx)
        {
            RawImage rawImg = GameObject.Find("ItemImg_" + idx).GetComponent<RawImage>();
            rawImg.texture = rawSprites[idx].texture;
            rawImg.color = Color.white;
        }
        
    }
    
}