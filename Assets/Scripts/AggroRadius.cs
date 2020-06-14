using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroRadius : MonoBehaviour
{
    public float _aggroRadius = 5;
    public float _returnToOriginRadius = 15;

    public Material _aggroMat;
    public Material _neutralMat;
    public Material _returningMat;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, _aggroRadius);
        Gizmos.color = new Color(0.25f, 0.25f, 0.25f, 0.1f);
        Gizmos.DrawSphere(transform.position, _returnToOriginRadius);
    }
}
