using UnityEngine;

public class SafeArea : MonoBehaviour
{
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        Vector2 minAnchor = safeArea.position;
        Vector2 maxAnchor = safeArea.position + safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;

        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;
    }
}