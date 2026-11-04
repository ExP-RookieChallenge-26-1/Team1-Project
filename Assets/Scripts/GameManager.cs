using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보드 게임 연결: 재료 버튼 → 생성, 스와이프 → 이동, 버튼/카운터 미리보기 갱신.
/// 손님 흐름(AdvancedMain)이 BeginRound로 시작하고 TakeBestStack으로 제출받는다.
/// 손님 흐름이 없는 씬에서는 바로 플레이 가능 상태로 시작한다 (보드 단독 테스트용).
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    [SerializeField] int boardSize = 4;
    [SerializeField] BoardView boardView;
    [SerializeField] SwipeInput swipeInput;
    [SerializeField] IngredientButton[] ingredientButtons;
    [Tooltip("씬에 AdvancedMain이 없으면 시작하자마자 플레이 가능 상태로 둔다 (보드 단독 테스트용)")]
    [SerializeField] bool autoStartWithoutFlow = true;

    /// <summary>재료 버튼으로 재료가 보드에 생성됐을 때. 손님 제한시간 시작에 사용.</summary>
    public event Action<Ingredient> IngredientSpawned;

    public BoardSystem Model { get; private set; }
    /// <summary>재료 생성/슬라이드가 가능한 상태인지 (손님 응대 중).</summary>
    public bool IsPlaying { get; private set; }

    void Awake()
    {
        Inst = this;
        Model = new BoardSystem(boardSize);
    }

    void Start()
    {
        boardView.Build(boardSize);
        swipeInput.Swiped += OnSwiped;
        if (autoStartWithoutFlow && AdvancedMain.Inst == null) BeginRound();
        else RefreshState();
    }

    void OnDestroy()
    {
        if (swipeInput != null) swipeInput.Swiped -= OnSwiped;
        if (Inst == this) Inst = null;
    }

    /// <summary>새 손님 응대 시작: 보드를 비우고 입력을 연다.</summary>
    public void BeginRound()
    {
        ClearBoard();
        IsPlaying = true;
        RefreshState();
    }

    /// <summary>입력을 막는다 (제출 연출, 시간 초과 등). 보드는 그대로 둔다.</summary>
    public void EndRound()
    {
        IsPlaying = false;
        RefreshState();
    }

    /// <summary>
    /// 지금 제출하면 나갈 버거 (가장 높은 더미, 동점이면 오른쪽 위에 가까운 것). 보드는 그대로.
    /// 얼린 패티는 버거에 들어가지 않는다 (미리보기/쌓이는 연출/요구사항 판정 모두 제외).
    /// </summary>
    public List<Ingredient> PeekBestStack()
    {
        var best = Model.GetBestStack();
        var burger = best == null ? new List<Ingredient>() : new List<Ingredient>(best.Stack);
        burger.RemoveAll(i => i == Ingredient.RawPatty);
        return burger;
    }

    /// <summary>제출: 가장 높은 더미를 꺼내고 보드를 전부 비운다 (다음 손님 준비).</summary>
    public List<Ingredient> TakeBestStack()
    {
        var stack = PeekBestStack();
        ClearBoard();
        RefreshState();
        return stack;
    }

    public void SpawnIngredient(Ingredient ingredient)
    {
        if (!IsPlaying || boardView.IsAnimating) return;
        // 패티 버튼은 항상 얼린 패티(RawPatty)를 꺼낸다. 4행에 생성되면 바로 구워진다.
        if (!Model.TrySpawn(ingredient, out var tile, out int row, out int col, out bool cooked)) return;   // 빈 칸 없음
        boardView.AddTile(tile, row, col);
        if (cooked) PlayCookedSfx();
        RefreshState();
        IngredientSpawned?.Invoke(ingredient);
    }

    void OnSwiped(Direction dir)
    {
        if (!IsPlaying || boardView.IsAnimating) return;
        var result = Model.Move(dir);
        if (!result.Changed) return;
        if (result.CookedTiles.Count > 0) PlayCookedSfx();
        // 슬라이드 후에는 새 재료를 만들지 않는다
        boardView.PlayMove(result, RefreshState);
    }

    // 패티가 구워질 때 효과음 (기존 AdvancedMain.cookedClip 사용, 손님 흐름이 있는 씬에서만)
    void PlayCookedSfx()
    {
        if (AdvancedMain.Inst != null && AdvancedMain.Inst.cookedClip != null && SFXPlayer.Instance != null)
            SFXPlayer.Instance.Play(AdvancedMain.Inst.cookedClip);
    }

    void ClearBoard()
    {
        Model.Reset();
        boardView.Clear();
    }

    void RefreshState()
    {
        bool canSpawn = IsPlaying && Model.HasEmptyCell;
        foreach (var b in ingredientButtons)
            if (b != null && b.Button != null) b.Button.interactable = canSpawn;

        UpdateCounterPreview();
    }

    // 카운터 위에 "지금 제출하면 나갈 버거"를 미리 보여준다 (SideBurgerMaker가 있는 씬에서만).
    // 플레이 중에만 갱신한다: 제출 연출(FallingRoutine)이 이 미리보기를 그대로 사용하기 때문.
    void UpdateCounterPreview()
    {
        if (SideBurgerMaker.Inst == null || !IsPlaying) return;
        var stack = PeekBestStack();
        if (stack.Count == 0) SideBurgerMaker.Inst.ClearPreview();
        else SideBurgerMaker.Inst.Make(stack, isChat: false);
    }
}
