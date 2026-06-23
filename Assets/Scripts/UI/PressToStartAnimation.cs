using TMPro;
using DG.Tweening;
using UnityEngine;

public class PressToStartAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI startText;

    private void Start()
    {
        startText.DOFade(0.2f, 0.8f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}