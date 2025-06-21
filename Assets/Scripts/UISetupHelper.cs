using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISetupHelper : MonoBehaviour
{
    [ContextMenu("Setup UI GameObjects")]
    public void SetupUI()
    {
        // Tạo Canvas chính
        GameObject canvasGO = new GameObject("GameCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        // Tạo Top Panel
        GameObject topPanel = CreatePanel("TopPanel", canvasGO.transform);
        SetAnchor(topPanel.GetComponent<RectTransform>(), AnchorPresets.TopStretch);

        // Speed Panel
        GameObject speedPanel = CreatePanel("SpeedPanel", topPanel.transform);
        CreateText("SpeedText", speedPanel.transform, "Speed: 0", 24, Color.white);
        CreateFillBar("SpeedBar", speedPanel.transform, Color.green);

        // Score Text
        CreateText("ScoreText", topPanel.transform, "Score: 0", 32, Color.white);

        // Combo Panel
        GameObject comboPanel = CreatePanel("ComboPanel", topPanel.transform);
        CreateText("ComboText", comboPanel.transform, "Combo: x1", 28, Color.orange);
        CreateFillBar("ComboBar", comboPanel.transform, Color.orange);

        // Crash Text
        CreateText("CrashText", topPanel.transform, "Crashes: 0/3", 20, Color.red);

        // Center Panel
        GameObject centerPanel = CreatePanel("CenterPanel", canvasGO.transform);
        SetAnchor(centerPanel.GetComponent<RectTransform>(), AnchorPresets.MiddleCenter);
        CreateText("TrickText", centerPanel.transform, "", 36, Color.yellow);

        // Bottom Panel
        GameObject bottomPanel = CreatePanel("BottomPanel", canvasGO.transform);
        SetAnchor(bottomPanel.GetComponent<RectTransform>(), AnchorPresets.BottomStretch);
        CreateText("PowerUpText", bottomPanel.transform, "SPACE (Invincible) | X (Super Boost) | Q (Trick)", 18, Color.cyan);

        Debug.Log("UI Setup completed! Don't forget to assign references to GameUI script.");
    }

    GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        return panel;
    }

    GameObject CreateText(string name, Transform parent, string text, int fontSize, Color color)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent);

        TextMeshProUGUI textMesh = textGO.AddComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.alignment = TextAlignmentOptions.Center;

        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;

        return textGO;
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
        rect.sizeDelta = new Vector2(200, 20);

        return barGO;
    }

    void SetAnchor(RectTransform rect, AnchorPresets preset)
    {
        switch (preset)
        {
            case AnchorPresets.TopStretch:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.anchoredPosition = new Vector2(0, -50);
                rect.sizeDelta = new Vector2(0, 100);
                break;
            case AnchorPresets.BottomStretch:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.anchoredPosition = new Vector2(0, 50);
                rect.sizeDelta = new Vector2(0, 100);
                break;
            case AnchorPresets.MiddleCenter:
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(400, 200);
                break;
        }
    }

    enum AnchorPresets
    {
        TopStretch,
        BottomStretch,
        MiddleCenter
    }
}