using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타일(재료 더미) 하나의 화면 표시. 기존 프로젝트(VisualManager)와 같은 방식:
/// 더미의 재료 그림을 같은 자리에 아래([0])부터 차례로 겹쳐 놓고, 맨 위 재료에만 쌓인 개수를 표시한다.
/// </summary>
public class SpriteView : MonoBehaviour
{
    readonly List<GameObject> layers = new List<GameObject>();
    GameObject ingredientPrefab;
    Func<Ingredient, Sprite> spriteOf;
    Font font;
    float size;

    public RectTransform Rect { get; private set; }

    public static void ApplySprite(Image image, Sprite sprite)
    {
        if (sprite == null) return;
        image.sprite = sprite;
        image.type = Image.Type.Sliced;
        image.pixelsPerUnitMultiplier = 0.25f;   // 둥근 모서리를 크게
    }

    /// <param name="ingredientPrefab">재료 한 장 프리팹 (Image + 개수 글자). 비워두면 기본 모양으로 만든다.</param>
    public static SpriteView Create(Transform parent, GameObject ingredientPrefab, Func<Ingredient, Sprite> spriteOf, Font font, float size)
    {
        var go = new GameObject("Tile", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var view = go.AddComponent<SpriteView>();
        view.Rect = (RectTransform)go.transform;
        view.Rect.anchorMin = view.Rect.anchorMax = new Vector2(0, 1);
        view.Rect.pivot = new Vector2(0.5f, 0.5f);
        view.Rect.sizeDelta = new Vector2(size, size);
        view.ingredientPrefab = ingredientPrefab;
        view.spriteOf = spriteOf;
        view.font = font;
        view.size = size;
        return view;
    }

    public void Refresh(Tile tile)
    {
        foreach (var layer in layers) Destroy(layer);
        layers.Clear();

        // 같은 자리에 겹쳐 쌓고, 나중에 만든 재료(위쪽 재료)가 앞에 보인다
        for (int i = 0; i < tile.Count; i++)
        {
            var layer = ingredientPrefab != null ? Instantiate(ingredientPrefab, transform) : CreateDefaultLayer();
            var rt = (RectTransform)layer.transform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(size, size);
            rt.localScale = Vector3.one;
            foreach (var graphic in layer.GetComponentsInChildren<Graphic>(true)) graphic.raycastTarget = false;

            SetIngredientImage(layer, tile.Stack[i]);
            // 맨 위 재료에만 쌓인 개수를 표시한다
            SetStackText(layer, i == tile.Count - 1 ? tile.Count.ToString() : "");
            layer.transform.SetAsLastSibling();
            layers.Add(layer);
        }
    }

    void SetIngredientImage(GameObject layer, Ingredient ingredient)
    {
        var image = layer.GetComponent<Image>();
        if (image == null) return;
        var sprite = spriteOf?.Invoke(ingredient);
        image.sprite = sprite;
        image.preserveAspect = true;
        // 그림이 아직 연결되지 않았으면 재료 색 사각형으로 대신 보여준다
        image.color = sprite != null ? Color.white : BackColor(ingredient);
    }

    static void SetStackText(GameObject layer, string text)
    {
        var tmp = layer.GetComponentInChildren<TMP_Text>(true);
        if (tmp != null) { tmp.text = text; return; }
        var legacy = layer.GetComponentInChildren<Text>(true);
        if (legacy != null) legacy.text = text;
    }

    // 프리팹이 없을 때 쓰는 기본 모양: 재료 그림 + 왼쪽 위 개수
    GameObject CreateDefaultLayer()
    {
        var go = new GameObject("Ingredient", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);

        var label = new GameObject("StackText", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(go.transform, false);
        var rt = (RectTransform)label.transform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(size * 0.4f, size * 0.4f);
        var text = label.GetComponent<Text>();
        text.font = font;
        text.fontSize = Mathf.RoundToInt(size * 0.25f);
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color32(0xD6, 0x41, 0x3A, 0xFF);
        return go;
    }

    // 재료 그림이 없을 때의 대체 색
    static Color BackColor(Ingredient i)
    {
        switch (i)
        {
            case Ingredient.RawPatty: return new Color32(0x9C, 0xC9, 0xE8, 0xFF);   // 얼린 패티: 얼음색
            case Ingredient.CookedPatty: return new Color32(0x8B, 0x4A, 0x2B, 0xFF);
            case Ingredient.Tomato: return new Color32(0xE8, 0x47, 0x3B, 0xFF);
            case Ingredient.Cheese: return new Color32(0xF6, 0xC4, 0x31, 0xFF);
            case Ingredient.Lettuce: return new Color32(0x6C, 0xC0, 0x4A, 0xFF);
            default: return new Color32(0xC9, 0xA0, 0xDC, 0xFF);
        }
    }

    public void PlaySpawn() => StartCoroutine(ScaleAnim(0f, 1f, 1f, 0.15f));
    public void PlayMerge() => StartCoroutine(ScaleAnim(1f, 1.15f, 1f, 0.15f));

    IEnumerator ScaleAnim(float from, float peak, float to, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float k = t / duration;
            float s = k < 0.5f ? Mathf.Lerp(from, peak, k * 2) : Mathf.Lerp(peak, to, (k - 0.5f) * 2);
            transform.localScale = Vector3.one * s;
            yield return null;
        }
        transform.localScale = Vector3.one * to;
    }
}
