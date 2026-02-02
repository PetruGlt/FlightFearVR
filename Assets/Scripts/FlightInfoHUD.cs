using UnityEngine;
using TMPro;

public class FlightInfoHUD : MonoBehaviour
{
    public static FlightInfoHUD Instance; // Singleton ca sa il apelam usor
    public TextMeshProUGUI infoText;
    public GameObject panelObj; // Referinta la panoul intreg (ca sa il ascundem la start)

    void Awake()
    {
        Instance = this;
        // Ascundem panoul la inceputul jocului
        if(panelObj != null) panelObj.SetActive(false);
    }

    public void ShowFlightInfo(string gate, string seat)
    {
        if(panelObj != null) panelObj.SetActive(true);
        if(infoText != null)
        {
            infoText.text = $"POARTA: <color=yellow>{gate}</color>\nLOC: <color=yellow>{seat}</color>";
        }
    }

    public void HideInfo()
    {
        if(panelObj != null) panelObj.SetActive(false);
    }
}