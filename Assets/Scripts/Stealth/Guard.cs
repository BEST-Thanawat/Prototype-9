using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public class Guard : MonoBehaviour
{
    public static event Action OnGuardHasSpottedPlayer;
    public Transform pathHolder;
    private Vector3[] waypoints;
    private float speed = 5f;
    private float waitTime = 5f;
    private float turnSpeed = 90;
    private float timeToSpotPlayer = 0.5f;

    public Light sportLight;
    public float viewDistance;
    float viewAngle;
    float playerVisibleTimer;

    Transform player;
    public LayerMask viewMask; 
    Color originalSpotLightColour;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = sportLight.spotAngle;
        originalSpotLightColour = sportLight.color;

        waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));
    }
    private void Update()
    {
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
            //sportLight.color = Color.red;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
            //sportLight.color = originalSpotLightColour;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        sportLight.color = Color.Lerp(originalSpotLightColour, Color.red, playerVisibleTimer / timeToSpotPlayer);
        
        if(playerVisibleTimer >= timeToSpotPlayer)
        {
            if(OnGuardHasSpottedPlayer != null)
            {
                OnGuardHasSpottedPlayer();
            }
        }
    }
    private bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if(angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if(!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach(Transform wayPoint in pathHolder)
        {
            Gizmos.DrawSphere(wayPoint.position, 0.3f);
            Gizmos.DrawLine(previousPosition, wayPoint.position);
            previousPosition = wayPoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }

            yield return null;
        }
    }
    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarger = lookTarget - transform.position;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarger.z, dirToLookTarger.x) * Mathf.Rad2Deg;
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            //transform.rotation = Quaternion.LookRotation(Vector3.up * angle);
            yield return null;
        }
        //transform.rotation = Quaternion.LookRotation(dirToLookTarger);
        //yield return null;
    }
}
