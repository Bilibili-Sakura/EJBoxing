using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    [Tooltip("音轨编号")] public int laneID;

    [Tooltip("此音轨使用的键盘按键")] public KeyCode keyboardButton;

    //按键效果位置
    public Transform targetVisual;
    
    //上下边界
    public Transform targetTop;
    public Transform targetBottom;
    private GameManager gameController;

    [Tooltip("对应音轨中的全部事件")] private List<KoreographyEvent> laneEvents;

    private int pendingEventIdx;

    //保存此音轨当前活动的所有音符对象的队列
    private readonly Queue<NoteObject> trackedNotes = new Queue<NoteObject>();

    public GameObject downEffectGo;

    public Vector3 TargetPosition => transform.position;
    void Start()
    {

    }

    void Update()
    {
        if (Pause.instan.isPause)
            return;

        //清理无效音符
        while (trackedNotes.Count > 0 && trackedNotes.Peek().IsMissed())
        {
            trackedNotes.Dequeue();
            gameController.LevelAnimator(0);
            gameController.comboNum = 0;
            gameController.HideComboText();
        }
        //检测新音符的产生
        CheckSpawnNext();
        //检测玩家的输入
        if (Input.GetKeyDown(keyboardButton))
        {

            ReturnEffect(gameController.hitEffectPool,gameController.hitEffectGo);
            CheckNoteHit();
            //显示按键效果
            downEffectGo.SetActive(true);
        }
        else if (Input.GetKey(keyboardButton))
        {
            
        }
        else if (Input.GetKeyUp(keyboardButton))
        {
            //隐藏按键效果
            downEffectGo.SetActive(false);
        }

    }
    public void Initialize(GameManager controller)
    {
        gameController = controller;
        laneEvents = new List<KoreographyEvent>();
    }
    public bool IsMatch(int noteID)
    {
        return noteID == laneID;
    }

    public void AddEventToLane(KoreographyEvent evt)
    {
        laneEvents.Add(evt);
    }
    //音符产生的sample偏移量
    private int GetSpawnSampleOffset()
    {
        //出生位置与目标点的距离
        var spawnDistToTarget = targetTop.position.y - transform.position.y;

        //到达目标点的事件
        double spawnPosToTargetTime = spawnDistToTarget / gameController.noteSpeed;

        return (int)spawnPosToTargetTime * gameController.SampleRate;
    }
    private void CheckSpawnNext()
    {
        var sampleToTarget = GetSpawnSampleOffset();

        var currentTime = gameController.DelayedSampleTime;

        while (pendingEventIdx < laneEvents.Count
               && laneEvents[pendingEventIdx].StartSample < currentTime + sampleToTarget)
        {
            var evt = laneEvents[pendingEventIdx];
            var noteID = evt.GetIntValue();
            var newNote = gameController.GetFreshNoteObject();
            bool isLongStart = noteID > 4;

            newNote.Initialize(evt, noteID, this, gameController, isLongStart);
            trackedNotes.Enqueue(newNote);
            pendingEventIdx++;
        }
    }
    //检测是否有击中音符对象
    //如果是，它将执行命中并删除
    public void CheckNoteHit()
    {
        if (Pause.instan.isPause)
            return;

        if (trackedNotes.Count > 0)
        {
            var noteObject = trackedNotes.Peek();
            if (noteObject.hitOffset < 2 * gameController.HitWindowSampleWidth)
            {
                trackedNotes.Dequeue();
                var hitLevel = noteObject.IsNoteHittable();
                gameController.LevelAnimator((int)hitLevel);

                if (hitLevel != NoteObject.Levels.Miss)
                {
                    //分数更新
                    gameController.UpdateScoreText(100000 * (((int)hitLevel * 3 + 1) * 0.1f));
                    //按键特效和命中音符特效
                    CreateHitEffect();
                    //连击数+1
                    gameController.comboNum++;
                }
                else
                {
                    //连击数=0
                    gameController.comboNum = 0;
                    gameController.HideComboText();
                }

                noteObject.OnNoteHit();
            }
        }

    }

    void ReturnEffect(Stack<GameObject> stack,GameObject effectGo)
    {
        gameController.ReturnEffectGoToPool(stack, effectGo);
    }

    void CreateHitEffect()
    {
        GameObject hitEffectGo = gameController.GetFreshEffectObject(gameController.hitEffectPool,
            gameController.hitEffectGo);
        hitEffectGo.transform.position = targetVisual.position;
    }

}
