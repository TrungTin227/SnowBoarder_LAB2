using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Collectible Score Settings")]
    [SerializeField] private int coinScore = 10;           // Đồng xu thường
    [SerializeField] private int silverCoinScore = 25;     // Đồng xu bạc  
    [SerializeField] private int goldCoinScore = 50;       // Đồng xu vàng
    [SerializeField] private int diamondScore = 100;       // Kim cương

    [Header("Speed Milestone Settings - Phương án 2")]
    [SerializeField]
    private SpeedMilestone[] speedMilestones = new SpeedMilestone[]
    {
        new SpeedMilestone(10f, 50, "Speed Boost!"),
        new SpeedMilestone(15f, 100, "Fast Rider!"),
        new SpeedMilestone(20f, 200, "Speed Demon!"),
        new SpeedMilestone(25f, 500, "Lightning Fast!"),
        new SpeedMilestone(30f, 1000, "SUPERSONIC!")
    };

    [Header("Trick Settings")]
    [SerializeField] private int manualTrickScore = 50;
    [SerializeField] private int trickScore = 100;
    [SerializeField] private float comboTimeWindow = 3f;
    [SerializeField] private float maxComboMultiplier = 5f;
    [SerializeField] private float minAirTime = 0.5f;

    // Score tracking
    private int totalScore = 0;
    private int currentCombo = 0;
    private float comboMultiplier = 1f;
    private float lastComboTime = 0f;

    // Speed milestone tracking - FIX: Không tính điểm liên tục nữa
    private HashSet<int> achievedMilestones = new HashSet<int>();

    // Trick detection
    private bool isAirborne = false;
    private float airTime = 0f;
    private float totalRotation = 0f;
    private float lastRotation = 0f;
    private float maxJumpHeight = 0f;
    private Vector3 lastPosition;

    // Components - FIX: Tìm GameObject "Tim"
    private PlayerController playerController;
    private Rigidbody2D playerRb;

    // Events
    public System.Action<int> OnScoreChanged;
    public System.Action<int, float> OnComboChanged;
    public System.Action<string> OnTrickPerformed;
    public System.Action<string> OnSpeedMilestone; // Thông báo milestone

    public static ScoreManager Instance { get; private set; }

    [System.Serializable]
    public class SpeedMilestone
    {
        public float speedThreshold;
        public int bonusPoints;
        public string message;

        public SpeedMilestone(float speed, int points, string msg)
        {
            speedThreshold = speed;
            bonusPoints = points;
            message = msg;
        }
    }

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
        // FIX: Tìm GameObject "Tim" thay vì FindObjectOfType
        GameObject timPlayer = GameObject.Find("Tim");
        if (timPlayer != null)
        {
            playerController = timPlayer.GetComponent<PlayerController>();
            playerRb = timPlayer.GetComponent<Rigidbody2D>();

            if (playerController != null)
            {
                lastPosition = timPlayer.transform.position;
                lastRotation = timPlayer.transform.eulerAngles.z;
                Debug.Log("✅ ScoreManager: Tìm thấy Player 'Tim'!");
            }
            else
            {
                Debug.LogError("❌ GameObject 'Tim' không có PlayerController component!");
            }
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy GameObject 'Tim' (Player)!");
        }
    }

    void Update()
    {
        if (playerController == null || playerRb == null) return;

        CheckSpeedMilestones(); // FIX: Thay thế UpdateSpeedScore
        UpdateComboSystem();
        DetectTricks();
    }

    // FIX: Thay thế hệ thống tính điểm tốc độ cũ
    void CheckSpeedMilestones()
    {
        float currentSpeed = Mathf.Abs(playerRb.linearVelocity.x);

        for (int i = 0; i < speedMilestones.Length; i++)
        {
            // Chỉ trigger milestone 1 lần duy nhất
            if (currentSpeed >= speedMilestones[i].speedThreshold && !achievedMilestones.Contains(i))
            {
                achievedMilestones.Add(i);
                AddScore(speedMilestones[i].bonusPoints, "Speed Milestone");
                OnSpeedMilestone?.Invoke($"{speedMilestones[i].message} +{speedMilestones[i].bonusPoints}");
                Debug.Log($"🚀 Speed Milestone: {speedMilestones[i].message} - {currentSpeed:F1} m/s");
            }
        }
    }

    void UpdateComboSystem()
    {
        if (currentCombo > 0 && Time.time - lastComboTime > comboTimeWindow)
        {
            ResetCombo();
        }

        // FIX: Làm rõ công thức combo multiplier
        comboMultiplier = 1f + (currentCombo * 0.5f);
        comboMultiplier = Mathf.Min(comboMultiplier, maxComboMultiplier);
    }

    void DetectTricks()
    {
        if (playerController == null) return;

        bool wasAirborne = isAirborne;
        isAirborne = !IsGrounded();

        if (isAirborne)
        {
            airTime += Time.deltaTime;

            // Tính toán độ cao nhảy
            jumpHeight = playerController.transform.position.y - lastPosition.y;
            if (jumpHeight > maxJumpHeight)
            {
                maxJumpHeight = jumpHeight;
            }

            // Tính toán rotation
            float currentRotation = playerController.transform.eulerAngles.z;
            float rotationDelta = Mathf.DeltaAngle(lastRotation, currentRotation);
            totalRotation += Mathf.Abs(rotationDelta);
            lastRotation = currentRotation;
        }
        else if (wasAirborne && !isAirborne)
        {
            EvaluateTrick();
            ResetTrickDetection();
        }

        if (!isAirborne)
        {
            lastPosition = playerController.transform.position;
        }
    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerController.transform.position, Vector2.down, 1.2f);
        return hit.collider != null && hit.collider.CompareTag("Ground");
    }

    void EvaluateTrick()
    {
        if (airTime < minAirTime) return;

        string trickName = "";
        int trickPoints = 0;

        if (totalRotation >= 720f)
        {
            trickName = "Double Spin";
            trickPoints = trickScore * 3;
        }
        else if (totalRotation >= 360f)
        {
            trickName = "Full Spin";
            trickPoints = trickScore * 2;
        }
        else if (totalRotation >= 180f)
        {
            trickName = "Half Spin";
            trickPoints = trickScore;
        }
        else if (maxJumpHeight > 3f)
        {
            trickName = "Big Air";
            trickPoints = trickScore;
        }
        else if (airTime > 1f)
        {
            trickName = "Long Jump";
            trickPoints = (int)(trickScore * 0.8f);
        }

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
        jumpHeight = 0f;
        lastRotation = playerController.transform.eulerAngles.z;
    }

    public void AddScore(int points, string source = "")
    {
        int finalPoints = Mathf.RoundToInt(points * comboMultiplier);
        totalScore += finalPoints;
        OnScoreChanged?.Invoke(totalScore);

        if (!string.IsNullOrEmpty(source))
        {
            Debug.Log($"💰 Score +{finalPoints} from {source} (x{comboMultiplier:F1})");
        }
    }

    // FIX: Thay thế CollectItem bằng CollectCoin với đúng giá trị
    public void CollectCoin(CollectibleType type)
    {
        int points = GetCollectiblePoints(type);
        AddScore(points, "Collectible");
        IncrementCombo();

        Debug.Log($"🪙 Collected {type}: +{points * (int)comboMultiplier} points");
    }

    // FIX: Đúng giá trị đồng xu theo yêu cầu
    int GetCollectiblePoints(CollectibleType type)
    {
        switch (type)
        {
            case CollectibleType.Coin: return coinScore;        // 10
            case CollectibleType.SilverCoin: return silverCoinScore; // 25
            case CollectibleType.GoldCoin: return goldCoinScore;     // 50
            case CollectibleType.Diamond: return diamondScore;       // 100
            default: return coinScore;
        }
    }

    // FIX: Sửa PerformTrick để dùng đúng interface
    public void PerformManualTrick(string trickName)
    {
        if (isAirborne)
        {
            AddScore(manualTrickScore, trickName);
            IncrementCombo();
            OnTrickPerformed?.Invoke($"{trickName} +{manualTrickScore * (int)comboMultiplier}");
        }
    }

    void IncrementCombo()
    {
        currentCombo++;
        lastComboTime = Time.time;
        OnComboChanged?.Invoke(currentCombo, comboMultiplier);
        Debug.Log($"🔥 Combo x{currentCombo} (Multiplier: {comboMultiplier:F1}x)");
    }

    void ResetCombo()
    {
        if (currentCombo > 0)
        {
            Debug.Log($"💥 Combo broken! Final combo: {currentCombo}");
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
    public float GetCurrentSpeed() => playerRb != null ? Mathf.Abs(playerRb.linearVelocity.x) : 0f;

    // Legacy support - để không break existing code
    public void CollectItem(int points)
    {
        AddScore(points, "Legacy Collectible");
        IncrementCombo();
    }

    // FIX: Thêm variable bị thiếu
    private float jumpHeight = 0f;
}

// FIX: Sửa enum theo yêu cầu 10, 25, 50, 100
public enum CollectibleType
{
    Coin,        // 10 điểm
    SilverCoin,  // 25 điểm  
    GoldCoin,    // 50 điểm
    Diamond      // 100 điểm
}