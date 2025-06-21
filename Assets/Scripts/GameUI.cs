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
                speedStatus = "SUPER BOOST!";
                speedText.color = superBoostColor;
            }
            else
            {
                speedStatus = $"Speed: {(speedModifier * 100):F0}%";
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
            crashCountText.text = $"Crashes: {currentCrashes}/{maxCrashes}";
        }
    }

    void UpdatePowerUpDisplay()
    {
        if (playerController != null && powerUpText != null)
        {
            string powerUpStatus = "";

            if (playerController.IsInvincible())
            {
                powerUpStatus += "INVINCIBLE! ";
            }

            if (playerController.HasSuperBoost())
            {
                powerUpStatus += "SUPER BOOST! ";
            }

            if (string.IsNullOrEmpty(powerUpStatus))
            {
                powerUpStatus = "SPACE (Invincible) | X (Super Boost) | Q (Trick)";
            }

            powerUpText.text = powerUpStatus;
        }
    }

    void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {newScore:N0}";
        }
    }

    void UpdateComboDisplay(int combo, float multiplier)
    {
        if (comboText != null)
        {
            if (combo > 0)
            {
                comboText.text = $"COMBO x{combo} ({multiplier:F1}x)";
                comboText.color = comboColor;
            }
            else
            {
                comboText.text = "";
            }
        }

        if (comboBar != null)
        {
            comboBar.fillAmount = combo > 0 ? Mathf.Clamp01(combo / 10f) : 0f;
            comboBar.color = comboColor;
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
        trickText.text = message;
        trickText.color = Color.yellow;

        yield return new WaitForSeconds(2f);

        trickText.text = "";
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
            ScoreManager.Instance.OnComboChanged -= UpdateComboDisplay;
            ScoreManager.Instance.OnTrickPerformed -= ShowTrickMessage;
        }
    }
}