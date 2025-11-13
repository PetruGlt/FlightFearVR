using UnityEngine;
using UnityEngine.Events;

public class SecurityGate : MonoBehaviour
{
    [Tooltip("Tag-ul obiectului care reprezinta playerul (XR Origin).")]
    public string playerTag = "Player";

    [Tooltip("Colliderul care blocheaza fizic trecerea prin poarta (perete invizibil).")]
    public Collider blockingCollider;

    [Tooltip("Sunetul care se reda cand playerul trece prin poarta.")]
    public AudioSource beepAudio;

    [Tooltip("Eveniment optional, apelat cand playerul trece prin poarta dupa ce a pus bagajul.")]
    public UnityEvent onPlayerPassedWithLuggage;

    private bool luggagePlaced = false;

    private void Start()
    {
        // la inceput poarta e blocata
        if (blockingCollider != null)
            blockingCollider.enabled = true;
    }

    // apelat din LuggagePlacement
    public void SetLuggagePlaced(bool value)
    {
        luggagePlaced = value;

        if (blockingCollider != null)
        {
            // daca bagajul e pus, dezactivam colliderul care blocheaza trecerea
            blockingCollider.enabled = !luggagePlaced;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!luggagePlaced) return;

        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player a trecut prin poarta cu bagajul pus.");

            if (beepAudio != null)
                beepAudio.Play();


            onPlayerPassedWithLuggage?.Invoke();
        }
    }
}
