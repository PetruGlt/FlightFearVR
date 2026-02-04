using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneMovement : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Drag waypoint GameObjects in order: backup -> taxi -> runway -> takeoff")]
    public Transform[] waypoints;
    
    [Header("Movement Settings")]
    public float moveSpeed = 2f; // Slow taxi speed
    public float rotationSpeed = 90f; // Fast sharp turns
    public float accelerationRate = 1f; // Gradual speed up
    public float landingSpeed = 10f; // Speed for landing
    
    [Header("Takeoff Settings")]
    public float takeoffSpeed = 50f; // Fast for takeoff
    public float takeoffAngle = 15f; // Nose up angle
    public float altitudeForTransition = 100f; // Height to trigger fade and scene change
    
    [Header("Scene Transition")]
    public string nextSceneName = "FlyingOverCity"; // Name of next scene
    public float fadeToBlackDuration = 2f; // How long fade takes
    
    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private float currentSpeed;
    private bool hasTriggeredTransition = false;
    private bool isReverseLanding = false; // Play waypoints in reverse
    
    [Header("Landing (Auto-detected)")]
    public bool shouldLandInsteadOfTakeoff = false; // Set by PlayerPrefs

    [Header("Seat Placement (Landing)")]
    [Tooltip("Optional offset applied when snapping the camera to the seat on landing. Use Y = -0.05 or -0.10 if too high.")]
    public Vector3 seatCameraOffset = Vector3.zero;

    public GameObject panelObj;
    
    void Start()
    {
        currentSpeed = moveSpeed;
        
        // Check if we're returning from the city
        if (FlightManager.isReturningForLanding)
        {
            shouldLandInsteadOfTakeoff = true;
            isReverseLanding = true;
            //PlayerPrefs.SetInt("ReturningFromCity", 0); // Reset
            //PlayerPrefs.Save();
            
            // Start clean landing sequence with fade
            StartCoroutine(LandingSequenceWithFade());
        }
    }

    private void SnapPlayerToSeatImmediately()
    {
        var teleporter = FindSeatTeleporterForThisPlane();
        if (teleporter == null || teleporter.seatPosition == null)
        {
            Debug.LogError("Could not find SeatTeleporter/seatPosition for landing.");
            return;
        }

        GameObject playerRig = null;
        try { playerRig = GameObject.FindGameObjectWithTag("Player"); }
        catch { /* ignored */ }

        if (playerRig == null)
        {
            var camMain = Camera.main;
            if (camMain != null)
                playerRig = camMain.transform.root.gameObject;
        }

        if (playerRig == null)
        {
            Debug.LogError("Could not find Player rig for landing.");
            return;
        }

        var cam = playerRig.GetComponentInChildren<Camera>(true);
        if (cam == null)
            cam = Camera.main;

        var targetPos = teleporter.seatPosition.position + seatCameraOffset;

        if (cam != null)
        {
            Vector3 cameraLocalOffset = playerRig.transform.InverseTransformPoint(cam.transform.position);
            playerRig.transform.rotation = teleporter.seatPosition.rotation;
            Vector3 cameraWorldOffset = playerRig.transform.TransformVector(cameraLocalOffset);
            playerRig.transform.position = targetPos - cameraWorldOffset;
        }
        else
        {
            playerRig.transform.SetPositionAndRotation(targetPos, teleporter.seatPosition.rotation);
        }

        // Parent to plane IMMEDIATELY so rotation happens together
        playerRig.transform.SetParent(transform, true);

        Debug.Log($"Player snapped to seat IMMEDIATELY on landing scene load.");
    }

    private SeatTeleporter FindSeatTeleporterForThisPlane()
    {
        var teleporters = FindObjectsByType<SeatTeleporter>(FindObjectsSortMode.None);
        foreach (var t in teleporters)
        {
            if (t == null || t.seatPosition == null) continue;
            if (t.planeMovement == this) return t;
            if (t.planeTransform == transform) return t;
        }

        return FindFirstObjectByType<SeatTeleporter>();
    }

    private IEnumerator SnapPlayerToSeatForLanding()
    {
        var teleporter = FindSeatTeleporterForThisPlane();
        if (teleporter == null || teleporter.seatPosition == null)
        {
            Debug.LogError("Could not find SeatTeleporter/seatPosition for landing.");
            yield break;
        }

        GameObject playerRig = null;
        try { playerRig = GameObject.FindGameObjectWithTag("Player"); }
        catch { /* ignored */ }

        if (playerRig == null)
        {
            var camMain = Camera.main;
            if (camMain != null)
                playerRig = camMain.transform.root.gameObject;
        }

        if (playerRig == null)
        {
            Debug.LogError("Could not find Player rig for landing.");
            yield break;
        }

        var cam = playerRig.GetComponentInChildren<Camera>(true);
        if (cam == null)
            cam = Camera.main;

        var targetPos = teleporter.seatPosition.position + seatCameraOffset;

        var cc = playerRig.GetComponent<CharacterController>();
        bool hadCcEnabled = false;
        if (cc != null)
        {
            hadCcEnabled = cc.enabled;
            cc.enabled = false;
        }

        if (cam != null)
        {
            Vector3 cameraLocalOffset = playerRig.transform.InverseTransformPoint(cam.transform.position);
            playerRig.transform.rotation = teleporter.seatPosition.rotation;
            Vector3 cameraWorldOffset = playerRig.transform.TransformVector(cameraLocalOffset);
            playerRig.transform.position = targetPos - cameraWorldOffset;
        }
        else
        {
            playerRig.transform.SetPositionAndRotation(targetPos, teleporter.seatPosition.rotation);
        }

        playerRig.transform.SetParent(transform, true);

        yield return null;

        if (cc != null)
            cc.enabled = hadCcEnabled;

        if (cam != null)
        {
            Debug.Log($"Landing snap complete. Camera at: {cam.transform.position}");
            Debug.Log($"Distance from seat: {Vector3.Distance(cam.transform.position, targetPos)}");
        }
    }
    
    System.Collections.IEnumerator LandingSequenceWithFade()
    {
        Debug.Log("Landing sequence starting (NO BLACK SCREEN for debugging)...");
        
        // STEP 1: Position and rotate plane first
        Debug.Log("Positioning plane at waypoint 5...");
        if (waypoints != null && waypoints.Length > 0)
        {
            Transform lastWaypoint = waypoints[waypoints.Length - 1];
            transform.position = lastWaypoint.position;
        }
        
        Debug.Log("Rotating plane 180 degrees...");
        transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
        
        yield return new WaitForSeconds(0.5f);
        
        // STEP 2: NOW trigger seat teleporter
        var teleporter = FindSeatTeleporterForThisPlane();
        if (teleporter != null)
        {
            GameObject playerRig = null;
            try { playerRig = GameObject.FindGameObjectWithTag("Player"); }
            catch { }
            
            if (playerRig == null)
            {
                var mainCam = Camera.main;
                if (mainCam != null)
                    playerRig = mainCam.transform.root.gameObject;
            }
            
            if (playerRig != null)
            {
                Debug.Log("Triggering seat teleport for landing...");
                teleporter.TriggerTeleport(playerRig);
            }
            else
            {
                Debug.LogError("Could not find player rig!");
            }
        }
        else
        {
            Debug.LogError("Could not find SeatTeleporter!");
        }
        
        // STEP 3: Wait for teleport to complete
        yield return new WaitForSeconds(1.5f);
        
        Debug.Log("Teleport complete. Starting landing movement.");
        
        // STEP 4: Start landing movement
        currentWaypointIndex = waypoints.Length - 2;
        currentSpeed = landingSpeed;
        isMoving = true;
        Debug.Log($"Landing approach started from waypoint {currentWaypointIndex}.");
    }
    
    System.Collections.IEnumerator RotateAndStartLanding()
    {
        // This is no longer used - kept for compatibility
        yield break;
    }
    
    void Update()
    {
        // Don't run if we're landing instead
        if (!isMoving || waypoints == null || waypoints.Length == 0) return;
        
        // Safety check: ensure currentWaypointIndex is within bounds
        if (currentWaypointIndex < 0 || currentWaypointIndex >= waypoints.Length)
        {
            Debug.LogWarning($"Waypoint index {currentWaypointIndex} out of bounds (waypoints count: {waypoints.Length}). Stopping movement.");
            isMoving = false;
            return;
        }
        
        // Get current target waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        
        if (Time.frameCount % 60 == 0) // Log every 60 frames
        {
            Debug.Log($"Update: Moving to waypoint {currentWaypointIndex}, distance: {Vector3.Distance(transform.position, targetWaypoint.position)}, isReverseLanding: {isReverseLanding}");
        }
        
        // Rotate towards target
        Vector3 directionToTarget = targetWaypoint.position - transform.position;
        bool isFacingTarget = false;
        
        if (directionToTarget.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            // Check if mostly facing the target
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            isFacingTarget = angle < 10f; // Within 10 degrees
        }
        else
        {
            isFacingTarget = true;
        }
        
        // Only move forward when mostly facing target (for sharper turns)
        // Exception: always move on last waypoint (takeoff) for smooth ascent
        bool isLastWaypoint = isReverseLanding ? (currentWaypointIndex == 0) : (currentWaypointIndex >= waypoints.Length - 1);
        if (isFacingTarget || isLastWaypoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, currentSpeed * Time.deltaTime);
        }
        
        // Check if reached waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.5f)
        {
            if (isReverseLanding)
            {
                // Landing: go backwards through waypoints
                currentWaypointIndex--;
                
                if (currentWaypointIndex < 0)
                {
                    isMoving = false;
                    Debug.Log("Landing complete! Re-enabling player controls.");

                    if (panelObj != null) panelObj.SetActive(true);

                    FlightManager.isReturningForLanding = false;

                    var audio = GetComponent<PlaneAudioController>();
                    if (audio != null)
                        audio.PlayLandingSequence();

                    // Re-enable player controls after landing
                    GameObject playerRig = null;
                    try { playerRig = GameObject.FindGameObjectWithTag("Player"); }
                    catch { }
                    
                    if (playerRig == null)
                    {
                        var cam = Camera.main;
                        if (cam != null) playerRig = cam.transform.root.gameObject;
                    }
                    
                    if (playerRig != null)
                    {
                        var debugLoco = playerRig.GetComponent<XRDebugLocomotion>();
                        if (debugLoco != null)
                        {
                            debugLoco.enabled = true;
                            Debug.Log("Re-enabled WASD movement");
                        }
                    }
                }
            }
            else
            {
                // Takeoff: go forward through waypoints
                currentWaypointIndex++;
                
                // Check if this is the last waypoint (takeoff)
                if (currentWaypointIndex >= waypoints.Length - 1)
                {
                    // Speed up for takeoff
                    currentSpeed = takeoffSpeed;
                    StartCoroutine(TakeoffRotation());
                }
                
                // If all waypoints reached, trigger fade to next scene
                if (currentWaypointIndex >= waypoints.Length && !hasTriggeredTransition)
                {
                    hasTriggeredTransition = true;
                    StartCoroutine(FadeAndLoadNextScene());
                }
            }
        }
        else if (!isReverseLanding)
        {
            // Gradually accelerate (only during takeoff)
            currentSpeed += accelerationRate * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, takeoffSpeed);
        }
    }
    
    System.Collections.IEnumerator FadeAndLoadNextScene()
    {
        Debug.Log($"Reached altitude {altitudeForTransition}. Starting fade to black...");
        
        // Create a black overlay UI if it doesn't exist
        GameObject fadeCanvas = new GameObject("FadeCanvas");
        Canvas canvas = fadeCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // On top of everything
        
        GameObject fadePanel = new GameObject("FadePanel");
        fadePanel.transform.SetParent(fadeCanvas.transform);
        UnityEngine.UI.Image img = fadePanel.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(0, 0, 0, 0); // Start transparent
        
        // Make panel fill screen
        RectTransform rt = fadePanel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        
        // Fade to black
        float elapsed = 0f;
        while (elapsed < fadeToBlackDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = elapsed / fadeToBlackDuration;
            img.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        img.color = Color.black; // Ensure fully black
        
        // Mark that we're going to the city (so we know to land when we return)
        PlayerPrefs.SetInt("ReturningFromCity", 1);
        PlayerPrefs.Save();
        
        Debug.Log($"Loading scene: {nextSceneName}");
        
        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }
    
    System.Collections.IEnumerator TakeoffRotation()
    {
        // Gradually pitch up for takeoff
        float elapsed = 0f;
        float duration = 3f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(-takeoffAngle, 0, 0);
        
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    // Call this to start the plane movement
    public void StartMovement()
    {
        isMoving = true;
        currentWaypointIndex = 0;
        currentSpeed = moveSpeed;

        var audio = GetComponent<PlaneAudioController>();
        if (audio != null)
            audio.PlayTakeOff();

        Debug.Log("Plane movement started!");
    }
    
    // Optional: stop movement
    public void StopMovement()
    {
        isMoving = false;
    }
}
