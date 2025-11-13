using UnityEngine;

public class AirportExitTrigger : MonoBehaviour
{
    [Tooltip("Tag-ul playerului (XR Origin sau child cu collider).")]
    public string playerTag = "Player";

    [Tooltip("Referinta la AmbientAudioFader-ul care controleaza sunetul de aeroport.")]
    public AmbientAudioFader audioFader;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player iese din aeroport -> fade out ambience.");
            if (audioFader != null)
            {
                audioFader.StartFadeOut();
            }
        }
    }
}
