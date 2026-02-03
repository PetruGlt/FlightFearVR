using UnityEngine;

public class PlaneLanding : MonoBehaviour
{
    [Header("Waypoints (Same as PlaneMovement)")]
    [Tooltip("Drag the SAME waypoints used for takeoff")]
    public Transform[] waypoints;
    
    [Header("Landing Settings")]
    public float landingSpeed = 15f;
    public float rotationSpeed = 90f;
    
    private int currentWaypointIndex;
    private bool isLanding = false;
    private bool hasRotated = false;
    
    public void StartLanding()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned for landing!");
            return;
        }
        
        isLanding = true;
        // Start from the last waypoint and go backwards
        currentWaypointIndex = waypoints.Length - 1;
        
        Debug.Log("Landing sequence started!");
        
        // Rotate plane 180 degrees first
        StartCoroutine(RotatePlane180());
    }
    
    System.Collections.IEnumerator RotatePlane180()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 180, 0);
        
        float elapsed = 0f;
        float duration = 2f;
        
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.rotation = targetRotation;
        hasRotated = true;
        Debug.Log("Plane rotated 180 degrees. Beginning descent.");
    }
    
    void Update()
    {
        if (!isLanding || !hasRotated || waypoints == null || waypoints.Length == 0) return;
        
        // Get current target waypoint (going in reverse)
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        
        // Rotate towards target
        Vector3 directionToTarget = targetWaypoint.position - transform.position;
        bool isFacingTarget = false;
        
        if (directionToTarget.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            isFacingTarget = angle < 10f;
        }
        else
        {
            isFacingTarget = true;
        }
        
        // Move when facing target
        if (isFacingTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, landingSpeed * Time.deltaTime);
        }
        
        // Check if reached waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.5f)
        {
            currentWaypointIndex--;
            
            // Check if landed (reached first waypoint)
            if (currentWaypointIndex < 0)
            {
                isLanding = false;
                Debug.Log("Landing complete! Plane has stopped.");
                // Could trigger door opening or next event here
            }
        }
    }
}
