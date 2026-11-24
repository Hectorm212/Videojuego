using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript obj;
    [Header("UI Game Over")]
    [SerializeField] private GameObject gameOverPanel; // Arrastrarás aquí tu panel o fondo negro de Game Over

    [Header("Reinicio")]
    [SerializeField] private string firstLevelName = "Nivel1"; // Nombre exacto de tu primera escena
    private bool isDead = false;

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

    [Header("Efectos de Sonido")]                         ///Sonido
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip deathSound;

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

        // Toma las vidas guardadas en GameManager si existe; si no, usa maxLives
        if (GameManager.Instance != null)
        {
            lives = GameManager.Instance.lives;
            GameManager.Instance.SetLives(lives);
        }
        else
        {
            lives = maxLives;
        }

        // Buscar el AudioSource de forma segura
        audioSource = GetComponent<AudioSource>();

        // Si por alguna razón sigue sin tenerlo, se lo agregamos automáticamente por código
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        CheckFlip();
        UpdateGroundedState();
        UpdateInvulnerability();
        CheckRestartInput();

        animator.SetFloat("Horizontal", Mathf.Abs(horizontalMovement));
        animator.SetBool("IsGrounded", isGrounded);
    }

    void FixedUpdate()
    {
        if (isDead) return;
        rb2D.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb2D.linearVelocity.y);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (isDead) return;
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isDead || !context.performed || !isGrounded)
            return;

        animator.SetTrigger("Jump");
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpHeight);

        // Reproducir sonido de salto
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (grounCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(grounCheck.position, groundCheckSize);
    }

    void UpdateGroundedState()
    {
        if (grounCheck == null) return;
        isGrounded = Physics2D.OverlapBox(grounCheck.position, groundCheckSize, 0, groundLayer);
    }

    void CheckFlip()
    {
        if (isDead || horizontalMovement == 0)
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

    private void CheckRestartInput()
    {
        if (!isDead) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetGameData(maxLives);
            }

            SceneManager.LoadScene(firstLevelName);
        }
    }

    // ============================================
    //   SISTEMA DE VIDAS / DAÑO / SONIDOS
    // ============================================

    /// <summary>
    /// Método público para que Coin.cs reproduzca el sonido al recogerla
    /// </summary>
    public void PlayCoinSound()
    {
        if (audioSource != null && coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }
    }

    public void GetDamage(int damageAmount = 1)
    {
        if (isDead || isInvulnerable || lives <= 0)
            return;

        lives -= damageAmount;
        if (lives < 0) lives = 0;

        Debug.Log($"Jugador recibió daño. Vidas restantes: {lives}");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetLives(lives);
        }

        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityTime;

        StartCoroutine(BlinkEffect());

        if (lives <= 0)
        {
            Die();
        }
    }

    private void UpdateInvulnerability()
    {
        if (!isInvulnerable) return;

        invulnerabilityTimer -= Time.deltaTime;

        if (invulnerabilityTimer <= 0f)
        {
            isInvulnerable = false;
        }
    }

    private System.Collections.IEnumerator BlinkEffect()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        float blinkInterval = 0.1f;
        while (invulnerabilityTimer > 0f && !isDead)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
        sr.enabled = true;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 Jugador ha muerto");

        // Detener el tiempo persistente al morir
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StopTimer();
        }

        // Reproducir sonido de muerte en la posición actual
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.simulated = false;
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    public void Heal(int amount)
    {
        lives = Mathf.Min(lives + amount, maxLives);
        Debug.Log($"Jugador curado. Vidas: {lives}");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetLives(lives);
        }
    }
}