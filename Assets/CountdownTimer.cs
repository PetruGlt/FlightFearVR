using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining = 300f;
    public bool timerRunning = false;

    public TextMeshProUGUI timerText;

    private void Start()
    {
        timerText.gameObject.SetActive(false); // Hide timer at start
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (timerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;
                UpdateTimerDisplay();
                Debug.Log("Timer finished!");
            }
        }
    }

    public void StartTimer()
    {
        timerText.gameObject.SetActive(true); // Show timer when starting
        timerRunning = true;
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
