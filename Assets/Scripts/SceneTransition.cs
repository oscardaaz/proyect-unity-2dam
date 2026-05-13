using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private float fadeDuration = 1f;

    private Image fadeImage;
    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateFadeCanvas();
    }

    public static void LoadScene(string sceneName)
    {
        EnsureInstance();

        if (Instance == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        Instance.StartTransition(sceneName);
    }

    public static void LoadScene(int sceneBuildIndex)
    {
        EnsureInstance();

        if (Instance == null)
        {
            SceneManager.LoadScene(sceneBuildIndex);
            return;
        }

        Instance.StartTransition(sceneBuildIndex);
    }

    private static void EnsureInstance()
    {
        if (Instance != null)
            return;

        GameObject transitionObject = new GameObject("SceneTransition");
        transitionObject.AddComponent<SceneTransition>();
    }

    private void StartTransition(string sceneName)
    {
        if (!isTransitioning)
            StartCoroutine(LoadSceneWithFade(sceneName));
    }

    private void StartTransition(int sceneBuildIndex)
    {
        if (!isTransitioning)
            StartCoroutine(LoadSceneWithFade(sceneBuildIndex));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        isTransitioning = true;

        yield return Fade(0f, 1f);
        SceneManager.LoadScene(sceneName);
        yield return null;
        yield return Fade(1f, 0f);

        isTransitioning = false;
    }

    private IEnumerator LoadSceneWithFade(int sceneBuildIndex)
    {
        isTransitioning = true;

        yield return Fade(0f, 1f);
        SceneManager.LoadScene(sceneBuildIndex);
        yield return null;
        yield return Fade(1f, 0f);

        isTransitioning = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }

    private void CreateFadeCanvas()
    {
        GameObject canvasObject = new GameObject("SceneTransitionCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject imageObject = new GameObject("FadeImage");
        imageObject.transform.SetParent(canvasObject.transform, false);

        fadeImage = imageObject.AddComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.raycastTarget = false;

        RectTransform rectTransform = fadeImage.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
