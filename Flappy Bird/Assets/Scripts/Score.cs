using System;
using UnityEngine;

public static class Score
{
    /*Como a nossa Classe é static temos de primeiro Inicializar esta funcao no GameManager para assim
     podermo nos subscrever ao evento*/
    public static void Start()
    {
        Bird.GetInstance().OnDied += OnBirdDied;
    }

    private static void OnBirdDied(object sender, EventArgs e)
    {
        TrySetNewHighScore(Level.GetInstance().GetScore());
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore");
    }
    public static bool TrySetNewHighScore(int score)
    {
        int current_score = GetHighScore();
        if (score > current_score)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
            return true;
        }
        else
        {
            return false;
        }
    }
}
