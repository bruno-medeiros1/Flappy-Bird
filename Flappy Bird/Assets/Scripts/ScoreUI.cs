using UnityEngine.UI;
using UnityEngine;
using System;

public class ScoreUI : MonoBehaviour
{
    private Text score_txt;
    private Text highscore_txt;
    private void Awake()
    {

        score_txt = transform.Find("ScoreText").GetComponent<Text>();//referencia do nosso UI
        highscore_txt = transform.Find("HighScoreTXT").GetComponent<Text>();
    }
    private void Start()
    {
        Hide();
        highscore_txt.text = "HIGHSCORE: " + Score.GetHighScore().ToString();
        Bird.GetInstance().OnDied += DiedEvent;
        Bird.GetInstance().OnStartedPlaying += StartedPlaying;
    }

    private void StartedPlaying(object sender, EventArgs e)
    {
        Show();
    }

    private void DiedEvent(object sender, EventArgs e)
    {
        Hide();
    }

    private void Update()
    {
        score_txt.text = Level.GetInstance().GetScore().ToString();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
}

