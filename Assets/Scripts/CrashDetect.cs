using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CrashDetect : MonoBehaviour
{
    [Header("Crash Settings")]
    [SerializeField] private int maxCrashes = 3;
    [SerializeField] private float invulnerabilityTime = 1f; // Thời gian bất tử sau crash

    private int crashCount = 0;
    private bool isInvulnerable = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu đang trong thời gian bất tử
        if (isInvulnerable) return;

        // Kiểm tra tag của obstacle
        if (other.CompareTag("Obstacle") || other.CompareTag("Enemy"))
        {
            HandleCrash();
        }
    }

    public void HandleCrash()
    {
        if (isInvulnerable) return;

        crashCount++;
        Debug.Log("Damn, it hits hard! Crash count: " + crashCount + "/" + maxCrashes);

        // Bắt đầu thời gian bất tử
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

        // Có thể thêm hiệu ứng nhấp nháy ở đây

        yield return new WaitForSeconds(invulnerabilityTime);

        isInvulnerable = false;
    }

    private IEnumerator RestartLevel()
    {
        // Có thể thêm delay và hiệu ứng trước khi restart
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Getter để UI có thể hiển thị số crash
    public int GetCrashCount()
    {
        return crashCount;
    }

    public int GetMaxCrashes()
    {
        return maxCrashes;
    }
}