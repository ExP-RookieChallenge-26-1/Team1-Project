using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;

    private void Awake()
    {
        Inst = this;
    }

    //뒤집개를 충분히 드래그해서, 조리탭이 올라왔을때 호출됩니다
    public void OnTongEndDrag()
    {
        //여기부터 게임 로직이 시작되면 될 것 같아요
        Debug.Log("OnTongEndDrag");
    }
}
