using System;
using UnityEngine;

public class WaitingToStartUI : MonoBehaviour
{

    void Start()
    {
        Bird.GetInstance().OnStartedPlaying += StartedPlay;
    }

    private void StartedPlay(object sender, EventArgs e)
    {
        Hide();
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
