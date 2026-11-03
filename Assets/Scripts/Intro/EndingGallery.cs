using UnityEngine;
using UnityEngine.UI;

public class EndingGallery : MonoBehaviour
{
    public Image[] endingSprites;

    void Awake()
    {

        Debug.Log("캬옹");
        RefreshIntroEndings();
    }

    public void RefreshIntroEndings()
    {
        for (int i = 0; i < endingSprites.Length; i++)
        {
            Debug.Log($"this is {i}th result :{PlayerPrefs.GetInt("SeenEnding: " + i, 0)}");
            if (endingSprites[i] != null)
            {
                Debug.Log($"관문 1 통과 with {i}");
                int hasSeen = PlayerPrefs.GetInt("SeenEnding: " + i, 0);

                if (hasSeen == 1) endingSprites[i].gameObject.SetActive(true);
                else endingSprites[i].gameObject.SetActive(false);
            }
        }
    }
}
