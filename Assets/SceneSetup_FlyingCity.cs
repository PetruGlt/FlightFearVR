using UnityEngine;

public class SceneSetup_FlyingCity : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The seat position inside the plane where camera should be")]
    public Transform seatPosition;
    
    [Tooltip("Will auto-find if not set")]
    public Camera mainCamera;
    
    void Start()
    {
        // Find XR Origin and camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera == null)
        {
            Debug.LogError("Cannot find main camera!");
            return;
        }
        
        // Find the XR Origin (root of the camera)
        Transform xrOrigin = mainCamera.transform.root;
        
        // Unparent first to ensure clean positioning
        xrOrigin.SetParent(null);
        
        // Position camera at seat BEFORE parenting (like in SeatTeleporter)
        if (seatPosition != null)
        {
            // Calculate the LOCAL offset between camera and XR Origin (in origin's local space)
            Vector3 cameraLocalOffset = xrOrigin.InverseTransformPoint(mainCamera.transform.position);
            Debug.Log($"Camera LOCAL offset from origin: {cameraLocalOffset}");
            
            // First rotate the origin to match the seat
            xrOrigin.rotation = seatPosition.rotation;
            
            // Then position it so the camera ends up at the seat position
            // Convert the local offset to world space with the new rotation
            Vector3 cameraWorldOffset = xrOrigin.TransformVector(cameraLocalOffset);
            xrOrigin.position = seatPosition.position - cameraWorldOffset;
            
            Debug.Log($"XR Origin positioned at: {xrOrigin.position}");
            Debug.Log($"Camera should be at: {mainCamera.transform.position}");
            Debug.Log($"Seat target was: {seatPosition.position}");
            Debug.Log($"Distance from target: {Vector3.Distance(mainCamera.transform.position, seatPosition.position)}");
        }
        
        // NOW parent to plane so they move together
        xrOrigin.SetParent(transform);
        
        Debug.Log("Player positioned in seat and parented to plane.");
    }
}
