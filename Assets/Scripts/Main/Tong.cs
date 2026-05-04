using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Tong : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public MainBG mainBG;
    
    private RectTransform _rect;
    private Vector2 _startPos;

    private bool _isTweening = false;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isTweening) return;

        _startPos = _rect.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isTweening) return;

        float newY = _rect.anchoredPosition.y + eventData.delta.y;
        _rect.anchoredPosition = new Vector2(_startPos.x, newY);

        float bgNewY = mainBG.rect.anchoredPosition3D.y + eventData.delta.y;
        mainBG.rect.anchoredPosition3D = new Vector3(0, bgNewY, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float currentY = _rect.anchoredPosition.y;

        _isTweening = true;

        if (currentY < 820f)
        {
            // 원위치 복귀
            _rect.DOAnchorPos(_startPos, 0.3f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => _isTweening = false);

            mainBG.rect.DOAnchorPos3DY(0, 0.3f).SetEase(Ease.OutCubic);
        }
        else
        {
            // 열림 위치로 이동 (y = 3540)
            Vector2 targetPos = new Vector2(_startPos.x, 3540f);

            _rect.DOAnchorPos(targetPos, 0.3f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => _isTweening = false);
            
            mainBG.rect.DOAnchorPos3DY(1763, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                GameManager.Inst.OnTongEndDrag();
            });
        }
    }
}