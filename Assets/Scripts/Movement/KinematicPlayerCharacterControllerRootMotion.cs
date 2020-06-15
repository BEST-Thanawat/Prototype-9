using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerCharacterInputsRootMotion
{
    public float MoveAxisForward;
    public float MoveAxisRight;
    //public Quaternion CameraRotation;
    public bool JumpDown;
    public bool CrouchDown;
    public bool CrouchUp;
    public Quaternion Destination;
}

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

    private Quaternion _lookAtRotationOnly_Y = Quaternion.identity;
    private KinematicMoverRootMotion kinematicMover;
    private void Awake()
    {
        kinematicMover = GetComponent<KinematicMoverRootMotion>();
        if (kinematicMover == null) Debug.LogError("PlayerKinematicCharacterControllerRootMotion: Object is null");
    }
    private void Start()
    {
        _rootMotionPositionDelta = Vector3.zero;
        _rootMotionRotationDelta = Quaternion.identity;

        // Assign to motor
        Motor.CharacterController = this;
    }

    private void Update()
    {
        Debug.Log(_forwardAxis);
        // Handle animation
        _forwardAxis = Mathf.Lerp(_forwardAxis, _targetForwardAxis, 1f - Mathf.Exp(-ForwardAxisSharpness * Time.deltaTime));
        _rightAxis = Mathf.Lerp(_rightAxis, _targetRightAxis, 1f - Mathf.Exp(-TurnAxisSharpness * Time.deltaTime));
        CharacterAnimator.SetFloat("forwardSpeed", _forwardAxis);
        CharacterAnimator.SetFloat("turnSpeed", _rightAxis);
        CharacterAnimator.SetBool("onGround", Motor.GroundingStatus.IsStableOnGround);
    }

    /// <summary>
    /// This is called every frame by MyPlayer in order to tell the character what its inputs are
    /// </summary>
    public void SetInputs(ref PlayerCharacterInputsRootMotion inputs)
    {
        // Axis inputs
        _targetForwardAxis = inputs.MoveAxisForward;
        _targetRightAxis = inputs.MoveAxisRight;

        //_lookAtRotationOnly_Y = inputs.Destination;
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
        currentRotation = _rootMotionRotationDelta * currentRotation;
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its velocity should be right now. 
    /// This is the ONLY place where you can set the character's velocity
    /// </summary>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (Motor.GroundingStatus.IsStableOnGround)
        {
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
}
