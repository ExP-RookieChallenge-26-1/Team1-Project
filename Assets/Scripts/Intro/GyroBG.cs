using UnityEngine;

public class GyroBG : MonoBehaviour
{
    public RectTransform target;

    public float moveAmount = 100f;

    void Start()
    {
        Input.gyro.enabled = true;
    }

    void Update()
    {
        float tilt = Input.gyro.gravity.x;

        Vector2 pos = target.anchoredPosition;
        pos.x = tilt * moveAmount;

        target.anchoredPosition = pos;
    }
}