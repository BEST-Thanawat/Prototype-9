using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForce : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3 v = new Vector3(0, 0, 1000f);
            GetComponent<Rigidbody>().AddForce(v);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Vector3 v = new Vector3(1000f, 0, 0);
            GetComponent<Rigidbody>().AddForce(v);
        }
    }
}
