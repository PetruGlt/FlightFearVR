using System.Collections;
using UnityEngine;

public class AmbientAudioFader : MonoBehaviour
{
    [Tooltip("AudioSource cu sunetul de ambient (aeroport).")]
    public AudioSource ambienceSource;

    [Tooltip("Durata fade-in in secunde.")]
    public float fadeInDuration = 3f;

    [Tooltip("Durata fade-out in secunde.")]
    public float fadeOutDuration = 3f;

    [Tooltip("Volumul final maxim al ambiantei (de ex. 0.6).")]
    public float targetVolume = 0.6f;

    private Coroutine currentFade;

    private void Start()
    {
        if (ambienceSource == null)
            ambienceSource = GetComponent<AudioSource>();

        if (ambienceSource == null)
        {
            Debug.LogWarning("AmbientAudioFader: Nu exista AudioSource!");
            return;
        }

        // Ne asiguram ca nu porneste singur
        ambienceSource.playOnAwake = false;

        // Pornim cu volum 0
        ambienceSource.volume = 0f;

        // Pornim fade-in-ul la inceputul scenei
        StartFadeIn();
    }

    public void StartFadeIn()
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeInCoroutine());
    }

    public void StartFadeOut()
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        if (!ambienceSource.isPlaying)
            ambienceSource.Play();

        float time = 0f;

        while (time < fadeInDuration)
        {
            float t = time / fadeInDuration;
            ambienceSource.volume = Mathf.Lerp(0f, targetVolume, t);
            time += Time.deltaTime;
            yield return null;
        }

        ambienceSource.volume = targetVolume;
        currentFade = null;
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = ambienceSource.volume;
        float time = 0f;

        while (time < fadeOutDuration)
        {
            float t = time / fadeOutDuration;
            ambienceSource.volume = Mathf.Lerp(startVolume, 0f, t);
            time += Time.deltaTime;
            yield return null;
        }

        ambienceSource.volume = 0f;
        ambienceSource.Stop();
        currentFade = null;
    }
}
