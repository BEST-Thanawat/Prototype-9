using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KinematicPlayer : MonoBehaviour
{
    //public ExampleCharacterCamera OrbitCamera;
    //public Transform CameraFollowPoint;
    [SerializeField]
    public float runSpeed = 3.25f, sprintSpeed = 5.841f, crouchSpeed = 0.56f;
    public KinematicPlayerCharacterController Character;

    private const string MouseXInput = "Mouse X";
    private const string MouseYInput = "Mouse Y";
    private const string MouseScrollInput = "Mouse ScrollWheel";
    private const string HorizontalInput = "Horizontal";
    private const string VerticalInput = "Vertical";

    private RaycastHit hit;
    private Quaternion lookAtRotationOnly_Y = Quaternion.identity;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool isCrouching = false;
    private bool isSprinting = false;
    private bool isRunning = false;
    private bool clickToMove = false;

    private void Awake()
    {
        animator = Character.GetComponent<Animator>();
        navMeshAgent = Character.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = runSpeed;
    }
    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;

        //// Tell camera to follow transform
        //OrbitCamera.SetFollowTransform(CameraFollowPoint);

        //// Ignore the character's collider(s) for camera obstruction checks
        //OrbitCamera.IgnoredColliders.Clear();
        //OrbitCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
    }

    private void Update()
    {
        HandleCharacterInput();

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
        CheckNavMeshDistance();
    }

    private void HandleCameraInput()
    {
        // Create the look input vector for the camera
        float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
        float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
        Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

        // Prevent moving the camera while the cursor isn't locked
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            lookInputVector = Vector3.zero;
        }

        // Input for zooming the camera (disabled in WebGL because it can cause problems)
        float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

        // Apply inputs to the camera
        //OrbitCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

        // Handle toggling zoom level
        //if (Input.GetMouseButtonDown(1))
        //{
        //    OrbitCamera.TargetDistance = (OrbitCamera.TargetDistance == 0f) ? OrbitCamera.DefaultDistance : 0f;
        //}
    }

    private void HandleCharacterInput()
    {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        // Build the CharacterInputs struct
        //characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
        //characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
        //characterInputs.CameraRotation = OrbitCamera.Transform.rotation;
        characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);

        // ***Get Physics.Raycast hit.point
        characterInputs.Destination = GetHitPointFromMouse();

        // ***Crouch
        characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
        characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);
        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }

    //------------Not Kinematic Controller Section -----------------------------------------------------------------------------------
    public Quaternion GetRotation()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Quaternion targetRotation = Quaternion.identity;
        float hitDist = 0f;

        Physics.Raycast(ray, out hit);

        if (playerPlane.Raycast(ray, out hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            targetPoint.x = 0;
            targetPoint.z = 0;
        }
        return targetRotation;
    }

    public Quaternion GetHitPointFromMouse()
    {
        //if (Input.GetMouseButton(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        // ***Calculate direction from tranform to mouse click
        //        Vector3 direction = hit.point.magnitude == 0 ? hit.point : hit.point - Character.transform.position;

        //        // ***Rotate only y axis
        //        lookAtRotationOnly_Y = Quaternion.Euler(Character.transform.rotation.eulerAngles.x, Quaternion.LookRotation(direction).eulerAngles.y, Character.transform.rotation.eulerAngles.z);
        //        return lookAtRotationOnly_Y;
        //    }
        //}
        //return lookAtRotationOnly_Y;


        // ***Calculate direction from tranform to mouse click
        Vector3 direction = hit.point.magnitude == 0 ? hit.point : hit.point - Character.transform.position;

        // ***Rotate only y axis
        lookAtRotationOnly_Y = Quaternion.Euler(Character.transform.rotation.eulerAngles.x, Quaternion.LookRotation(direction).eulerAngles.y, Character.transform.rotation.eulerAngles.z);
        return lookAtRotationOnly_Y;


        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out hit))
        //{
        //    // ***Calculate direction from tranform to mouse click
        //    Vector3 direction = hit.point.magnitude == 0 ? hit.point : hit.point - Character.transform.position;

        //    // ***Rotate only y axis
        //    lookAtRotationOnly_Y = Quaternion.Euler(Character.transform.rotation.eulerAngles.x, Quaternion.LookRotation(direction).eulerAngles.y, Character.transform.rotation.eulerAngles.z);
        //    return lookAtRotationOnly_Y;
        //}
        //return lookAtRotationOnly_Y;
    }
    private void MoveToCursor()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit rhit;
        //if (navMeshAgent.enabled)
        //{
        //    if (Physics.Raycast(ray, out rhit))
        //    {
        //        navMeshAgent.destination = rhit.point;
        //        isRunning = true;
        //    }
        //}

        //Debug.DrawRay(ray.origin, ray.direction * 100);
        clickToMove = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (navMeshAgent.enabled)
        {
            if (Physics.Raycast(ray, out hit))
            {
                navMeshAgent.destination = hit.point;
                isRunning = true;
                clickToMove = false;
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * 100);
    }
    private void UpdateAnimatorSpeed()
    {
        //Get velocity from NavMeshAgent
        Vector3 velocity = navMeshAgent.velocity;
        Vector3 localVelocity = Character.transform.InverseTransformDirection(velocity);

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
    public void CheckNavMeshDistance()
    {
        if (!navMeshAgent.pathPending)
        {
            if ((navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) || (navMeshAgent.remainingDistance <= 0.1f))
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    isRunning = false;
                }
            }
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
    public bool IsMoving()
    {
        return !((isSprinting && isCrouching) && isRunning);
    }
    public bool ClickToMove()
    {
        return clickToMove;
    }
}