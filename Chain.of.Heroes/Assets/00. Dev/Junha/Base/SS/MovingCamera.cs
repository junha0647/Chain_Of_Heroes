using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCamera : MonoBehaviour
{
    public Transform targetTransform;
    public Vector3 CameraOffset;

    private void Update()
    {
        transform.position = targetTransform.position + CameraOffset;
    }
}
