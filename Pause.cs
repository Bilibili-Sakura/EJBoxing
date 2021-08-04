using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public bool isPause = false; 

    public GameObject pauseBG;

    public static Pause instan;

    private void Awake()
    {
        instan = this;
    }

    public void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = true;
            GameManager.instan.player.Pause();
            pauseBG.SetActive(true);
            
        }
    }

    public void ReGamePlay()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = false;
            GameManager.instan.player.Play();
            pauseBG.SetActive(false);
        }   
    }

 
}
