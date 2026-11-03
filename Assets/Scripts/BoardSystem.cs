using System;
using System.Collections.Generic;

/// <summary>
/// 보드 위의 한 칸에 놓인 재료 더미(쌓인 햄버거).
/// 숫자(Count) = 쌓인 재료 개수. 재료 1개를 생성하면 1, 합칠수록 2, 4, 8 ... 로 커진다.
/// </summary>
public sealed class Tile
{
    public int Id { get; }
    public List<Ingredient> Stack { get; } = new List<Ingredient>();   // [0] = 맨 아래
    public int Count => Stack.Count;
    /// <summary>얼린 패티(RawPatty)가 들어 있으면 다른 더미와 합쳐지지 않는다.</summary>
    public bool HasRawPatty => Stack.Contains(Ingredient.RawPatty);

    public Tile(int id, Ingredient ingredient)
    {
        Id = id;
        Stack.Add(ingredient);
    }
}

/// <summary>한 번의 슬라이드에서 타일 하나가 어디로 갔는지.</summary>
public struct TileMove
{
    public Tile Tile;
    public int ToRow;
    public int ToCol;
    public Tile MergedInto;   // null이 아니면 이 타일은 도착 후 MergedInto에 흡수되어 사라짐
}

public sealed class MoveResult
{
    public bool Changed;
    public readonly List<TileMove> Moves = new List<TileMove>();
    public readonly List<Tile> MergedTiles = new List<Tile>();
    public readonly List<Tile> CookedTiles = new List<Tile>();   // 그릴 줄에 도착해 패티가 익은 더미
}

/// <summary>
/// Unity에 의존하지 않는 순수 게임 로직.
/// - 재료 버튼 → Spawn: 빈 칸 중 랜덤 위치에 재료 1개 생성 (빈 칸이 없으면 생성 불가)
/// - 슬라이드 → Move: 2048 규칙. 재료 종류와 상관없이 숫자(재료 개수)가 같으면 합쳐짐.
///   한 번 합쳐진 타일은 같은 턴에 다시 합쳐지지 않음. 슬라이드 후 새 재료는 생기지 않음.
/// - 패티: 꺼낼 때는 항상 얼린 패티(RawPatty). 얼린 패티는 합쳐지지 않고,
///   4행(맨 아래 줄, GrillRow)에 놓이면 구운 패티(CookedPatty)로 바뀐 뒤부터 정상적으로 합쳐짐.
/// - 제출: 가장 높이 쌓인 더미 1개 (GetBestStack). 점수 계산은 ScoreRules.
/// </summary>
public sealed class BoardSystem
{
    public int Size { get; }

    /// <summary>패티가 구워지는 줄 = 4행(맨 아래 줄).</summary>
    public int GrillRow => Size - 1;

    /// <summary>
    /// 제출할 더미: 가장 높은 더미. 높이가 같으면 오른쪽 위 칸에 가장 가까운 것,
    /// 거리까지 같으면 더 위쪽 줄. 빈 보드면 null.
    /// </summary>
    public Tile GetBestStack()
    {
        Tile best = null;
        double bestDist = double.MaxValue;
        int bestRow = int.MaxValue;
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
            {
                var t = grid[r, c];
                if (t == null) continue;
                int dr = r, dc = Size - 1 - c;
                double dist = Math.Sqrt(dr * dr + dc * dc);
                bool better = best == null
                    || t.Count > best.Count
                    || (t.Count == best.Count && (dist < bestDist - 1e-6
                        || (Math.Abs(dist - bestDist) <= 1e-6 && r < bestRow)));
                if (better)
                {
                    best = t;
                    bestDist = dist;
                    bestRow = r;
                }
            }
        return best;
    }

    readonly Tile[,] grid;   // [row, col], row 0 = 맨 위
    readonly Random random;
    int nextId = 1;

    public BoardSystem(int size = 4, Random random = null)
    {
        Size = size;
        grid = new Tile[size, size];
        this.random = random ?? new Random();
    }

    public Tile Get(int row, int col) => grid[row, col];

    public void Reset()
    {
        Array.Clear(grid, 0, grid.Length);
        nextId = 1;
    }

    public bool HasEmptyCell
    {
        get
        {
            foreach (var t in grid)
                if (t == null) return true;
            return false;
        }
    }

    /// <summary>빈 칸 중 랜덤 위치에 재료를 생성한다. 빈 칸이 없으면 false.</summary>
    public bool TrySpawn(Ingredient ingredient, out Tile tile, out int row, out int col)
        => TrySpawn(ingredient, out tile, out row, out col, out _);

