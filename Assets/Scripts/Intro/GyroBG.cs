using TMPro;
using UnityEngine;

public class GyroBG : MonoBehaviour
{
    public RectTransform target;

    public float moveAmount = 100f;

    void Start()
    {
        parent = target.parent as RectTransform;

        if (GravitySensor.current != null)
        {
            InputSystem.EnableDevice(GravitySensor.current);
        }
    }

    void Update()
    {
        float tilt = Input.gyro.gravity.x;

        Vector2 pos = target.anchoredPosition;
        pos.x = tilt * moveAmount;

        target.anchoredPosition = pos;
    }
}