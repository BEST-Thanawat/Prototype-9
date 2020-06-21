using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AddForce : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private List<Transform> transforms;
    [SerializeField] private GameObject player;
    // Update is called once per frame
    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3 v = new Vector3(0, 100f, 0);
            GetComponent<Rigidbody>().AddForce(v);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Vector3 v = new Vector3(1000f, 0, 0);
            GetComponent<Rigidbody>().AddForce(v);
        }

        Debug.DrawRay(transform.position, Vector3.up, Color.green);
        Debug.DrawRay(transform.position, Vector3.right, Color.red);
        Debug.DrawRay(transform.position, Vector3.forward, Color.blue);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            
            Debug.DrawLine(ray.origin, ray.direction * 100, Color.red);

            if(hit.transform.tag == "Cube")
            {
                Debug.Log(hit.transform.name);
            }
        }

        for (int i = 0; i < transforms.Count; i++)
        {
            var vector1 = ray.direction;
            var vector2 = transforms[i].position - ray.origin;
            var lookPercentage = Vector3.Dot(vector1.normalized, vector2.normalized);
            text.text = lookPercentage.ToString("#.##");

            Vector3 v3 = Vector3.up;
            text.transform.position = Camera.main.WorldToScreenPoint(transform.position - v3);
        }

        Vector3 distance = player.transform.position - transform.position;
        Vector3 direction = distance.normalized;
        
        Vector3 velocity = direction * 1f;
        if(distance.magnitude > 1.5f) transform.Translate(velocity * Time.deltaTime);
    }
}
