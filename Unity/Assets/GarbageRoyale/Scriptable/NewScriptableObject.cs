using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NewScriptableObject", order = 1)]
public class NewScriptableObject : ScriptableObject
{
    [SerializeField]
    private List<Data> killPoint;

    public List<Data> killPoints => this.killPoint;

    public GameObject[] Prefabs;
    public GameObject floorTransition;

    public Dictionary<string, string>[] roomLinksList = new Dictionary<string, string>[8];
    public Dictionary<string, int>[] roomTrap = new Dictionary<string, int>[8];
    public Dictionary<string, int>[] itemRoom = new Dictionary<string, int>[8];

    public int[][,] floors;
    public int[][,] floorsRooms;

    public void initMap(Dictionary<string, string>[] rll, Dictionary<string, int>[] rt, Dictionary<string, int>[] ir, 
        GameObject[] pref,
        GameObject trans,
        int[][,] f,
        int[][,] fr
    )
    {
        roomLinksList = rll;
        roomTrap = rt;
        itemRoom = ir;
        Prefabs = pref;
        floorTransition = trans;
        floors = f;
        floorsRooms = fr;
    }

    public void RegisterKillPoint(GameObject go, int id, float time)
    {
        killPoint.Add(new Data(go.transform.position, id, time));
    }
}


[Serializable]
public class Data
{
    public Vector3 position;
    public int playerIndex;
    public float deathTime;

    public Data(Vector3 pos, int id, float time)
    {
        position = pos;
        playerIndex = id;
        deathTime = time;
    }
}
