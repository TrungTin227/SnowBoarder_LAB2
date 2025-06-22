using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    [Header("Speed Boost UI - New")]
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Image cooldownBar;
    [SerializeField] private GameObject speedBoostPanel;

    [Header("Colors")]
    [SerializeField] private Color normalSpeedColor = Color.green;
    [SerializeField] private Color reducedSpeedColor = Color.red;
    [SerializeField] private Color superBoostColor = Color.cyan;
    [SerializeField] private Color megaBoostColor = Color.red;
    [SerializeField] private Color invincibilityColor = Color.gold;
    [SerializeField] private Color comboColor = Color.orange;
    [SerializeField] private Color powerUpActiveColor = Color.green;
    [SerializeField] private Color powerUpInactiveColor = Color.white;
    [SerializeField] private Color normalBoostColor = Color.yellow;

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

        // Create Speed Boost UI if not exists
        CreateSpeedBoostUI();
    }

    void Update()
    {
        UpdateSpeedDisplay();
        UpdateCrashDisplay();
        UpdatePowerUpDisplay();
        UpdateCooldownDisplay();
    }

    void CreateSpeedBoostUI()
    {
        // CHỈ TẠO UI NẾU CHƯA CÓ VÀ AutoUICreator KHÔNG TẠO
        if (speedBoostPanel == null && !FindObjectOfType<AutoUICreator>())
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("GameCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            CreateAdvancedSpeedBoostPanel(canvas.gameObject);
        }
    }

    void CreateAdvancedSpeedBoostPanel(GameObject parent)
    {
        // KIỂM TRA XEM CÓ TopPanel HAY KHÔNG
        bool hasTopPanel = parent.transform.Find("TopPanel") != null;

        // Main Speed Boost Panel
        speedBoostPanel = new GameObject("SpeedBoostPanel");
        speedBoostPanel.transform.SetParent(parent.transform);

        RectTransform panelRect = speedBoostPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);

        // ĐIỀU CHỈNH VỊ TRÍ DỰA TRÊN TopPanel
        if (hasTopPanel)
        {
            panelRect.anchoredPosition = new Vector2(20, -120); // Dịch xuống dưới TopPanel
        }
        else
        {
            panelRect.anchoredPosition = new Vector2(20, -20); // Vị trí ban đầu
        }

        panelRect.sizeDelta = new Vector2(320, 200);
        panelRect.localScale = Vector3.one;

        // Background
        Image panelBg = speedBoostPanel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

        // Layout
        VerticalLayoutGroup layout = speedBoostPanel.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 8f;
        layout.padding = new RectOffset(15, 15, 15, 15);
        layout.childControlWidth = true;
        layout.childControlHeight = false;

        // Title
        CreateTextElement("Title", speedBoostPanel.transform, "⚡ POWER-UP SYSTEM", 18, Color.yellow);

        // Current Status Display
        CreateCurrentStatusDisplay(speedBoostPanel.transform);

        // Cooldown Display
        CreateCooldownDisplay(speedBoostPanel.transform);

        // Instructions
        CreateInstructionsDisplay(speedBoostPanel.transform);
    }

    // ... giữ nguyên tất cả các method khác

    void CreateCurrentStatusDisplay(Transform parent)
    {
        GameObject statusDisplay = new GameObject("CurrentStatusDisplay");
        statusDisplay.transform.SetParent(parent);

        RectTransform rect = statusDisplay.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 40);

        VerticalLayoutGroup layout = statusDisplay.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 5f;

        // Speed Status
        if (speedText == null)
        {
            GameObject speedTextObj = CreateTextElement("SpeedText", statusDisplay.transform, "Speed: 100%", 16, Color.white);
            speedText = speedTextObj.GetComponent<TextMeshProUGUI>();
        }

        // Speed Bar
        if (speedBar == null)
        {
            GameObject speedBarBG = new GameObject("SpeedBarBG");
            speedBarBG.transform.SetParent(statusDisplay.transform);

            RectTransform bgRect = speedBarBG.AddComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(0, 15);

            Image bgImage = speedBarBG.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

            GameObject speedBarObj = new GameObject("SpeedBar");
            speedBarObj.transform.SetParent(speedBarBG.transform);

            RectTransform fillRect = speedBarObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            speedBar = speedBarObj.AddComponent<Image>();
            speedBar.color = Color.green;
            speedBar.type = Image.Type.Filled;
            speedBar.fillMethod = Image.FillMethod.Horizontal;
        }
    }

    void CreateCooldownDisplay(Transform parent)
    {
        GameObject cooldownDisplay = new GameObject("CooldownDisplay");
        cooldownDisplay.transform.SetParent(parent);

        RectTransform rect = cooldownDisplay.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 40);

        VerticalLayoutGroup layout = cooldownDisplay.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 5f;

        // Cooldown Text
        GameObject cooldownTextObj = CreateTextElement("CooldownText", cooldownDisplay.transform, "All Powers Ready!", 14, Color.green);
        cooldownText = cooldownTextObj.GetComponent<TextMeshProUGUI>();

        // Cooldown Bar
        GameObject cooldownBarBG = new GameObject("CooldownBarBG");
        cooldownBarBG.transform.SetParent(cooldownDisplay.transform);

        RectTransform bgRect = cooldownBarBG.AddComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(0, 15);

        Image bgImage = cooldownBarBG.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

        GameObject cooldownBarObj = new GameObject("CooldownBar");
        cooldownBarObj.transform.SetParent(cooldownBarBG.transform);

        RectTransform fillRect = cooldownBarObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        cooldownBar = cooldownBarObj.AddComponent<Image>();
        cooldownBar.color = Color.red;
        cooldownBar.type = Image.Type.Filled;
        cooldownBar.fillMethod = Image.FillMethod.Horizontal;
    }

    void CreateInstructionsDisplay(Transform parent)
    {
        GameObject instructionsDisplay = new GameObject("InstructionsDisplay");
        instructionsDisplay.transform.SetParent(parent);

        RectTransform rect = instructionsDisplay.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 60);

        VerticalLayoutGroup layout = instructionsDisplay.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 2f;

        CreateTextElement("Inst1", instructionsDisplay.transform, "Shift: Normal Boost | Space: Invincible", 10, Color.gray);
        CreateTextElement("Inst2", instructionsDisplay.transform, "X: Super Boost | C: Mega Boost", 10, Color.gray);
        CreateTextElement("Inst3", instructionsDisplay.transform, "Q: Trick (in air)", 10, Color.gray);
    }

    GameObject CreateTextElement(string name, Transform parent, string text, int fontSize, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, fontSize + 5);

        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.alignment = TextAlignmentOptions.Left;

        return textObj;
    }

    void UpdateSpeedDisplay()
    {
        if (playerController != null && speedText != null)
        {
            float speedModifier = playerController.GetCurrentSpeedModifier();
            string speedStatus = "";
            Color textColor = normalSpeedColor;

            // Ưu tiên hiển thị theo thứ tự: Invincible > Mega > Super > Normal Boost > Normal
            if (playerController.IsInvincible())
            {
                speedStatus = "🛡️ INVINCIBLE!";
                textColor = invincibilityColor;
            }
            else if (playerController.HasMegaBoost())
            {
                speedStatus = "🔥 MEGA BOOST!";
                textColor = megaBoostColor;
            }
            else if (playerController.HasSuperBoost())
            {
                speedStatus = "⚡ SUPER BOOST!";
                textColor = superBoostColor;
            }
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                speedStatus = "🚀 NORMAL BOOST!";
                textColor = normalBoostColor;
            }
            else
            {
                speedStatus = $"⚡ Speed: {(speedModifier * 100):F0}%";
                textColor = speedModifier < 1f ? reducedSpeedColor : normalSpeedColor;
            }

            speedText.text = speedStatus;
            speedText.color = textColor;

            if (speedBar != null)
            {
                float barValue = 0.5f; // Base value

                if (playerController.HasMegaBoost())
                    barValue = 1f;
                else if (playerController.HasSuperBoost())
                    barValue = 0.8f;
                else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    barValue = 0.6f;
                else
                    barValue = Mathf.Clamp01(speedModifier);

                speedBar.fillAmount = barValue;
                speedBar.color = textColor;
            }
        }
    }

    void UpdateCooldownDisplay()
    {
        if (playerController != null && cooldownText != null)
        {
            float superCooldown = playerController.GetSuperBoostCooldownRemaining();
            float megaCooldown = playerController.GetMegaBoostCooldownRemaining();
            float invincibilityCooldown = playerController.GetInvincibilityCooldownRemaining();

            float maxCooldown = Mathf.Max(superCooldown, megaCooldown, invincibilityCooldown);

            if (maxCooldown > 0)
            {
                string cooldownInfo = "";

                if (invincibilityCooldown > 0)
                    cooldownInfo += $"🛡️{invincibilityCooldown:F1}s ";
                if (superCooldown > 0)
                    cooldownInfo += $"⚡{superCooldown:F1}s ";
                if (megaCooldown > 0)
                    cooldownInfo += $"🔥{megaCooldown:F1}s ";

                cooldownText.text = $"Cooldowns: {cooldownInfo}";
                cooldownText.color = Color.yellow;

                // Update cooldown bar with longest cooldown
                if (cooldownBar != null)
                {
                    float totalCooldownTime = 10f; // Max cooldown time
                    if (invincibilityCooldown == maxCooldown) totalCooldownTime = 10f;
                    else if (megaCooldown == maxCooldown) totalCooldownTime = 8f;
                    else if (superCooldown == maxCooldown) totalCooldownTime = 5f;

                    cooldownBar.fillAmount = 1f - (maxCooldown / totalCooldownTime);
                    cooldownBar.color = Color.Lerp(Color.red, Color.green, cooldownBar.fillAmount);
                }
            }
            else
            {
                cooldownText.text = "🟢 All Powers Ready!";
                cooldownText.color = Color.green;

                if (cooldownBar != null)
                {
                    cooldownBar.fillAmount = 1f;
                    cooldownBar.color = Color.green;
                }
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
                powerUpStatus = "🛡️ INVINCIBLE ACTIVE!";
                powerUpText.color = invincibilityColor;
            }
            else if (playerController.HasMegaBoost())
            {
                powerUpStatus = "🔥 MEGA BOOST ACTIVE!";
                powerUpText.color = megaBoostColor;
            }
            else if (playerController.HasSuperBoost())
            {
                powerUpStatus = "⚡ SUPER BOOST ACTIVE!";
                powerUpText.color = superBoostColor;
            }
            else
            {
                powerUpStatus = "Use Space/X/C for Power-ups!";
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

    IEnumerator ShowTrickMessageCoroutine(string message)
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