using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int baseSpeedScore = 1; // Điểm cơ bản từ tốc độ
    [SerializeField] private int collectibleScore = 50; // Điểm từ vật phẩm
    [SerializeField] private int trickScore = 100; // Điểm từ thủ thuật
    [SerializeField] private float comboTimeWindow = 3f; // Thời gian giữ combo
    [SerializeField] private float maxComboMultiplier = 5f; // Hệ số nhân tối đa

    [Header("Trick Requirements")]
    [SerializeField] private float minAirTime = 0.5f; // Thời gian tối thiểu trong không khí
    [SerializeField] private float minRotationSpeed = 180f; // Tốc độ xoay tối thiểu

    // Score tracking
    private int totalScore = 0;
    private int currentCombo = 0;
    private float comboMultiplier = 1f;
    private float lastComboTime = 0f;

    // Speed scoring
    private float speedScoreAccumulator = 0f;
    private float lastSpeedScoreTime = 0f;
    private float speedScoreInterval = 0.1f; // Tính điểm tốc độ mỗi 0.1s

    // Trick detection
    private bool isAirborne = false;
    private float airTime = 0f;
    private float totalRotation = 0f;
    private float lastRotation = 0f;
    private Vector3 lastPosition;
    private float jumpHeight = 0f;
    private float maxJumpHeight = 0f;

    // Components
    private PlayerController playerController;
    private Rigidbody2D playerRb;

    // Events
    public System.Action<int> OnScoreChanged;
    public System.Action<int, float> OnComboChanged;
    public System.Action<string> OnTrickPerformed;

    public static ScoreManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerRb = playerController.GetComponent<Rigidbody2D>();
        lastPosition = transform.position;
        lastRotation = transform.eulerAngles.z;
    }

    void Update()
    {
        UpdateSpeedScore();
        UpdateComboSystem();
        DetectTricks();
    }

    void UpdateSpeedScore()
    {
        if (playerController == null) return;

        // Tính điểm dựa trên tốc độ hiện tại
        float currentSpeed = Mathf.Abs(playerRb.linearVelocity.x);
        float speedModifier = playerController.GetCurrentSpeedModifier();

        // Tính điểm tốc độ theo thời gian
        if (Time.time - lastSpeedScoreTime >= speedScoreInterval)
        {
            int speedPoints = Mathf.RoundToInt(currentSpeed * speedModifier * baseSpeedScore);
            if (speedPoints > 0)
            {
                AddScore(speedPoints, "Speed");
            }
            lastSpeedScoreTime = Time.time;
        }
    }

    void UpdateComboSystem()
    {
        // Giảm combo nếu quá lâu không thực hiện trick
        if (currentCombo > 0 && Time.time - lastComboTime > comboTimeWindow)
        {
            ResetCombo();
        }

        // Cập nhật combo multiplier
        comboMultiplier = 1f + (currentCombo * 0.5f);
        comboMultiplier = Mathf.Min(comboMultiplier, maxComboMultiplier);
    }

    void DetectTricks()
    {
        if (playerController == null) return;

        // Kiểm tra xem player có đang bay không
        bool wasAirborne = isAirborne;
        isAirborne = !IsGrounded();

        if (isAirborne)
        {
            airTime += Time.deltaTime;

            // Tính toán độ cao nhảy
            jumpHeight = transform.position.y - lastPosition.y;
            if (jumpHeight > maxJumpHeight)
            {
                maxJumpHeight = jumpHeight;
            }

            // Tính toán rotation
            float currentRotation = transform.eulerAngles.z;
            float rotationDelta = Mathf.DeltaAngle(lastRotation, currentRotation);
            totalRotation += Mathf.Abs(rotationDelta);
            lastRotation = currentRotation;
        }
        else if (wasAirborne && !isAirborne)
        {
            // Player vừa đáp đất - kiểm tra tricks
            EvaluateTrick();
            ResetTrickDetection();
        }

        if (!isAirborne)
        {
            lastPosition = transform.position;
        }
    }

    bool IsGrounded()
    {
        // Sử dụng raycast để kiểm tra ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.2f);
        return hit.collider != null && hit.collider.CompareTag("Ground");
    }

    void EvaluateTrick()
    {
        if (airTime < minAirTime) return;

        string trickName = "";
        int trickPoints = 0;

        // Đánh giá các loại trick dựa trên thời gian bay và rotation
        if (totalRotation >= 720f) // 2 vòng
        {
            trickName = "Double Spin";
            trickPoints = trickScore * 3;
        }
        else if (totalRotation >= 360f) // 1 vòng
        {
            trickName = "Full Spin";
            trickPoints = trickScore * 2;
        }
        else if (totalRotation >= 180f) // Nửa vòng
        {
            trickName = "Half Spin";
            trickPoints = trickScore;
        }
        else if (maxJumpHeight > 3f) // Nhảy cao
        {
            trickName = "Big Air";
            trickPoints = trickScore;
        }
        else if (airTime > 1f) // Bay lâu
        {
            trickName = "Long Jump";
            trickPoints = (int)(trickScore * 0.8f);
        }

        // Trick thành công
        if (!string.IsNullOrEmpty(trickName))
        {
            AddScore(trickPoints, trickName);
            IncrementCombo();
            OnTrickPerformed?.Invoke($"{trickName} +{trickPoints * (int)comboMultiplier}");
        }
    }

    void ResetTrickDetection()
    {
        airTime = 0f;
        totalRotation = 0f;
        maxJumpHeight = 0f;
        lastRotation = transform.eulerAngles.z;
    }

    public void AddScore(int points, string source = "")
    {
        int finalPoints = Mathf.RoundToInt(points * comboMultiplier);
        totalScore += finalPoints;
        OnScoreChanged?.Invoke(totalScore);

        if (!string.IsNullOrEmpty(source))
        {
            Debug.Log($"Score +{finalPoints} from {source} (x{comboMultiplier:F1})");
        }
    }

    public void CollectItem(int points)
    {
        AddScore(points, "Collectible");
        IncrementCombo();
    }

    void IncrementCombo()
    {
        currentCombo++;
        lastComboTime = Time.time;
        OnComboChanged?.Invoke(currentCombo, comboMultiplier);
        Debug.Log($"Combo x{currentCombo} (Multiplier: {comboMultiplier:F1}x)");
    }

    void ResetCombo()
    {
        if (currentCombo > 0)
        {
            Debug.Log($"Combo broken! Final combo: {currentCombo}");
        }
        currentCombo = 0;
        comboMultiplier = 1f;
        OnComboChanged?.Invoke(currentCombo, comboMultiplier);
    }

    public void ResetComboOnCrash()
    {
        ResetCombo();
    }

    // Getters
    public int GetTotalScore() => totalScore;
    public int GetCurrentCombo() => currentCombo;
    public float GetComboMultiplier() => comboMultiplier;
    public bool IsInAir() => isAirborne;
    public float GetAirTime() => airTime;
}