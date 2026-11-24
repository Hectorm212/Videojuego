using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance; // Permite acceder al manager desde otros scripts fácilmente

    [SerializeField] private TextMeshProUGUI coinsText;
    private int coinCount = 0;

    private void Awake()
    {
        // Configuración del patrón Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Si ya existe un GameManager, recuperamos las monedas guardadas entre niveles
        if (GameManager.Instance != null)
        {
            coinCount = GameManager.Instance.coins;
        }

        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;

        // Guardamos el total actualizado en el GameManager persistente
        if (GameManager.Instance != null)
        {
            GameManager.Instance.coins = coinCount;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinsText != null)
        {
            coinsText.text = ": " + coinCount;
        }
    }
}