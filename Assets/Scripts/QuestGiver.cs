using UnityEngine;
using UnityEngine.UI; // Sau TMPro

public class QuestGiver : MonoBehaviour
{
    [Header("Componente UI")]
    public GameObject exclamationMark; 
    public Text dialogueText;          
    public GameObject phoneObjectOnTable; 

    [Header("Setari Quest")]
    public string targetItemName = "Phone"; // <--- AI GRIJA AICI SA FIE CA IN PREFAB
    [TextArea] public string questDialogue = "Ajută-mă! Mi-am pierdut telefonul. Cred că l-am uitat la Cafeneaua de la etaj, la Security sau la Bagaje.";
    [TextArea] public string thankYouDialogue = "Mersiii mult! M-ai salvat!";

    private bool hasGivenQuest = false;
    private bool isQuestCompleted = false;

    void Start()
    {
        if(exclamationMark != null) exclamationMark.SetActive(true);
        if(phoneObjectOnTable != null) phoneObjectOnTable.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
       
        if (!hasGivenQuest && (other.CompareTag("Player") || other.name.Contains("Head")))
        {
            GiveQuest();
            return;
        }

        
        if (hasGivenQuest && !isQuestCompleted)
        {
            
            if (other.name.Contains(targetItemName))
            {
                if(!other.CompareTag("Player"))
                {
                    CompleteQuest(other.gameObject);
                }
            }
        }
    }

    void GiveQuest()
    {
        hasGivenQuest = true;
        if(exclamationMark != null) exclamationMark.SetActive(false);
        if(dialogueText != null) dialogueText.text = questDialogue;
        if(phoneObjectOnTable != null) phoneObjectOnTable.SetActive(true);
    }

    void CompleteQuest(GameObject itemAdus)
    {
        isQuestCompleted = true;
        
        if(dialogueText != null) dialogueText.text = thankYouDialogue;

        
        AdvancedInventorySystem inventory = FindObjectOfType<AdvancedInventorySystem>();
        if(inventory != null)
        {
            // Putem forta o reimprospatare a UI-ului sau resetarea slotului, 
            // dar cel mai simplu e sa lasam inventarul sa vada ca obiectul e null data viitoare.
            // Pentru un prototip e ok doar sa distrugem obiectul.
        }

        // Distrugem telefonul (NPC-ul l-a luat)
        Destroy(itemAdus);

        Debug.Log("Quest Completat!");
    }
}