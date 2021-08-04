using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{    public void PlayMusicGame()
    {
        SceneManager.LoadScene("MusicChoose");
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void ReturnChoose()
    {
        SceneManager.LoadScene("MusicChoose");
    }

    public void GameSetGo()
    {
        SceneManager.LoadScene("GameSet");
    }
    
    public void ReturnMain()
    {
        SceneManager.LoadScene("Main");
    }
}
