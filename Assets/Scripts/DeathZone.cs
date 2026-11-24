using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Punto de Reaparición (Respawn)")]
    public Transform respawnPoint; // Arrastra aquí un objeto vacío que esté arriba en el nivel

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerScript player = collision.GetComponent<PlayerScript>();

        if (player != null)
        {
            // 1. Le quitamos una vida usando la función que ya tienes
            player.GetDamage(1);

            // 2. Si el jugador sigue vivo, lo teletransportamos arriba
            if (player.Lives > 0)
            {
                RespawnPlayer(collision.gameObject);
            }
        }
    }

    private void RespawnPlayer(GameObject playerObj)
    {
        if (respawnPoint != null)
        {
            // Apagamos momentáneamente el Rigidbody para que no arrastre velocidad al reaparecer
            Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // Movemos al jugador a la posición del punto de reaparición
            playerObj.transform.position = respawnPoint.position;
            Debug.Log("🔄 Jugador reapareció en el Checkpoint");
        }
        else
        {
            Debug.LogWarning("⚠️ Falta asignar el Respawn Point en el inspector de la DeathZone.");
        }
    }
}
