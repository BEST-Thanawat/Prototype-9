using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class Mover : MonoBehaviour
{
    //public float movementSpeed;
    //public GameObject playerObj;

    //private void Update()
    //{
    //    Plane playerPlane = new Plane(Vector3.up, transform.position);
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    float hitDist = 0f;

    //    if (playerPlane.Raycast(ray, out hitDist))
    //    {
    //        Vector3 targetPoint = ray.GetPoint(hitDist);
    //        Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
    //        targetPoint.x = 0;
    //        targetPoint.z = 0;
    //        playerObj.transform.rotation = Quaternion.Slerp(playerObj.transform.rotation, targetRotation, 7f * Time.deltaTime);
    //    }

    //    if (Input.GetKey(KeyCode.W))
    //    {
    //        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
    //    }
    //    if (Input.GetKey(KeyCode.A))
    //    {
    //        transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
    //    }
    //    if (Input.GetKey(KeyCode.S))
    //    {
    //        transform.Translate(Vector3.back * movementSpeed * Time.deltaTime);
    //    }
    //    if (Input.GetKey(KeyCode.D))
    //    {
    //        transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
    //    }
    //}
    [SerializeField]
    public float runSpeed = 3.25f, sprintSpeed = 5.841f, jumpHeight = 1f;

    public float movementSpeed;
    public GameObject playerObj;

    private bool isGrounded;
    private float distToGround = 0.5f;
    private CapsuleCollider capsule;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rigidbody;

    private void Awake()
    {
        capsule = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        
        if (Input.GetMouseButton(0))
        {
            MoveToCursor();
        }
        UpdateAnimator();

        
        //MoveByKeyboard();
    }

    private void UpdateAnimator()
    {
        //Get velocity from NavMeshAgent
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            navMeshAgent.speed = sprintSpeed;
        }
        
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            navMeshAgent.speed = runSpeed;
        }

        Vector3 velocity = navMeshAgent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        float speed = localVelocity.z;

        //Set animator parameter
        animator.SetFloat("forwardSpeed", speed);
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            //rigidbody.isKinematic = false;
            //navMeshAgent.enabled = false;
            rigidbody.AddForce(Vector3.up * 4.5f, ForceMode.Impulse);
            animator.SetTrigger("jump");
        }
    }

    //Check character is on ground.
    public bool IsGrounded()
    {
        if (capsule != null)
        {
            Debug.DrawRay(capsule.bounds.center, Vector3.down, Color.red);
            isGrounded = Physics.Raycast(capsule.bounds.center, Vector3.down, capsule.bounds.extents.y + 0.1f);
            Debug.Log(isGrounded);
        }

        return isGrounded;
    }

    private void MoveToCursor()
    {
        //if (rigidbody.velocity.magnitude <= 0.1f)
        //{
        //    rigidbody.isKinematic = true;
        //    navMeshAgent.enabled = true;
        //}

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (navMeshAgent.enabled)
        {
            if (Physics.Raycast(ray, out hit))
            {
                navMeshAgent.destination = hit.point;
            }
        }
        
        Debug.DrawRay(ray.origin, ray.direction * 100);
    }

    private void MoveByKeyboard()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitDist = 0f;

        if (playerPlane.Raycast(ray, out hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            targetPoint.x = 0;
            targetPoint.z = 0;
            playerObj.transform.rotation = Quaternion.Slerp(playerObj.transform.rotation, targetRotation, 7f * Time.deltaTime);
        }

        //if (Input.GetKey(KeyCode.W))
        //{
        //    transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
        //    GetComponent<Animator>().SetFloat("forwardSpeed", 3.5f);
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        //    GetComponent<Animator>().SetFloat("forwardSpeed", 3.5f);
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    transform.Translate(Vector3.back * movementSpeed * Time.deltaTime);
        //    GetComponent<Animator>().SetFloat("forwardSpeed", 3.5f);
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
        //    GetComponent<Animator>().SetFloat("forwardSpeed", 3.5f);
        //}
    }
}
