using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField]
    private GameObject waterObject;

    private int timerWaterUp;
    private int cooldownWaterUp;
    private float speedWaterUp;

    private bool startWater;

    void Start()
    {
        startWater = false;

        timerWaterUp = 0;
        speedWaterUp = 0.05f;
        cooldownWaterUp = 0;
    }

    void Update()
    {
        if(startWater)
        {
            if(waterObject.transform.position.y < 2)
            {
                waterObject.transform.position += Vector3.up * speedWaterUp;
            }
        }
    }

    public void setStartWater(bool isStarting)
    {
        startWater = isStarting;
    }

    public bool getStartWater()
    {
        return startWater;
    }
}
