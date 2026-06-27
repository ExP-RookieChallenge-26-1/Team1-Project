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

    [Header("���� �����ι�")]
    public Sprite endingBody;

    [Header("���� ���")]
    [TextArea(2, 4)]
    public string[] dialogues;
}

public class EndingManager : MonoBehaviour
{
    public static EndingManager Inst;
    [SerializeField] private List<EndingConfig> endingConfigs;

    private void Awake() { Inst = this; }

    public void TriggerEnding(int score)
    {
        PlayEnding(score);
    }

    public void PlayEnding(int finalScore)
    {
        EndingTier tier = GetTier(finalScore);
        EndingConfig cfg = endingConfigs.Find(x => x.tier == tier);
        if (cfg == null) return;

        CustomerStateManager.Inst.ShowEndingVIP(cfg.endingBody);

        AdvancedDialogue.Inst.StartEndingDialogue(cfg.dialogues);
    }

    private EndingTier GetTier(int score)
    {
        if (score >= 550) return EndingTier.Perfect;
        if (score >= 400) return EndingTier.Good;
        if (score >= 250) return EndingTier.Normal;
        return EndingTier.Bad;
    }
}