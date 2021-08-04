using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    [Tooltip("������")] public int laneID;

    [Tooltip("������ʹ�õļ��̰���")] public KeyCode keyboardButton;

    //����Ч��λ��
    public Transform targetVisual;
    
    //���±߽�
    public Transform targetTop;
    public Transform targetBottom;
    private GameManager gameController;

    [Tooltip("��Ӧ�����е�ȫ���¼�")] private List<KoreographyEvent> laneEvents;

    private int pendingEventIdx;

    //��������쵱ǰ���������������Ķ���
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

        //������Ч����
        while (trackedNotes.Count > 0 && trackedNotes.Peek().IsMissed())
        {
            trackedNotes.Dequeue();
            gameController.LevelAnimator(0);
            gameController.comboNum = 0;
            gameController.HideComboText();
        }
        //����������Ĳ���
        CheckSpawnNext();
        //�����ҵ�����
        if (Input.GetKeyDown(keyboardButton))
        {

            ReturnEffect(gameController.hitEffectPool,gameController.hitEffectGo);
            CheckNoteHit();
            //��ʾ����Ч��
            downEffectGo.SetActive(true);
        }
        else if (Input.GetKey(keyboardButton))
        {
            
        }
        else if (Input.GetKeyUp(keyboardButton))
        {
            //���ذ���Ч��
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
    //����������sampleƫ����
    private int GetSpawnSampleOffset()
    {
        //����λ����Ŀ���ľ���
        var spawnDistToTarget = targetTop.position.y - transform.position.y;

        //����Ŀ�����¼�
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
    //����Ƿ��л�����������
    //����ǣ�����ִ�����в�ɾ��
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
                    //��������
                    gameController.UpdateScoreText(100000 * (((int)hitLevel * 3 + 1) * 0.1f));
                    //������Ч������������Ч
                    CreateHitEffect();
                    //������+1
                    gameController.comboNum++;
                }
                else
                {
                    //������=0
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
