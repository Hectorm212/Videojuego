using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Reproducimos el sonido de la moneda desde el Player
            if (PlayerScript.obj != null)
            {
                PlayerScript.obj.PlayCoinSound();
            }

            // Sumamos el valor de la moneda al contador global
            if (CoinManager.instance != null)
            {
                CoinManager.instance.AddCoins(coinValue);
            }

            Destroy(gameObject);
        }
    }
}