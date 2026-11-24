using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private float fallbackElapsedTime = 0f;

    void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (timerText == null) return;

        float currentTime;

        if (GameManager.Instance != null)
        {
            currentTime = GameManager.Instance.elapsedTime;
        }
        else
        {
            // Respaldo por si pruebas una escena sin GameManager
            fallbackElapsedTime += Time.deltaTime;
            currentTime = fallbackElapsedTime;
        }

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = string.Format(": {0:00}:{1:00}", minutes, seconds);
    }
}