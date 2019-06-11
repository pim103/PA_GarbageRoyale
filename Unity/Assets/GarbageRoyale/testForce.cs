using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testForce : MonoBehaviour
{
    [SerializeField]
    private GameObject origin;

    private Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        rigid = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            rigid.AddRelativeForce(Vector3.left * 50);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            transform.position = origin.transform.position + (Vector3.left * 2);
            transform.localEulerAngles = origin.transform.localEulerAngles;
        }
    }
}
