using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;


public class SpecialBase : MonoBehaviour
{
    public virtual void StartAnimation()
    {
        StartCoroutine(AnimationRoutine());
    }

    public virtual void UpdateEmotion(CustomerEmotion emotion)
    {
        
    }

    public virtual void HideAnimation()
    {
        
    }
    
    public virtual IEnumerator AnimationRoutine()
    {
        yield return null;
    }
}
