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

    void Start()
    {
        timerWaterUp = 0;
        speedWaterUp = 0.05f;
        cooldownWaterUp = 0;
    }

    void Update()
    {
        if(waterObject.transform.position.y < 2)
        {
            waterObject.transform.position += Vector3.up * speedWaterUp;
        }
    }
}
