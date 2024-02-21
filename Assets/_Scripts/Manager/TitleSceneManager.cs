using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    public Sprite[] guide;
    public int guideIndex;
    public GameObject guideUI;
    public GameObject guideSprite;

    public void GameStart()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void GameOver()
    {
        Application.Quit();
    }

    public void GuideUIOpen()
    {
        guideUI.SetActive(true);
    }

    public void GuideUINext()
    {
        if (guideIndex < guide.Length)
            guideIndex++;
        else
            return;

        guideSprite.GetComponent<Image>().sprite = guide[guideIndex];
    }

    public void GuideUIClose()
    {
        guideUI.SetActive(false);
        guideIndex = 0;
        guideSprite.GetComponent<Image>().sprite = guide[guideIndex];
    }
}
