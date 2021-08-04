using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo.Players;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicChoose : MonoBehaviour
{
    public GameObject prefab;
    public Transform prefabParent;

    public static MusicChoose Instance;
    public List<MusicDat> musicDataList=new List<MusicDat>();

    public Sprite[] cMusicSprite;

    public Sprite noSelect;
    public Slider[] levelSlider;
    public int levelNum;
    public List<MusicLayer> musics;
    public GameObject origin;
    private bool hasChosen;

    public int chooseNum;

    public GameObject levelPanel;
    bool isPlay = false;
    int musicDistance = 140;

    public Animator playAnima;

    public Slider loadingSlider;
    public Text loadingText;

    AsyncOperation op;
    int toProgress;
    int displayProgress;

    bool isCanPlay = false;

    public Image musicToLevelImg;
    int clevelnum;

    public Text MaxScore;
    public Text LastMaxScore;

    private void Start()
    {
        Instance = this;
        musicDataList = DataManager.Instance.LoadMusicData();
        chooseNum = 0;

        for (var m = 0; m < musicDataList.Count; m++)
        {
            musicDataList[m].musicPrefab = Instantiate(prefab,prefabParent);
            musicDataList[m].musicPrefab.GetComponentInChildren<Text>().text = musicDataList[m].Name;
            musicDataList[m].musicPrefab.GetComponentInChildren<Image>().sprite = cMusicSprite[m];
            Debug.Log(musicDataList[m]);
        }

    }

    private void Update()
    {
        if (!isPlay)
        {
            if (hasChosen)
                LevelChoose();
            else
                Musicmusic();
        }
        else
        {
            if (isCanPlay)
            {
                StartCoroutine(LoadingGame());
                isCanPlay = false;
            }

            playAnima.gameObject.SetActive(true);
        }

    }

    //左右键选歌
    void Musicmusic()
    {
        musicDataList[chooseNum].musicPrefab.GetComponent<Animator>().SetBool("isBig", true);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (chooseNum != 0)
            {
                for (var i = 0; i < musicDataList.Count; i++)
                    musicDataList[i].musicPrefab.transform.position = new Vector2(musicDataList[i].musicPrefab.transform.position.x + musicDistance,
                        musicDataList[i].musicPrefab.transform.position.y);

                musicDataList[chooseNum].musicPrefab.GetComponent<Animator>().SetBool("isBig", false);

                chooseNum--;
            }
            else
                return;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (chooseNum != musicDataList.Count - 1)
            {
                for (var i = 0; i < musicDataList.Count; i++)
                    musicDataList[i].musicPrefab.transform.position = new Vector2(musicDataList[i].musicPrefab.transform.position.x - musicDistance,
                            musicDataList[i].musicPrefab.transform.position.y);

                musicDataList[chooseNum].musicPrefab.GetComponent<Animator>().SetBool("isBig", false);

                chooseNum++;
            }
            else
                return;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            levelNum = 0;
            musicToLevelImg.sprite = musicDataList[chooseNum].musicPrefab.GetComponentInChildren<Image>().sprite;
            hasChosen = true;
        }

        PX();
    }

    //选难度
    void LevelChoose()
    {
        levelPanel.GetComponent<Animator>().SetBool("isGo", true);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (musicDataList[chooseNum].Levels[levelNum].TrackID == "")
                return;

            isPlay = true;
            isCanPlay = true;
            hasChosen = false;

        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            hasChosen = false;
            levelPanel.GetComponent<Animator>().SetBool("isGo", false);

            for (var i = 0; i < levelNum; i++)
                if (levelSlider[i].value != 0)
                    levelSlider[i].value = 0;

        }

        LevelPanelEvent();
    }

void LevelPanelEvent()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && levelNum != 0)
        {
            clevelnum = levelNum;
            levelNum--;

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && levelNum != levelSlider.Length - 1)
        {
            clevelnum = levelNum;
            levelNum++;

        }

        if (clevelnum != levelNum)
            levelSlider[clevelnum].value -= 0.1f;

        levelSlider[levelNum].value += 0.1f;


    }

    //排序
    void PX()
    {
        musicDataList[chooseNum].musicPrefab.transform.position = origin.transform.position;
        Vector2 pos = musicDataList[chooseNum].musicPrefab.transform.position;

        if (chooseNum != 0)
            for (var i = chooseNum - 1; i >= 0; i--)
                musicDataList[i].musicPrefab.transform.position = new Vector2(pos.x - musicDistance * (chooseNum - i), pos.y);

        if (chooseNum != musicDataList.Count - 1)
            for (var i = chooseNum + 1; i < musicDataList.Count; i++)
                musicDataList[i].musicPrefab.transform.position = new Vector2(pos.x + musicDistance * (i - chooseNum), pos.y);
    }

    //进度条
    IEnumerator LoadingGame()
    {
        op = SceneManager.LoadSceneAsync("Game");
        //停止跳Scene
        op.allowSceneActivation = false;
        //目标值
        toProgress = 0;
        //滚动条的value值
        displayProgress = 0;
        //AsyncOperation.progress 异步操作
        while (op.progress < 0.9f)
        {
            //进行百分制处理
            toProgress = (int)op.progress * 100;

            while (displayProgress < toProgress)
            {
                ++displayProgress;
                UpdateLoadingSlider();

                yield return new WaitForEndOfFrame();
            }
        }

        toProgress = 100;

        while (displayProgress < toProgress)
        {
            ++displayProgress;
            UpdateLoadingSlider();
            yield return new WaitForEndOfFrame();
        }

        op.allowSceneActivation = true;
   }

    //进度条更新
    public void UpdateLoadingSlider()
    {
        loadingSlider.value = displayProgress * 0.01f;
        loadingText.text = displayProgress.ToString() + "%";
    }

    
}