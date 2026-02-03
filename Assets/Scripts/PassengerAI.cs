using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PassengerAI : MonoBehaviour
{
    [Header("Configurare Waypoints")]
    [Tooltip("Trage aici obiectul parinte care contine toate punctele (ex: Airport_Waypoints)")]
    public Transform waypointContainer; // <--- AICI TRAGI OBIECTUL PARINTE
    
    // Aceasta lista se va completa singura, nu trebuie sa umbli la ea
    public List<Transform> waypoints = new List<Transform>(); 

    [Header("Componente")]
    public Animator animator;         

    [Header("Comportament")]
    public float minWaitTime = 3f;
    public float maxWaitTime = 8f;

    private NavMeshAgent agent;
    private bool isWaiting = false;
    private Queue<int> visitQueue = new Queue<int>(); 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // --- 1. POPULARE AUTOMATA A LISTEI ---
        // Daca am asignat un Container in Inspector
        if (waypointContainer != null)
        {
            // Golim lista veche (ca sa nu avem duplicate)
            waypoints.Clear();

            // Trecem prin toti copiii obiectului Container
            foreach (Transform child in waypointContainer)
            {
                waypoints.Add(child);
            }
        }
        // Fallback: Daca ai uitat sa pui containerul, incercam sa-l gasim dupa nume (pentru siguranta)
        else if (waypoints.Count == 0)
        {
            GameObject group = GameObject.Find("Airport_Waypoints");
            if (group != null)
            {
                foreach (Transform t in group.transform) waypoints.Add(t);
            }
        }
        // -------------------------------------

        if (waypoints.Count > 0)
        {
            GenerateRandomRoute();
            GoToNextPoint();
        }
        else
        {
            Debug.LogError("Nu am gasit niciun waypoint! Verifica daca ai asignat 'Waypoint Container'.");
        }
    }

    void Update()
    {
        if (isWaiting) return;
        if (waypoints.Count == 0) return; // Siguranta

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                StartCoroutine(WaitAndMoveRoutine());
            }
        }
    }

    System.Collections.IEnumerator WaitAndMoveRoutine()
    {
        isWaiting = true; 
        if (animator != null) animator.SetBool("isWalking", false); 

        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        GoToNextPoint();
    }

    void GoToNextPoint()
    {
        if (visitQueue.Count == 0)
        {
            GenerateRandomRoute();
        }

        int nextIndex = visitQueue.Dequeue();
        
        // Verificare extra in caz ca lista s-a schimbat
        if (nextIndex < waypoints.Count)
        {
            agent.SetDestination(waypoints[nextIndex].position);
            isWaiting = false; 
            if (animator != null) animator.SetBool("isWalking", true);
        }
    }

    void GenerateRandomRoute()
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < waypoints.Count; i++)
        {
            indices.Add(i);
        }

        // Shuffle
        for (int i = 0; i < indices.Count; i++)
        {
            int temp = indices[i];
            int randomIndex = Random.Range(i, indices.Count);
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        foreach (int index in indices)
        {
            visitQueue.Enqueue(index);
        }
    }
}