using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicPlayerRootMotion : MonoBehaviour
{
    //public ExampleCharacterCamera OrbitCamera;
    //public Transform CameraFollowPoint;
    public KinematicPlayerCharacterControllerRootMotion Character;

    private const string MouseXInput = "Mouse X";
    private const string MouseYInput = "Mouse Y";
    private const string MouseScrollInput = "Mouse ScrollWheel";
    public const string HorizontalInput = "Horizontal";
    public const string VerticalInput = "Vertical";

    [SerializeField]
    public float runSpeed = 3.25f, sprintSpeed = 5.841f, crouchSpeed = 0.56f;
    private RaycastHit hit;
    private Quaternion lookAtRotationOnly_Y = Quaternion.identity;
    public float timeTakenDuringLerp = 1f;

    public float distanceToMove = 10;
    //private bool isLerping;
    //private Quaternion startPosition;
    //private Quaternion endPosition;
    //private Quaternion rotationAngle;
    //private float timeStartedLerping;
    //private Ray ray;

    private bool m_Crouching = false;
    [Range(0,10)]
    [SerializeField]
    private int idleCrouchAnimCount = 2;
    [Range(0, 10)]
    [SerializeField]
    private int idleStandAnimCount = 6;
    [SerializeField]
    private float coolDownRandomIdleTime = 7f;
    [SerializeField]
    private float coolDownRandomStandTime = 5f;
    private float timeSinceRandomCrouch;
    private float timeSinceRandomStand;
    private float randomCrouchNumber;
    private float randomStandNumber;
    private void Start()
    {
    }

    private void Update()
    {
        //GetRaycastHit();
        //DetectMouseMovement();
        HandleCharacterInput();
    }

    //void FixedUpdate()
    //{
    //    if (isLerping)
    //    {
    //        float timeSinceStarted = Time.time - timeStartedLerping;
    //        float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

    //        rotationAngle = Quaternion.Lerp(startPosition, endPosition, percentageComplete);

    //        //When we've completed the lerp, we set _isLerping to false
    //        if (percentageComplete >= 1.0f)
    //        {
    //            isLerping = false;
    //        }
    //    }
    //}
    private void HandleCharacterInput()
    {
        PlayerCharacterInputsRootMotion characterInputs = new PlayerCharacterInputsRootMotion();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
        characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
        //characterInputs.CameraRotation = OrbitCamera.Transform.rotation;
        characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);

        // ***Get Physics.Raycast hit.point
        //characterInputs.Destination = GetHitPointFromMouse();
        // ***Get rotation angle
        //characterInputs.Rotation = rotationAngle;

        //characterInputs.MoveToPosition = hit;
        //characterInputs.WalkSpeed = runSpeed;
        //characterInputs.MouseClick = mouseClick;

        // ***Crouch
        if (Input.GetKeyDown(KeyCode.C))
        {
            timeSinceRandomCrouch = Time.time + coolDownRandomIdleTime;
            m_Crouching = !m_Crouching;
        }

        if (m_Crouching)
        {
            if (Time.time >= timeSinceRandomCrouch)
            {
                randomCrouchNumber = (float)Random.Range(0, idleCrouchAnimCount);
                SetCooldownCrouchTime();
            }
        }

        if (Time.time >= timeSinceRandomStand)
        {
            randomStandNumber = (float)Random.Range(0, idleStandAnimCount);
            SetCooldownStandTime();
        }
        

        characterInputs.Crouch = m_Crouching;
        //characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
        //characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);
        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }

    public float GetRandomCrouchNumber()
    {
        return randomCrouchNumber;
    }
    public float GetCooldownCrouchTime()
    {
        return timeSinceRandomCrouch;
    }
    public void SetCooldownCrouchTime()
    {
        timeSinceRandomCrouch = Time.time + coolDownRandomIdleTime;
    }

    public float GetRandomStandNumber()
    {
        return randomStandNumber;
    }
    public float GetCooldownStandTime()
    {
        return timeSinceRandomStand;
    }
    public void SetCooldownStandTime()
    {
        timeSinceRandomStand = Time.time + coolDownRandomStandTime;
    }

    //public void GetRaycastHit()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        if(Physics.Raycast(ray2, out hit))
    //        {
    //            // ***Calculate direction from tranform to mouse click
    //            Vector3 direction = hit.point - Character.transform.position; // hit.point.magnitude == 0 ? hit.point :

    //            // ***Rotate only y axis
    //            lookAtRotationOnly_Y = Quaternion.Euler(Character.transform.rotation.eulerAngles.x, Quaternion.LookRotation(direction).eulerAngles.y, Character.transform.rotation.eulerAngles.z);
    //        }
    //    }
    //}
    //public Quaternion DetectMouseMovement()
    //{
    //    if (Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0)
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        // ***Set raycast hit for character rotation.
    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            Quaternion targetRotation = Quaternion.identity;
    //            Vector3 targetPoint = hit.point;
    //            targetRotation = Quaternion.LookRotation(targetPoint - Character.transform.position);

    //            StartLerping();
    //            return targetRotation;
    //        }
    //    }

    //    return Quaternion.identity;
    //}
    //public Vector3 GetHitPointFromMouse()
    //{
    //    // ***Calculate direction from tranform to mouse click
    //    Vector3 direction = Vector3.zero;
    //    direction = hit.point - Character.transform.position; // hit.point.magnitude == 0 ? hit.point :

    //    // ***Rotate only y axis

    //    lookAtRotationOnly_Y = Quaternion.Euler(Character.transform.rotation.eulerAngles.x, Quaternion.LookRotation(direction).eulerAngles.y, Character.transform.rotation.eulerAngles.z);
    //    return hit.point;
    //}

    //public void DetectMouseMovement()
    //{
    //    if (Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0) 
    //    {
    //        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        // ***Set raycast hit for character rotation.
    //        Physics.Raycast(ray, out hit);

    //        StartLerping();
    //    }
    //}

    //public Quaternion GetRotation()
    //{
    //    Plane playerPlane = new Plane(Vector3.up, Character.transform.position);
    //    float hitDist = 0f;
    //    Quaternion targetRotation = Quaternion.identity;

    //    if (playerPlane.Raycast(ray, out hitDist))
    //    {
    //        Vector3 targetPoint = ray.GetPoint(hitDist);
    //        targetRotation = Quaternion.LookRotation(targetPoint - Character.transform.position);
    //        targetPoint.x = 0;
    //        targetPoint.z = 0;
    //    }
    //    return targetRotation;
    //}

    //void StartLerping()
    //{
    //    isLerping = true;
    //    timeStartedLerping = Time.time;

    //    //We set the start position to the current position, and the finish to 10 spaces in the 'forward' direction
    //    startPosition = Character.transform.rotation;
    //    endPosition = GetRotation();
    //}
    //void StartLerping()
    //{
    //    isLerping = true;
    //    timeStartedLerping = Time.time;

    //    //We set the start position to the current position, and the finish to 10 spaces in the 'forward' direction
    //    startPosition = Character.transform.rotation;
    //    endPosition = DetectMouseMovement();
    //}
    //public Quaternion GetResultRotation()
    //{
    //    return rotationAngle;
    //}

    //public RaycastHit GetResultRaycastHit()
    //{
    //    return hit;
    //}
}