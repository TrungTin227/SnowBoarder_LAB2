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

    private bool isCollected = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
        if (isCollected) return;

        isCollected = true;
        Debug.Log($"✅ Collecting {type}...");

        // 🔧 FIX: Sử dụng đúng method CollectCoin
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.CollectCoin(type); // ✅ ĐÚNG
            Debug.Log($"💰 Collected {type} successfully!");
        }
        else
        {
            Debug.LogError("❌ ScoreManager.Instance is null!");
        }

        PlayCollectEffects();
        DisableCollectible();
    }

    void DisableCollectible()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = false;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        rotateAnimation = false;
        Destroy(gameObject, 1f);
    }

    void PlayCollectEffects()
    {
        if (collectEffect != null)
        {
            collectEffect.Play();
        }

        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
    }

    public void SetCollectibleType(CollectibleType newType)
    {
        type = newType;
    }

    public CollectibleType GetCollectibleType()
    {
        return type;
    }
}

public enum CollectibleType
{
    Coin,        // 10 điểm
    SilverCoin,  // 25 điểm
    GoldCoin,    // 50 điểm
    Diamond      // 100 điểm
}