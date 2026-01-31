using UnityEngine;

public class SecurityBelt : MonoBehaviour
{
    [Header("Setup")]
    public Transform endPoint;   
    public float speed = 0.8f;   
    
    [Header("Security Logic")]
    public GameObject barrierToOpen; // TRAGE AICI OBIECTUL "Security_Barrier"

    [Header("Debug")]
    [SerializeField] private GameObject currentItem = null; 

    void OnTriggerEnter(Collider other)
    {
        // Verificam daca e obiect valid si nu e mana jucatorului
        if (other.GetComponent<Rigidbody>() != null && currentItem == null)
        {
            if (other.CompareTag("Player") || other.name.Contains("Hand")) return;

            currentItem = other.gameObject;

           if (barrierToOpen != null)
            {
                barrierToOpen.SetActive(false); 
                
                // ANUNTAM MANAGERUL CA AM TERMINAT PRIMUL TASK!
                if (TaskManager.Instance != null)
                {
                    TaskManager.Instance.CompleteCurrentTask();
                }
            }

            // Logica de transport (Fizica oprita)
            Rigidbody rb = currentItem.GetComponent<Rigidbody>();
            rb.isKinematic = true;      
            rb.useGravity = false;      
            
            // Fix Unity 6 vs Old
            #if UNITY_6000_0_OR_NEWER
                 rb.linearVelocity = Vector3.zero;
            #else
                 rb.velocity = Vector3.zero;
            #endif
            
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        if (currentItem != null)
        {
            float step = speed * Time.deltaTime;
            currentItem.transform.position = Vector3.MoveTowards(currentItem.transform.position, endPoint.position, step);

            if (Vector3.Distance(currentItem.transform.position, endPoint.position) < 0.05f)
            {
                ReleaseItem();
            }
        }
    }

    void ReleaseItem()
    {
        if (currentItem == null) return;

        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        currentItem = null;
    }
}