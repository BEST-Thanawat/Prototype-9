using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class KinematicMover : MonoBehaviour
{
    [SerializeField]
    public float runSpeed = 3.25f, sprintSpeed = 5.841f, crouchSpeed = 0.56f;

    //public float movementSpeed;
    //public GameObject playerObj;

    //private bool isGrounded;
    //private float distToGround = 0.5f;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

   
    
    private bool isCrouching = false;
    private bool isSprinting = false;
    private bool isRunning = false;

    Quaternion returnQuatation = Quaternion.identity;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = runSpeed;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MoveToCursor();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isCrouching)
        {
            isSprinting = true;
            navMeshAgent.speed = sprintSpeed;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && !isCrouching)
        {
            isSprinting = false;
            navMeshAgent.speed = runSpeed;
        }

        if (Input.GetKeyUp(KeyCode.C) && !isSprinting)
        {
            if (isCrouching)
            {
                navMeshAgent.speed = crouchSpeed;
            }
            else
            {
                navMeshAgent.speed = runSpeed;
            }
        }

        UpdateAnimatorSpeed();
    }

    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rhit;
        if (navMeshAgent.enabled)
        {
            if (Physics.Raycast(ray, out rhit))
            {
                navMeshAgent.destination = rhit.point;
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * 100);
    }
    private void UpdateAnimatorSpeed()
    {
        //Get velocity from NavMeshAgent
        Vector3 velocity = navMeshAgent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        float speed = localVelocity.z;

        //Set animator parameter
        if (!isCrouching)
        {
            animator.SetFloat("forwardSpeed", speed);
            animator.SetFloat("crouchSpeed", 0f);
        }
        else
        {
            animator.SetFloat("forwardSpeed", 0f);
            animator.SetFloat("crouchSpeed", speed);
        }
    }
    public Vector3 GetVelocity()
    {
        Vector3 velocity = navMeshAgent.velocity;
        //Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        return velocity;
    }
    public void JumpAnimation()
    {
        if (!isCrouching)
        {
            animator.SetTrigger("jump");

            // ***Reset navmeshagent to prevent the character stuck at the jump time at the destination.
            //navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }
    }
    public void CrouchAnimation()
    {
        if (!isSprinting)
        {
            isCrouching = animator.GetBool("crouch");
            animator.SetBool("crouch", !isCrouching);
            isCrouching = !isCrouching;
        }
    }
    public void TurnAnimation(float rotation)
    {
        animator.SetFloat("turnSpeed", rotation, 0.1f, Time.deltaTime);
    }

    public bool IsSprinting()
    {
        return isSprinting;
    }
    public bool IsCrouching()
    {
        return isCrouching;
    }
    public bool IsRunning()
    {
        return isRunning;
    }
    
    // ***Rotate character by mouse moving
    public Quaternion GetRotation()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitDist = 0f;
        Quaternion targetRotation = Quaternion.identity;

        if (playerPlane.Raycast(ray, out hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            targetPoint.x = 0;
            targetPoint.z = 0;
        }
        return targetRotation;
    }

    //private void Jump()
    //{
    //    if (IsGrounded())
    //    {
    //        //rigidbody.isKinematic = false;
    //        //navMeshAgent.enabled = false;
    //        rigidbody.AddForce(Vector3.up * 4.5f, ForceMode.Impulse);
    //        animator.SetTrigger("jump");
    //    }
    //}

    ////Check character is on ground.
    //public bool IsGrounded()
    //{
    //    if (capsule != null)
    //    {
    //        Debug.DrawRay(capsule.bounds.center, Vector3.down, Color.red);
    //        isGrounded = Physics.Raycast(capsule.bounds.center, Vector3.down, capsule.bounds.extents.y + 0.1f);
    //        Debug.Log(isGrounded);
    //    }

    //    return isGrounded;
    //}

    //private void MoveByKeyboard()
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

    //    //if (Input.GetKey(KeyCode.W))
    //    //{
    //    //    transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
    //    //    GetComponent<Animator>().SetFloat("forwardSpeed", 3.5f);
    //    //}
    //    //if (Input.GetKey(KeyCode.A))
    //    //{
    //    //    transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
    //    //    GetComponent<Animator>().SetFloat("forwardSpeed", 3.5f);
    //    //}
    //    //if (Input.GetKey(KeyCode.S))
    //    //{
    //    //    transform.Translate(Vector3.back * movementSpeed * Time.deltaTime);
    //    //    GetComponent<Animator>().SetFloat("forwardSpeed", 3.5f);
    //    //}
    //    //if (Input.GetKey(KeyCode.D))
    //    //{
    //    //    transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
    //    //    GetComponent<Animator>().SetFloat("forwardSpeed", 3.5f);
    //    //}
    //}
}
