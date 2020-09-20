using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    public void Start()
    {
        Debug.Log("Gamemanager Started!");
        Score.Start();
    }

}
