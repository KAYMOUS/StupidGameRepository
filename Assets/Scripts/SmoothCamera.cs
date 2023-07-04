using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;
    public float minSpeed = 0.5f;
    public float maxSpeed = 2.0f;
    public float distanceThreshold = 1.0f;
    public Vector3 offset;

    private void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
        float distance = Vector3.Distance(transform.position, desiredPosition);
        float smoothSpeed = Mathf.Lerp(minSpeed, maxSpeed, distance / distanceThreshold);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
