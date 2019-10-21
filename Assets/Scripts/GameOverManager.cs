using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject restart;
    [SerializeField] GameObject textrestart;
    [SerializeField] GameObject textgameover;

    public void GameOver()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(GameOverAnimation());
    }

    IEnumerator GameOverAnimation()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i <= 50; i++)
        {
            float a = (float)i / 50;
            Color _color = panel.GetComponent<Image>().color;
            _color.a = a;
            panel.GetComponent<Image>().color = _color;

            _color = restart.GetComponent<Image>().color;
            _color.a = a;
            restart.GetComponent<Image>().color = _color;

            _color = textrestart.GetComponent<Text>().color;
            _color.a = a;
            textrestart.GetComponent<Text>().color = _color;

            _color = textgameover.GetComponent<Text>().color;
            _color.a = a;
            textgameover.GetComponent<Text>().color = _color;

            yield return null;
        }
    }

    public void GameClear(float time)
    {
        this.gameObject.SetActive(true);
        textgameover.GetComponent<Text>().text = "Game Clear\nTime : " + ((int)time / 60).ToString() + "m " + (time % 60).ToString("f1") + "s";
        StartCoroutine(GameClearAnimation(time));

        
    }

    IEnumerator GameClearAnimation(float time)
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i <= 50; i++)
        {
            float a = (float)i / 50;
            Color _color = panel.GetComponent<Image>().color;
            _color.a = a;
            panel.GetComponent<Image>().color = _color;

            _color = restart.GetComponent<Image>().color;
            _color.a = a;
            restart.GetComponent<Image>().color = _color;

            _color = textrestart.GetComponent<Text>().color;
            _color.a = a;
            textrestart.GetComponent<Text>().color = _color;

            _color = textgameover.GetComponent<Text>().color;
            _color.a = a;
            textgameover.GetComponent<Text>().color = _color;

            yield return null;
        }

        var millsec = (int)(time * 1000);
        var timeScore = new System.TimeSpan(0, 0, 0, 0, millsec);
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(timeScore);
    }
}
