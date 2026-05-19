using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset menuFont;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private const string MusicVolumePrefKey = "MusicVolume";
    private static readonly Color OverlayColor = new Color(0f, 0f, 0f, 0.72f);
    private static readonly Color PanelColor = new Color(0.12f, 0.105f, 0.095f, 0.96f);
    private static readonly Color ButtonColor = new Color(0.055f, 0.052f, 0.048f, 1f);
    private static readonly Color ButtonHighlightedColor = new Color(0.18f, 0.145f, 0.095f, 1f);
    private static readonly Color ButtonPressedColor = new Color(0.30f, 0.23f, 0.13f, 1f);
    private static readonly Color TitleColor = new Color(0.95f, 0.72f, 0.28f, 1f);
    private static readonly Color TextColor = new Color(0.95f, 0.88f, 0.72f, 1f);
    private static readonly Color ShadowColor = new Color(0f, 0f, 0f, 0.62f);
    private const float CleanFontSize = 68f;
    private const float SmallTextScale = 60f / CleanFontSize;

    private GameObject pauseCanvas;
    private GameObject firstSelectedButton;
    private Slider volumeSlider;
    private Material cleanTextMaterial;
    private readonly List<AudioSource> musicSources = new List<AudioSource>();
    private readonly List<float> baseMusicVolumes = new List<float>();
    private readonly Dictionary<int, float> baseMusicVolumeBySource = new Dictionary<int, float>();
    private bool isPaused;

    private void Start()
    {
        if (menuFont == null)
            menuFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

        RefreshMusicSources();
        ApplyMusicVolume(PlayerPrefs.GetFloat(MusicVolumePrefKey, 1f));
        CreatePauseMenu();
        SetPause(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetPause(!isPaused);
    }

    private void OnDisable()
    {
        if (isPaused)
            Time.timeScale = 1f;
    }

    public void ResumeGame()
    {
        SetPause(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneTransition.LoadScene(mainMenuSceneName);
    }

    private void SetPause(bool pause)
    {
        isPaused = pause;
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            float volume = PlayerPrefs.GetFloat(MusicVolumePrefKey, 1f);
            RefreshMusicSources();
            ApplyMusicVolume(volume);

            if (volumeSlider != null)
                volumeSlider.SetValueWithoutNotify(volume);
        }

        if (pauseCanvas != null)
            pauseCanvas.SetActive(isPaused);

        if (!isPaused || firstSelectedButton == null)
            return;

        EnsureEventSystem();
        if (EventSystem.current == null)
            return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    private void CreatePauseMenu()
    {
        EnsureEventSystem();

        pauseCanvas = new GameObject("PauseMenuCanvas");
        pauseCanvas.transform.SetParent(transform, false);

        Canvas canvas = pauseCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9000;

        CanvasScaler canvasScaler = pauseCanvas.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        pauseCanvas.AddComponent<GraphicRaycaster>();

        GameObject overlayObject = new GameObject("Overlay");
        overlayObject.transform.SetParent(pauseCanvas.transform, false);

        Image overlay = overlayObject.AddComponent<Image>();
        overlay.color = OverlayColor;

        RectTransform overlayRect = overlay.rectTransform;
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        GameObject panelObject = new GameObject("Panel");
        panelObject.transform.SetParent(pauseCanvas.transform, false);

        Image panel = panelObject.AddComponent<Image>();
        panel.color = PanelColor;
        AddShadow(panel.gameObject, new Vector2(8f, -8f), new Color(0f, 0f, 0f, 0.55f));

        RectTransform panelRect = panel.rectTransform;
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(680f, 660f);

        VerticalLayoutGroup layout = panelObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(56, 56, 22, 44);
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        AddText(panelObject.transform, "PAUSA", 95f, TitleColor, 96f);
        AddSeparator(panelObject.transform);
        AddSpacer(panelObject.transform, 69f);
        AddVolumeControl(panelObject.transform);
        firstSelectedButton = AddButton(panelObject.transform, "CONTINUAR", ResumeGame);
        AddSpacer(panelObject.transform, 10f);
        AddButton(panelObject.transform, "ABANDONAR", ReturnToMainMenu);

        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
    }

    private TextMeshProUGUI AddText(Transform parent, string text, float fontSize, Color color, float height)
    {
        GameObject textObject = new GameObject(text);
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI label = textObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.font = menuFont;
        label.fontSize = fontSize;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = color;
        label.raycastTarget = false;
        CleanTextMaterial(label);

        LayoutElement layoutElement = textObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;

        return label;
    }

    private void AddSeparator(Transform parent)
    {
        GameObject separatorObject = new GameObject("Separator");
        separatorObject.transform.SetParent(parent, false);

        Image separator = separatorObject.AddComponent<Image>();
        separator.color = new Color(0.95f, 0.72f, 0.28f, 0.9f);

        LayoutElement layoutElement = separatorObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 4f;
        layoutElement.minHeight = 4f;
    }

    private void AddSpacer(Transform parent, float height)
    {
        GameObject spacerObject = new GameObject("Spacer");
        spacerObject.transform.SetParent(parent, false);

        LayoutElement layoutElement = spacerObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;
        layoutElement.minHeight = height;
    }

    private GameObject AddButton(Transform parent, string text, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = new GameObject(text);
        buttonObject.transform.SetParent(parent, false);

        Image background = buttonObject.AddComponent<Image>();
        background.color = ButtonColor;
        AddShadow(buttonObject, new Vector2(5f, -5f), new Color(0f, 0f, 0f, 0.56f));

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = background;
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.normalColor = ButtonColor;
        colors.highlightedColor = ButtonHighlightedColor;
        colors.pressedColor = ButtonPressedColor;
        colors.selectedColor = ButtonHighlightedColor;
        button.colors = colors;

        LayoutElement buttonLayout = buttonObject.AddComponent<LayoutElement>();
        buttonLayout.preferredHeight = 78f;

        TextMeshProUGUI label = AddText(buttonObject.transform, text, CleanFontSize, TitleColor, 78f);
        label.rectTransform.localScale = Vector3.one * SmallTextScale;

        RectTransform textRect = label.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(58f, 0f);
        textRect.offsetMax = Vector2.zero;

        LayoutElement textLayout = label.GetComponent<LayoutElement>();
        if (textLayout != null)
            Destroy(textLayout);

        AddButtonIcon(buttonObject.transform);

        return buttonObject;
    }

    private void AddVolumeControl(Transform parent)
    {
        GameObject groupObject = new GameObject("Volumen");
        groupObject.transform.SetParent(parent, false);

        VerticalLayoutGroup groupLayout = groupObject.AddComponent<VerticalLayoutGroup>();
        groupLayout.spacing = 14f;
        groupLayout.childAlignment = TextAnchor.MiddleCenter;
        groupLayout.childControlWidth = true;
        groupLayout.childControlHeight = true;
        groupLayout.childForceExpandWidth = true;
        groupLayout.childForceExpandHeight = false;

        LayoutElement groupElement = groupObject.AddComponent<LayoutElement>();
        groupElement.preferredHeight = 120f;

        AddVolumeTitle(groupObject.transform);

        GameObject sliderObject = new GameObject("MusicVolumeSlider");
        sliderObject.transform.SetParent(groupObject.transform, false);

        Image background = sliderObject.AddComponent<Image>();
        background.color = new Color(0.04f, 0.04f, 0.04f, 0.78f);
        AddShadow(sliderObject, new Vector2(3f, -3f), new Color(0f, 0f, 0f, 0.48f));

        volumeSlider = sliderObject.AddComponent<Slider>();
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = PlayerPrefs.GetFloat(MusicVolumePrefKey, 1f);
        volumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        LayoutElement sliderLayout = sliderObject.AddComponent<LayoutElement>();
        sliderLayout.preferredHeight = 40f;

        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(520f, 40f);

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObject.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f, 0.32f);
        fillAreaRect.anchorMax = new Vector2(1f, 0.68f);
        fillAreaRect.offsetMin = new Vector2(14f, 0f);
        fillAreaRect.offsetMax = new Vector2(-14f, 0f);

        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(fillArea.transform, false);
        Image fill = fillObject.AddComponent<Image>();
        fill.color = TitleColor;
        RectTransform fillRect = fill.rectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObject.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(14f, 0f);
        handleAreaRect.offsetMax = new Vector2(-14f, 0f);

        GameObject handleObject = new GameObject("Handle");
        handleObject.transform.SetParent(handleArea.transform, false);
        Image handle = handleObject.AddComponent<Image>();
        handle.color = TextColor;
        AddShadow(handleObject, new Vector2(3f, -3f), ShadowColor);
        RectTransform handleRect = handle.rectTransform;
        handleRect.sizeDelta = new Vector2(34f, 34f);

        volumeSlider.fillRect = fillRect;
        volumeSlider.handleRect = handleRect;
        volumeSlider.targetGraphic = handle;
        volumeSlider.direction = Slider.Direction.LeftToRight;
    }

    private void CleanTextMaterial(TextMeshProUGUI text)
    {
        if (cleanTextMaterial == null)
            cleanTextMaterial = CreateCleanTextMaterial(text.fontSharedMaterial);

        text.outlineWidth = 0f;
        text.outlineColor = Color.clear;
        text.fontSharedMaterial = new Material(cleanTextMaterial);
    }

    private Material CreateCleanTextMaterial(Material sourceMaterial)
    {
        Material material = new Material(sourceMaterial);
        material.DisableKeyword("OUTLINE_ON");
        material.DisableKeyword("UNDERLAY_ON");

        if (material.HasProperty("_OutlineWidth"))
            material.SetFloat("_OutlineWidth", 0f);

        if (material.HasProperty("_FaceDilate"))
            material.SetFloat("_FaceDilate", 0f);

        if (material.HasProperty("_UnderlayColor"))
            material.SetColor("_UnderlayColor", Color.black);

        if (material.HasProperty("_OutlineColor"))
            material.SetColor("_OutlineColor", Color.black);

        return material;
    }

    private void AddVolumeTitle(Transform parent)
    {
        GameObject titleObject = new GameObject("VolumeTitle");
        titleObject.transform.SetParent(parent, false);

        HorizontalLayoutGroup titleLayout = titleObject.AddComponent<HorizontalLayoutGroup>();
        titleLayout.spacing = 12f;
        titleLayout.childAlignment = TextAnchor.MiddleCenter;
        titleLayout.childControlWidth = false;
        titleLayout.childControlHeight = true;
        titleLayout.childForceExpandWidth = false;
        titleLayout.childForceExpandHeight = false;

        LayoutElement titleElement = titleObject.AddComponent<LayoutElement>();
        titleElement.preferredHeight = 44f;

        TextMeshProUGUI icon = AddText(titleObject.transform, "♪", CleanFontSize, TitleColor, 44f);
        icon.rectTransform.localScale = Vector3.one * SmallTextScale;
        icon.rectTransform.sizeDelta = new Vector2(44f, 44f);

        TextMeshProUGUI label = AddText(titleObject.transform, "VOLUMEN", CleanFontSize, TitleColor, 44f);
        label.rectTransform.localScale = Vector3.one * SmallTextScale;
        label.rectTransform.sizeDelta = new Vector2(360f, 44f);
    }

    private void AddButtonIcon(Transform buttonTransform)
    {
        GameObject iconObject = new GameObject("Icon");
        iconObject.transform.SetParent(buttonTransform, false);

        TextMeshProUGUI icon = iconObject.AddComponent<TextMeshProUGUI>();
        icon.text = "◆";
        icon.font = menuFont;
        icon.fontSize = 28f;
        icon.fontStyle = FontStyles.Bold;
        icon.alignment = TextAlignmentOptions.Center;
        icon.color = TitleColor;
        icon.raycastTarget = false;
        CleanTextMaterial(icon);

        RectTransform iconRect = icon.rectTransform;
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = new Vector2(42f, 0f);
        iconRect.sizeDelta = new Vector2(42f, 42f);
    }

    private void AddShadow(GameObject target, Vector2 distance, Color color)
    {
        Shadow shadow = target.AddComponent<Shadow>();
        shadow.effectDistance = distance;
        shadow.effectColor = color;
        shadow.useGraphicAlpha = true;
    }

    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(MusicVolumePrefKey, value);
        PlayerPrefs.Save();
        RefreshMusicSources();
        ApplyMusicVolume(value);
    }

    private void RefreshMusicSources()
    {
        musicSources.Clear();
        baseMusicVolumes.Clear();

        AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in sources)
        {
            if (!IsMusicSource(source))
                continue;

            int sourceId = source.GetInstanceID();
            if (!baseMusicVolumeBySource.ContainsKey(sourceId))
                baseMusicVolumeBySource[sourceId] = source.volume;

            musicSources.Add(source);
            baseMusicVolumes.Add(baseMusicVolumeBySource[sourceId]);
        }
    }

    private bool IsMusicSource(AudioSource source)
    {
        Transform current = source.transform;

        while (current != null)
        {
            if (current.name.ToLowerInvariant().Contains("cancion"))
                return true;

            current = current.parent;
        }

        return source.loop && source.playOnAwake && source.clip != null;
    }

    private void ApplyMusicVolume(float volume)
    {
        for (int i = 0; i < musicSources.Count; i++)
        {
            if (musicSources[i] != null)
                musicSources[i].volume = baseMusicVolumes[i] * volume;
        }
    }

    private void EnsureEventSystem()
    {
        if (EventSystem.current != null)
            return;

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }
}
