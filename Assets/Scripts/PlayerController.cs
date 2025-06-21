using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private bool _canMove = true;

    [Header("Speed Settings")]
    [SerializeField] float normalSpeed = 10f;
    [SerializeField] float boostSpeed = 20f;
    [SerializeField] float superBoostSpeed = 30f;

    [Header("Torque Settings")]
    [SerializeField] float torque = 30f;

    [Header("Power-up Settings")]
    [SerializeField] private float invincibilityDuration = 5f;
    [SerializeField] private float superBoostDuration = 3f;

    // Speed modifier system
    private float currentSpeedModifier = 1f;
    private Coroutine speedModifierCoroutine;

    // Power-up system
    private bool isInvincible = false;
    private bool hasSuperBoost = false;
    private Coroutine invincibilityCoroutine;
    private Coroutine superBoostCoroutine;

    // Trick detection helper
    private bool isGrounded = true;
    private bool wasGrounded = true;

    Rigidbody2D rbdy;

    void Start()
    {
        rbdy = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!_canMove) return;

        UpdateGroundState();
        HandleRotation();
        HandleMovement();
        HandlePowerUps();
    }

    void UpdateGroundState()
    {
        wasGrounded = isGrounded;

        // Kiểm tra ground bằng raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.2f);
        isGrounded = hit.collider != null && hit.collider.CompareTag("Ground");

        // Thông báo cho score manager về trạng thái nhảy
        if (wasGrounded && !isGrounded && ScoreManager.Instance != null)
        {
            // Bắt đầu nhảy - có thể trigger trick detection
        }
    }

    void HandlePowerUps()
    {
        // Kích hoạt bất khả chiến bại (Space)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateInvincibility();
        }

        // Kích hoạt siêu tăng tốc (X)
        if (Input.GetKeyDown(KeyCode.X))
        {
            ActivateSuperBoost();
        }

        // Trick controls (thêm cho việc ghi điểm trick)
        if (!isGrounded)
        {
            // Q key - thêm điểm trick nếu đang trong không khí
            if (Input.GetKeyDown(KeyCode.Q))
            {
                PerformTrick("Manual Trick", 75);
            }
        }
    }

    void PerformTrick(string trickName, int points)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(points, trickName);
            Debug.Log($"Performed {trickName}!");
        }
    }

    public void ActivateInvincibility()
    {
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }

        invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
    }

    public void ActivateSuperBoost()
    {
        if (superBoostCoroutine != null)
        {
            StopCoroutine(superBoostCoroutine);
        }

        superBoostCoroutine = StartCoroutine(SuperBoostCoroutine());
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        Debug.Log("Invincibility activated!");

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
        Debug.Log("Invincibility ended!");
    }

    private IEnumerator SuperBoostCoroutine()
    {
        hasSuperBoost = true;
        Debug.Log("Super boost activated!");

        yield return new WaitForSeconds(superBoostDuration);

        hasSuperBoost = false;
        Debug.Log("Super boost ended!");
    }

    public void DisableInput()
    {
        _canMove = false;
        rbdy.linearVelocity = Vector2.zero;
        rbdy.angularVelocity = 0f;
    }

    public void EnableInput()
    {
        _canMove = true;
    }

    void HandleRotation()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rbdy.AddTorque(torque);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rbdy.AddTorque(-torque);
        }
    }

    void HandleMovement()
    {
        float baseSpeed = normalSpeed;

        if (hasSuperBoost)
        {
            baseSpeed = superBoostSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            baseSpeed = boostSpeed;
        }

        float finalSpeed = baseSpeed * currentSpeedModifier;
        rbdy.linearVelocity = new Vector2(finalSpeed, rbdy.linearVelocity.y);
    }

    public void ApplySpeedModifier(float modifier, float duration)
    {
        if (isInvincible)
        {
            Debug.Log("Player is invincible! Speed modifier ignored.");
            return;
        }

        if (speedModifierCoroutine != null)
        {
            StopCoroutine(speedModifierCoroutine);
        }

        speedModifierCoroutine = StartCoroutine(SpeedModifierCoroutine(modifier, duration));
    }

    private IEnumerator SpeedModifierCoroutine(float modifier, float duration)
    {
        currentSpeedModifier = modifier;
        yield return new WaitForSeconds(duration);
        currentSpeedModifier = 1f;
        Debug.Log("Speed restored to normal!");
    }

    // Getters
    public float GetCurrentSpeedModifier() => currentSpeedModifier;
    public bool IsInvincible() => isInvincible;
    public bool HasSuperBoost() => hasSuperBoost;
    public bool IsGrounded() => isGrounded;
}