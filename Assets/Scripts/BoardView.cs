using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 4x4 보드 그리기. 정사각형 RectTransform(보드 패널)에 붙인다.
/// 배경 칸 16개는 Start에서 자동 생성되고, 타일은 BoardSystem 결과에 맞춰 생성/이동/삭제된다.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class BoardView : MonoBehaviour
{
    [SerializeField] float gap = 20f;
    [Tooltip("재료 한 칸 더미가 이동하는 시간(초)")]
    [SerializeField] float slideDuration = 0.1f;
    [Tooltip("스와이프 시 아래 줄부터 위 줄로 한 줄씩 늦게 출발하는 간격(초). 0이면 모두 동시에 이동")]
    [SerializeField] float rowDelay = 0.03f;
    [SerializeField] Color cellColor = new Color32(0xD9, 0xA5, 0x6C, 0xFF);
    [SerializeField] Font font;
    [Tooltip("칸/타일 모양 스프라이트 (9-slice). 비워두면 사각형")]
    [SerializeField] Sprite cellSprite;

    [Header("재료 그림")]
    [Tooltip("재료 한 장 프리팹 (기존 Prefabs/IngredientUI). 비워두면 기본 모양으로 만든다")]
    [SerializeField] GameObject ingredientUIPrefab;
    [SerializeField] Sprite spriteFrozenPatty;
    [SerializeField] Sprite spriteBakedPatty;
    [SerializeField] Sprite spriteCheese;
    [SerializeField] Sprite spriteOnion;
    [SerializeField] Sprite spriteLettuce;
    [SerializeField] Sprite spriteTomato;

    readonly Dictionary<int, SpriteView> views = new Dictionary<int, SpriteView>();
    RectTransform rect;
    RectTransform tileLayer;
    int size;

    public bool IsAnimating { get; private set; }
    public RectTransform Rect => rect;

    float CellSize => (rect.rect.width - gap * (size + 1)) / size;

    public void Build(int boardSize)
    {
        size = boardSize;
        rect = (RectTransform)transform;
        if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 칸/타일 위치는 BoardView가 직접 계산한다. 레이아웃 그룹이 붙어 있으면 위치를 덮어쓰므로 끈다.
        foreach (var layoutGroup in GetComponents<LayoutGroup>())
        {
            Debug.LogWarning($"[BoardView] {name}의 {layoutGroup.GetType().Name}을(를) 껐습니다. 보드 오브젝트에서 지워주세요.", this);
            layoutGroup.enabled = false;
        }

        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
            {
                var cell = new GameObject($"Cell {r},{c}", typeof(RectTransform), typeof(Image));
                cell.transform.SetParent(transform, false);
                var rt = (RectTransform)cell.transform;
                rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
                rt.sizeDelta = new Vector2(CellSize, CellSize);
                rt.anchoredPosition = CellPosition(r, c);
                var img = cell.GetComponent<Image>();
                img.color = cellColor;
                img.raycastTarget = false;
                SpriteView.ApplySprite(img, cellSprite);
            }

        // 타일은 배경 칸 위에 그려지도록 별도 레이어에 둔다
        var layer = new GameObject("Tiles", typeof(RectTransform));
        layer.transform.SetParent(transform, false);
        tileLayer = (RectTransform)layer.transform;
        tileLayer.anchorMin = Vector2.zero;
        tileLayer.anchorMax = Vector2.one;
        tileLayer.offsetMin = tileLayer.offsetMax = Vector2.zero;
    }

    Vector2 CellPosition(int row, int col)
    {
        float cell = CellSize;
        return new Vector2(gap + col * (cell + gap) + cell / 2, -(gap + row * (cell + gap) + cell / 2));
    }

    public Sprite GetIngredientSprite(Ingredient ingredient)
    {
        switch (ingredient)
        {
            case Ingredient.RawPatty: return spriteFrozenPatty;
            case Ingredient.CookedPatty: return spriteBakedPatty;
            case Ingredient.Cheese: return spriteCheese;
            case Ingredient.Onion: return spriteOnion;
            case Ingredient.Lettuce: return spriteLettuce;
            case Ingredient.Tomato: return spriteTomato;
            default: return null;
        }
    }

    // 화면 위치 → 보드 줄 번호 (0 = 맨 위)
    int RowAt(Vector2 anchoredPosition)
    {
        float cell = CellSize;
        int row = Mathf.RoundToInt((-anchoredPosition.y - gap - cell / 2) / (cell + gap));
        return Mathf.Clamp(row, 0, size - 1);
    }

    public void Clear()
    {
        StopAllCoroutines();
        foreach (var v in views.Values) Destroy(v.gameObject);
        views.Clear();
        IsAnimating = false;
    }

    public void AddTile(Tile tile, int row, int col)
    {
        var view = SpriteView.Create(tileLayer, ingredientUIPrefab, GetIngredientSprite, font, CellSize);
        view.Rect.anchoredPosition = CellPosition(row, col);
        view.Refresh(tile);
        view.PlaySpawn();
        views[tile.Id] = view;
    }

    public void PlayMove(MoveResult result, System.Action onComplete)
    {
        StartCoroutine(MoveRoutine(result, onComplete));
    }

    IEnumerator MoveRoutine(MoveResult result, System.Action onComplete)
    {
        IsAnimating = true;

        var starts = new List<Vector2>();
        var ends = new List<Vector2>();
        var delays = new List<float>();
        foreach (var m in result.Moves)
        {
            var start = views[m.Tile.Id].Rect.anchoredPosition;
            starts.Add(start);
            ends.Add(CellPosition(m.ToRow, m.ToCol));
            // 맨 아래 줄이 먼저 출발하고, 한 줄 위로 갈수록 rowDelay만큼 늦게 출발한다
            delays.Add((size - 1 - RowAt(start)) * rowDelay);
            // 흡수될 타일은 아래로 깔리도록
            if (m.MergedInto != null) views[m.Tile.Id].transform.SetAsFirstSibling();
        }

        float total = slideDuration + (size - 1) * rowDelay;
        for (float t = 0; t < total; t += Time.deltaTime)
        {
            for (int i = 0; i < result.Moves.Count; i++)
            {
                float k = Mathf.Clamp01((t - delays[i]) / slideDuration);
                k = 1f - (1f - k) * (1f - k) * (1f - k);   // 빠르게 출발해서 칸에 착 붙는 느낌 (ease-out cubic)
                views[result.Moves[i].Tile.Id].Rect.anchoredPosition = Vector2.Lerp(starts[i], ends[i], k);
            }
            yield return null;
        }

        for (int i = 0; i < result.Moves.Count; i++)
        {
            var m = result.Moves[i];
            var view = views[m.Tile.Id];
            view.Rect.anchoredPosition = ends[i];
            if (m.MergedInto != null)
            {
                Destroy(view.gameObject);
                views.Remove(m.Tile.Id);
            }
        }

        foreach (var merged in result.MergedTiles)
        {
            var view = views[merged.Id];
            view.Refresh(merged);
            view.PlayMerge();
        }

        // 그릴 줄(4행)에 도착해 얼린 패티가 구운 패티로 바뀐 더미
        foreach (var cooked in result.CookedTiles)
        {
            if (!views.TryGetValue(cooked.Id, out var view)) continue;
            view.Refresh(cooked);
            view.PlayMerge();
        }

        IsAnimating = false;
        onComplete?.Invoke();
    }
}
