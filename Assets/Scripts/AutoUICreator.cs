using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoUICreator : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private bool createSpeedBoostUI = true;
    [SerializeField] private bool createAdvancedSpeedUI = true;

    void Start()
    {
        CreateGameUI();
    }

    void CreateGameUI()
    {
        // Create Canvas if doesn't exist
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("GameCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        if (createSpeedBoostUI)
        {
            CreateSpeedBoostPanel(canvas.gameObject);
        }

        if (createAdvancedSpeedUI)
        {
            CreateAdvancedSpeedUI(canvas.gameObject);
        }
    }

    GameObject CreateSpeedBoostPanel(GameObject parent)
    {
        // Main Speed Boost Panel
        GameObject speedBoostPanel = new GameObject("SpeedBoostPanel");
        speedBoostPanel.transform.SetParent(parent.transform);

        RectTransform panelRect = speedBoostPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(20, -20);
        panelRect.sizeDelta = new Vector2(320, 220);
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

        // Current Speed Display
        CreateCurrentSpeedDisplay(speedBoostPanel.transform);

        // Invincibility Button
        CreateBoostButton(speedBoostPanel.transform, "InvincibilityBtn", "🛡️ INVINCIBILITY (Space)", Color.gold, "Space");

        // Super Boost Button
        CreateBoostButton(speedBoostPanel.transform, "SuperBoostBtn", "⚡ SUPER BOOST (X)", Color.cyan, "X");

        // Mega Boost Button  
        CreateBoostButton(speedBoostPanel.transform, "MegaBoostBtn", "🔥 MEGA BOOST (C)", Color.red, "C");

        // REMOVED: SpeedBoostUI uiManager = speedBoostPanel.AddComponent<SpeedBoostUI>();
        // GameUI sẽ tự động quản lý UI này

        return speedBoostPanel;
    }

    void CreateCurrentSpeedDisplay(Transform parent)
    {
        GameObject speedDisplay = new GameObject("CurrentSpeedDisplay");
        speedDisplay.transform.SetParent(parent);

        RectTransform rect = speedDisplay.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 30);

        HorizontalLayoutGroup layout = speedDisplay.AddComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.spacing = 10f;

        // Speed Text
        GameObject speedText = CreateTextElement("SpeedText", speedDisplay.transform, "Speed: 100%", 16, Color.white);

        // Speed Bar Background
        GameObject speedBarBG = new GameObject("SpeedBarBG");
        speedBarBG.transform.SetParent(speedDisplay.transform);

        RectTransform bgRect = speedBarBG.AddComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(100, 20);

        Image bgImage = speedBarBG.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

        // Speed Bar Fill
        GameObject speedBar = new GameObject("SpeedBar");
        speedBar.transform.SetParent(speedBarBG.transform);

        RectTransform fillRect = speedBar.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        Image fillImage = speedBar.AddComponent<Image>();
        fillImage.color = Color.green;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
    }

    GameObject CreateBoostButton(Transform parent, string name, string text, Color color, string key)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 35);

        // Button component
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(color.r * 0.3f, color.g * 0.3f, color.b * 0.3f, 0.8f);

        // Button colors
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        colors.highlightedColor = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f, 0.9f);
        colors.pressedColor = color;
        colors.disabledColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        button.colors = colors;

        // Button layout
        HorizontalLayoutGroup layout = buttonObj.AddComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        layout.spacing = 10f;
        layout.padding = new RectOffset(10, 10, 5, 5);

        // Button Text
        GameObject textObj = CreateTextElement("ButtonText", buttonObj.transform, text, 14, Color.white);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(180, 0);

        // Cooldown Fill (background for cooldown)
        GameObject cooldownBG = new GameObject("CooldownBG");
        cooldownBG.transform.SetParent(buttonObj.transform);

        RectTransform cooldownRect = cooldownBG.AddComponent<RectTransform>();
        cooldownRect.sizeDelta = new Vector2(40, 25);

        Image cooldownBGImage = cooldownBG.AddComponent<Image>();
        cooldownBGImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        // Cooldown Fill
        GameObject cooldownFill = new GameObject("CooldownFill");
        cooldownFill.transform.SetParent(cooldownBG.transform);

        RectTransform fillRect = cooldownFill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        Image fillImage = cooldownFill.AddComponent<Image>();
        fillImage.color = color;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Vertical;

        // Cooldown Text
        GameObject cooldownText = CreateTextElement("CooldownText", cooldownBG.transform, "Ready!", 10, Color.white);
        RectTransform cooldownTextRect = cooldownText.GetComponent<RectTransform>();
        cooldownTextRect.anchorMin = Vector2.zero;
        cooldownTextRect.anchorMax = Vector2.one;
        cooldownTextRect.offsetMin = Vector2.zero;
        cooldownTextRect.offsetMax = Vector2.zero;

        return buttonObj;
    }

    GameObject CreateAdvancedSpeedUI(GameObject parent)
    {
        // Advanced Speed Panel (right side)
        GameObject advancedPanel = new GameObject("AdvancedSpeedPanel");
        advancedPanel.transform.SetParent(parent.transform);

        RectTransform rect = advancedPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-20, -20);
        rect.sizeDelta = new Vector2(220, 120);
        rect.localScale = Vector3.one;

        // Background
        Image bg = advancedPanel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);

        // Layout
        VerticalLayoutGroup layout = advancedPanel.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 5f;
        layout.padding = new RectOffset(10, 10, 10, 10);

        // Instructions
        CreateTextElement("Instructions0", advancedPanel.transform, "🎮 CONTROLS", 14, Color.yellow);
        CreateTextElement("Instructions1", advancedPanel.transform, "Shift: Normal Boost", 12, Color.gray);
        CreateTextElement("Instructions2", advancedPanel.transform, "Space: Invincibility", 12, Color.gold);
        CreateTextElement("Instructions3", advancedPanel.transform, "X: Super Boost", 12, Color.cyan);
        CreateTextElement("Instructions4", advancedPanel.transform, "C: Mega Boost", 12, Color.red);
        CreateTextElement("Instructions5", advancedPanel.transform, "Q: Trick (in air)", 12, Color.yellow);

        return advancedPanel;
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
        textComponent.alignment = TextAlignmentOptions.Center;

        return textObj;
    }
}