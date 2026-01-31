using UnityEngine;
using UnityEngine.InputSystem;

public class VRToolbarManager : MonoBehaviour
{
    public GameObject[] itemsInHand; // Trage aici obiectele tale (Pașaport, etc.)
    public InputActionProperty changeItemAction; // Mapare pentru buton/joystick
    private int currentIndex = 0;

    void Start()
    {
        if (itemsInHand != null && itemsInHand.Length > 0)
        {
            UpdateToolbar();
        }
    }

    void Update()
    {
        // Verificăm dacă jucătorul a apăsat butonul de schimbare
        // Adaugam verificare: "itemsInHand" nu trebuie sa fie null si trebuie sa aiba elemente
        if (changeItemAction.action != null && changeItemAction.action.WasPressedThisFrame())
        {
            if (itemsInHand == null || itemsInHand.Length == 0)
            {
                Debug.LogWarning("Nu ai adaugat niciun obiect in lista 'Items In Hand' din Inspector!");
                return; // Oprim functia aici ca sa nu dea eroare
            }

            currentIndex = (currentIndex + 1) % itemsInHand.Length;
            UpdateToolbar();
        }
    }

    void UpdateToolbar()
    {
        for (int i = 0; i < itemsInHand.Length; i++)
        {
            itemsInHand[i].SetActive(i == currentIndex);
        }
    }
}