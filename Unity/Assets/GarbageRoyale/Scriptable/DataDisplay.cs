using GarbageRoyale.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DataDisplay : MonoBehaviour
{
    private List<GameObject> dataToDisplay;

    [SerializeField]
    private NewScriptableObject scriptable;

    [SerializeField]
    private GameObject deathDot;

    [SerializeField]
    private MazeMeshGenerator mmg;

    private void OnEnable()
    {
        dataToDisplay = new List<GameObject>();

        foreach (var pos in scriptable.killPoints)
        {
            dataToDisplay.Add(Instantiate(deathDot, pos.position, Quaternion.identity));
        }

        for(var i = 0; i < 8; i++)
        {
            mmg.FromData(scriptable.floors[i], i * 16, scriptable.Prefabs, scriptable.floorsRooms[i], scriptable.floorTransition, scriptable.roomTrap[i], scriptable.itemRoom[i], true);
        }
    }

    private void OnDisable()
    {
        foreach (var gObject in dataToDisplay)
        {
            DestroyImmediate(gObject);
        }
    }
}
