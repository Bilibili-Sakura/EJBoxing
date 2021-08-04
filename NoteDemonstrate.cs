using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDemonstrate : MonoBehaviour
{
    public GameObject demonstrateNote;

    public GameObject demonstrateStart;
    public GameObject demonstrateEnd;


    float deltatime;
    float time;

    float noteSpeed;

    private void Update()
    {
        noteSpeed = PlayerPrefs.GetFloat("SpeedValue");
        UpdatePos();
    }

    void UpdatePos()
    {
        deltatime += Time.deltaTime;
        if (demonstrateNote.transform.position == demonstrateEnd.transform.position && deltatime>=2)
        {
            time = 0;
            deltatime = 0;
            demonstrateNote.transform.position = demonstrateStart.transform.position;
            demonstrateNote.SetActive(true);
        }


        time += (Time.deltaTime * 0.2f) * noteSpeed;
        demonstrateNote.transform.position = Vector2.Lerp(demonstrateStart.transform.position, demonstrateEnd.transform.position, time);

        if (demonstrateNote.transform.position == demonstrateEnd.transform.position)
            demonstrateNote.SetActive(false);
    }
}
