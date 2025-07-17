using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerController playerController;

    private float mobileInputX = 0f;
    private Vector2 moveInput;

    private enum MovementState { idle, walk, jump, fall, run }

    [Header("Jump Settings")]
    [SerializeField] private LayerMask jumpableGround;
    private BoxCollider2D coll;

    [Header("Health System")]
    public int maxHealth = 100;
    private int currentHealth;
    public TextMeshProUGUI healthText;

    [Header("Coin System")]
    public int currentCoin = 0;
    public TextMeshProUGUI coinText;

    [Header("Knockback Settings")]
    [SerializeField] private float knockBackTime = 0.2f;
    [SerializeField] private float knockBackThrust = 10f;
    private bool isKnockedBack = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
        UpdateHealthUI();

        playerController = new PlayerController();
    }

    private void OnEnable()
    {
        playerController.Enable();
        playerController.Movement.Move.performed += OnMove;
        playerController.Movement.Move.canceled += ctx => moveInput = Vector2.zero;
        playerController.Movement.Jump.performed += ctx => Jump();
    }

    private void OnDisable()
    {
        playerController.Disable();
        playerController.Movement.Move.performed -= OnMove;
        playerController.Movement.Move.canceled -= ctx => moveInput = Vector2.zero;
        playerController.Movement.Jump.performed -= ctx => Jump();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (Application.isMobilePlatform)
        {
            moveInput = new Vector2(mobileInputX, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedBack) return;

        float currentMoveSpeed = moveSpeed;
        Vector2 targetVelocity = new Vector2((moveInput.x + mobileInputX) * currentMoveSpeed, rb.velocity.y);
        rb.velocity = targetVelocity;

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        MovementState state;
        float horizontal = moveInput.x != 0 ? moveInput.x : mobileInputX;

        if (horizontal > 0f)
        {
            state = MovementState.walk;
            sprite.flipX = false;
        }
        else if (horizontal < 0f)
        {
            state = MovementState.walk;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > 0.1f)
        {
            state = MovementState.jump;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.fall;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private void Jump()
    {
        if (isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    public void MoveRight(bool isPressed)
    {
        mobileInputX = isPressed ? 1f : (mobileInputX == 1f ? 0f : mobileInputX);
    }

    public void MoveLeft(bool isPressed)
    {
        mobileInputX = isPressed ? -1f : (mobileInputX == -1f ? 0f : mobileInputX);
    }

    public void MobileJump()
    {
        Jump();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth.ToString();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage, Vector2 direction)
    {
        if (isKnockedBack) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player Mati");
        }

        StartCoroutine(HandleKnockback(direction.normalized));
        UpdateHealthUI();
    }

    private void Die()
    {
        Debug.Log("Player died");
        // Tambahkan animasi dan reset/restart scene
    }

    public void ApplyKnockback(Vector2 direction)
    {
        if (!isKnockedBack)
        {
            StartCoroutine(KnockbackRoutine(direction));
        }
    }

    private IEnumerator KnockbackRoutine(Vector2 direction)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * knockBackThrust, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockBackTime);
        isKnockedBack = false;
    }

    private IEnumerator HandleKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;

        Vector2 force = direction * knockBackThrust * rb.mass;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockBackTime);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    public void AddCoin(int amount)
    {
        currentCoin += amount;
        if (coinText != null)
        {
            coinText.text = "Coin : " + currentCoin.ToString();
        }
    }
}
