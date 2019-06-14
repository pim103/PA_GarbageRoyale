using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField]
    public GameObject waterObject;

    private int timerWaterUp;
    private int cooldownWaterUp;

    private float speedWaterUp;
    private float speedWaterUpTransition;

    private bool startWater;

    void Start()
    {
        startWater = false;

        timerWaterUp = 0;
        speedWaterUp = 0.005f;
        cooldownWaterUp = 0;
        speedWaterUpTransition = 0.02f;
    }

    void Update()
    {
        if(startWater)
        {
            if(waterObject.transform.position.y < (4 + 8) * 4)
            {
                if( waterObject.transform.position.y % 16 > 0.5 && waterObject.transform.position.y % 16 < 4)
                {
                    waterObject.transform.position += Vector3.up * speedWaterUp;
                }
                else
                {
                    waterObject.transform.position += Vector3.up * speedWaterUpTransition;
                }
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
