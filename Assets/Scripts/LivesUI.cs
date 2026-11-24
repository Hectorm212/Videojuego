using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour
{
    private TextMeshProUGUI livesText;
    private PlayerScript playerScript;

    void Start()
    {
        livesText = GetComponent<TextMeshProUGUI>();

        // Buscamos automáticamente al jugador en la escena por su etiqueta o componente
        playerScript = Object.FindAnyObjectByType<PlayerScript>();
    }

    void Update()
    {
        // Si no lo habíamos encontrado antes, lo buscamos de nuevo
        if (playerScript == null)
        {
            playerScript = Object.FindAnyObjectByType<PlayerScript>();
        }

        // Si ya tenemos al jugador, actualizamos las vidas en pantalla
        if (playerScript != null)
        {
            livesText.text = ": " + playerScript.Lives;
        }
        else
        {
            livesText.text = ": --";
        }
    }
}