using UnityEngine;

public class LuggagePlacement : MonoBehaviour
{
    [Tooltip("Tag-ul bagajului.")]
    public string luggageTag = "Luggage";

    [Tooltip("Referinta la scriptul security gate-ului.")]
    public SecurityGate securityGate;

    [Header("Sounds")]
    [Tooltip("Sound played when luggage is placed correctly.")]
    public AudioSource allowSound;

    // memoram bagajul care e in zona
    private Collider currentLuggage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(luggageTag))
        {
            Debug.Log("Bagaj plasat corect.");
            currentLuggage = other;

            if (allowSound != null)
                allowSound.Play();

            if (securityGate != null)
            {
                securityGate.SetLuggagePlaced(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // doar daca iese chiar bagajul nostru, nu alt collider
        if (other == currentLuggage)
        {
            Debug.Log("Bagaj scos de pe zona.");
            currentLuggage = null;

            if (securityGate != null)
            {
                securityGate.SetLuggagePlaced(false);
            }
        }
    }
}
