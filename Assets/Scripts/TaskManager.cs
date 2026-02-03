using UnityEngine;
using TMPro; 

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance; // Singleton ca sa il accesam usor de oriunde

    [Header("UI")]
    public TextMeshProUGUI taskText; // Trage textul aici

    [Header("Lista de Misiuni")]
    [TextArea] // Ne lasa sa scriem mult text in Inspector
    public string[] tasks; 

    private int currentTaskIndex = 0;

    void Awake()
    {
        // Facem acest script accesibil global
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    // Aceasta e functia pe care o vom striga din alte scripturi
    public void CompleteCurrentTask()
    {
        currentTaskIndex++;
        
        // Sunet de succes 
        // AudioSource.PlayClipAtPoint(successClip, transform.position);

        UpdateUI();
    }

    void UpdateUI()
    {
        // Verificam daca mai avem misiuni
        if (currentTaskIndex < tasks.Length)
        {
            taskText.text = "OBIECTIV:\n" + tasks[currentTaskIndex];
            taskText.color = Color.yellow;
        }
        else
        {
            taskText.text = "TOATE MISIUNILE FINALIZATE!";
            taskText.color = Color.green;
        }
    }
}