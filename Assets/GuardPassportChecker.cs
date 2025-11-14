using UnityEngine;

public class GuardPassportChecker : MonoBehaviour
{
    [Tooltip("Collider that blocks the passage (the door).")]
    public Collider blockingDoorCollider;

    [Tooltip("Tag used by the passport object.")]
    public string passportTag = "Passport";

    [Tooltip("The red 'LOCKED' text above the door.")]
    public GameObject lockedText;

    [Tooltip("Sound played when the passport is accepted.")]
    public AudioSource allowSound;

    private bool hasUnlockedDoor = false;

    private void OnTriggerEnter(Collider other)
    {
        // Only react if door is still locked and the entering object is the passport
        if (!hasUnlockedDoor && other.CompareTag(passportTag))
        {
            hasUnlockedDoor = true;

            Destroy(other.gameObject);

            if (blockingDoorCollider != null)
            {
                blockingDoorCollider.enabled = false;
            }

            if (lockedText != null)
            {
                lockedText.SetActive(false);
            }

            if (allowSound != null)
                allowSound.Play();

            Debug.Log("Guard accepted passport, door unlocked.");
        }
    }
}
