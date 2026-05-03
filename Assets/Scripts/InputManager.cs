using UnityEngine;

public class InputManager : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        // 동일 오브젝트에 부착된 GameManager 컴포넌트 참조 연결
        gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        // 키보드 연결 상태 예외 처리
        if (UnityEngine.InputSystem.Keyboard.current == null) return;

        // 이동 입력 처리 (방향키)
        if (UnityEngine.InputSystem.Keyboard.current.rightArrowKey.wasPressedThisFrame) gameManager.OnMoveInput("Right");
        if (UnityEngine.InputSystem.Keyboard.current.leftArrowKey.wasPressedThisFrame) gameManager.OnMoveInput("Left");
        if (UnityEngine.InputSystem.Keyboard.current.upArrowKey.wasPressedThisFrame) gameManager.OnMoveInput("Up");
        if (UnityEngine.InputSystem.Keyboard.current.downArrowKey.wasPressedThisFrame) gameManager.OnMoveInput("Down");

        // 제출 입력 처리 (Space)
        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame) gameManager.OnSubmitInput();

        // 초기화 및 재시작 입력 처리 (R)
        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame) gameManager.OnResetInput();
    }
}