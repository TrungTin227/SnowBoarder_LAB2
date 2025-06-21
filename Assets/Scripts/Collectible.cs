using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private CollectibleType type = CollectibleType.Coin;
    [SerializeField] private bool rotateAnimation = true;
    [SerializeField] private float rotationSpeed = 90f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem collectEffect;
    [SerializeField] private AudioClip collectSound;

    // FIX: Enum mới với đúng giá trị 10, 25, 50, 100
    public enum CollectibleType
    {
        Coin,        // 10 điểm
        SilverCoin,  // 25 điểm
        GoldCoin,    // 50 điểm
        Diamond      // 100 điểm
    }

    private bool isCollected = false; // FIX: Tránh collect nhiều lần
    private AudioSource audioSource;

    void Start()
    {
        // FIX: Setup AudioSource properly
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set volume thấp để không làm ồn
        audioSource.volume = 0.5f;
    }

    void Update()
    {
        if (rotateAnimation && !isCollected)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🟡 COIN TRIGGERED by: {other.name} with tag: {other.tag}");

        if (other.CompareTag("Player") && !isCollected)
        {
            Debug.Log($"🪙 Player chạm vào {type}!");
            CollectItem();
        }
    }

    void CollectItem()
    {
        // FIX: Double check để tránh collect nhiều lần
        if (isCollected)
        {
            Debug.Log("⚠️ Item đã được collect rồi!");
            return;
        }

        isCollected = true;
        Debug.Log($"✅ Collecting {type}...");

        // FIX: Thông báo ScoreManager với đúng method
        if (ScoreManager.Instance != null)
        {
            int points = GetCollectiblePoints();
            ScoreManager.Instance.CollectItem(points); // Dùng method có sẵn
            Debug.Log($"💰 Added {points} points to score!");
        }
        else
        {
            Debug.LogError("❌ ScoreManager.Instance is null!");
        }

        // Phát hiệu ứng
        PlayCollectEffects();

        // FIX: QUAN TRỌNG - Thay vì SetActive(false)
        // Ta disable từng component riêng biệt
        DisableCollectible();
    }

    void DisableCollectible()
    {
        // FIX: Disable components thay vì toàn bộ GameObject
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
            Debug.Log("🔍 Disabled renderer");
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            Debug.Log("📦 Disabled collider");
        }

        // Dừng animation
        rotateAnimation = false;

        // FIX: Destroy object sau delay để audio effect phát xong
        Destroy(gameObject, 1f);
        Debug.Log("🗑️ Scheduled object destruction in 1s");
    }

    int GetCollectiblePoints()
    {
        // FIX: Đúng giá trị theo yêu cầu
        switch (type)
        {
            case CollectibleType.Coin: return 10;
            case CollectibleType.SilverCoin: return 25;
            case CollectibleType.GoldCoin: return 50;
            case CollectibleType.Diamond: return 100;
            default: return 10;
        }
    }

    void PlayCollectEffects()
    {
        // Particle effect
        if (collectEffect != null)
        {
            collectEffect.Play();
            Debug.Log("✨ Playing particle effect");
        }

        // Sound effect
        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
            Debug.Log("🔊 Playing sound effect");
        }
    }

    // FIX: Method để set type từ Inspector
    public void SetCollectibleType(CollectibleType newType)
    {
        type = newType;
        Debug.Log($"🏷️ Set collectible type to {newType}");
    }

    // Debug method
    public CollectibleType GetCollectibleType()
    {
        return type;
    }
}