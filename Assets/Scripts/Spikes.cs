using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private int damageToGive = 1; // Cuántas vidas quitan los picos

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si el objeto que tocó los picos tiene el script del Player
        PlayerScript player = collision.GetComponent<PlayerScript>();

        if (player != null)
        {
            // Llamamos a la función de daño que ya tienes en tu PlayerScript
            player.GetDamage(damageToGive);
        }
    }
} 