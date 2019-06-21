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
    private bool WaterIsUp;

    void Start()
    {
        startWater = false;
        WaterIsUp = false;

        timerWaterUp = 0;
        speedWaterUp = 0.0007f;
        cooldownWaterUp = 0;
        speedWaterUpTransition = 0.02f;
    }

    private IEnumerator WaterUp()
    {
        WaterIsUp = true;
        while (startWater)
        {
            if (waterObject.transform.position.y < (4 + 12) * 8)
            {
                if (waterObject.transform.position.y % 16 > 0.5 && waterObject.transform.position.y % 16 < 4.5)
                {
                    waterObject.transform.position += Vector3.up * speedWaterUp;
                }
                else
                {
                    waterObject.transform.position += Vector3.up * speedWaterUpTransition;
                }
                yield return new WaitForSeconds(0.01f);
            } else
            {
                startWater = false;
            }

        }

        WaterIsUp = false;
    }

    public void ToggleStartWater()
    {
        startWater = !startWater;

        if (startWater && !WaterIsUp)
        {
            StartCoroutine(WaterUp());
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
    
    public void setSpeedWater(float speed)
    {
        speedWaterUp = speed;
    }

    public float getSpeedWater()
    {
        return speedWaterUp;
    }
}
