using System.Collections;
using UnityEngine;

public class PlaneAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource pilotTakeoff;
    public AudioSource pilotLanding;
    public AudioSource takeoff;
    public AudioSource beep;
    public AudioSource clapping;

    void Start()
    {
        StopAllAudio();
    }

    /* ---------------- SEQUENCES ---------------- */

    public void PlaySeatAnnouncement()
    {
        StopAllAudio();
        StartCoroutine(SeatSequence());
    }

    public void PlayTakeOff()
    {
        StopAllAudio();
        StartCoroutine(TakeoffSequence());
    }

    public void PlayLandingSequence()
    {
        StopAllAudio();
        StartCoroutine(LandingSequence());
    }

    IEnumerator SeatSequence()
    {
        // Beep
        beep.Play();
        yield return new WaitForSeconds(beep.clip.length);

        // Pilot takeoff announcement
        pilotTakeoff.Play();
        yield return new WaitForSeconds(pilotTakeoff.clip.length);
    }

    IEnumerator TakeoffSequence()
    {
        // Takeoff sound
        takeoff.loop = true;
        takeoff.Play();

        yield return null;
    }

    IEnumerator LandingSequence()
    {
        beep.Play();
        yield return new WaitForSeconds(beep.clip.length);

        pilotLanding.Play();

        clapping.Play();

        yield return null;
    }

    /* ---------------- HELPERS ---------------- */

    void StopAllAudio()
    {
        pilotTakeoff.Stop();
        pilotLanding.Stop();
        takeoff.Stop();
        beep.Stop();
        clapping.Stop();
    }
}
