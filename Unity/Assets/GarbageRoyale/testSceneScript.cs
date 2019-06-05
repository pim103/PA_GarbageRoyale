using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testSceneScript : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem ps;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
    }
}
