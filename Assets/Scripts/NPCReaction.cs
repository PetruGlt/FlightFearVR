using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCReaction : MonoBehaviour
{
    [Header("Setari")]
    public float stopDuration = 2.5f; // Cat timp sta oprit
    public AudioClip[] reactionSounds; // Sunete 
    
    private NavMeshAgent agent;
    private AudioSource audioSource;
    private bool isReacting = false;
    private Animator anim; // Optional, animatii

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();

        // Ne asiguram ca avem AudioSource
        if (audioSource == null) 
            audioSource = gameObject.AddComponent<AudioSource>();
            
        audioSource.spatialBlend = 1.0f; // Sunet 3D (se aude din directia NPC-ului)
    }

    // Aceasta functie se activeaza cand Jucatorul intra in Trigger
    void OnTriggerEnter(Collider other)
    {
        if (isReacting) return; // Daca deja reactioneaza, ignora

        // Verificam daca e Jucatorul (Verifica daca XR Origin sau Camera are tag-ul "Player")
        if (other.CompareTag("Player") || other.name.Contains("Player") || other.name.Contains("Head"))
        {
            StartCoroutine(ReactRoutine(other.transform));
        }
    }

    IEnumerator ReactRoutine(Transform playerTransform)
    {
        isReacting = true;

        // 1. Oprim NPC-ul
        if(agent != null) agent.isStopped = true;

        // 2. Animatie (Optional - animatie de Idle sau Surpriza)
        if(anim != null) anim.SetTrigger("Surprised"); // Asigura-te ca ai parametrul in Animator

        // 3. Redam sunet random
        if (reactionSounds.Length > 0)
        {
            AudioClip clip = reactionSounds[Random.Range(0, reactionSounds.Length)];
            audioSource.PlayOneShot(clip);
        }

        // 4. Se uita la jucator (Rotatie lina)
        float timer = 0;
        while(timer < 1.0f)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            direction.y = 0; // Sa nu se uite in sus/jos, doar stanga/dreapta
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            timer += Time.deltaTime;
            yield return null;
        }

        // 5. Asteptam restul timpului
        yield return new WaitForSeconds(stopDuration - 1.0f);

        // 6. Repornim NPC-ul
        if(agent != null) agent.isStopped = false;
        if(anim != null) anim.SetTrigger("Walk"); // Revine la mers

        // Pauza scurta ca sa nu se activeze iar imediat
        yield return new WaitForSeconds(2.0f);
        isReacting = false;
    }
}