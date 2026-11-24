using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de nivel

public class LevelKey : MonoBehaviour
{
    [Header("Configuración de Nivel")]
    [Tooltip("Escribe el nombre exacto de la escena del siguiente nivel")]
    [SerializeField] private string nextLevelName = "Nivel2";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si lo que tocó la llave es el jugador
        if (collision.CompareTag("Player"))
        {
            Debug.Log("¡Llave recogida! Cargando: " + nextLevelName);
            SceneManager.LoadScene(nextLevelName);
        }
    }
}