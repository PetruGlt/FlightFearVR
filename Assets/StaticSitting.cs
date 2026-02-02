using UnityEngine;

public class StaticSitting : MonoBehaviour
{
    public Transform seatAnchor; 

    void Start()
    {
        if (seatAnchor != null)
        {
            transform.position = seatAnchor.position;
            transform.rotation = seatAnchor.rotation;
            transform.SetParent(seatAnchor);
        }

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("isSitting", true);
            anim.Play("Sitting_Idle", 0, Random.value); 
        }
        
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }
}