using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts.InventoryScripts
{
    public class InventorySpritesExposer : MonoBehaviour
    {
        [SerializeField] 
        public RawImage[] ItemSlots;
        [SerializeField] 
        public RawImage[] CraftingSlots;
        [SerializeField] 
        public RawImage CraftingResult;
        [SerializeField] 
        public RawImage[] skillSlots;
        [SerializeField] 
        public GameObject DropArea;
    }
}
