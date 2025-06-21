using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private CollectibleType type;
    [SerializeField] private int scoreValue = 50;
    [SerializeField] private bool rotateAnimation = true;
    [SerializeField] private float rotationSpeed = 90f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem collectEffect;
    [SerializeField] private AudioClip collectSound;

    public enum CollectibleType
    {
        Coin,        // 50 điểm
        Gem,         // 100 điểm
        Star,        // 200 điểm
        PowerUp      // 150 điểm + effect
    }

    void Start()
    {
        SetCollectibleProperties();
    }

    void Update()
    {
        if (rotateAnimation)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }

    void SetCollectibleProperties()
    {
        switch (type)
        {
            case CollectibleType.Coin:
                scoreValue = 50;
                break;
            case CollectibleType.Gem:
                scoreValue = 100;
                break;
            case CollectibleType.Star:
                scoreValue = 200;
                break;
            case CollectibleType.PowerUp:
                scoreValue = 150;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectItem(other);
        }
    }

    void CollectItem(Collider2D player)
    {
        // Thêm điểm vào score manager
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.CollectItem(scoreValue);
        }

        // Áp dụng effect đặc biệt cho PowerUp
        if (type == CollectibleType.PowerUp)
        {
            ApplyPowerUpEffect(player);
        }

        // Hiệu ứng thu thập
        PlayCollectEffects();

        // Ẩn object
        gameObject.SetActive(false);
    }

    void ApplyPowerUpEffect(Collider2D player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Có thể kích hoạt temporary boost hoặc invincibility
            playerController.ActivateSuperBoost();
        }
    }

    void PlayCollectEffects()
    {
        // Particle effect
        if (collectEffect != null)
        {
            collectEffect.Play();
        }

        // Sound effect
        if (collectSound != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
        }
    }
}