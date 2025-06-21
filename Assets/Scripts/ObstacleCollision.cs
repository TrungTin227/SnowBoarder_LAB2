using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] private ObstacleType obstacleType;
    [SerializeField] private float speedReduction = 0.5f; // Giảm tốc độ 50%
    [SerializeField] private float effectDuration = 2f; // Thời gian hiệu ứng
    [SerializeField] private bool causesCrash = false; // Có gây crash không
    [SerializeField] private float bounceForce = 5f; // Lực đẩy ngược khi va chạm

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;

    private AudioSource audioSource;

    public enum ObstacleType
    {
        Rock,        // Va chạm mạnh, giảm tốc độ nhiều
        Tree,        // Va chạm mạnh, giảm tốc độ nhiều  
        SmallRock,   // Giảm tốc độ vừa
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
                speedReduction = 0.2f; // Giảm còn 20% tốc độ thay vì dừng hoàn toàn
                effectDuration = 2.5f;
                bounceForce = 8f;
                break;

            case ObstacleType.Tree:
                speedReduction = 0.15f; // Giảm còn 15% tốc độ
                effectDuration = 3f;
                bounceForce = 10f;
                break;

            case ObstacleType.SmallRock:
                speedReduction = 0.4f; // Giảm còn 40% tốc độ
                effectDuration = 1.5f;
                bounceForce = 4f;
                break;

            case ObstacleType.IcePatch:
                speedReduction = 0.3f; // Giảm còn 30% tốc độ
                effectDuration = 2f;
                bounceForce = 2f;
                break;

            case ObstacleType.SnowPile:
                speedReduction = 0.6f; // Giảm còn 60% tốc độ
                effectDuration = 1f;
                bounceForce = 3f;
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

                // Áp dụng hiệu ứng va chạm (giảm tốc độ + bounce effect)
                HandleCollision(playerController, other.GetComponent<Rigidbody2D>());

                // Kiểm tra nếu có CrashDetect component để tăng crash count
                CrashDetect crashDetect = other.GetComponent<CrashDetect>();
                if (crashDetect != null)
                {
                    crashDetect.HandleCrash();
                }
            }
        }
    }

    void HandleCollision(PlayerController playerController, Rigidbody2D playerRb)
    {
        Debug.Log($"Player hit {obstacleType}! Speed reduced to {(speedReduction * 100):F0}%");

        // Áp dụng giảm tốc độ
        playerController.ApplySpeedModifier(speedReduction, effectDuration);

        // Áp dụng hiệu ứng bounce (đẩy ngược lại)
        if (playerRb != null)
        {
            Vector2 bounceDirection = (playerRb.transform.position - transform.position).normalized;
            playerRb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
        }

        // Có thể thêm hiệu ứng visual ở đây (particle, screen shake, etc.)
        CreateCollisionEffect();
    }

    void CreateCollisionEffect()
    {
        // Tạo hiệu ứng particle khi va chạm
        // Bạn có thể thêm ParticleSystem component vào đây

        // Tạm thời dùng debug để test
        Debug.Log($"Collision effect for {obstacleType}!");
    }

    void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
}