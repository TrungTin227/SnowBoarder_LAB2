using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI crashCountText;
    [SerializeField] private Image speedBar;
    [SerializeField] private Color normalSpeedColor = Color.green;
    [SerializeField] private Color reducedSpeedColor = Color.red;

    private PlayerController playerController;
    private CrashDetect crashDetect;

    void Start()
    {
        // Tìm components
        playerController = FindObjectOfType<PlayerController>();
        crashDetect = FindObjectOfType<CrashDetect>();
    }

    void Update()
    {
        UpdateSpeedDisplay();
        UpdateCrashDisplay();
    }

    void UpdateSpeedDisplay()
    {
        if (playerController != null && speedText != null)
        {
            float speedModifier = playerController.GetCurrentSpeedModifier();
            speedText.text = $"Speed: {(speedModifier * 100):F0}%";

            // Cập nhật màu sắc speed bar
            if (speedBar != null)
            {
                speedBar.fillAmount = speedModifier;
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
}