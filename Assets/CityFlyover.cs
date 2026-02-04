using UnityEngine;
using UnityEngine.SceneManagement;

public class CityFlyover : MonoBehaviour
{
    [Header("Flight Settings")]
    public float flySpeed = 30f;
    public float flightDuration = 5f; // 5 seconds over city
    
    [Header("Scene Transition")]
    public string airportSceneName = "Airplane_VR"; // Name of airport scene
    public float fadeToBlackDuration = 2f;
    
    [Header("Optional")]
    public bool autoFly = true; // Automatically fly forward
    
    private float flightTime = 0f;
    private bool hasTriggeredTransition = false;
    
    void Start()
    {
        Debug.Log("City flyover started. Flying over city...");
    }
    
    void Update()
    {
        if (!autoFly) return;
        
        // Fly forward
        transform.position += transform.forward * flySpeed * Time.deltaTime;
        
        flightTime += Time.deltaTime;
        
        // Trigger return after duration
        if (flightTime >= flightDuration && !hasTriggeredTransition)
        {
            hasTriggeredTransition = true;
            Debug.Log("City flyover complete! Returning to airport...");
            StartCoroutine(FadeAndReturnToAirport());
        }
    }
    
    System.Collections.IEnumerator FadeAndReturnToAirport()
    {
        Debug.Log("Switching Global Brain to LANDING MODE...");

        // Set our static variable to true
        FlightManager.isReturningForLanding = true;

        // Brief pause to ensure the logic registers (optional)
        yield return new WaitForSeconds(0.1f);

        Debug.Log("Starting fade to black...");

        // Create a black overlay UI
        GameObject fadeCanvas = new GameObject("FadeCanvas");
        Canvas canvas = fadeCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        GameObject fadePanel = new GameObject("FadePanel");
        fadePanel.transform.SetParent(fadeCanvas.transform);
        UnityEngine.UI.Image img = fadePanel.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(0, 0, 0, 0);

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

        img.color = Color.black;

        Debug.Log($"Loading scene: {airportSceneName}");
        
        // Try loading the scene
        try
        {
            SceneManager.LoadScene(airportSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load scene '{airportSceneName}': {e.Message}");
            Debug.LogError("Make sure the Airport scene is added to Build Settings (File → Build Settings)");
            
            // Try loading by index as fallback
            Debug.Log("Attempting to load scene by index 0...");
            SceneManager.LoadScene(0);
        }
    }
}
