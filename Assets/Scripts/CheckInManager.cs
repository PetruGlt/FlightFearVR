using UnityEngine;
using UnityEngine.UI; // Sau TMPro daca folosesti TextMeshPro
using System.Collections;

public class CheckInManager : MonoBehaviour
{
    [Header("Configurare")]
    public Transform luggageEndPoint; // Trage obiectul "Luggage_EndPoint" aici
    public Text agentDialogueText;    // Trage textul de deasupra capului NPC-ului
    public float beltSpeed = 0.5f;
    public float waitTime = 2.0f;

    [Header("Setari Nume")]
    // Cum se numesc obiectele tale in Hierarchy? (Scrie o parte din nume)
    public string passportKeyword = "Passport"; 
    public string luggageKeyword = "Travel";      

    // Starea sistemului
    private bool isPassportVerified = false;
    private bool isLuggageWeighed = false;
    private bool hasGreeted = false; // Tine minte daca ne-a salutat deja
    private GameObject currentLuggage = null;

    void Start()
    {
        UpdateDialogue("");
    }

    public void OnPlayerApproach(Collider other)
    {
        // Daca ne-a salutat deja, nu mai zice nimic
        if (hasGreeted) return;

        // Verificam daca cel care a intrat este Jucatorul
        // (Verificam Tag-ul "Player" sau daca numele contine "Player"/"Camera"/"Body")
        if (other.CompareTag("Player") || other.name.Contains("Player") || other.name.Contains("Head") || other.name.Contains("Body"))
        {
            UpdateDialogue("Bună ziua! Vă rog să prezentați pașaportul.");
            hasGreeted = true; // Gata, am salutat
        }
    }

    // --- FUNCTIILE APELATE DE TRIGGERE ---

    // Legam asta de Trigger_Passport
    public void OnPassportZoneEnter(Collider other)
    {
        if (isPassportVerified) return; // Daca e gata, ignoram

        // Verificam daca obiectul pus e pasaportul
        if (other.name.Contains(passportKeyword)) 
        {
            StartCoroutine(VerifyPassportRoutine());
        }
    }

    // Legam asta de Trigger_Scale
    public void OnScaleZoneEnter(Collider other)
    {
        // Nu primim bagajul daca nu am verificat pasaportul
        if (!isPassportVerified)
        {
            UpdateDialogue("Vă rog întâi pașaportul!");
            return;
        }

        if (isLuggageWeighed) return; // Deja luat

        // Verificam daca e bagajul
        if (other.name.Contains(luggageKeyword) && currentLuggage == null)
        {
            // Verificam sa nu fie mana jucatorului
            if(other.name.Contains("Hand")) return;

            currentLuggage = other.gameObject;
            StartCoroutine(WeighLuggageRoutine());
        }
    }

    // --- SECVENTELE DE TIMP (COROUTINES) ---

    IEnumerator VerifyPassportRoutine()
    {
        UpdateDialogue("Verific... O secundă.");
        // Asteptam 3 secunde (simulam tastatul la calculator)
        yield return new WaitForSeconds(waitTime); 

        isPassportVerified = true;
        UpdateDialogue("Totul în regulă. Vă rog puneți bagajul pe cântar.");
    }

    IEnumerator WeighLuggageRoutine()
    {
        UpdateDialogue("Cântăresc...");
        
        // Oprim fizica bagajului ca sa stea cuminte pe cantar
        Rigidbody rb = currentLuggage.GetComponent<Rigidbody>();
        if(rb != null) 
        {
            rb.isKinematic = true;
            // Fix Unity 6 velocity warning
            rb.angularVelocity = Vector3.zero;
        }

        yield return new WaitForSeconds(waitTime);

        // Calculam o greutate random pentru realism
        float weight = Random.Range(18.5f, 22.0f);
        UpdateDialogue($"Greutate: {weight:F1} kg. Perfect! Drum bun!");

        yield return new WaitForSeconds(waitTime);
        isLuggageWeighed = true; // Asta declanseaza miscarea in Update
    }

    void Update()
    {
        // Daca bagajul a fost cantarit, il miscam spre gaura din perete
        if (isLuggageWeighed && currentLuggage != null)
        {
            float step = beltSpeed * Time.deltaTime;
            currentLuggage.transform.position = Vector3.MoveTowards(currentLuggage.transform.position, luggageEndPoint.position, step);

            // Daca a ajuns in spatele perdelei (la EndPoint)
            if (Vector3.Distance(currentLuggage.transform.position, luggageEndPoint.position) < 0.1f)
            {
                Destroy(currentLuggage); 
                UpdateDialogue("Următorul pasager!");

                // --- AICI ESTE MODIFICAREA ---
                // Dupa ce bagajul a plecat, task-ul e gata
                if (TaskManager.Instance != null)
                {
                    TaskManager.Instance.CompleteCurrentTask();
                }
                // -----------------------------
            }
        }
    }

    void UpdateDialogue(string text)
    {
        if (agentDialogueText != null) agentDialogueText.text = text;
    }
}