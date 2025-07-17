using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Pengaturan Pergerakan")]
    public float enemySpeed = 2.0f;
    public float detectionRange = 10.0f;
    public float randomMoveInterval = 3.0f;
    public float returnSpeed = 1.5f;

    [Header("Pengaturan Kesehatan")]
    public int maxHealth = 50;
    private int currentHealth;

    private GameObject player;
    private Vector3 initialPosition;
    private Vector3 randomTargetPosition;
    private float timeSinceLastRandomMove = 0.0f;
    private bool isMovingRandomly = false;
    private bool isReturning = false;

    private Rigidbody2D rb;
    private Collider2D enemyCollider;
    private Animator animator;

    void Start()
    {
        initialPosition = transform.position;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            Debug.LogError("Player tidak ditemukan! Pastikan objek player memiliki tag 'Player'.");

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Tidak ada Rigidbody2D, menambahkan komponen...");
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        if (rb.bodyType == RigidbodyType2D.Static)
            rb.bodyType = RigidbodyType2D.Dynamic;

        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        enemyCollider = GetComponent<Collider2D>();
        if (enemyCollider == null)
            Debug.LogError("Tidak ada Collider2D!");

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning("Tidak ada komponen Animator!");

        currentHealth = maxHealth;
        ChooseNewRandomTarget();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            StopRandomMovement();
            isReturning = false;

            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
            rb.velocity = new Vector2(directionToPlayer.x * enemySpeed, rb.velocity.y);

            FlipSprite(directionToPlayer.x);
            SetAnimatorBool("run", true);
        }
        else
        {
            if (Vector2.Distance(transform.position, initialPosition) > 0.1f)
            {
                isReturning = true;
                StopRandomMovement();

                Vector2 directionToInitial = (initialPosition - transform.position).normalized;
                rb.velocity = new Vector2(directionToInitial.x * returnSpeed, rb.velocity.y);

                FlipSprite(directionToInitial.x);
                SetAnimatorBool("run", true);
            }
            else
            {
                isReturning = false;
                HandleRandomMovement();
            }
        }

        SetAnimatorBool("grounded", IsGrounded());
    }

    void HandleRandomMovement()
    {
        if (isReturning) return;

        timeSinceLastRandomMove += Time.deltaTime;

        if (timeSinceLastRandomMove >= randomMoveInterval || !isMovingRandomly)
        {
            ChooseNewRandomTarget();
            timeSinceLastRandomMove = 0.0f;
            isMovingRandomly = true;
        }

        Vector2 currentPos = transform.position;
        Vector2 targetPosHorizontal = new Vector2(randomTargetPosition.x, currentPos.y);

        Vector2 moveDirection = (targetPosHorizontal - currentPos).normalized;
        rb.velocity = new Vector2(moveDirection.x * enemySpeed, rb.velocity.y);

        FlipSprite(moveDirection.x);
        SetAnimatorBool("run", true);
    }

    void ChooseNewRandomTarget()
    {
        float randomXOffset = Random.Range(-5f, 5f);
        randomTargetPosition = new Vector3(transform.position.x + randomXOffset, transform.position.y, transform.position.z);
    }

    void StopRandomMovement()
    {
        isMovingRandomly = false;
        if (rb != null)
            rb.velocity = new Vector2(0f, rb.velocity.y);

        SetAnimatorBool("run", false);
    }

    void FlipSprite(float horizontalDirection)
    {
        if (horizontalDirection > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (horizontalDirection < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " menerima " + damage + " damage. HP: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log(gameObject.name + " mati!");

        if (enemyCollider != null)
            enemyCollider.enabled = false;

        SetAnimatorBool("run", false);
        animator?.SetTrigger("die");

        Destroy(gameObject, 0.5f);
    }

    bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 1.0f, LayerMask.GetMask("Ground"));
    }

    void SetAnimatorBool(string parameter, bool value)
    {
        if (animator != null && animator.HasParameterOfType(parameter, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(parameter, value);
        }
    }

    // ✅ Revisi Fungsi: Player injak musuh dari atas → musuh mati
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float playerY = collision.transform.position.y;
            float enemyY = transform.position.y;

            bool isPlayerAbove = playerY > enemyY + 0.3f;

            if (isPlayerAbove)
            {
                Die();

                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 10f); // lompat balik ke atas
                }
            }
            else
            {
                PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
                if (player != null)
                {
                    player.TakeDamage(10); // kalau tabrakan dari samping
                }
            }
        }
    }

    // Deteksi serangan player (misal pakai bullet atau melee)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            TakeDamage(20);
            Debug.Log("Kena serangan Player");
        }
    }
}

public static class AnimatorExtensions
{
    public static bool HasParameterOfType(this Animator animator, string name, AnimatorControllerParameterType type)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == type && param.name == name)
                return true;
        }
        return false;
    }
}
