using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Forces XR input actions to stay enabled for head/camera tracking.
/// Attach this to your XR Origin.
/// </summary>
public class ForceEnableXRInput : MonoBehaviour
{
    [Header("Debug")]
    public bool logInputStatus = true;
    
    private PlayerInput playerInput;
    private InputActionAsset inputActionAsset;
    
    void Start()
    {
        // Find PlayerInput component
        playerInput = GetComponentInChildren<PlayerInput>();
        
        // Try to find the Input Action Asset
        if (playerInput != null)
        {
            inputActionAsset = playerInput.actions;
        }
        
        // Also check for direct InputActionAsset reference
        if (inputActionAsset == null)
        {
            var inputActionManager = GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.Inputs.InputActionManager>();
            if (inputActionManager != null)
            {
                var assetField = inputActionManager.GetType().GetField("m_ActionAssets", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (assetField != null)
                {
                    var assets = assetField.GetValue(inputActionManager) as System.Collections.IList;
                    if (assets != null && assets.Count > 0)
                    {
                        inputActionAsset = assets[0] as InputActionAsset;
                    }
                }
            }
        }
        
        EnsureInputEnabled();
    }
    
    void Update()
    {
        // Continuously ensure XR inputs stay enabled
        EnsureInputEnabled();
    }
    
    void EnsureInputEnabled()
    {
        // Enable via PlayerInput
        if (playerInput != null && !playerInput.inputIsActive)
        {
            playerInput.ActivateInput();
            if (logInputStatus) Debug.Log("Activated PlayerInput");
        }
        
        // Enable the action asset directly
        if (inputActionAsset != null && !inputActionAsset.enabled)
        {
            inputActionAsset.Enable();
            if (logInputStatus) 
            {
                Debug.Log("Force-enabled Input Action Asset");
                logInputStatus = false; // Only log once
            }
        }
        
        // Ensure XR HMD action maps stay enabled (critical for head tracking)
        if (inputActionAsset != null)
        {
            var hmdMap = inputActionAsset.FindActionMap("XRI Head");
            if (hmdMap != null && !hmdMap.enabled)
            {
                hmdMap.Enable();
                Debug.Log("Re-enabled XRI Head action map");
            }
            
            var leftHandMap = inputActionAsset.FindActionMap("XRI LeftHand");
            if (leftHandMap != null && !leftHandMap.enabled)
            {
                leftHandMap.Enable();
                Debug.Log("Re-enabled XRI LeftHand action map");
            }
            
            var rightHandMap = inputActionAsset.FindActionMap("XRI RightHand");
            if (rightHandMap != null && !rightHandMap.enabled)
            {
                rightHandMap.Enable();
                Debug.Log("Re-enabled XRI RightHand action map");
            }
        }
    }
    
    void OnEnable()
    {
        EnsureInputEnabled();
    }
}
