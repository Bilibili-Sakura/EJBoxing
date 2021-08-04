using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSet : MonoBehaviour
{
    [Tooltip("箭头")]
    public List<Image> chooseImg = new List<Image>();
    public Image tempImg;
    public int index;

    [Tooltip("0  音量滚动条\n1   速度滚动条")]
    public List<Slider> sliderList = new List<Slider>();

    float setTime;
    float soundValue;
    float noteSpeed;

    public static GameSet instance;

    private void Awake()
    {
        soundValue = PlayerPrefs.GetFloat("SoundValue");
        noteSpeed = PlayerPrefs.GetFloat("SpeedValue") / 15f;
    }

    private void Start()
    {
        instance = this;

        index = 0;
        sliderList[0].value = soundValue;
        sliderList[1].value = noteSpeed;
    }

    void Update()
    {
        setTime += Time.deltaTime;

        if (setTime >= 0.2f)
            UpdateChoose();
    }

    void UpdateSoundSlider()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            soundValue -= 0.1f;
            setTime = 0;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            soundValue += 0.1f;
            setTime = 0;
        }

        sliderList[0].value = soundValue;
    }

    void UpdateSpeedSlider()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            noteSpeed -= 0.05f;
            setTime = 0;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            noteSpeed += 0.05f;
            setTime = 0;
        }

        sliderList[1].value = noteSpeed;
    }

    void UpdateChoose()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && index != sliderList.Count - 1)
        {
            index++;

            tempImg.sprite = chooseImg[index].sprite;
            chooseImg[index].sprite = chooseImg[index - 1].sprite;
            chooseImg[index].color = new Color(1, 1, 1, 1);
            chooseImg[index - 1].sprite = tempImg.sprite;
            chooseImg[index - 1].color = new Color(1, 1, 1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && index != 0)
        {
            index--;

            tempImg.sprite = chooseImg[index].sprite;
            chooseImg[index].sprite = chooseImg[index + 1].sprite;
            chooseImg[index].color = new Color(1, 1, 1, 1);
            chooseImg[index + 1].sprite = tempImg.sprite;
            chooseImg[index + 1].color = new Color(1, 1, 1, 0);
        }

        ConfirmChoose();
    }

    void ConfirmChoose()
    {
        switch(index)
        {
            case 0:
                {
                    UpdateSoundSlider();
                    return;
                }
            case 1:
                {
                    UpdateSpeedSlider();
                    return;
                }
        }    
    }

    public void UpdateSoundValue()
    {
        PlayerPrefs.SetFloat("SoundValue", soundValue);
    }
    public void UpdateSpeedValue()
    {
        PlayerPrefs.SetFloat("SpeedValue", noteSpeed * 15f);
    }
}
