using System.Collections.Generic;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //�ı���̬
    public static GameManager instan;

    //Combo�ı�
    public Text comboText;

    //Combo����
    public int comboNum;

    //Score�ı�
    public Text scoreText;

    //Score��
    public float score;
    //��hit����
    public float scorePerHit;
    //��λ����
    public int keyCount;

    //�����ٶ�
    public float noteSpeed = 1;

    /// <summary>
    ///     Ԥ������Դ
    /// </summary>
    //����
    public NoteObject noteObject;

    //���г�������Ч
    public GameObject hitLongEffectGo;

    //����������Ч
    public GameObject hitEffectGo;

    public List<LaneManager> noteLanes;
    public MultiMusicPlayer player;

    public Sprite[] levelSprite;
    public Animator levelAnimator;
    public GameObject levelEffectGo;

    //����
    [Tooltip("��ʼ������Ƶ֮ǰ�ṩ��ʱ����������Ϊ��λ��")] public float leadInTime;
    [Tooltip("����Ŀ�����ɵĹ�����¼�ID")] [EventID] public string eventID;
    
    [Tooltip("�������ж����䣬��λΪMs")] [Range(8f, 300f)]
    public float hitWindowRangeInMS = 100;

    //���������
    private readonly Stack<NoteObject> noteObjectsPool = new Stack<NoteObject>();
    public Stack<GameObject> hitEffectPool = new Stack<GameObject>();

    //��Ƶ����֮ǰ��ʣ��ʱ����
    private float leadInTimeLeft;

    //����
    private Koreography playingKoreo;

    //���ֿ�ʼ֮ǰ�ĵ���ʱ��
    private float timeLeftToPlay;

    //sample��λ�����д���
    public int HitWindowSampleWidth { get; private set; }

    public int SampleRate => playingKoreo.SampleRate;

    //��ǰ�Ĳ���ʱ�䣬�����κα�Ҫ���ӳ�
    public int DelayedSampleTime => playingKoreo.GetLatestSampleTime() - (int) (SampleRate * leadInTimeLeft);

    //�������
    public GameObject endingCanvas;//����UI
    public Text endingScoreText;//�ܷ�
    public Text rankText;//����
    public int exLevel=980000;//ex���������
    public int aaLevel=950000;//aa���������
    public int aLevel=900000;//a���������
    public int bLevel=800000;//b���������
    private float timer;
    public Text perfectText;//perfect
    public int perfectCount;
    public Text greatText;//great
    public int greatCount;

    public Text goodText;//good
    public int goodCount;

    public Text missText;//miss
    public int missCount;

    public Text maxComboText;//���������
    public int maxCombo;
    private void Awake()
    {
        instan = this;
        endingCanvas.SetActive(false);
    }

    private void Start()
    {
        InitializeLeadIn();
        for (var i = 0; i < noteLanes.Count; i++) noteLanes[i].Initialize(this);
        //��ȡKoreography����
        playingKoreo =
            Koreographer.Instance.GetKoreographyAtIndex(MusicChoose.Instance.musicDataList[MusicChoose.Instance.chooseNum].MusicID);
        eventID = MusicChoose.Instance.musicDataList[MusicChoose.Instance.chooseNum].Levels[MusicChoose.Instance.levelNum].TrackID;
        //��ȡ�¼��켣
        var rhythmTrack = playingKoreo.GetTrackByID(eventID);
        //��ȡ�¼�
        var rawEvents = rhythmTrack.GetAllEvents();
        scorePerHit = 1000000f / rawEvents.Count;
        for (var i = 0; i < rawEvents.Count; i++)
        {
            var evt = rawEvents[i];
            var noteID = evt.GetIntValue();

            //������������
            for (var j = 0; j < noteLanes.Count; j++)
            {
                var lane = noteLanes[j];
                if (noteID > 4) noteID -= 4;

                if (lane.IsMatch(noteID))
                {
                    lane.AddEventToLane(evt);
                    break;
                }
            }
        }

        HitWindowSampleWidth = (int) (SampleRate * hitWindowRangeInMS * 0.001);

        comboNum = 1;
    }

    private void Update()
    {
        if (Pause.instan.isPause)
        {
            Pause.instan.ReGamePlay();
            return;
        }

        //�������ֲ���ʱ��
        if (timeLeftToPlay > 0)
        {
            timeLeftToPlay -= Time.unscaledDeltaTime;
            if (timeLeftToPlay <= 0)
            {
                player.Play();
                timeLeftToPlay = 0;
            }
        }

        //��������ʱ��
        if (leadInTimeLeft > 0)
            leadInTimeLeft = Mathf.Max(leadInTimeLeft - Time.unscaledDeltaTime, 0);

        Pause.instan.PauseGame();
        AudioClip a=playingKoreo.SourceClip;
        timer += Time.deltaTime;

        if (timer-leadInTime>=a.length+1)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log(1);
        endingCanvas.SetActive(true);
        endingScoreText.text = " "+(int)score;
        if (score>=exLevel)
        {
            rankText.text = "EX";
        }
        else if (score>=aaLevel)
        {
            rankText.text = "AA";

        }
        else if(score>=aLevel)
        {
            rankText.text = "A";
        }
        else if (score >= bLevel)
        {
            rankText.text = "B";
        }
        else
        {
            rankText.text = "C";
        }

        perfectText.text = "" + perfectCount;
        greatText.text = "" + greatCount;
        goodText.text = "" + goodCount;
        missText.text = "" + missCount;
        maxComboText.text = "" + maxCombo;
    }
    private void InitializeLeadIn()
    {
        if (leadInTime > 0)
        {
            leadInTimeLeft = leadInTime;
            timeLeftToPlay = leadInTime;
        }
        else
        {
            player.Play();
        }
    }

    //����������Żض����
    public void ReturnNoteObjectToPool(NoteObject no)
    {
        if (no != null)
        {
            no.enabled = false;
            no.gameObject.SetActive(false);
            noteObjectsPool.Push(no);
        }
    }

    //����������ȡ����ķ���
    public NoteObject GetFreshNoteObject()
    {
        NoteObject retObj;
        if (noteObjectsPool.Count > 0)
            retObj = noteObjectsPool.Pop();
        else
            retObj = Instantiate(noteObject);
        retObj.gameObject.SetActive(true);
        retObj.enabled = true;
        return retObj;
    }

    public GameObject GetFreshEffectObject(Stack<GameObject> stack, GameObject effectObject)
    {
        GameObject effectGo;

        if (stack.Count > keyCount)
            effectGo = stack.Pop();
        else
            effectGo = Instantiate(effectObject);

        effectGo.gameObject.SetActive(true);

        return effectGo;
    }

    public void ReturnEffectGoToPool(Stack<GameObject> stack, GameObject effectGo)
    {
        if (effectGo != null)
        {
            effectGo.gameObject.SetActive(false);
            stack.Push(effectGo);
        }
    }

    public void LevelAnimator(int hitlevel)
    {
        SpriteRenderer levelRenderer;
        levelRenderer = levelEffectGo.GetComponent<SpriteRenderer>();

        if (levelRenderer.sprite != levelSprite[hitlevel])
        {
            levelRenderer.sprite = levelSprite[hitlevel];
            levelEffectGo.SetActive(false);
        }
        else
        {
            levelEffectGo.SetActive(false);
        }

        levelEffectGo.SetActive(true);

        if (comboNum >= 5)
        {
            comboText.gameObject.SetActive(false);
            comboText.gameObject.SetActive(true);
            comboText.text = "Combo\n" + comboNum;
        }
    }

    public void HideComboText()
    {
        comboText.gameObject.SetActive(false);
    }

    public void UpdateScoreText(float addNum)
    {
        score += addNum;
        scoreText.text = "Score:" + (int)score;
    }
}