using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public struct PlayerCharacterInputsRootMotion
{
    public float MoveAxisForward;
    public float MoveAxisRight;
    //public Quaternion CameraRotation;
    public bool JumpDown;
    public bool Crouch;
    //public bool CrouchDown;
    //public bool CrouchUp;
    public Vector3 Destination;
    public Quaternion Rotation;
    public RaycastHit MoveToPosition;
    public bool MouseClick;
    public float WalkSpeed;
    public float RunSpeed;
    public float SprintSpeed;
    public float CrouchSpeed;
}

[RequireComponent(typeof(NavMeshAgent))]
public class KinematicPlayerCharacterControllerRootMotion : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor Motor;

    [Header("Stable Movement")]
    public float MaxStableMoveSpeed = 10f;
    public float StableMovementSharpness = 15;
    public float OrientationSharpness = 10;

    [Header("Air Movement")]
    public float MaxAirMoveSpeed = 10f;
    public float AirAccelerationSpeed = 5f;
    public float Drag = 0.1f;

    [Header("Animation Parameters")]
    public Animator CharacterAnimator;
    public float ForwardAxisSharpness = 10;
    public float TurnAxisSharpness = 5;

    [Header("Jumping")]
    public bool AllowJumpingWhenSliding = false;
    public bool AllowDoubleJump = false;
    public float JumpSpeed = 10f;
    public float JumpPreGroundingGraceTime = 0f;
    public float JumpPostGroundingGraceTime = 0f;

    [Header("Misc")]
    public Vector3 Gravity = new Vector3(0, -30f, 0);
    public Transform MeshRoot;
    public KinematicPlayerRootMotion KinematicPlayerRootMotion;

    [Header("Roration")]
    public float RotationSpeed = 6f;

    private Collider[] _probedColliders = new Collider[8];
    private Vector3 _moveInputVector;
    private Vector3 _lookInputVector;
    private float _forwardAxis;
    private float _rightAxis;
    private float _targetForwardAxis;
    private float _targetRightAxis;
    private bool _jumpRequested = false;
    private bool _jumpConsumed = false;
    private bool _jumpedThisFrame = false;
    private float _timeSinceJumpRequested = Mathf.Infinity;
    private float _timeSinceLastAbleToJump = 0f;
    private bool _doubleJumpConsumed = false;
    private bool _shouldBeCrouching = false;
    private bool _isCrouching = false;
    private Vector3 _rootMotionPositionDelta;
    private Quaternion _rootMotionRotationDelta;

    //private Vector3 destination;
    //private Quaternion rotation;
    //private RaycastHit movePosition;
    //private float walkSpeed;
    //private float runSpeed;
    //private float sprintSpeed;
    //private float crouchSpeed;
    //private Vector2 smoothDeltaPosition = Vector2.zero;
    //private Vector2 velocity = Vector2.zero;
    //private bool mouseClick;

    Vector3 direction = Vector3.zero;
    public int ObjectiveAsSpeed = -1;
    public float MovementSpeed = 0.5f;
    private NavMeshAgent navAgent;
    private Quaternion rotate = Quaternion.identity;

    //Moving navmesh
    [SerializeField] float m_MovingTurnSpeed = 360;
    [SerializeField] float m_StationaryTurnSpeed = 180;
    float m_TurnAmount;
    float m_ForwardAmount;
    Vector3 m_GroundNormal;
    bool m_IsGrounded;
    bool m_Crouching;
    bool m_jumping;
    [SerializeField] float m_GroundCheckDistance = 0.2f;
    [SerializeField] float m_AnimSpeedMultiplier = 1f;
    
    private void Awake()
    {
    }

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.stoppingDistance = 0.2f;
        navAgent.updateRotation = false;

        _rootMotionPositionDelta = Vector3.zero;
        _rootMotionRotationDelta = Quaternion.identity;

        // Assign to motor
        Motor.CharacterController = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                navAgent.SetDestination(hit.point);
            } 
        }

        if (navAgent.remainingDistance > navAgent.stoppingDistance)
        {
            Move(navAgent.desiredVelocity, false, false);
        }
        else
        {
            //Move(navAgent.desiredVelocity, false, false);
            //Move(Vector3.zero, false, false);
            StopMoving();
        }
    }

    /// <summary>
    /// This is called every frame by MyPlayer in order to tell the character what its inputs are
    /// </summary>
    public void SetInputs(ref PlayerCharacterInputsRootMotion inputs)
    {
        // Axis inputs
        _targetForwardAxis = inputs.MoveAxisForward;
        _targetRightAxis = inputs.MoveAxisRight;

        // Crouching input
        if (inputs.Crouch)
        {
            m_Crouching = true;
            if (m_Crouching)
            {
                Motor.SetCapsuleDimensions(0.7f, 1f, 0.7f);
                navAgent.height = 1.2f;
                //MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
            }
        }
        else
        {
            m_Crouching = false;
            Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
            navAgent.height = 2f;
        }

        if (inputs.JumpDown)
        {
            m_jumping = true;
            _timeSinceJumpRequested = 0f;
            _jumpRequested = true;
        }
        else
        {
            m_jumping = false;
        }

        //rotation = inputs.Rotation;
        //destination = inputs.Destination;
        //movePosition = inputs.MoveToPosition;
        //walkSpeed = inputs.WalkSpeed;
        //runSpeed = inputs.RunSpeed;
        //sprintSpeed = inputs.SprintSpeed;
        //crouchSpeed = inputs.CrouchSpeed;
        //mouseClick = inputs.MouseClick;
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called before the character begins its movement update
    /// </summary>
    public void BeforeCharacterUpdate(float deltaTime)
    {
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its rotation should be right now. 
    /// This is the ONLY place where you should set the character's rotation
    /// </summary>
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        currentRotation = rotate * currentRotation;
    }
    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its velocity should be right now. 
    /// This is the ONLY place where you can set the character's velocity
    /// </summary>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity = navAgent.desiredVelocity;
        if (Motor.GroundingStatus.IsStableOnGround)
        {
            currentVelocity = Vector3.zero;
            if (deltaTime > 0)
            {
                // The final velocity is the velocity from root motion reoriented on the ground plane
                currentVelocity = _rootMotionPositionDelta / deltaTime;
                currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;
            }
            else
            {
                // Prevent division by zero
                currentVelocity = Vector3.zero;
            }
        }
        else
        {
            if (_forwardAxis > 0f)
            {
                // If we want to move, add an acceleration to the velocity
                Vector3 targetMovementVelocity = Motor.CharacterForward * _forwardAxis * MaxAirMoveSpeed;
                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
            }

            // Gravity
            currentVelocity += Gravity * deltaTime;

            // Drag
            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
        }

        // Handle jumping
        _jumpedThisFrame = false;
        _timeSinceJumpRequested += deltaTime;
        if (_jumpRequested)
        {
            // Handle double jump
            if (AllowDoubleJump)
            {
                if (_jumpConsumed && !_doubleJumpConsumed && (AllowJumpingWhenSliding ? !Motor.GroundingStatus.FoundAnyGround : !Motor.GroundingStatus.IsStableOnGround))
                {
                    Motor.ForceUnground(0.1f);

                    // Add to the return velocity and reset jump state
                    currentVelocity += (Motor.CharacterUp * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                    _jumpRequested = false;
                    _doubleJumpConsumed = true;
                    _jumpedThisFrame = true;
                }
            }

            // See if we actually are allowed to jump
            if (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
            {
                // Calculate jump direction before ungrounding
                Vector3 jumpDirection = Motor.CharacterUp;
                if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                {
                    jumpDirection = Motor.GroundingStatus.GroundNormal;
                }

                // Makes the character skip ground probing/snapping on its next update. 
                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                Motor.ForceUnground(0.1f);

                // Add to the return velocity and reset jump state
                currentVelocity += (jumpDirection * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                _jumpRequested = false;
                _jumpConsumed = true;
                _jumpedThisFrame = true;
            }
        }
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called after the character has finished its movement update
    /// </summary>
    public void AfterCharacterUpdate(float deltaTime)
    {
        // Reset root motion deltas
        _rootMotionPositionDelta = Vector3.zero;
        _rootMotionRotationDelta = Quaternion.identity;

        if (m_ForwardAmount == 0)
        {
            rotate = CharacterLookAtMouse();
        }
        else
        {
            rotate = GetRotate();
        }
            
        // Handle jump-related values
        // Handle jumping pre-ground grace period
        if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
        {
            _jumpRequested = false;
        }

        if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
        {
            // If we're on a ground surface, reset jumping values
            if (!_jumpedThisFrame)
            {
                _doubleJumpConsumed = false;
                _jumpConsumed = false;
            }
            _timeSinceLastAbleToJump = 0f;
        }
        else
        {
            // Keep track of time since we were last able to jump (for grace period)
            _timeSinceLastAbleToJump += deltaTime;
        }
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void AddVelocity(Vector3 velocity)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    private void OnAnimatorMove()
    {
        // Accumulate rootMotion deltas between character updates 
        _rootMotionPositionDelta += CharacterAnimator.deltaPosition;
        _rootMotionRotationDelta = CharacterAnimator.deltaRotation * _rootMotionRotationDelta;
    }




    // ***Navmesh navigator and animator
    public void Move(Vector3 move, bool crouch, bool jump)
    {
        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);

        //if (m_ForwardAmount < 0.99f)
        //Debug.Log(move.z);
        m_ForwardAmount = Mathf.Clamp(move.z, 0f, 0.75f);
        

        // send input and other state parameters to the animator
        UpdateAnimator(move);
    }
    void UpdateAnimator(Vector3 move)
    {
        // update the animator parameters
        CharacterAnimator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
        if(m_ForwardAmount <= 0)
        {
            CharacterAnimator.SetFloat("Idle_Random", KinematicPlayerRootMotion.GetRandomStandNumber(), 0.5f, Time.deltaTime);
        }

        CharacterAnimator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
        CharacterAnimator.SetBool("OnGround", m_IsGrounded);
        CharacterAnimator.SetBool("Crouch", m_Crouching);
        if (m_Crouching == true)
        {
            CharacterAnimator.SetFloat("Idle_Random", KinematicPlayerRootMotion.GetRandomCrouchNumber(), 0.1f, Time.deltaTime);
        }


        if (m_IsGrounded && m_jumping)
        {
            CharacterAnimator.SetTrigger("Jump");
            if (m_ForwardAmount == 1) CharacterAnimator.SetFloat("JumpState", 3);
            else if (m_ForwardAmount >= 0.75f) CharacterAnimator.SetFloat("JumpState", 2);
            else if (m_ForwardAmount >= 0.50f) CharacterAnimator.SetFloat("JumpState", 1);
            else CharacterAnimator.SetFloat("JumpState", 0);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (m_IsGrounded && move.magnitude > 0)
        {
            CharacterAnimator.speed = m_AnimSpeedMultiplier;
        }
        else
        {
            // don't use that while airborne
            CharacterAnimator.speed = 1;
        }

    }
    public Quaternion GetRotate()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        return Quaternion.Euler(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    // ***Character look at mouse
    public Quaternion CharacterLookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Vector3 move = transform.InverseTransformPoint(hit.point);
            m_TurnAmount = Mathf.Atan2(move.x, move.z);
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
            return Quaternion.Euler(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        }
        return Quaternion.identity;
    }
    public void StopMoving()
    {
        //CheckGroundStatus();
        Vector3 stop = Vector3.zero;

        m_TurnAmount = Mathf.Atan2(stop.x, stop.z);
        m_ForwardAmount = stop.z;
        UpdateAnimator(stop);
    }
    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            CharacterAnimator.applyRootMotion = true;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
            CharacterAnimator.applyRootMotion = false;
        }
    }
    
}
