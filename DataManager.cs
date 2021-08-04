using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    //音乐数据文件路径
    public string musicDataPath = "Assets/Data/musicData.xml";

    private void Awake()
    {
        Instance = this;
    }

    public List<MusicDat> LoadMusicData()
    {
        var musicDataList = new List<MusicDat>();

        var xmlDoc = new XmlDocument();
        xmlDoc.Load(musicDataPath);
        var musicListNode = xmlDoc.FirstChild.FirstChild;
        foreach (XmlNode musicNode in musicListNode)
        {
            var music = new MusicDat();
            music.Name = musicNode["Name"].InnerText;
            music.MusicID = int.Parse(musicNode["MusicID"].InnerText);
            var LevelsNode = musicNode.LastChild;
            foreach (XmlNode levelNode in LevelsNode)
            {
                Level level = new Level();
                XmlAttributeCollection levelAttributes=levelNode.Attributes;
                level.Difficulty = levelAttributes["dif"].InnerText;
                level.TrackID = levelAttributes["trackID"].InnerText;
                level.HighScore = Int32.Parse(levelAttributes["highScore"].InnerText);
                music.Levels.Add(level);

            }

            musicDataList.Add(music);
        }

        return musicDataList;
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}