using UnityEngine;

/// <summary>
/// Keeps the XR rig persistent across scenes and prevents duplicates.
/// Attach this to your XR Origin/Rig in the first scene.
/// </summary>
public class XRRigPersistence : MonoBehaviour
{
    private static XRRigPersistence instance;
    
    void Awake()
    {
        // If an instance already exists and it's not this one, destroy this duplicate
        if (instance != null && instance != this)
        {
            Debug.Log($"Destroying duplicate XR Rig: {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        
        // Set this as the instance and make it persistent
        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"XR Rig made persistent: {gameObject.name}");
    }
    
    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
