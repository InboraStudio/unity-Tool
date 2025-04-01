using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);
    
    [Header("Smooth Settings")]
    public float smoothSpeed = 5f;
    public float rotationSpeed = 2f;
    
    [Header("Boundaries")]
    public float minY = 2f;
    public float maxY = 10f;
    public float minX = -10f;
    public float maxX = 10f;
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Clamp position within boundaries
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        
        // Smoothly move camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        
        // Look at target
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
} 