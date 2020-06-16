﻿using System.Collections;
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

    //private Quaternion targetRotation;
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
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //}

        //HandleCameraInput();
        HandleCharacterInput();
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
        PlayerCharacterInputsRootMotion characterInputs = new PlayerCharacterInputsRootMotion();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
        characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
        //characterInputs.CameraRotation = OrbitCamera.Transform.rotation;
        characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);

        // ***Get Physics.Raycast hit.point
        characterInputs.Destination = Character.GetComponent<KinematicMoverRootMotion>().GetHitPointFromMouse();

        // ***Crouch
        characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
        characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);
        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }
}