    /// <summary>
    /// 빈 칸 중 랜덤 위치에 재료를 생성한다. 패티는 항상 얼린 패티로 나오고,
    /// 그릴 줄에 생성되면 바로 구워진다(cooked = true).
    /// </summary>
    public bool TrySpawn(Ingredient ingredient, out Tile tile, out int row, out int col, out bool cooked)
    {
        cooked = false;
        if (ingredient == Ingredient.CookedPatty) ingredient = Ingredient.RawPatty;

        var empties = new List<(int r, int c)>();
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                if (grid[r, c] == null) empties.Add((r, c));

        if (empties.Count == 0)
        {
            tile = null; row = col = -1;
            return false;
        }

        (row, col) = empties[random.Next(empties.Count)];
        tile = new Tile(nextId++, ingredient);
        grid[row, col] = tile;
        cooked = CookIfOnGrill(tile, row);
        return true;
    }

    /// <summary>그릴 줄에 있는 더미의 얼린 패티를 모두 구운 패티로 바꾼다. 바뀐 게 있으면 true.</summary>
    bool CookIfOnGrill(Tile tile, int row)
    {
        if (row != GrillRow || !tile.HasRawPatty) return false;
        for (int i = 0; i < tile.Stack.Count; i++)
            if (tile.Stack[i] == Ingredient.RawPatty) tile.Stack[i] = Ingredient.CookedPatty;
        return true;
    }

    static bool CanMerge(Tile a, Tile b) => a.Count == b.Count && !a.HasRawPatty && !b.HasRawPatty;

    /// <summary>테스트/디버그용: 특정 칸에 직접 타일을 놓는다.</summary>
    public Tile Place(int row, int col, params Ingredient[] stack)
    {
        var tile = new Tile(nextId++, stack[0]);
        for (int i = 1; i < stack.Length; i++) tile.Stack.Add(stack[i]);
        grid[row, col] = tile;
        return tile;
    }

    public MoveResult Move(Direction dir)
    {
        var result = new MoveResult();
        var newGrid = new Tile[Size, Size];

        for (int line = 0; line < Size; line++)
        {
            // 타일이 몰려가는 쪽 끝부터 순서대로 좌표를 나열
            var coords = new (int r, int c)[Size];
            for (int i = 0; i < Size; i++)
                coords[i] = LineCoord(dir, line, i);

            int target = -1;            // newGrid에 마지막으로 놓인 위치(coords 인덱스)
            bool targetMerged = false;  // 그 타일이 이번 턴에 이미 합쳐졌는지

            for (int i = 0; i < Size; i++)
            {
                var (r, c) = coords[i];
                var tile = grid[r, c];
                if (tile == null) continue;

                if (target >= 0 && !targetMerged)
                {
                    var (tr, tc) = coords[target];
                    var dest = newGrid[tr, tc];
                    if (CanMerge(dest, tile))
                    {
                        // 도착 칸에 있던 더미 위에 슬라이드해 온 더미를 얹는다
                        dest.Stack.AddRange(tile.Stack);
                        targetMerged = true;
                        result.Moves.Add(new TileMove { Tile = tile, ToRow = tr, ToCol = tc, MergedInto = dest });
                        result.MergedTiles.Add(dest);
                        result.Changed = true;
                        continue;
                    }
                }

                target++;
                targetMerged = false;
                var (nr, nc) = coords[target];
                newGrid[nr, nc] = tile;
                result.Moves.Add(new TileMove { Tile = tile, ToRow = nr, ToCol = nc });
                if (nr != r || nc != c) result.Changed = true;
            }
        }

        if (result.Changed)
        {
            Array.Copy(newGrid, grid, grid.Length);
            // 이동이 끝난 뒤 그릴 줄(4행)에 도착한 얼린 패티를 굽는다. 합쳐지는 건 다음 슬라이드부터.
            for (int c = 0; c < Size; c++)
                if (grid[GrillRow, c] is Tile t && CookIfOnGrill(t, GrillRow))
                    result.CookedTiles.Add(t);
        }
        return result;
    }

    (int r, int c) LineCoord(Direction dir, int line, int i)
    {
        switch (dir)
        {
            case Direction.Left: return (line, i);
            case Direction.Right: return (line, Size - 1 - i);
            case Direction.Up: return (i, line);
            default: return (Size - 1 - i, line);   // Down
        }
    }
}
