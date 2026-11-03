using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EndingTier
{
    Perfect,
    Good,
    Normal,
    Bad
}

[System.Serializable]
public class EndingConfig
{
    public EndingTier tier;
    public string endingTitle;

    [Header("엔딩 손님 일러스트")]
    public Sprite endingBody;

    [Header("엔딩 대사")]
    [TextArea(2, 4)]
    public string[] dialogues;
}

public class EndingManager : MonoBehaviour
{
    public static EndingManager Inst;
    [SerializeField] private List<EndingConfig> endingConfigs;

    [Header("엔딩 기준 점수 (전체 누적 점수 이상이면 해당 엔딩)")]
    [Tooltip("새 점수 방식(더미 개수 × 100 × 2/0.5)에 맞춰 기획 값으로 바꿔야 합니다")]
    [SerializeField] private int perfectScore = 550;
    [SerializeField] private int goodScore = 400;
    [SerializeField] private int normalScore = 250;

    private void Awake() { Inst = this; }

    public void PlayEnding(int finalScore)
    {
        EndingTier tier = GetTier(finalScore);
        EndingConfig cfg = endingConfigs.Find(x => x.tier == tier);
        if (cfg == null) return;

        CustomerStateManager.Inst.ShowEndingVIP(cfg.endingBody);

        AdvancedDialogue.Inst.StartEndingDialogue(cfg.dialogues);
    }

    public EndingTier GetTier(int score)
    {
        if (score >= perfectScore) return EndingTier.Perfect;
        if (score >= goodScore) return EndingTier.Good;
        if (score >= normalScore) return EndingTier.Normal;
        return EndingTier.Bad;
    }
}
