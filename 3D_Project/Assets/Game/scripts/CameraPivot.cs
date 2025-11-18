using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    public Camera MyCamera;
    public float MaxDistance;
    public LayerMask MyLayerMask;

    private void LateUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, MaxDistance, MyLayerMask))
        {
            MyCamera.transform.localPosition = new Vector3(0f, 0f, -hit.distance);
        }
        else
        {
            MyCamera.transform.localPosition = new Vector3(0f, 0f, -MaxDistance);
        }
    }
}
