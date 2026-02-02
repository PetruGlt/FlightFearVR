using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PassengerAI : MonoBehaviour
{
    [Header("Configurare")]
    public List<Transform> waypoints; 
    public Animator animator;         

    [Header("Comportament")]
    public float minWaitTime = 3f;
    public float maxWaitTime = 8f;

    private NavMeshAgent agent;
    private bool isWaiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Cautam punctele automat daca lista e goala
        if (waypoints.Count == 0)
        {
            GameObject group = GameObject.Find("Airport_Waypoints");
            if (group != null)
            {
                foreach (Transform t in group.transform) waypoints.Add(t);
            }
        }

        GoToRandomPoint();
    }

    void Update()
    {
        // Daca deja asteapta, nu mai facem nimic
        if (isWaiting) return;

        // Verificam daca a ajuns la destinatie
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
        isWaiting = true; // Marcam ca asteptam

        // --- SCHIMBARE ANIMATIE: IDLE ---
        if (animator != null)
        {
            animator.SetBool("isWalking", false); // Oprim mersul -> Trece in Idle
        }
        // --------------------------------

        // NPC-ul asteapta un timp aleatoriu
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        // Alege o noua destinatie
        GoToRandomPoint();
    }

    void GoToRandomPoint()
    {
        if (waypoints.Count == 0) return;

        int randomIndex = Random.Range(0, waypoints.Count);
        agent.SetDestination(waypoints[randomIndex].position);
        
        isWaiting = false; 

        // --- SCHIMBARE ANIMATIE: WALK ---
        if (animator != null)
        {
            animator.SetBool("isWalking", true); // Pornim mersul
        }
        // --------------------------------
    }
}