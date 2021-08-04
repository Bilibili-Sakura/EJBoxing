using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;

public class Music : MonoBehaviour
{
    float soundValue;

    private MultiMusicPlayer player;//播放器
    public int musicNum;//总音乐数量

    private void Awake()
    {
        soundValue = PlayerPrefs.GetFloat("SoundValue");
    }

    void Start()
    {
        player= GetComponent<MultiMusicPlayer>();
        player.LoadSong(MusicChoose.Instance.musics);
        player.Stop();
        //先把所有音乐静音
        for (int i = 0; i < musicNum; i++)
        {
            player.SetVolumeForLayer(i, 0);
        }
        //再将选中音乐声音大小还原
        player.SetVolumeForLayer(MusicChoose.Instance.chooseNum, soundValue);
        //player.SetVolumeForLayer(num, 1);
    }

}
