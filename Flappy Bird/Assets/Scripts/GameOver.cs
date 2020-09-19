using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameOver : MonoBehaviour
{
    /*Referencia ao nosso text Component do GameOverUI*/
    private Text scoretxt;

    private Text HighScoretxt;

    private void Awake()
    {
        scoretxt = transform.Find("ScoreTxt").GetComponent<Text>();
        HighScoretxt = transform.Find("HighScoretxt").GetComponent<Text>();
    }

    public void RestartGame()
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ReturnMain() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    private void Start()
    {
        //nao mostra a UI do gameOver
        Hide();

        //Subscrevemos ao evento
        Bird.GetInstance().OnDied += OnBirdDied;
    }

    private void OnBirdDied(object sender, EventArgs e)
    {
        scoretxt.text = Level.GetInstance().GetScore().ToString();

        if(Level.GetInstance().GetScore() >= Score.GetHighScore()) 
        {
            /* NEW HIGHSCCORE*/
            HighScoretxt.text = "NOVO HIGHSCORE";
        }
        else 
        {
            HighScoretxt.text = "HIGHSCORE: " + Score.GetHighScore();
        }
        Show();
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
