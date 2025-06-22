using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Collectible Score Settings")]
    [SerializeField] private int coinScore = 10;
    [SerializeField] private int silverCoinScore = 25;
    [SerializeField] private int goldCoinScore = 50;
    [SerializeField] private int diamondScore = 100;

    [Header("Speed Boost Score Settings")]
    [SerializeField] private int normalBoostScore = 100;     // Shift boost
    [SerializeField] private int superBoostScore = 400;      // X super boost
    [SerializeField] private int megaBoostScore = 800;       // Phím mới (ví dụ: C)

    [Header("Speed Boost Tracking")]
    private bool wasUsingNormalBoost = false;
    private bool wasUsingSuperBoost = false;
    private bool wasUsingMegaBoost = false;

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

    // Trick detection
    private bool isAirborne = false;
    private float airTime = 0f;
    private float totalRotation = 0f;
    private float lastRotation = 0f;
    private float maxJumpHeight = 0f;
    private Vector3 lastPosition;
    private float jumpHeight = 0f;

    // Components
    private PlayerController playerController;
    private Rigidbody2D playerRb;

    // Events
    public System.Action<int> OnScoreChanged;
    public System.Action<int, float> OnComboChanged;
    public System.Action<string> OnTrickPerformed;
    public System.Action<string> OnSpeedBoostUsed; // Thông báo speed boost

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

        CheckSpeedBoostUsage(); // Thay thế CheckSpeedMilestones
        UpdateComboSystem();
        DetectTricks();
    }

    // HỆ THỐNG MỚI: Cộng điểm khi người chơi chủ động sử dụng speed boost
    void CheckSpeedBoostUsage()
    {
        bool isUsingNormalBoost = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isUsingSuperBoost = playerController.HasSuperBoost();
        bool isUsingMegaBoost = 
          playerController.HasMegaBoost();
        // Kiểm tra nếu bắt đầu sử dụng Normal Boost (Shift)
        if (isUsingNormalBoost && !wasUsingNormalBoost)
        {
            AddScore(normalBoostScore, "Normal Boost");
            OnSpeedBoostUsed?.Invoke($"Speed Boost! +{normalBoostScore}");
            Debug.Log($"🚀 Normal Boost activated! +{normalBoostScore} points");
        }

        // Kiểm tra nếu bắt đầu sử dụng Super Boost (X)
        if (isUsingSuperBoost && !wasUsingSuperBoost)
        {
            AddScore(superBoostScore, "Super Boost");
            OnSpeedBoostUsed?.Invoke($"SUPER BOOST! +{superBoostScore}");
            Debug.Log($"⚡ Super Boost activated! +{superBoostScore} points");
        }

        if (isUsingMegaBoost && !wasUsingMegaBoost)
        {
            AddScore(megaBoostScore, "Mega Boost");
            OnSpeedBoostUsed?.Invoke($"MEGA BOOST! +{megaBoostScore}");
            Debug.Log($"💥 Mega Boost activated! +{megaBoostScore} points");
        }

        // Cập nhật trạng thái
        wasUsingNormalBoost = isUsingNormalBoost;
        wasUsingSuperBoost = isUsingSuperBoost;
        wasUsingMegaBoost = isUsingMegaBoost;
    }

    void UpdateComboSystem()
    {
        if (currentCombo > 0 && Time.time - lastComboTime > comboTimeWindow)
        {
            ResetCombo();
        }

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

            jumpHeight = playerController.transform.position.y - lastPosition.y;
            if (jumpHeight > maxJumpHeight)
            {
                maxJumpHeight = jumpHeight;
            }

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

    public void CollectCoin(CollectibleType type)
    {
        int points = GetCollectiblePoints(type);
        AddScore(points, "Collectible");
        IncrementCombo();

        Debug.Log($"🪙 Collected {type}: +{points * (int)comboMultiplier} points");
    }

    int GetCollectiblePoints(CollectibleType type)
    {
        switch (type)
        {
            case CollectibleType.Coin: return coinScore;
            case CollectibleType.SilverCoin: return silverCoinScore;
            case CollectibleType.GoldCoin: return goldCoinScore;
            case CollectibleType.Diamond: return diamondScore;
            default: return coinScore;
        }
    }

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

    // Legacy support
    public void CollectItem(int points)
    {
        AddScore(points, "Legacy Collectible");
        IncrementCombo();
    }
}