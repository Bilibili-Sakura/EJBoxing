using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    //当前音轨上所有事件
    private KoreographyEvent trackedEvent;
    //渲染器
    public SpriteRenderer visuals;
    private LaneManager laneController;
    private GameManager gameController;
    private bool isLongNote;
    public Sprite[] noteSprites;
    //命中偏差,单位为sample
    public int hitOffset;
    public enum Levels
    {
        Miss,
        Good,
        Great,
        Perfect
    }
    
    public float perfectRange = 0.3f;
    public float greatRange = 0.7f;

    

    void Start()
    {
        
    }

    void Update()
    {
        if (Pause.instan.isPause)
            return;

        UpdatePosition();

        GetHitOffset();
        if (transform.position.y <= laneController.targetBottom.position.y)
        {
            ReturnToPool();
        }
    }
    //初始化音符
    public void Initialize(KoreographyEvent evt, int noteID, LaneManager lane, GameManager cont,
        bool isLong)
    {
        trackedEvent = evt;
        laneController = lane;
        gameController = cont;
        isLongNote = isLong;
        if (noteID%2==0)
        {
            GetComponent<SpriteRenderer>().color = new Color32(179, 236, 219,255);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color32(236, 204, 179,255);

        }
        visuals.sprite = isLongNote ? noteSprites[1] : noteSprites[0];
    }
    //重置音符
    private void ResetNote()
    {
        trackedEvent = null;
        gameController = null;
        laneController = null;
        GetComponent<Renderer>().enabled = false;
    }
    //将音符返回对象池
    void ReturnToPool()
    {
        gameController.ReturnNoteObjectToPool(this);
        ResetNote();

    }
    //击中音符的回调函数
    public void OnNoteHit()
    {
        ReturnToPool();
    }
    //更新位置的方法
    void UpdatePosition()
    {
        Vector3 pos = laneController.TargetPosition;
        if (gameController.DelayedSampleTime < trackedEvent.StartSample)
        {
            GetComponent<Renderer>().enabled = true;

        }
        pos.y -= (float)(gameController.DelayedSampleTime - trackedEvent.StartSample) / (float)gameController.SampleRate *
                 gameController.noteSpeed;

        transform.position = pos;
    }
    void GetHitOffset()
    {
        int curTime = gameController.DelayedSampleTime;
        int noteTime = trackedEvent.StartSample;
        int hitWindow = gameController.HitWindowSampleWidth;
        hitOffset = Mathf.Abs(noteTime - curTime);
    }

    //是否Miss
    public bool IsMissed()
    {
        bool bMissed = true;
        if (enabled)
        {
            bMissed = gameController.DelayedSampleTime - trackedEvent.StartSample > gameController.HitWindowSampleWidth;

            if (bMissed)
            {
                gameController.missCount++;
            }
        }

        return bMissed;
    }
    //音符的命中等级
    public Levels IsNoteHittable()
    {
        Levels hitLevel =Levels.Miss ;
        if (hitOffset <= gameController.HitWindowSampleWidth)
        {
            if (hitOffset <= perfectRange * gameController.HitWindowSampleWidth)
            {
                hitLevel = Levels.Perfect;
                gameController.perfectCount++;
            }
            else if (hitOffset<=greatRange*gameController.HitWindowSampleWidth)
            { 
                hitLevel = Levels.Great;
                gameController.greatCount++;
            }
            else
            {
                hitLevel = Levels.Good;
                gameController.goodCount++;
            }
        }
        else
        {
            this.enabled = false;
            gameController.missCount++;
        }
        //Debug.Log(hitLevel);
        return hitLevel;
    }
}
