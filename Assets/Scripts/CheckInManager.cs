using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class CheckInManager : MonoBehaviour
{
    [Header("Configurare")]
    public Transform luggageEndPoint; // obiectul "Luggage_EndPoint" aici
    public Text agentDialogueText;    // textul de deasupra capului NPC-ului
    public float beltSpeed = 0.5f;
    public float waitTime = 2.0f;

    [Header("Setari Nume")]
    // Cum se numesc obiectele in Hierarchy? (o parte din nume)
    public string passportKeyword = "Passport"; 
    public string luggageKeyword = "Travel";      

    [Header("Info Zbor (Pentru UI)")]
    // Acestea sunt doar informatii pe care le afisam pe ecran
    public string assignedGate = "04";   
    public string assignedSeat = "12A";
    
    // Starea sistemului
    private bool isPassportVerified = false;
    private bool isLuggageWeighed = false;
    private bool hasGreeted = false; // Tine minte daca a salutat deja
    private GameObject currentLuggage = null;

    void Start()
    {
        UpdateDialogue("");
    }

    public void OnPlayerApproach(Collider other)
    {
        // Daca a salutat deja, nu mai zice nimic
        if (hasGreeted) return;

        // Verificam daca cel care a intrat este Jucatorul
        // (Verificam Tag-ul "Player" sau daca numele contine "Player"/"Camera"/"Body")
        if (other.CompareTag("Player") || other.name.Contains("Player") || other.name.Contains("Head") || other.name.Contains("Body"))
        {
            UpdateDialogue("Buna ziua! Va rog sa prezentati pasaportul.");
            hasGreeted = true; 
        }
    }

    // --- FUNCTIILE APELATE DE TRIGGERE ---

    // Trigger_Passport
    public void OnPassportZoneEnter(Collider other)
    {
        if (isPassportVerified) return; 

        // Verificam daca obiectul pus e pasaportul
        if (other.name.Contains(passportKeyword)) 
        {
            StartCoroutine(VerifyPassportRoutine());
        }
    }

    // Trigger_Scale
    public void OnScaleZoneEnter(Collider other)
    {
        if (!isPassportVerified)
        {
            UpdateDialogue("Va rog intai pasaportul!");
            return;
        }

        if (isLuggageWeighed) return; 

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
        UpdateDialogue("Verific... O secunda.");
        
        yield return new WaitForSeconds(waitTime); 

        isPassportVerified = true;
        UpdateDialogue("Totul in regula. Va rog puneti bagajul pe cantar.");
    }

    IEnumerator WeighLuggageRoutine()
    {
        UpdateDialogue("Cantaresc...");
        
        // Oprim fizica bagajului 
        Rigidbody rb = currentLuggage.GetComponent<Rigidbody>();
        if(rb != null) 
        {
            rb.linearVelocity = Vector3.zero;        // 1. viteza
            rb.angularVelocity = Vector3.zero; // 2. rotatia
            rb.isKinematic = true;             // 3. Kinematic (blocat)
        }

        yield return new WaitForSeconds(waitTime);

        float weight = Random.Range(18.5f, 22.0f);
        UpdateDialogue($"Greutate: {weight:F1} kg. Perfect! Drum bun!");

        yield return new WaitForSeconds(waitTime);
        isLuggageWeighed = true; 
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
                UpdateDialogue("Urmatorul pasager!");


                if (FlightHUD.Instance != null)
                {
                    FlightHUD.Instance.ShowFlightInfo(assignedGate, assignedSeat);
                }

                // Dupa ce bagajul a plecat, task-ul e gata
                if (TaskManager.Instance != null)
                {
                    TaskManager.Instance.CompleteCurrentTask();
                }
            }
        }
    }

    void UpdateDialogue(string text)
    {
        if (agentDialogueText != null) agentDialogueText.text = text;
    }
}