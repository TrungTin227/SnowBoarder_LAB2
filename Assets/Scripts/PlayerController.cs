using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private bool _canMove = true;

    [Header("Speed Settings")]
    [SerializeField] float normalSpeed = 10f;
    [SerializeField] float boostSpeed = 20f;

    [Header("Torque Settings")]
    [SerializeField] float torque = 30f;

    // Speed modifier system
    private float currentSpeedModifier = 1f;
    private Coroutine speedModifierCoroutine;

    Rigidbody2D rbdy;

    void Start()
    {
        rbdy = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!_canMove) return;

        HandleRotation();
        HandleMovement();
    }

    public void DisableInput()
    {
        _canMove = false;
        rbdy.linearVelocity = Vector2.zero; // Dừng chuyển động ngay lập tức
        rbdy.angularVelocity = 0f; // Dừng xoay
    }
    public void EnableInput() {
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
        float baseSpeed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)
            ? boostSpeed
            : normalSpeed;

        // Áp dụng speed modifier
        float finalSpeed = baseSpeed * currentSpeedModifier;

        rbdy.linearVelocity = new Vector2(finalSpeed, rbdy.linearVelocity.y);
    }

    // Method để các obstacle gọi
    public void ApplySpeedModifier(float modifier, float duration)
    {
        // Dừng coroutine cũ nếu có
        if (speedModifierCoroutine != null)
        {
            StopCoroutine(speedModifierCoroutine);
        }

        // Bắt đầu coroutine mới
        speedModifierCoroutine = StartCoroutine(SpeedModifierCoroutine(modifier, duration));
    }

    private IEnumerator SpeedModifierCoroutine(float modifier, float duration)
    {
        currentSpeedModifier = modifier;

        // Đợi trong thời gian effect
        yield return new WaitForSeconds(duration);

        // Khôi phục tốc độ bình thường
        currentSpeedModifier = 1f;

        Debug.Log("Speed restored to normal!");
    }

    // Getter để các script khác có thể check tốc độ hiện tại
    public float GetCurrentSpeedModifier()
    {
        return currentSpeedModifier;
    }
}