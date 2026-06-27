using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GyroBG : MonoBehaviour
{
    public RectTransform target;
    public TextMeshProUGUI debugTxt;

    [Header("Gyro Settings")]
    public float moveAmount = 100f;
    
    [Range(-1f, 1f)] // 에디터에서 슬라이더로 쉽게 테스트할 수 있게 변경
    public float debugTilt;

    private RectTransform parent;

    void Start()
    {
        // New Input System을 쓰므로 레거시 line은 주석 처리하거나 지워도 무방합니다.
        // Input.gyro.enabled = true; 

        parent = target.parent as RectTransform;

        if (GravitySensor.current != null)
        {
            InputSystem.EnableDevice(GravitySensor.current);
        }
    }

    void Update()
    {
        // 1. 센서 존재 여부 체크 및 값 할당
        float rawSensorX = 0f;
        if (GravitySensor.current != null)
        {
            rawSensorX = GravitySensor.current.gravity.ReadValue().x;
        }

        // 2. 플랫폼에 따른 tilt 값 결정
        float tilt = (Application.platform == RuntimePlatform.WindowsEditor ||
                      Application.platform == RuntimePlatform.WindowsPlayer)
            ? debugTilt
            : rawSensorX;

        // 3. 연산 과정 디버깅을 위한 변수 분리
        float desiredX = tilt * moveAmount;

        // 가용 범위 계산
        float targetWidth = target.rect.width;
        float parentWidth = parent.rect.width;
        float maxOffset = Mathf.Max(0, (targetWidth - parentWidth) * 0.5f);

        // 최종 고정값
        float clampedX = Mathf.Clamp(desiredX, -maxOffset, maxOffset);

        // 위치 적용
        Vector2 pos = target.anchoredPosition;
        pos.x = clampedX;
        target.anchoredPosition = pos;

        // 4. 정보가 한눈에 들어오도록 디버그 텍스트 확장
        if (debugTxt != null)
        {
            debugTxt.text = 
                $"[Platform] {Application.platform}\n" +
                $"[Sensor Active] {(GravitySensor.current != null ? "ON" : "OFF (Null)")}\n" +
                $"[Raw Sensor X] {rawSensorX:F3}\n" +
                "--------------------------------\n" +
                $"[Current Tilt] {tilt:F2}\n" +
                $"[Desired X] {desiredX:F1}\n" +
                $"[Max Offset] ±{maxOffset:F1}\n" +
                $"[Clamped Pos] {clampedX:F1}\n" +
                "--------------------------------\n" +
                $"[UI Size] Target: {targetWidth:F0} | Parent: {parentWidth:F0}";
        }
    }
}