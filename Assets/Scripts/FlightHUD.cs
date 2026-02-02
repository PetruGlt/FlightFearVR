using UnityEngine;
using TMPro; // Important pentru TextMeshPro

public class FlightHUD : MonoBehaviour
{
    // Singleton = Putem accesa acest script de oriunde scriind FlightHUD.Instance
    public static FlightHUD Instance; 

    [Header("Componente UI")]
    public GameObject panelObject;    // Referinta la intreg panelul (ca sa il ascundem/afisam)
    public TextMeshProUGUI infoText;  // Referinta la textul care se schimba

    void Awake()
    {
        Instance = this;
        // La startul jocului, ascundem informatiile
        HideInfo(); 
    }

    // Aceasta functie e apelata de CheckInManager
    public void ShowFlightInfo(string gate, string seat)
    {
        if(panelObject != null) 
        {
            panelObject.SetActive(true); // Aprindem panelul
            
            // Scriem textul folosind culori HTML (Rich Text)
            
            if(infoText != null)
                infoText.text = $"POARTA: <color=yellow>{gate}</color>\nLOC: <color=yellow>{seat}</color>";
        }
    }

    // Aceasta functie e apelata de GateManager cand predai biletul
    public void HideInfo()
    {
        if(panelObject != null) 
        {
            panelObject.SetActive(false); // Stingem panelul
        }
    }
}