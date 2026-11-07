using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Detección del Jugador")]
    public Transform player;
    public float detectionRadius = 5f;

    [Header("Movimiento")]
    public float speed = 2f;

    [Header("Detección de Suelo / Borde")]
    public Transform edgeCheck;                                  // Punto delantero para detectar borde
    public Vector2 edgeCheckSize = new Vector2(0.2f, 0.2f);      // Tamaño del área de detección
    public LayerMask Ground;                                // Capa del suelo

    [Header("Daño al Jugador")]
    public int damageAmount = 1;

    private Rigidbody2D rb2D;
    private Vector2 movement;
    private bool facingRight = true;        // Para saber hacia dónde mira
    private bool hasGroundAhead = true;     // ¿Hay suelo delante?

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            movement = Vector2.zero;
            return;
        }

        // 1) Comprobamos si hay suelo delante
        UpdateGroundAhead();

        // 2) Calculamos distancia al jugador
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float desiredDir = Mathf.Sign(direction.x); // -1 = izquierda, 1 = derecha

            // 3) Si el jugador está en la dirección en la que NO hay suelo → no avanzamos
            bool playerAhead = (desiredDir > 0 && facingRight) || (desiredDir < 0 && !facingRight);

            if (!hasGroundAhead && playerAhead)
            {
                // Borde delante → se detiene (no se cae)
                movement = Vector2.zero;
            }
            else
            {
                movement = new Vector2(desiredDir, 0);
            }
        }
        else
        {
            movement = Vector2.zero;
        }

        CheckFlip();
    }

    void FixedUpdate()
    {
        rb2D.MovePosition(rb2D.position + movement * speed * Time.fixedDeltaTime);
    }

    void CheckFlip()
    {
        if (movement.x > 0)
        {
            facingRight = true;
            transform.localScale = new Vector3(-1, 1, 1); // Ajusta el signo según tu sprite
        }
        else if (movement.x < 0)
        {
            facingRight = false;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // ============================================
    //   DETECCIÓN DE BORDE (igual estilo que PlayerScript)
    // ============================================

    /// <summary>
    /// Verifica si hay suelo justo delante del enemigo.
    /// </summary>
    void UpdateGroundAhead()
    {
        if (edgeCheck == null) return;

        hasGroundAhead = Physics2D.OverlapBox(
            edgeCheck.position,
            edgeCheckSize,
            0f,
            Ground
        );
    }

    private void OnDrawGizmosSelected()
    {
        // Área de detección del jugador (radio)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Área de detección de borde
        if (edgeCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(edgeCheck.position, edgeCheckSize);
        }
    }

    // ============================================
    //   CONTACTO CON EL JUGADOR
    // ============================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DamagePlayer(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DamagePlayer(other.gameObject);
        }
    }

    private void DamagePlayer(GameObject playerObj)
    {
        PlayerScript playerScript = playerObj.GetComponent<PlayerScript>();
        if (playerScript != null)
        {
            playerScript.GetDamage(damageAmount);
            Debug.Log($"Enemigo tocó al jugador. Vidas restantes: {playerScript.Lives}");
        }
    }
}