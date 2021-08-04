using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("SoundValue"))
            PlayerPrefs.SetFloat("SoundValue", 1.0f);
        if (!PlayerPrefs.HasKey("SpeedValue"))
            PlayerPrefs.SetFloat("SpeedValue", 7.0f);
        if (!PlayerPrefs.HasKey("LastLevelNumInLevel"))
            PlayerPrefs.SetInt("LastLevelNumInLevel", 0);

    }

    private void Update()
    {
       
    }
}
