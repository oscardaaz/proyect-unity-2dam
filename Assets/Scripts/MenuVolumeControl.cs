using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuVolumeControl : MonoBehaviour
{
    private const string MusicVolumePrefKey = "MusicVolume";
    private static readonly Color TitleColor = new Color(0.95f, 0.72f, 0.28f, 1f);
    private static readonly Color TextColor = new Color(0.95f, 0.88f, 0.72f, 1f);
    private static readonly Color SliderBackgroundColor = new Color(0.04f, 0.04f, 0.04f, 0.82f);

    private readonly List<AudioSource> musicSources = new List<AudioSource>();
    private readonly List<float> baseMusicVolumes = new List<float>();
    private readonly Dictionary<int, float> baseMusicVolumeBySource = new Dictionary<int, float>();

    private Slider volumeSlider;
    private VolumeLayout layoutSettings;

    private struct VolumeLayout
    {
        public float YOffset;
        public Vector2 Size;
        public float LabelFontSize;
        public float LabelHeight;
        public Vector2 SliderSize;
        public Vector2 HandleSize;

        public VolumeLayout(float yOffset, Vector2 size, float labelFontSize, float labelHeight, Vector2 sliderSize, Vector2 handleSize)
        {
            YOffset = yOffset;
            Size = size;
            LabelFontSize = labelFontSize;
            LabelHeight = labelHeight;
            SliderSize = sliderSize;
            HandleSize = handleSize;
        }
    }

    public static void CreateBelowButtons(MonoBehaviour owner, params string[] buttonNames)
    {
        CreateBelowButtons(
            owner,
            buttonNames,
            new VolumeLayout(-62f, new Vector2(260f, 42f), 28f, 28f, new Vector2(340f, 24f), new Vector2(22f, 22f)));
    }

    public static void CreateBelowButtons(MonoBehaviour owner, string[] buttonNames, float yOffset, Vector2 size, float labelFontSize, float labelHeight, Vector2 sliderSize, Vector2 handleSize)
    {
        CreateBelowButtons(
            owner,
            buttonNames,
            new VolumeLayout(yOffset, size, labelFontSize, labelHeight, sliderSize, handleSize));
    }

    private static void CreateBelowButtons(MonoBehaviour owner, string[] buttonNames, VolumeLayout settings)
    {
        if (owner == null || FindObjectsByType<MenuVolumeControl>(FindObjectsSortMode.None).Length > 0)
            return;

        RectTransform lowestButton = FindLowestButton(buttonNames);
        if (lowestButton == null || lowestButton.parent == null)
            return;

        GameObject volumeObject = new GameObject("MenuVolumeControl", typeof(RectTransform));
        volumeObject.transform.SetParent(lowestButton.parent, false);

        MenuVolumeControl control = volumeObject.AddComponent<MenuVolumeControl>();
        control.layoutSettings = settings;
        control.Build(lowestButton);
    }

    private static RectTransform FindLowestButton(string[] buttonNames)
    {
        RectTransform lowestButton = null;
        RectTransform[] rectTransforms = FindObjectsByType<RectTransform>(FindObjectsSortMode.None);

        foreach (string buttonName in buttonNames)
        {
            foreach (RectTransform rectTransform in rectTransforms)
            {
                if (rectTransform.name != buttonName)
                    continue;

                if (lowestButton == null || rectTransform.anchoredPosition.y < lowestButton.anchoredPosition.y)
                    lowestButton = rectTransform;
            }
        }

        return lowestButton;
    }

    private void Awake()
    {
        EnsureEventSystem();
    }

    private void Start()
    {
        RefreshMusicSources();
        ApplyMusicVolume(PlayerPrefs.GetFloat(MusicVolumePrefKey, 1f));
    }

    private void Build(RectTransform lowestButton)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchorMin = lowestButton.anchorMin;
        rect.anchorMax = lowestButton.anchorMax;
        rect.pivot = lowestButton.pivot;
        rect.anchoredPosition = lowestButton.anchoredPosition + new Vector2(0f, layoutSettings.YOffset);
        rect.sizeDelta = layoutSettings.Size;
        rect.localScale = Vector3.one;

        VerticalLayoutGroup layout = gameObject.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 4f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        TMP_FontAsset font = FindMenuFont(lowestButton);
        AddVolumeTitle(transform, font);
        AddSlider(transform);
    }

    private TMP_FontAsset FindMenuFont(RectTransform reference)
    {
        TextMeshProUGUI text = reference.root.GetComponentInChildren<TextMeshProUGUI>(true);
        return text != null ? text.font : Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
    }

    private void AddVolumeTitle(Transform parent, TMP_FontAsset font)
    {
        GameObject titleObject = new GameObject("VolumeTitle");
        titleObject.transform.SetParent(parent, false);

        TextMeshProUGUI label = titleObject.AddComponent<TextMeshProUGUI>();
        label.text = "VOLUMEN";
        label.font = font;
        label.fontSize = layoutSettings.LabelFontSize;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = TitleColor;
        label.raycastTarget = false;
        CleanTextMaterial(label);

        LayoutElement layoutElement = titleObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = layoutSettings.LabelHeight;
    }

    private void AddSlider(Transform parent)
    {
        GameObject sliderObject = new GameObject("MusicVolumeSlider");
        sliderObject.transform.SetParent(parent, false);

        Image background = sliderObject.AddComponent<Image>();
        background.color = SliderBackgroundColor;

        volumeSlider = sliderObject.AddComponent<Slider>();
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = PlayerPrefs.GetFloat(MusicVolumePrefKey, 1f);
        volumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        LayoutElement sliderLayout = sliderObject.AddComponent<LayoutElement>();
        sliderLayout.preferredHeight = layoutSettings.SliderSize.y;

        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.sizeDelta = layoutSettings.SliderSize;

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
        RectTransform handleRect = handle.rectTransform;
        handleRect.sizeDelta = layoutSettings.HandleSize;

        volumeSlider.fillRect = fillRect;
        volumeSlider.handleRect = handleRect;
        volumeSlider.targetGraphic = handle;
        volumeSlider.direction = Slider.Direction.LeftToRight;
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
            string objectName = current.name.ToLowerInvariant();
            if (objectName.Contains("cancion") || objectName.Contains("music") || objectName.Contains("musica"))
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

    private void CleanTextMaterial(TextMeshProUGUI text)
    {
        Material material = new Material(text.fontSharedMaterial);
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

        text.outlineWidth = 0f;
        text.outlineColor = Color.clear;
        text.fontSharedMaterial = material;
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
