using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private Inventory playerInventory;
    [SerializeField]
    private GameObject playerPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        playerInventory = playerPrefab.AddComponent<Inventory>();
        playerInventory.initInventory();
        //Debug.Log(string.Format("Inventory : {0}", playerInventory.getItemInventory()[0]));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
