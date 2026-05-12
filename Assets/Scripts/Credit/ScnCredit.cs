using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScnCredit : MonoBehaviour
{
    private static bool _loaded;
    private static CreditData _creditData;
    public TextMeshProUGUI creditTxt;
    public RectTransform creditContainer;

    private bool _canExit;

    private void Awake()
    {
        if(!_loaded)
            LoadData();
        
        ShowCredit();
    }

    void LoadData()
    {
        var json = Resources.Load<TextAsset>("CreditData").text;
        Debug.Log(json);
        _creditData = JsonUtility.FromJson<CreditData>(json);
        _loaded = true;
    }

    void ShowCredit()
    {
        if(_creditData == null)
            return;

        creditTxt.text = "";
        foreach (var partData in _creditData.parts)
        {
            string nameTxt = "";
            foreach (var playerName in partData.names)
            {
                nameTxt += playerName + "\n";
            }
            var txt = $"<color=#C3C3C3>{partData.partName}</color>\n<size=100><b>{nameTxt}</b></size>\n\n\n";
            creditTxt.text += txt;
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(creditContainer);
        StartCoroutine(WaitForRebuild());
    }

    //레이아웃 리빌드 이후 몇 프레임 뒤에 height가 적용됨
    IEnumerator WaitForRebuild()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        var height = creditContainer.rect.height;
        creditContainer.anchoredPosition3D = new Vector3(0, -height, 0);
        creditContainer.DOAnchorPos3DY(height, 30).SetEase(Ease.Linear).OnComplete(() =>
        {
            SceneManager.LoadScene("Intro");
        });
        yield return new WaitForSeconds(3);
        _canExit = true;
    }

    private void Update()
    {
        if (_canExit)
        {
            bool isTouchPressed = false;
            bool isMousePressed = false;

            if (Touchscreen.current != null)
            {
                isTouchPressed = Touchscreen.current.primaryTouch.press.isPressed;
            }

            if (Mouse.current != null)
            {
                isMousePressed = Mouse.current.leftButton.isPressed;
            }
            
            if(isTouchPressed || isMousePressed)
            {
                SceneManager.LoadScene("Intro");
            }
        }
    }
}
