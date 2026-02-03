using System.Collections;
using UnityEngine;

public class SeatTeleporter : MonoBehaviour
{
    [Header("Teleportation Settings")]
    [Tooltip("The target position where the player will be teleported (the window seat)")]
    public Transform seatPosition;
    
    [Tooltip("Tag of the XR Origin (usually 'Player' or 'XR Origin')")]
    public string playerTag = "Player";
    
    [Tooltip("Reference to the Main Camera (will auto-find if not set)")]
    public Camera mainCamera;

    [Tooltip("Optional offset applied to the seat target (use Y = -0.05 or -0.10 if you feel too high)")]
    public Vector3 seatCameraOffset = Vector3.zero;
    
    [Tooltip("The plane transform (to parent player to it)")]
    public Transform planeTransform;
    
    [Tooltip("Reference to PlaneMovement script to trigger flight")]
    public PlaneMovement planeMovement;
    
    [Header("Locomotion Control")]
    [Tooltip("Disable these movement providers when seated")]
    public MonoBehaviour[] locomotionComponents;
    
    [Tooltip("Also disable the CharacterController if present")]
    public bool disableCharacterController = true;
    
    [Header("Optional")]
    [Tooltip("Fade screen before teleporting (optional)")]
    public bool useFade = false;
    
    [Tooltip("Fade duration in seconds")]
    public float fadeDuration = 0.5f;
    
    private bool hasBeenTriggered = false;
    private GameObject xrOrigin;
    private CharacterController characterController;
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player touched the collider
        if (hasBeenTriggered) return;
        
        // Don't trigger if plane is currently landing
        if (planeMovement != null && planeMovement.shouldLandInsteadOfTakeoff)
        {
            Debug.Log("Ignoring collider trigger - plane is landing.");
            return;
        }
        
        if (other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag))
        {
            // Find the XR Origin
            xrOrigin = other.transform.root.gameObject;
            
            if (seatPosition != null)
            {
                hasBeenTriggered = true;
                StartCoroutine(TeleportToSeatRoutine());
            }
            else
            {
                Debug.LogError("Seat position is not assigned in SeatTeleporter!");
            }
        }
    }

    private IEnumerator TeleportToSeatRoutine()
    {
        if (xrOrigin == null)
            yield break;

        // DISABLE CONTROLS (as requested: "disable player controls everywhere")
        DisablePlayerControls(xrOrigin);

        // Find a camera that actually belongs to this XR Origin.
        if (mainCamera == null || mainCamera.transform.root != xrOrigin.transform)
        {
            var camInRig = xrOrigin.GetComponentInChildren<Camera>(true);
            if (camInRig != null)
                mainCamera = camInRig;
            else
                mainCamera = Camera.main;
        }

        // Apply a small default downward offset if not set, or use user's value
        // Use the user's Inspector value seatCameraOffset
        var targetPos = seatPosition.position + seatCameraOffset;

        // === DETAILED DEBUG INFO ===
        Debug.Log("=== BEFORE TELEPORT ===");
        Debug.Log($"XR Origin position: {xrOrigin.transform.position}");
        Debug.Log($"Camera position: {(mainCamera != null ? mainCamera.transform.position.ToString() : "null")}");

        Vector3 cameraLocalOffset = Vector3.zero;
        if (mainCamera != null)
        {
            cameraLocalOffset = xrOrigin.transform.InverseTransformPoint(mainCamera.transform.position);
            Debug.Log($"Camera LOCAL offset from origin: {cameraLocalOffset}");
        }

        // Rotate first, then position so the camera ends up exactly on the target.
        xrOrigin.transform.rotation = seatPosition.rotation;
        Vector3 cameraWorldOffset = xrOrigin.transform.TransformVector(cameraLocalOffset);
        xrOrigin.transform.position = targetPos - cameraWorldOffset;

        // Apply changes
        yield return null;

        Debug.Log("=== AFTER TELEPORT ===");
        Debug.Log($"XR Origin moved to: {xrOrigin.transform.position}");
        
        // Parent player to plane so they move together.
        if (planeTransform != null)
        {
            xrOrigin.transform.SetParent(planeTransform, true);
            Debug.Log("Player parented to plane.");
        }

        // Start plane movement ONLY if not landing (landing sequence handles its own movement)
        if (planeMovement != null && !planeMovement.shouldLandInsteadOfTakeoff)
        {
            planeMovement.StartMovement();
            Debug.Log("Plane movement triggered!");
        }
        else if (planeMovement != null && planeMovement.shouldLandInsteadOfTakeoff)
        {
            Debug.Log("Landing mode - plane movement already controlled by landing sequence.");
        }

        Debug.Log("Player teleported to window seat (Controls Disabled).");
    }

    // Public method to trigger teleport from external scripts
    public void TriggerTeleport(GameObject playerRigRoot)
    {
        xrOrigin = playerRigRoot;
        StartCoroutine(TeleportToSeatRoutine());
    }

    private void DisablePlayerControls(GameObject rig)
    {
        // ONLY disable WASD movement (XRDebugLocomotion)
        // DO NOT disable CharacterController - it's needed for VR body tracking!
        var debugLoco = rig.GetComponent<XRDebugLocomotion>();
        if (debugLoco != null) 
        {
            debugLoco.enabled = false;
            Debug.Log("Disabled XRDebugLocomotion (WASD movement)");
        }

        // Disable teleport/snap turn locomotion providers if any
        var providers = rig.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Locomotion.LocomotionProvider>(true);
        foreach (var p in providers) 
        {
            p.enabled = false;
            Debug.Log($"Disabled {p.GetType().Name}");
        }
        
        Debug.Log("Player WASD controls disabled. Head tracking (look around) still works.");
    }
}
