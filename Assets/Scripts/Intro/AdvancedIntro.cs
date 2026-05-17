using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdvancedIntro : MonoBehaviour
{
    public NeonSignEffect neonSignEffect;
    public Image fadeImg;
    public AudioSource bellAudio;

    private void Awake()
    {
        neonSignEffect = GetComponent<NeonSignEffect>();
    }

    public void OnClickOpen()
    {
        StartCoroutine(CorAnim());
    }

    IEnumerator CorAnim()
    {
        neonSignEffect.Play();
        yield return new WaitForSeconds(2f);

        bellAudio.Play();

        fadeImg.DOFade(1, 2.5f).OnComplete(() =>
        {
            GoToGameScene();
        });
    }

    void GoToGameScene()
    {
        SceneManager.LoadScene("Main");
        if (PausedController.Inst)
        {
            PausedController.Inst.fullFadeImg.gameObject.SetActive(true);
            PausedController.Inst.fullFadeImg.color = Color.black;
            PausedController.Inst.fullFadeImg.DOFade(0, 1f).SetDelay(1.5f).OnComplete(() =>
            {
                PausedController.Inst.fullFadeImg.gameObject.SetActive(false);
            });
        }
    }
}
