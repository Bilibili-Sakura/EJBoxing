using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicDat
{
    public GameObject musicPrefab;

    public string Name { get; set; }
    public int MusicID { get; set; }

    public List<Level> Levels = new List<Level>();

    public override string ToString()
    {
        string res = Name + " " + MusicID + '\n';

        foreach (var l in Levels)
        {
            res += l.Difficulty + " " + l.TrackID + " " + l.HighScore + '\n';
        }
        return res;
    }
}

public class Level
{
    public string Difficulty { get; set; }
    public string TrackID { get; set; }
    public int HighScore { get; set; }


}