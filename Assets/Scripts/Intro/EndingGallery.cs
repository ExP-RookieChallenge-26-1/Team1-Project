using UnityEngine;
using UnityEngine.UI;

public class EndingGallery : MonoBehaviour
{
    public Image[] endingSprites;

    void Start()
    {
        RefreshIntroEndings();
    }

    public void RefreshIntroEndings()
    {
        for (int i = 0; i < endingSprites.Length; i++)
        {
            if (endingSprites[i] != null) continue;

            int hasSeen = PlayerPrefs.GetInt("SeenEnding: " + 1, 0);

            if (hasSeen == 1) endingSprites[i].gameObject.SetActive(true);
            else endingSprites[i].gameObject.SetActive(false);
        }
    }
}
