using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class KinematicMoverRootMotion : MonoBehaviour
{
    [SerializeField]
    public float runSpeed = 3.25f, sprintSpeed = 5.841f, crouchSpeed = 0.56f;

    //public float movementSpeed;
    //public GameObject playerObj;

    //private bool isGrounded;
    //private float distToGround = 0.5f;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private RaycastHit hit;
    private Quaternion lookAtRotationOnly_Y = Quaternion.identity;
    private bool isCrouching = false;
    private bool isSprinting = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        //navMeshAgent = GetComponent<NavMeshAgent>();
        //navMeshAgent.speed = runSpeed;
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        MoveToCursor();
    //    }

    //    if (Input.GetKeyDown(KeyCode.LeftShift) && !isCrouching)
    //    {
    //        isSprinting = true;
    //        navMeshAgent.speed = sprintSpeed;
    //    }

    //    if (Input.GetKeyUp(KeyCode.LeftShift) && !isCrouching)
    //    {
    //        isSprinting = false;
    //        navMeshAgent.speed = runSpeed;
    //    }

    //    if (Input.GetKeyUp(KeyCode.C) && !isSprinting)
    //    {
    //        if (isCrouching)
    //        {
    //            navMeshAgent.speed = crouchSpeed;
    //        }
    //        else
    //        {
    //            navMeshAgent.speed = runSpeed;
    //        }
    //    }

    //    UpdateAnimatorSpeed();
    //}

    //private void MoveToCursor()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit rhit;
    //    if (navMeshAgent.enabled)
    //    {
    //        if (Physics.Raycast(ray, out rhit))
    //        {
    //            navMeshAgent.destination = rhit.point;
    //        }
    //    }

    //    Debug.DrawRay(ray.origin, ray.direction * 100);
    //}
    //private void UpdateAnimatorSpeed()
    //{
    //    //Get velocity from NavMeshAgent
    //    Vector3 velocity = navMeshAgent.velocity;
    //    Vector3 localVelocity = transform.InverseTransformDirection(velocity);

    //    float speed = localVelocity.z;

    //    //Set animator parameter
    //    if (!isCrouching)
    //    {
    //        animator.SetFloat("forwardSpeed", speed);
    //        animator.SetFloat("crouchSpeed", 0f);
    //    }
    //    else
    //    {
    //        animator.SetFloat("forwardSpeed", 0f);
    //        animator.SetFloat("crouchSpeed", speed);
    //    }
    //}
    //public Vector3 GetVelocity()
    //{
    //    Vector3 velocity = navMeshAgent.velocity;
    //    //Vector3 localVelocity = transform.InverseTransformDirection(velocity);

    //    return velocity;
    //}
    //public void JumpAnimation()
    //{
    //    if (!isCrouching)
    //    {
    //        animator.SetTrigger("jump");

    //        // ***Reset navmeshagent to prevent the character stuck at the jump time at the destination.
    //        //navMeshAgent.isStopped = true;
    //        navMeshAgent.ResetPath();
    //    }
    //}
    public void CrouchAnimation()
    {
        if (!isSprinting)
        {
            isCrouching = animator.GetBool("crouch");
            animator.SetBool("crouch", !isCrouching);
            isCrouching = !isCrouching;
        }
    }
    public bool IsSprinting()
    {
        return isSprinting;
    }
    public bool IsCrouching()
    {
        return isCrouching;
    }
    public Quaternion GetHitPointFromMouse()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // ***Calculate direction from tranform to mouse click
                Vector3 direction = hit.point.magnitude == 0 ? hit.point : hit.point - gameObject.transform.position;

                // ***Rotate only y axis
                lookAtRotationOnly_Y = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x, Quaternion.LookRotation(direction).eulerAngles.y, gameObject.transform.rotation.eulerAngles.z);
                return lookAtRotationOnly_Y;
            }
        }
        return lookAtRotationOnly_Y;
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
            //playerObj.transform.rotation = Quaternion.Slerp(playerObj.transform.rotation, targetRotation, 7f * Time.deltaTime);
        }
        return targetRotation;
    }
}
