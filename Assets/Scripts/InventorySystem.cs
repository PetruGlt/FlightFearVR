using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
 // Namespace-ul standard

// Daca ai eroare la linia de mai sus, sterge-o si scrie: 
// using UnityEngine.XR.Interaction.Toolkit.Interactables; 

public class AdvancedInventorySystem : MonoBehaviour
{
    [Header("Configurare")]
    public GameObject[] physicalItems; 
    public Button[] uiButtons;         
    public Transform spawnPoint;       

    [Range(0.1f, 1.0f)]
    public float spawnOffset = 0.3f; 

    [Header("Feedback Vizual")]
    public Color activeColor = Color.green;   
    public Color inactiveColor = Color.white; 
    public Color droppedColor = Color.gray;   

    private bool[] isItemOut;
    private GameObject currentEquippedItem = null;

    // AICI salvam referintele corect, fara "dynamic"
    // Folosim XRBaseInteractable care este parintele universal pentru Grab
    private List<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable> cachedGrabScripts = new List<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();

    void Start()
    {
        isItemOut = new bool[physicalItems.Length];

        for (int i = 0; i < physicalItems.Length; i++)
        {
            GameObject item = physicalItems[i];
            int index = i;

            if (i < uiButtons.Length && uiButtons[i] != null) 
            {
                uiButtons[i].interactable = true;
            }

            // 1. Cautam componenta de Grab (indiferent de versiune, toate mostenesc din XRBaseInteractable)
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable grabScript = item.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
            
            // Daca nu o gaseste direct, cautam in copii (uneori scriptul e pe modelul 3D)
            if (grabScript == null) grabScript = item.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();

            // O salvam in lista noastra
            cachedGrabScripts.Add(grabScript);

            if (grabScript != null)
            {
                // Ascultam evenimentele
                grabScript.selectExited.AddListener((args) => OnItemDropped(index, item));
                grabScript.selectEntered.AddListener((args) => OnItemGrabbed(item));
            }
            else
            {
                Debug.LogError($"EROARE: Obiectul '{item.name}' nu are componenta XR Grab Interactable!");
            }

            item.SetActive(false);
            isItemOut[i] = false; 
            UpdateUI(i);
        }
    }

    public void SummonItem(int index)
    {
        if (index < 0 || index >= physicalItems.Length) return;

        GameObject candidate = physicalItems[index];
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable grabScript = cachedGrabScripts[index]; // Luam direct din lista salvata

        // CAZUL 1: VREI SA IL PUI LA LOC (STASH)
        if (isItemOut[index])
        {
            bool isHolding = false;
            
            if (grabScript != null)
            {
                // isSelected este proprietatea universala care ne zice daca e tinut in mana
                isHolding = grabScript.isSelected;
            }

            if (isHolding)
            {
                StoreItem(index, candidate);
            }
            else
            {
                Debug.Log($"Nu poti pune {candidate.name} in inventar daca nu il tii in mana!");
            }
            return; 
        }

        // CAZUL 2: VREI SA IL SCOTI (SUMMON)
        if (currentEquippedItem != null && currentEquippedItem != candidate)
        {
            Rigidbody rbOld = currentEquippedItem.GetComponent<Rigidbody>();
            if (rbOld != null && rbOld.isKinematic == true)
            {
                int oldIndex = GetIndexOfItem(currentEquippedItem);
                if (oldIndex != -1) StoreItem(oldIndex, currentEquippedItem);
            }
        }

        candidate.SetActive(true);
        currentEquippedItem = candidate;
        isItemOut[index] = true; 

        Rigidbody rb = candidate.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            // Fix simplu pentru viteza
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Vector3 spawnPos = spawnPoint.position + (spawnPoint.forward * spawnOffset);
        candidate.transform.position = spawnPos;
        candidate.transform.rotation = spawnPoint.rotation;
        candidate.transform.SetParent(spawnPoint);

        UpdateUI(index);
    }

    void StoreItem(int index, GameObject item)
    {
        item.SetActive(false); 
        item.transform.SetParent(null); 
        isItemOut[index] = false; 
        
        if (currentEquippedItem == item) currentEquippedItem = null;

        UpdateUI(index);
    }

    void OnItemGrabbed(GameObject item)
    {
        if (item.transform.parent == spawnPoint)
        {
            item.transform.SetParent(null);
        }
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
    }

    void OnItemDropped(int index, GameObject item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        item.transform.SetParent(null);
        UpdateUI(index);
    }

    void UpdateUI(int unusedIndex) 
    {
        for (int i = 0; i < physicalItems.Length; i++)
        {
            if (i < uiButtons.Length && uiButtons[i] != null)
            {
                if (isItemOut[i])
                {
                    if (physicalItems[i] == currentEquippedItem)
                        uiButtons[i].image.color = activeColor;
                    else
                        uiButtons[i].image.color = droppedColor;
                }
                else
                {
                    uiButtons[i].image.color = inactiveColor;
                }
            }
        }
    }

    int GetIndexOfItem(GameObject item)
    {
        for(int i=0; i<physicalItems.Length; i++)
        {
            if (physicalItems[i] == item) return i;
        }
        return -1;
    }
}