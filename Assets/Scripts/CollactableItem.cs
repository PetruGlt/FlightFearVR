using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Necesar pentru versiunile noi XR

public class CollectableItem : MonoBehaviour
{
    [Header("Setări Colectare")]
    public GameObject itemPrefabForInventory; // Prefab-ul care va fi creat în inventar
    public int targetSlotIndex = 3; // 3 înseamnă Slotul 4 (0, 1, 2, 3)

    private XRBaseInteractable grabInteractable;
    private bool isCollected = false;

    void Start()
    {
        grabInteractable = GetComponent<XRBaseInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (isCollected) return;

        if (args.interactorObject.transform.CompareTag("Player") || 
            args.interactorObject.transform.name.Contains("Hand") ||
            args.interactorObject.transform.parent.name.Contains("Controller") ||
            args.interactorObject.transform.root.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        isCollected = true;

   
        AdvancedInventorySystem inventory = FindObjectOfType<AdvancedInventorySystem>();

        if (inventory != null)
        {
           
            inventory.AddItemToSlot(targetSlotIndex, itemPrefabForInventory);
 
        }
        else
        {
            Debug.LogError("Nu am găsit scriptul 'AdvancedInventorySystem' în scenă!");
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
        }
    }
}