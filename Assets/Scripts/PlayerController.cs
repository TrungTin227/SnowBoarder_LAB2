using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private bool _canMove = true;

    [Header("Speed Settings")]
    [SerializeField] float normalSpeed = 10f;
    [SerializeField] float boostSpeed = 20f;
    [SerializeField] float superBoostSpeed = 30f;
    [SerializeField] float megaBoostSpeed = 40f; // Tốc độ mới

    [Header("Torque Settings")]
    [SerializeField] float torque = 30f;

    [Header("Power-up Settings")]
    [SerializeField] private float invincibilityDuration = 5f;
    [SerializeField] private float superBoostDuration = 3f;
    [SerializeField] private float megaBoostDuration = 2f; // Thời gian ngắn hơn vì mạnh hơn

    // Speed modifier system
    private float currentSpeedModifier = 1f;
    private Coroutine speedModifierCoroutine;

    // Power-up system
    private bool isInvincible = false;
    private bool hasSuperBoost = false;
    private bool hasMegaBoost = false; // Boost mới
    private Coroutine invincibilityCoroutine;
    private Coroutine superBoostCoroutine;
    private Coroutine megaBoostCoroutine; // Coroutine mới

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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.2f);
        isGrounded = hit.collider != null && hit.collider.CompareTag("Ground");

        if (wasGrounded && !isGrounded && ScoreManager.Instance != null)
        {
            // Bắt đầu nhảy
        }
    }

    void HandlePowerUps()
    {
        // Bất khả chiến bại (Space)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateInvincibility();
        }

        // Siêu tăng tốc (X)
        if (Input.GetKeyDown(KeyCode.X))
        {
            ActivateSuperBoost();
        }

        // MEGA BOOST mới (C) - Mạnh nhất nhưng thời gian ngắn
        if (Input.GetKeyDown(KeyCode.C))
        {
            ActivateMegaBoost();
        }

        // Trick controls
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool groundedNow = Physics2D.Raycast(transform.position, Vector2.down, 1.2f, LayerMask.GetMask("Default")) &&
                               Physics2D.Raycast(transform.position, Vector2.down, 1.2f).collider.CompareTag("Ground");

            if (!groundedNow)
            {
                PerformTrick("Manual Trick", 50);
            }
            else
            {
                Debug.Log("❌ Không thể trick khi đang đứng trên mặt đất!");
            }
        }
    }

    void PerformTrick(string trickName, int points)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.PerformManualTrick(trickName);
            Debug.Log($"✅ Đã trick: {trickName}");
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

    // PHƯƠNG THỨC MỚI
    public void ActivateMegaBoost()
    {
        if (megaBoostCoroutine != null)
        {
            StopCoroutine(megaBoostCoroutine);
        }

        megaBoostCoroutine = StartCoroutine(MegaBoostCoroutine());
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

    // COROUTINE MỚI
    private IEnumerator MegaBoostCoroutine()
    {
        hasMegaBoost = true;
        Debug.Log("🔥 MEGA BOOST activated!");

        yield return new WaitForSeconds(megaBoostDuration);

        hasMegaBoost = false;
        Debug.Log("Mega boost ended!");
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

        // Ưu tiên Mega Boost > Super Boost > Normal Boost
        if (hasMegaBoost)
        {
            baseSpeed = megaBoostSpeed;
        }
        else if (hasSuperBoost)
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
    public bool HasMegaBoost() => hasMegaBoost; // Getter mới
    public bool IsGrounded() => isGrounded;
}