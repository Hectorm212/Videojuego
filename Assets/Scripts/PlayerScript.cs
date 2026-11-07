using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript obj;

    [Header("Vidas")]
    [SerializeField] private int maxLives = 3;      // Vidas máximas configurables
    private int lives;                              // Vidas actuales (privada)
    public int Lives => lives;                      // Propiedad de solo lectura

    [Header("Movimiento")]
    private Rigidbody2D rb2D;
    public float moveSpeed = 5f;
    float horizontalMovement;
    public float jumpHeight = 5f;

    [Header("Detección de Suelo")]
    public Transform grounCheck;
    public Vector2 groundCheckSize = new Vector2(0.25f, 0.25f);
    public LayerMask groundLayer;
    public bool isGrounded;

    [Header("Invulnerabilidad")]
    public float invulnerabilityTime = 1.5f;        // Tiempo de invulnerabilidad tras recibir daño
    private bool isInvulnerable = false;
    private float invulnerabilityTimer = 0f;

    // Animators
    private Animator animator;

    void Awake()
    {
        // Patrón Singleton seguro
        if (obj == null)
        {
            obj = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        lives = maxLives; // Inicializamos con las vidas máximas
    }

    private void Update()
    {
        CheckFlip();
        UpdateGroundedState(); // Corregido: typo
        UpdateInvulnerability(); // Nueva función

        animator.SetFloat("Horizontal", Mathf.Abs(horizontalMovement));
        animator.SetBool("IsGrounded", isGrounded);

    }

    void FixedUpdate()
    {
        rb2D.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb2D.linearVelocity.y);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.performed || !isGrounded)
            return;

        animator.SetTrigger("Jump");
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpHeight);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(grounCheck.position, groundCheckSize);
    }

    void UpdateGroundedState()
    {
        isGrounded = Physics2D.OverlapBox(grounCheck.position, groundCheckSize, 0, groundLayer);
    }

    void CheckFlip()
    {
        if (horizontalMovement == 0)
            return;

        bool movingRight = horizontalMovement > 0;
        bool facingRight = transform.localScale.x > 0;

        if (movingRight != facingRight)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // ============================================
    //   SISTEMA DE VIDAS / DAÑO
    // ============================================

    /// <summary>
    /// Llamar SOLO cuando el jugador reciba daño (colisión con enemigo, trampa, etc.)
    /// </summary>
    public void GetDamage(int damageAmount = 1)
    {
        // Si está invulnerable o ya está muerto, ignoramos el daño
        if (isInvulnerable || lives <= 0)
            return;

        lives -= damageAmount;
        Debug.Log($"Jugador recibió daño. Vidas restantes: {lives}");

        // Activamos invulnerabilidad temporal
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityTime;

        // Feedback visual (parpadeo)
        StartCoroutine(BlinkEffect());

        // Verificamos muerte
        if (lives <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Actualiza el temporizador de invulnerabilidad
    /// </summary>
    private void UpdateInvulnerability()
    {
        if (!isInvulnerable) return;

        invulnerabilityTimer -= Time.deltaTime;

        if (invulnerabilityTimer <= 0f)
        {
            isInvulnerable = false;
        }
    }

    /// <summary>
    /// Efecto de parpadeo mientras es invulnerable
    /// </summary>
    private System.Collections.IEnumerator BlinkEffect()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        float blinkInterval = 0.1f;
        while (invulnerabilityTimer > 0f)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
        sr.enabled = true;
    }

    /// <summary>
    /// Lógica de muerte del jugador
    /// </summary>
    private void Die()
    {
        Debug.Log("💀 Jugador ha muerto");
        // esta se activa solo si tenemos la animacion de muerte animator.SetTrigger("Death"); // Si tienes animación de muerte
        rb2D.linearVelocity = Vector2.zero;
        this.gameObject.SetActive(false);

        // Aquí puedes agregar:
        // - GameManager.OnPlayerDeath();
        // - Reiniciar nivel
        // - Mostrar pantalla de Game Over
    }

    /// <summary>
    /// Curar al jugador (útil para pickups)
    /// </summary>
    public void Heal(int amount)
    {
        lives = Mathf.Min(lives + amount, maxLives);
        Debug.Log($"Jugador curado. Vidas: {lives}");
    }
}