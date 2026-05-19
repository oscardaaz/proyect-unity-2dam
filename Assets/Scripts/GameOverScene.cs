using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameOverScene : MonoBehaviour
{
    private const float ContentWidth = 960f;
    private const float ContentHeight = 640f;
    private const float ContentCenterY = 30f;
    private const float PlayButtonScale = 2.5f;

    private void Start()
    {
        ConfigureCanvasScaler();
        FitBackgroundToScreen();
        ArrangeContent();
    }

    public void LoadMainMenu()
    {
        SceneTransition.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void FitBackgroundToScreen()
    {
        RectTransform panel = FindRectTransform("Panel");
        if (panel != null)
        {
            panel.SetAsFirstSibling();
            panel.anchorMin = Vector2.zero;
            panel.anchorMax = Vector2.one;
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            panel.localScale = Vector3.one;
        }

        RawImage background = GetComponentInChildren<RawImage>(true);
        if (background == null)
            return;

        RectTransform backgroundRect = background.rectTransform;
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        backgroundRect.localScale = Vector3.one;
        background.raycastTarget = false;
    }

    private void ConfigureCanvasScaler()
    {
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
            return;

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor = 1f;
    }

    private void ArrangeContent()
    {
        RectTransform panel = FindRectTransform("Panel");
        RectTransform title = FindTitleText();
        RectTransform restartButton = FindRectTransform("ButtonRestart");
        RectTransform quitButton = FindRectTransform("ButtonQuit");

        if (panel == null || title == null || restartButton == null || quitButton == null)
            return;

        RectTransform content = GetOrCreateContent(panel);
        title.SetParent(content, false);
        restartButton.SetParent(content, false);
        quitButton.SetParent(content, false);

        ConfigureTitle(title);
        PositionButton(restartButton, new Vector2(0f, -110f));
        PositionButton(quitButton, new Vector2(0f, -210f));
        ScaleButtonForPlay(restartButton);
        ScaleButtonForPlay(quitButton);
        MenuVolumeControl.CreateBelowButtons(
            this,
            new[] { "ButtonRestart", "ButtonQuit" },
            -110f,
            new Vector2(620f, 96f),
            50f,
            50f,
            new Vector2(580f, 34f),
            new Vector2(34f, 34f));
    }

    private RectTransform GetOrCreateContent(RectTransform panel)
    {
        Transform existing = panel.Find("GameOverContent");
        if (existing != null && existing.TryGetComponent(out RectTransform existingRect))
            return existingRect;

        GameObject contentObject = new GameObject("GameOverContent", typeof(RectTransform));
        RectTransform content = contentObject.GetComponent<RectTransform>();
        content.SetParent(panel, false);
        content.SetAsLastSibling();
        content.anchorMin = new Vector2(0.5f, 0.5f);
        content.anchorMax = new Vector2(0.5f, 0.5f);
        content.pivot = new Vector2(0.5f, 0.5f);
        content.anchoredPosition = new Vector2(0f, ContentCenterY);
        content.sizeDelta = new Vector2(ContentWidth, ContentHeight);
        content.localScale = Vector3.one;

        return content;
    }

    private void ConfigureTitle(RectTransform title)
    {
        title.anchorMin = new Vector2(0.5f, 0.5f);
        title.anchorMax = new Vector2(0.5f, 0.5f);
        title.pivot = new Vector2(0.5f, 0.5f);
        title.anchoredPosition = new Vector2(0f, 145f);
        title.sizeDelta = new Vector2(900f, 345f);
        title.localScale = Vector3.one;

        if (title.TryGetComponent(out TextMeshProUGUI titleText))
        {
            titleText.enableAutoSizing = false;
            titleText.fontSize = 70f;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.raycastTarget = false;
        }
    }

    private void PositionButton(RectTransform button, Vector2 position)
    {
        button.anchorMin = new Vector2(0.5f, 0.5f);
        button.anchorMax = new Vector2(0.5f, 0.5f);
        button.pivot = new Vector2(0.5f, 0.5f);
        button.anchoredPosition = position;
        button.localScale = Vector3.one;
    }

    private void ScaleButtonForPlay(RectTransform button)
    {
        button.sizeDelta *= PlayButtonScale;

        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (buttonText == null)
            return;

        buttonText.enableAutoSizing = false;
        buttonText.fontSize *= PlayButtonScale;
        buttonText.fontSizeMax *= PlayButtonScale;
    }

    private RectTransform FindTitleText()
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI text in texts)
        {
            if (text.text.Contains("HAS PERDIDO") && text.TryGetComponent(out RectTransform rectTransform))
                return rectTransform;
        }

        return null;
    }

    private RectTransform FindRectTransform(string objectName)
    {
        Transform[] children = GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (child.name == objectName && child.TryGetComponent(out RectTransform rectTransform))
                return rectTransform;
        }

        return null;
    }
}
