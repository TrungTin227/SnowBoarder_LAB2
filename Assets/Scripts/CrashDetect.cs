using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CrashDetect : MonoBehaviour
{
    [Header("Crash Settings")]
    [SerializeField] private int maxCrashes = 3;
    [SerializeField] private float invulnerabilityTime = 1f;

    private int crashCount = 0;
    private bool isInvulnerable = false;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvulnerable || (playerController != null && playerController.IsInvincible()))
            return;

        if (other.CompareTag("Obstacle"))
        {
            HandleCrash();
        }
    }

    public void HandleCrash()
    {
        if (isInvulnerable || (playerController != null && playerController.IsInvincible()))
            return;

        crashCount++;
        Debug.Log("Damn, it hits hard! Crash count: " + crashCount + "/" + maxCrashes);

        // Reset combo khi crash
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetComboOnCrash();
        }

        StartCoroutine(InvulnerabilityCoroutine());

        if (crashCount >= maxCrashes)
        {
            Debug.Log("Too many crashes. Restarting level...");
            StartCoroutine(RestartLevel());
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int GetCrashCount() => crashCount;
    public int GetMaxCrashes() => maxCrashes;
}