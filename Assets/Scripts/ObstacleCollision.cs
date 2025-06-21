using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] private ObstacleType obstacleType;
    [SerializeField] private float speedReduction = 0.5f; // Giảm tốc độ 50%
    [SerializeField] private float effectDuration = 2f; // Thời gian hiệu ứng
    [SerializeField] private bool causesCrash = false; // Có gây crash không

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;

    private AudioSource audioSource;

    public enum ObstacleType
    {
        Rock,        // Gây crash
        Tree,        // Gây crash  
        SmallRock,   // Giảm tốc độ
        IcePatch,    // Giảm tốc độ tạm thời
        SnowPile     // Giảm tốc độ nhẹ
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Thiết lập properties dựa trên loại obstacle
        SetObstacleProperties();
    }

    void SetObstacleProperties()
    {
        switch (obstacleType)
        {
            case ObstacleType.Rock:
            case ObstacleType.Tree:
                causesCrash = true;
                speedReduction = 0f; // Dừng hoàn toàn
                break;

            case ObstacleType.SmallRock:
                speedReduction = 0.3f; // Giảm 70% tốc độ
                effectDuration = 1.5f;
                break;

            case ObstacleType.IcePatch:
                speedReduction = 0.2f; // Giảm 80% tốc độ
                effectDuration = 3f;
                break;

            case ObstacleType.SnowPile:
                speedReduction = 0.7f; // Giảm 30% tốc độ
                effectDuration = 1f;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Phát âm thanh
                PlayHitSound();

                if (causesCrash)
                {
                    // Gây crash
                    HandleCrash(other);
                }
                else
                {
                    // Giảm tốc độ
                    HandleSpeedReduction(playerController);
                }
            }
        }
    }

    void HandleCrash(Collider2D player)
    {
        Debug.Log($"Player crashed into {obstacleType}!");

        // Gọi CrashDetect nếu có
        CrashDetect crashDetect = player.GetComponent<CrashDetect>();
        if (crashDetect != null)
        {
            // Trigger crash detection
            crashDetect.SendMessage("OnTriggerEnter2D", player);
        }

        // Có thể thêm hiệu ứng visual ở đây (particle, shake camera, etc.)
    }

    void HandleSpeedReduction(PlayerController playerController)
    {
        Debug.Log($"Player hit {obstacleType}! Speed reduced by {(1 - speedReduction) * 100}%");

        // Áp dụng giảm tốc độ
        playerController.ApplySpeedModifier(speedReduction, effectDuration);

        // Có thể thêm hiệu ứng visual ở đây
    }

    void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
}