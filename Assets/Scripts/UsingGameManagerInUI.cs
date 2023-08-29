using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingGameManagerInUI : MonoBehaviour
{
    public void Restart()
    {
        GameManager.instance.RestartGame();
    }

    public void PlayGame()
    {
        GameManager.instance.SetPlay();
    }

    public void PauseGame()
    {
        GameManager.instance.SetPause();
    }
}
