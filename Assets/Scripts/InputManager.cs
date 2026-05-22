using UnityEngine;

public class InputManager : MonoBehaviour
{
    private GameManager gameManager;

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
        if (UnityEngine.InputSystem.Keyboard.current.rightArrowKey.wasPressedThisFrame) gameManager.OnMoveInput("Right");
        if (UnityEngine.InputSystem.Keyboard.current.leftArrowKey.wasPressedThisFrame) gameManager.OnMoveInput("Left");
        if (UnityEngine.InputSystem.Keyboard.current.upArrowKey.wasPressedThisFrame) gameManager.OnMoveInput("Up");
        if (UnityEngine.InputSystem.Keyboard.current.downArrowKey.wasPressedThisFrame) gameManager.OnMoveInput("Down");

        // ?쒖텧 ?낅젰 泥섎━ (Space)
        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame || UnityEngine.InputSystem.Keyboard.current.tabKey.wasPressedThisFrame) gameManager.OnSubmitInput();

        // 珥덇린??諛??ъ떆???낅젰 泥섎━ (R)
        //if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame) gameManager.OnResetInput();
    }
}