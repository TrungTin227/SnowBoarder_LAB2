using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI crashCountText;
    [SerializeField] private TextMeshProUGUI powerUpText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI trickText;
    [SerializeField] private Image speedBar;
    [SerializeField] private Image comboBar;

    [Header("Colors")]
    [SerializeField] private Color normalSpeedColor = Color.green;
    [SerializeField] private Color reducedSpeedColor = Color.red;
    [SerializeField] private Color superBoostColor = Color.yellow;
    [SerializeField] private Color comboColor = Color.orange;
    [SerializeField] private Color powerUpActiveColor = Color.green; // Màu khi active
    [SerializeField] private Color powerUpInactiveColor = Color.white; // Màu khi inactive

    private PlayerController playerController;
    private CrashDetect crashDetect;
    private Coroutine trickMessageCoroutine;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        crashDetect = FindObjectOfType<CrashDetect>();

        // Subscribe to score manager events
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScoreDisplay;
            ScoreManager.Instance.OnComboChanged += UpdateComboDisplay;
            ScoreManager.Instance.OnTrickPerformed += ShowTrickMessage;
        }
    }

    void Update()
    {
        UpdateSpeedDisplay();
        UpdateCrashDisplay();
        UpdatePowerUpDisplay();
    }

    void UpdateSpeedDisplay()
    {
        if (playerController != null && speedText != null)
        {
            float speedModifier = playerController.GetCurrentSpeedModifier();
            string speedStatus = "";

            if (playerController.HasSuperBoost())
            {
                speedStatus = "🚀 SUPER BOOST!";
                speedText.color = superBoostColor;
            }
            else
            {
                speedStatus = $"⚡ Speed: {(speedModifier * 100):F0}%";
                speedText.color = speedModifier < 1f ? reducedSpeedColor : normalSpeedColor;
            }

            speedText.text = speedStatus;

            if (speedBar != null)
            {
                float barValue = playerController.HasSuperBoost() ? 1.5f : speedModifier;
                speedBar.fillAmount = Mathf.Clamp01(barValue);

                if (playerController.HasSuperBoost())
                    speedBar.color = superBoostColor;
                else
                    speedBar.color = Color.Lerp(reducedSpeedColor, normalSpeedColor, speedModifier);
            }
        }
    }

    void UpdateCrashDisplay()
    {
        if (crashDetect != null && crashCountText != null)
        {
            int currentCrashes = crashDetect.GetCrashCount();
            int maxCrashes = crashDetect.GetMaxCrashes();
            crashCountText.text = $"💥 Crashes: {currentCrashes}/{maxCrashes}";

            // Đổi màu dựa trên số lần crash
            if (currentCrashes >= maxCrashes - 1)
                crashCountText.color = Color.red;
            else if (currentCrashes >= maxCrashes / 2)
                crashCountText.color = Color.yellow;
            else
                crashCountText.color = Color.white;
        }
    }

    void UpdatePowerUpDisplay()
    {
        if (playerController != null && powerUpText != null)
        {
            string powerUpStatus = "";

            if (playerController.IsInvincible())
            {
                powerUpStatus += "🛡️ INVINCIBLE! ";
                powerUpText.color = powerUpActiveColor;
            }
            else if (playerController.HasSuperBoost())
            {
                powerUpStatus += "🚀 SUPER BOOST! ";
                powerUpText.color = powerUpActiveColor;
            }
            else
            {
                powerUpStatus = "SPACE (Invincible) | X (Super Boost) | Q (Trick)";
                powerUpText.color = powerUpInactiveColor;
            }

            powerUpText.text = powerUpStatus;
        }
    }

    void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"🏆 Score: {newScore:N0}";
        }
    }

    void UpdateComboDisplay(int combo, float multiplier)
    {
        if (comboText != null)
        {
            if (combo > 0)
            {
                comboText.text = $"🔥 COMBO x{combo} ({multiplier:F1}x)";
                comboText.color = comboColor;
            }
            else
            {
                comboText.text = "Ready for Combo!";
                comboText.color = Color.gray;
            }
        }

        if (comboBar != null)
        {
            comboBar.fillAmount = combo > 0 ? Mathf.Clamp01(combo / 10f) : 0f;
            comboBar.color = combo > 0 ? comboColor : Color.gray;
        }
    }

    void ShowTrickMessage(string message)
    {
        if (trickText != null)
        {
            if (trickMessageCoroutine != null)
            {
                StopCoroutine(trickMessageCoroutine);
            }
            trickMessageCoroutine = StartCoroutine(ShowTrickMessageCoroutine(message));
        }
    }

    System.Collections.IEnumerator ShowTrickMessageCoroutine(string message)
    {
        // Hiệu ứng xuất hiện
        trickText.text = $"✨ {message} ✨";
        trickText.color = Color.yellow;

        // Scale animation
        trickText.transform.localScale = Vector3.one * 1.2f;

        float timer = 0f;
        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(1.2f, 1f, timer / 0.3f);
            trickText.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        // Fade out
        timer = 0f;
        Color startColor = trickText.color;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / 0.5f);
            trickText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        trickText.text = "";
        trickText.color = Color.yellow;
        trickText.transform.localScale = Vector3.one;
    }

    void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
            ScoreManager.Instance.OnComboChanged -= UpdateComboDisplay;
            ScoreManager.Instance.OnTrickPerformed -= ShowTrickMessage;
        }
    }
}