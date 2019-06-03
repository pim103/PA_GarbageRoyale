using UnityEngine;
using UnityEngine.UI;

namespace GarbageRoyale.Scripts
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
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
