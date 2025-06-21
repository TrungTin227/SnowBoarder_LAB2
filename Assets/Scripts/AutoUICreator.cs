using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoUICreator : MonoBehaviour
{
    [ContextMenu("Create Complete UI")]
    public void CreateCompleteUI()
    {
        // Xóa UI cũ nếu có
        GameObject oldCanvas = GameObject.Find("GameCanvas");
        if (oldCanvas != null)
        {
            DestroyImmediate(oldCanvas);
        }

        // 1. TẠO CANVAS CHÍNH
        GameObject gameCanvas = CreateCanvas();

        // 2. TẠO TOP PANEL
        GameObject topPanel = CreateTopPanel(gameCanvas);
        CreateTopPanelElements(topPanel);

        // 3. TẠO CENTER PANEL  
        CreateCenterPanel(gameCanvas);

        // 4. TẠO BOTTOM PANEL
        CreateBottomPanel(gameCanvas);

        Debug.Log("✅ UI Created Successfully! Assign references to GameUI script.");
        Debug.Log("📍 Elements created: SpeedText, SpeedBar, ScoreText, ComboText, ComboBar, CrashText, TrickText, PowerUpText");
    }

    GameObject CreateCanvas()
    {
        // Tạo Canvas
        GameObject canvasGO = new GameObject("GameCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        // Canvas Scaler
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // Graphic Raycaster
        canvasGO.AddComponent<GraphicRaycaster>();

        return canvasGO;
    }

    GameObject CreateTopPanel(GameObject canvas)
    {
        GameObject topPanel = new GameObject("TopPanel");
        topPanel.transform.SetParent(canvas.transform);

        RectTransform rect = topPanel.AddComponent<RectTransform>();

        // Anchor Top Stretch
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(0, -60);
        rect.sizeDelta = new Vector2(0, 120);
        rect.localScale = Vector3.one;

        // Background (optional)
        Image bg = topPanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.2f);

        return topPanel;
    }

    void CreateTopPanelElements(GameObject topPanel)
    {
        // SPEED PANEL (Top-Left)
        GameObject speedPanel = CreateSpeedPanel(topPanel);

        // SCORE TEXT (Top-Center-Left)  
        CreateScoreText(topPanel);

        // COMBO PANEL (Top-Center)
        CreateComboPanel(topPanel);

        // CRASH TEXT (Top-Right)
        CreateCrashText(topPanel);
    }

    GameObject CreateSpeedPanel(GameObject parent)
    {
        GameObject speedPanel = new GameObject("SpeedPanel");
        speedPanel.transform.SetParent(parent.transform);

        RectTransform rect = speedPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(0, 0.5f);
        rect.anchoredPosition = new Vector2(140, 0);
        rect.sizeDelta = new Vector2(250, 80);
        rect.localScale = Vector3.one;

        // Background
        Image bg = speedPanel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);

        // Vertical Layout
        VerticalLayoutGroup layout = speedPanel.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 8f;
        layout.padding = new RectOffset(10, 10, 15, 10);
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;

        // Speed Text
        GameObject speedText = CreateTextElement("SpeedText", speedPanel.transform,
            "⚡ Speed: 100%", 20, Color.white);

        // Speed Bar Background
        GameObject speedBarBG = CreateBarBackground("SpeedBarBG", speedPanel.transform,
            new Color(0.3f, 0.3f, 0.3f, 0.8f));

        // Speed Bar
        GameObject speedBar = CreateFillBar("SpeedBar", speedPanel.transform, Color.green);

        return speedPanel;
    }

    void CreateScoreText(GameObject parent)
    {
        GameObject scoreText = CreateTextElement("ScoreText", parent.transform,
            "🏆 Score: 0", 26, Color.white);

        RectTransform rect = scoreText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.3f, 0.5f);
        rect.anchorMax = new Vector2(0.3f, 0.5f);
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(200, 50);
    }

    GameObject CreateComboPanel(GameObject parent)
    {
        GameObject comboPanel = new GameObject("ComboPanel");
        comboPanel.transform.SetParent(parent.transform);

        RectTransform rect = comboPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(250, 80);
        rect.localScale = Vector3.one;

        // Background
        Image bg = comboPanel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);

        // Vertical Layout
        VerticalLayoutGroup layout = comboPanel.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 8f;
        layout.padding = new RectOffset(10, 10, 15, 10);
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;

        // Combo Text
        GameObject comboText = CreateTextElement("ComboText", comboPanel.transform,
            "🔥 Ready for Combo!", 20, Color.orange);

        // Combo Bar Background
        GameObject comboBarBG = CreateBarBackground("ComboBarBG", comboPanel.transform,
            new Color(0.3f, 0.3f, 0.3f, 0.8f));

        // Combo Bar
        GameObject comboBar = CreateFillBar("ComboBar", comboPanel.transform, Color.orange);

        return comboPanel;
    }

    void CreateCrashText(GameObject parent)
    {
        GameObject crashText = CreateTextElement("CrashText", parent.transform,
            "💥 Crashes: 0/3", 18, Color.red);

        RectTransform rect = crashText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0.5f);
        rect.anchorMax = new Vector2(1, 0.5f);
        rect.anchoredPosition = new Vector2(-100, 0);
        rect.sizeDelta = new Vector2(150, 50);
    }

    void CreateCenterPanel(GameObject canvas)
    {
        GameObject centerPanel = new GameObject("CenterPanel");
        centerPanel.transform.SetParent(canvas.transform);

        RectTransform rect = centerPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(500, 100);
        rect.localScale = Vector3.one;

        // Trick Text
        GameObject trickText = CreateTextElement("TrickText", centerPanel.transform,
            "", 36, Color.yellow);

        // Set full size
        RectTransform trickRect = trickText.GetComponent<RectTransform>();
        trickRect.anchorMin = Vector2.zero;
        trickRect.anchorMax = Vector2.one;
        trickRect.offsetMin = Vector2.zero;
        trickRect.offsetMax = Vector2.zero;

        // Add outline effect
        TextMeshProUGUI trickTMP = trickText.GetComponent<TextMeshProUGUI>();
        trickTMP.fontStyle = FontStyles.Bold;
        trickTMP.alignment = TextAlignmentOptions.Center;
    }

    void CreateBottomPanel(GameObject canvas)
    {
        GameObject bottomPanel = new GameObject("BottomPanel");
        bottomPanel.transform.SetParent(canvas.transform);

        RectTransform rect = bottomPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 0);
        rect.anchoredPosition = new Vector2(0, 50);
        rect.sizeDelta = new Vector2(0, 100);
        rect.localScale = Vector3.one;

        // Background
        Image bg = bottomPanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.3f);

        // Power-up Text
        GameObject powerUpText = CreateTextElement("PowerUpText", bottomPanel.transform,
            "SPACE (Invincible) | X (Super Boost) | Q (Trick)", 18, Color.white);

        // Set full size  
        RectTransform powerRect = powerUpText.GetComponent<RectTransform>();
        powerRect.anchorMin = Vector2.zero;
        powerRect.anchorMax = Vector2.one;
        powerRect.offsetMin = Vector2.zero;
        powerRect.offsetMax = Vector2.zero;

        TextMeshProUGUI powerTMP = powerUpText.GetComponent<TextMeshProUGUI>();
        powerTMP.alignment = TextAlignmentOptions.Center;
    }

    GameObject CreateTextElement(string name, Transform parent, string text, int fontSize, Color color)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent);

        TextMeshProUGUI textMesh = textGO.AddComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontStyle = FontStyles.Bold;

        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;

        return textGO;
    }

    GameObject CreateBarBackground(string name, Transform parent, Color color)
    {
        GameObject barBG = new GameObject(name);
        barBG.transform.SetParent(parent);

        Image image = barBG.AddComponent<Image>();
        image.color = color;

        RectTransform rect = barBG.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.sizeDelta = new Vector2(0, 12); // Full width, 12px height

        return barBG;
    }

    GameObject CreateFillBar(string name, Transform parent, Color color)
    {
        GameObject barGO = new GameObject(name);
        barGO.transform.SetParent(parent);

        Image image = barGO.AddComponent<Image>();
        image.color = color;
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;

        RectTransform rect = barGO.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.sizeDelta = new Vector2(0, 12); // Full width, 12px height

        return barGO;
    }
}