using UnityEngine;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class InputManager : MonoBehaviour
{
    private GameManager gameManager;
    private Vector2 startPos;
    [SerializeField] private float minSwipeDistance = 50;

    void Start()
    {
        // ?숈씪 ?ㅻ툕?앺듃??遺李⑸맂 GameManager 而댄룷?뚰듃 李몄“ ?곌껐
        gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        // ?ㅻ낫???곌껐 ?곹깭 ?덉쇅 泥섎━
        if (UnityEngine.InputSystem.Keyboard.current == null) return;

        // ?대룞 ?낅젰 泥섎━ (諛⑺뼢??
        if (UnityEngine.InputSystem.Keyboard.current.rightArrowKey.wasPressedThisFrame || CheckSlide() == "Right") gameManager.OnMoveInput("Right");
        if (UnityEngine.InputSystem.Keyboard.current.leftArrowKey.wasPressedThisFrame || CheckSlide() == "Left") gameManager.OnMoveInput("Left");
        if (UnityEngine.InputSystem.Keyboard.current.upArrowKey.wasPressedThisFrame || CheckSlide() == "Up") gameManager.OnMoveInput("Up");
        if (UnityEngine.InputSystem.Keyboard.current.downArrowKey.wasPressedThisFrame || CheckSlide() == "Down") gameManager.OnMoveInput("Down");
        

        // 珥덇린??諛??ъ떆???낅젰 泥섎━ (R)
        //if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame) gameManager.OnResetInput();
    }
    
    private string CheckSlide()
    {
        // 화면에 활성화된 터치가 없다면 중단
        if (Touch.activeTouches.Count == 0)
            return null;

        // 가장 먼저 시작된 첫 번째 터치(primaryTouch 역할)를 가져옵니다.
        var touch = Touch.activeTouches[0];

        // 1. 손가락이 화면에 딱 닿은 순간 (wasPressedThisFrame 대치)
        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            startPos = touch.screenPosition;
        }

        // 2. 손가락이 화면에서 떨어진 순간 (wasReleasedThisFrame 대치)
        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended || 
            touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            Vector2 endPos = touch.screenPosition;
            Vector2 delta = endPos - startPos;

            // 설정한 최소 거리보다 짧게 움직였다면 무시
            if (delta.magnitude < minSwipeDistance)
                return null;

            // X축 이동량이 Y축 이동량보다 크면 가로 슬라이드
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x > 0 ? "Right" : "Left";
            }
            // 그 외에는 세로 슬라이드
            else
            {
                return delta.y > 0 ? "Up" : "Down";
            }
        }

        return null;
    }
}