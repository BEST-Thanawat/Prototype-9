using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMover : MonoBehaviour
{
    [SerializeField]
    public GameObject obj;

    public UnityEngine.AI.NavMeshAgent navAgent;
    public ThirdPersonCharacter character;
    private Quaternion rotate = Quaternion.identity;
    // Start is called before the first frame update
    void Start()
    {
        navAgent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (navAgent.enabled)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    Instantiate(obj, hit.point, Quaternion.identity);
                    Debug.DrawRay(ray.origin, ray.direction * 100);
                    navAgent.destination = hit.point;
                }
            }
        }



        if (navAgent.remainingDistance > navAgent.stoppingDistance)
        {
            character.Move(navAgent.desiredVelocity, false, false);
        }
        else
        {
            //character.StopMoving();
            character.Move(Vector3.zero, false, false);
        }
    }
}
