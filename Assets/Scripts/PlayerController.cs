using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private bool _canMove = true;

    [Header("Speed Settings")]
    [SerializeField] float normalSpeed = 10f;
    [SerializeField] float boostSpeed = 20f;
    [SerializeField] float superBoostSpeed = 30f;
    [SerializeField] float megaBoostSpeed = 40f;

    [Header("Torque Settings")]
    [SerializeField] float torque = 30f;

    [Header("Power-up Settings")]
    [SerializeField] private float invincibilityDuration = 5f;
    [SerializeField] private float superBoostDuration = 3f;
    [SerializeField] private float megaBoostDuration = 2f;

    [Header("Cooldown Settings")]
    [SerializeField] private float invincibilityCooldown = 10f;
    [SerializeField] private float superBoostCooldown = 5f;
    [SerializeField] private float megaBoostCooldown = 8f;

    // Speed modifier system
    private float currentSpeedModifier = 1f;
    private Coroutine speedModifierCoroutine;

    // Power-up system
    private bool isInvincible = false;
    private bool hasSuperBoost = false;
    private bool hasMegaBoost = false;
    private Coroutine invincibilityCoroutine;
    private Coroutine superBoostCoroutine;
    private Coroutine megaBoostCoroutine;

    // Cooldown timers - THÊM MỚI
    private float invincibilityCooldownRemaining = 0f;
    private float superBoostCooldownRemaining = 0f;
    private float megaBoostCooldownRemaining = 0f;

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
        UpdateCooldowns(); // THÊM MỚI
    }

    // THÊM METHOD MỚI
    void UpdateCooldowns()
    {
        if (invincibilityCooldownRemaining > 0)
            invincibilityCooldownRemaining -= Time.deltaTime;

        if (superBoostCooldownRemaining > 0)
            superBoostCooldownRemaining -= Time.deltaTime;

        if (megaBoostCooldownRemaining > 0)
            megaBoostCooldownRemaining -= Time.deltaTime;
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

        // MEGA BOOST mới (C)
        if (Input.GetKeyDown(KeyCode.C))
        {
            ActivateMegaBoost();
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
        // THÊM COOLDOWN CHECK
        if (invincibilityCooldownRemaining > 0)
        {
            Debug.Log($"Invincibility on cooldown: {invincibilityCooldownRemaining:F1}s remaining");
            return;
        }

        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }

        invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
    }

    public void ActivateSuperBoost()
    {
        // THÊM COOLDOWN CHECK
        if (superBoostCooldownRemaining > 0)
        {
            Debug.Log($"Super Boost on cooldown: {superBoostCooldownRemaining:F1}s remaining");
            return;
        }

        if (superBoostCoroutine != null)
        {
            StopCoroutine(superBoostCoroutine);
        }

        superBoostCoroutine = StartCoroutine(SuperBoostCoroutine());
    }

    public void ActivateMegaBoost()
    {
        // THÊM COOLDOWN CHECK
        if (megaBoostCooldownRemaining > 0)
        {
            Debug.Log($"Mega Boost on cooldown: {megaBoostCooldownRemaining:F1}s remaining");
            return;
        }

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
        invincibilityCooldownRemaining = invincibilityCooldown; // THÊM COOLDOWN
        Debug.Log("Invincibility ended!");
    }

    private IEnumerator SuperBoostCoroutine()
    {
        hasSuperBoost = true;
        Debug.Log("Super boost activated!");

        yield return new WaitForSeconds(superBoostDuration);

        hasSuperBoost = false;
        superBoostCooldownRemaining = superBoostCooldown; // THÊM COOLDOWN
        Debug.Log("Super boost ended!");
    }

    private IEnumerator MegaBoostCoroutine()
    {
        hasMegaBoost = true;
        Debug.Log("🔥 MEGA BOOST activated!");

        yield return new WaitForSeconds(megaBoostDuration);

        hasMegaBoost = false;
        megaBoostCooldownRemaining = megaBoostCooldown; // THÊM COOLDOWN
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
    public bool HasMegaBoost() => hasMegaBoost;
    public bool IsGrounded() => isGrounded;

    // THÊM MISSING COOLDOWN METHODS
    public float GetInvincibilityCooldownRemaining() => Mathf.Max(0f, invincibilityCooldownRemaining);
    public float GetSuperBoostCooldownRemaining() => Mathf.Max(0f, superBoostCooldownRemaining);
    public float GetMegaBoostCooldownRemaining() => Mathf.Max(0f, megaBoostCooldownRemaining);
}