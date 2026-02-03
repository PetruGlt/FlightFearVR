using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; // <--- NECESAR PENTRU TELEPORTARE IN ALTA SCENA
using System.Collections;

public class GateManager : MonoBehaviour
{
    [Header("Componente Vizuale")]
    public Text agentDialogueText;    
    public Animator npcAnimator;      

    [Header("Setari")]
    public float checkTime = 3.0f;    
    public string nextSceneName = "Scene_Airplane"; // TODO: Numele exact al scenei cu avionul

    private bool hasGreeted = false;  
    private bool isBusy = false;      

    void Start()
    {
        UpdateDialogue(""); 
    }

    // --- FUNCTIA 1: SALUTUL ---
    public void SayHello(Collider other)
    {
        if (hasGreeted || isBusy) return;

        if (other.CompareTag("Player") || other.name.Contains("Player") || other.name.Contains("Head"))
        {
            UpdateDialogue("Buna ziua! Va rog sa prezentati biletul.");
            hasGreeted = true;
            if(npcAnimator != null) npcAnimator.SetTrigger("Wave");
        }
    }

    // --- FUNCTIA 2: VERIFICAREA ---
    public void VerifyItem(Collider other)
    {
        if (isBusy) return;

        BoardingTicket ticket = other.GetComponent<BoardingTicket>();

        if (ticket != null)
        {
            StartCoroutine(CheckTicketRoutine(ticket, other.gameObject));
        }
        else if (!other.name.Contains("Hand")) 
        {
            UpdateDialogue("Am nevoie de biletul de imbarcare.");
        }
    }

    // --- LOGICA DE TELEPORTARE ---
    IEnumerator CheckTicketRoutine(BoardingTicket ticket, GameObject ticketObj)
    {
        isBusy = true;
        UpdateDialogue("Verific biletul... Un moment.");

        Rigidbody rb = ticketObj.GetComponent<Rigidbody>();
        if(rb != null) { rb.isKinematic = true; rb.angularVelocity = Vector3.zero; }

        yield return new WaitForSeconds(checkTime);

        // Feedback Final
        UpdateDialogue($"Totul e in regula! Locul {ticket.seatNumber}. Zbor placut!");
        
        if(npcAnimator != null) npcAnimator.SetTrigger("Nod");

        // Distrugem biletul si ascundem UI-ul
        Destroy(ticketObj);

        // if (FlightHUD.Instance != null) FlightHUD.Instance.HideInfo();
        
        if (TaskManager.Instance != null) TaskManager.Instance.CompleteCurrentTask();

        yield return new WaitForSeconds(4.0f); 

        // --- TELEPORTAREA (SCHIMBAREA SCENEI) ---
        Debug.Log("Teleportare catre avion...");
        SceneManager.LoadScene(nextSceneName);
    }

    void UpdateDialogue(string text)
    {
        if (agentDialogueText != null) agentDialogueText.text = text;
    }
}