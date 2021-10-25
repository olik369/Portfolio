using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MyGizmos : MonoBehaviour
{
    public Color gizmoColor;
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(this.transform.position, 3.0f);
    }
}
