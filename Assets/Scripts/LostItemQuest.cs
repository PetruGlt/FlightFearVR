using UnityEngine;
using UnityEngine.UI; // Sau TMPro

public class LostItemQuest : MonoBehaviour
{
    [Header("Configurare")]
    public string targetItemName = "LostPhone"; // Cum se numeste obiectul cautat (in Hierarchy)
    public GameObject rewardItem; 
    public Text dialogueText; // Textul de deasupra capului

    [Header("Dialog")]
    public string startMessage = "Mi-am pierdut telefonul! Cred ca l-am uitat la cafenea...";
    public string thankYouMessage = "Mersiii! M-ai salvat!";

    private bool questCompleted = false;

    void Start()
    {
        UpdateText(startMessage);
        if(rewardItem != null) rewardItem.SetActive(false); // Ascundem premiul
    }

    // Trigger-ul NPC-ului detecteaza obiectul adus
    void OnTriggerEnter(Collider other)
    {
        if (questCompleted) return;

        if (other.name.Contains(targetItemName))
        {
            CompleteQuest(other.gameObject);
        }
    }

    void CompleteQuest(GameObject item)
    {
        questCompleted = true;
        UpdateText(thankYouMessage);

        // 1. Luam obiectul din mana jucatorului si il facem sa dispara 
        Destroy(item); 

        // 2. Dam premiul (daca exista)
        if (rewardItem != null)
        {
            rewardItem.SetActive(true);
            // Spawn langa NPC
            rewardItem.transform.position = transform.position + transform.forward * 0.5f + Vector3.up * 1.0f;
        }

        // 3. (Optional) Sunet de succes
        AudioSource audio = GetComponent<AudioSource>();
        if(audio != null) audio.Play();
    }

    void UpdateText(string text)
    {
        if(dialogueText != null) dialogueText.text = text;
    }
}