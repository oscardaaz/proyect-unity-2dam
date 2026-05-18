using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudLayoutController : MonoBehaviour
{
    private const float Padding = 18f;
    private const float RowSpacing = 8f;
    private const float ItemSpacing = 24f;
    private const float IconTextSpacing = 6f;

    private void Start()
    {
        BuildStableLayout();
    }

    private void BuildStableLayout()
    {
        RectTransform root = transform as RectTransform;
        if (root == null)
            return;

        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.pivot = new Vector2(0.5f, 0.5f);
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;
        root.localScale = Vector3.one;

        RectTransform block = CreateLayoutObject("HudBlock", root);
        block.anchorMin = new Vector2(0f, 1f);
        block.anchorMax = new Vector2(0f, 1f);
        block.pivot = new Vector2(0f, 1f);
        block.anchoredPosition = new Vector2(Padding, -Padding);

        VerticalLayoutGroup blockLayout = block.gameObject.AddComponent<VerticalLayoutGroup>();
        blockLayout.spacing = RowSpacing;
        blockLayout.childAlignment = TextAnchor.UpperLeft;
        blockLayout.childControlWidth = true;
        blockLayout.childControlHeight = true;
        blockLayout.childForceExpandWidth = false;
        blockLayout.childForceExpandHeight = false;

        ContentSizeFitter blockFitter = block.gameObject.AddComponent<ContentSizeFitter>();
        blockFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        blockFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        RectTransform statsRow = CreateLayoutObject("StatsRow", block);
        AddHorizontalLayout(statsRow, ItemSpacing, TextAnchor.MiddleLeft);

        RectTransform coinGroup = CreateLayoutObject("CoinGroup", statsRow);
        AddHorizontalLayout(coinGroup, IconTextSpacing, TextAnchor.MiddleLeft);

        RectTransform diamondGroup = CreateLayoutObject("DiamondGroup", statsRow);
        AddHorizontalLayout(diamondGroup, IconTextSpacing, TextAnchor.MiddleLeft);

        RectTransform heartsRow = CreateLayoutObject("HeartsRow", block);
        AddHorizontalLayout(heartsRow, 2f, TextAnchor.MiddleLeft);

        RectTransform bossRow = CreateLayoutObject("BossRow", block);
        AddHorizontalLayout(bossRow, 6f, TextAnchor.MiddleLeft);

        MoveToGroup(FindRect("Image", "ImageCoin"), coinGroup, new Vector2(32f, 32f));
        MoveToGroup(FindRect("TextCoin"), coinGroup, new Vector2(72f, 36f));

        MoveToGroup(FindRect("Imagediamond", "ImageDiamond"), diamondGroup, new Vector2(32f, 32f));
        MoveToGroup(FindRect("TextDiamond"), diamondGroup, new Vector2(92f, 36f));

        MoveToGroup(FindRect("Heart1"), heartsRow, new Vector2(32f, 32f));
        MoveToGroup(FindRect("Heart2"), heartsRow, new Vector2(32f, 32f));
        MoveToGroup(FindRect("Heart3"), heartsRow, new Vector2(32f, 32f));

        MoveToGroup(FindRect("BossIcon"), bossRow, new Vector2(40f, 40f));
        MoveToGroup(FindRect("BossLife1"), bossRow, new Vector2(30f, 30f));
        MoveToGroup(FindRect("BossLife2"), bossRow, new Vector2(30f, 30f));
        MoveToGroup(FindRect("BossLife3"), bossRow, new Vector2(30f, 30f));
        MoveToGroup(FindRect("BossLife4"), bossRow, new Vector2(30f, 30f));
        MoveToGroup(FindRect("BossLife5"), bossRow, new Vector2(30f, 30f));

        coinGroup.gameObject.SetActive(coinGroup.childCount > 0);
        diamondGroup.gameObject.SetActive(diamondGroup.childCount > 0);
        bossRow.gameObject.SetActive(bossRow.childCount > 0);
    }

    private RectTransform CreateLayoutObject(string objectName, RectTransform parent)
    {
        Transform existing = parent.Find(objectName);
        if (existing != null)
            Destroy(existing.gameObject);

        GameObject layoutObject = new GameObject(objectName, typeof(RectTransform));
        RectTransform rect = layoutObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.localScale = Vector3.one;
        return rect;
    }

    private void AddHorizontalLayout(RectTransform rect, float spacing, TextAnchor alignment)
    {
        HorizontalLayoutGroup layout = rect.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = spacing;
        layout.childAlignment = alignment;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
    }

    private RectTransform FindRect(params string[] names)
    {
        foreach (string objectName in names)
        {
            Transform found = transform.Find(objectName);
            if (found != null && found.TryGetComponent(out RectTransform rect))
                return rect;
        }

        return null;
    }

    private void MoveToGroup(RectTransform rect, RectTransform group, Vector2 size)
    {
        if (rect == null)
            return;

        rect.SetParent(group, false);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
        rect.localScale = Vector3.one;

        LayoutElement layoutElement = rect.GetComponent<LayoutElement>();
        if (layoutElement == null)
            layoutElement = rect.gameObject.AddComponent<LayoutElement>();

        layoutElement.preferredWidth = size.x;
        layoutElement.preferredHeight = size.y;

        if (rect.TryGetComponent(out Image image))
            image.preserveAspect = true;

        if (rect.TryGetComponent(out TextMeshProUGUI text))
        {
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Overflow;
        }
    }
}
