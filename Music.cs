using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;

public class Music : MonoBehaviour
{
    float soundValue;

    private MultiMusicPlayer player;//������
    public int musicNum;//����������

    private void Awake()
    {
        soundValue = PlayerPrefs.GetFloat("SoundValue");
    }

    void Start()
    {
        player= GetComponent<MultiMusicPlayer>();
        player.LoadSong(MusicChoose.Instance.musics);
        player.Stop();
        //�Ȱ��������־���
        for (int i = 0; i < musicNum; i++)
        {
            player.SetVolumeForLayer(i, 0);
        }
        //�ٽ�ѡ������������С��ԭ
        player.SetVolumeForLayer(MusicChoose.Instance.chooseNum, soundValue);
        //player.SetVolumeForLayer(num, 1);
    }

}
