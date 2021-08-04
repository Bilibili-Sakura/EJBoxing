using System.Collections.Generic;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //文本静态
    public static GameManager instan;

    //Combo文本
    public Text comboText;

    //Combo数量
    public int comboNum;

    //Score文本
    public Text scoreText;

    //Score数
    public float score;
    //单hit分数
    public float scorePerHit;
    //键位数量
    public int keyCount;

    //音符速度
    public float noteSpeed = 1;

    /// <summary>
    ///     预制体资源
    /// </summary>
    //音符
    public NoteObject noteObject;

    //击中长音符特效
    public GameObject hitLongEffectGo;

    //击中音符特效
    public GameObject hitEffectGo;

    public List<LaneManager> noteLanes;
    public MultiMusicPlayer player;

    public Sprite[] levelSprite;
    public Animator levelAnimator;
    public GameObject levelEffectGo;

    //其他
    [Tooltip("开始播放音频之前提供的时间量（以秒为单位）")] public float leadInTime;
    [Tooltip("用于目标生成的轨道的事件ID")] [EventID] public string eventID;
    
    [Tooltip("音符的判定区间，单位为Ms")] [Range(8f, 300f)]
    public float hitWindowRangeInMS = 100;

    //音符对象池
    private readonly Stack<NoteObject> noteObjectsPool = new Stack<NoteObject>();
    public Stack<GameObject> hitEffectPool = new Stack<GameObject>();

    //音频播放之前的剩余时间量
    private float leadInTimeLeft;

    //引用
    private Koreography playingKoreo;

    //音乐开始之前的倒计时器
    private float timeLeftToPlay;

    //sample单位的命中窗口
    public int HitWindowSampleWidth { get; private set; }

    public int SampleRate => playingKoreo.SampleRate;

    //当前的采样时间，包括任何必要的延迟
    public int DelayedSampleTime => playingKoreo.GetLatestSampleTime() - (int) (SampleRate * leadInTimeLeft);

    //结算相关
    public GameObject endingCanvas;//结束UI
    public Text endingScoreText;//总分
    public Text rankText;//评级
    public int exLevel=980000;//ex级所需分数
    public int aaLevel=950000;//aa级所需分数
    public int aLevel=900000;//a级所需分数
    public int bLevel=800000;//b级所需分数
    private float timer;
    public Text perfectText;//perfect
    public int perfectCount;
    public Text greatText;//great
    public int greatCount;

    public Text goodText;//good
    public int goodCount;

    public Text missText;//miss
    public int missCount;

    public Text maxComboText;//最大连击数
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
        //获取Koreography对象
        playingKoreo =
            Koreographer.Instance.GetKoreographyAtIndex(MusicChoose.Instance.musicDataList[MusicChoose.Instance.chooseNum].MusicID);
        eventID = MusicChoose.Instance.musicDataList[MusicChoose.Instance.chooseNum].Levels[MusicChoose.Instance.levelNum].TrackID;
        //获取事件轨迹
        var rhythmTrack = playingKoreo.GetTrackByID(eventID);
        //获取事件
        var rawEvents = rhythmTrack.GetAllEvents();
        scorePerHit = 1000000f / rawEvents.Count;
        for (var i = 0; i < rawEvents.Count; i++)
        {
            var evt = rawEvents[i];
            var noteID = evt.GetIntValue();

            //遍历所有音轨
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

        //倒数音乐播放时间
        if (timeLeftToPlay > 0)
        {
            timeLeftToPlay -= Time.unscaledDeltaTime;
            if (timeLeftToPlay <= 0)
            {
                player.Play();
                timeLeftToPlay = 0;
            }
        }

        //倒数引导时间
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

    //将音符对象放回对象池
    public void ReturnNoteObjectToPool(NoteObject no)
    {
        if (no != null)
        {
            no.enabled = false;
            no.gameObject.SetActive(false);
            noteObjectsPool.Push(no);
        }
    }

    //从音符池中取对象的方法
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