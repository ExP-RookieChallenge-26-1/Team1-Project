using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class AdvancedTong : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rect;
    private Vector2 _startPos;

    private bool _isTweening;
    
    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _startPos = _rect.anchoredPosition;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (_isTweening) return;
        
        _rect.anchoredPosition = _rect.anchoredPosition3D.AddY(eventData.delta.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float currentY = _rect.anchoredPosition.y;
        _isTweening = true;
        if (currentY < Screen.height/2.3f)
        {
            _rect.DOAnchorPos(_startPos, 0.3f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => _isTweening = false);
        }
        else
        {
            Vector2 targetPos = new Vector2(_startPos.x, Screen.height + _rect.sizeDelta.y);

            _rect.DOAnchorPos(targetPos, 0.3f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    _isTweening = false;
                    MainUIManager.Inst.OpenGameView();
                });
            _rect.transform.DOScale(Vector3.zero, 0.3f).SetDelay(0.1f);
        }
    }
}
