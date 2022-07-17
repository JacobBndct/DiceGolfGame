using UnityEngine;

public class SmoothFollowObject : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    [SerializeField] private float smoothSpeed = 0.125f;
    
    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position;
        desiredPosition.y = transform.position.y;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}