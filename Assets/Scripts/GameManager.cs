using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Usa 'using UnityEngine.UI;' si usas Text normal en vez de TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Datos persistentes")]
    public int lives = 3;
    public int coins = 0;
    public float elapsedTime = 0f;
    public bool countTime = true;

    [Header("Referencias UI (opcionales)")]
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text timerText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (countTime)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si perdimos las referencias de UI al cambiar de escena, intentamos encontrarlas por nombre
        if (livesText == null)
        {
            GameObject obj = GameObject.Find("LivesText");
            if (obj != null) livesText = obj.GetComponent<TMP_Text>();
        }
        if (coinsText == null)
        {
            GameObject obj = GameObject.Find("CoinsText");
            if (obj != null) coinsText = obj.GetComponent<TMP_Text>();
        }
        if (timerText == null)
        {
            GameObject obj = GameObject.Find("TimerText");
            if (obj != null) timerText = obj.GetComponent<TMP_Text>();
        }

        UpdateAllUI();
    }

    public void AddCoin(int amount = 1)
    {
        coins += amount;
        UpdateAllUI();
    }

    public void SetLives(int newLives)
    {
        lives = newLives;
        UpdateAllUI();
    }

    private void UpdateAllUI()
    {
        if (livesText != null) livesText.text = ": " + lives;
        if (coinsText != null) coinsText.text = ": " + coins;
        UpdateTimerUI();
    }

    public void StopTimer()
    {
        countTime = false;
    }

    public void SetCountingTime(bool value)
    {
        countTime = value;
    }

    public void ResetGameData(int initialLives = 3)
    {
        lives = initialLives;
        coins = 0;
        elapsedTime = 0f;
        countTime = true;
        UpdateAllUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            timerText.text = string.Format(": {0:00}:{1:00}", minutes, seconds);
        }
    }
}