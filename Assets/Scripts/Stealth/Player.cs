using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event Action OnReachedEndOfLevel;
    public float moveSpeed = 7;
    public float smoothMoveTime = 0.1f;
    public float turnSpeed = 8f;
    
    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;

    Rigidbody rigidbody;
    Vector3 velocity;
    bool disabled;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpottedPlayer += Disable;
    }
    private void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
    }
    private void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if (!disabled)
        {
            inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        }
        
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
        
        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
        //transform.eulerAngles = Vector3.up * angle;
        //transform.Translate(transform.forward * moveSpeed * Time.deltaTime * smoothInputMagnitude, Space.World);
    }
    private void Disable()
    {
        disabled = true;
    }

    private void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Finish")
        {
            Disable();
            if (OnReachedEndOfLevel != null)
            {
                OnReachedEndOfLevel();
            }
        }
    }
}
