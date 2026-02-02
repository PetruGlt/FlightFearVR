using UnityEngine;
using UnityEngine.AI;

public class NPCVariations : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // 1. Spargem simetria prioritatii
            // Unii vor fi mai agresivi (30), altii mai pasivi (70)
            agent.avoidancePriority = Random.Range(30, 70);

            // 2. Variem putin viteza (sa nu mearga toti in pas cadentat)
            // Daca viteza ta standard e 1.5, va varia intre 1.3 si 1.7
            float speedBase = agent.speed;
            agent.speed = Random.Range(speedBase - 0.2f, speedBase + 0.2f);
        }
    }
}