using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource calmMusicSource;
    [SerializeField] private AudioSource battleMusicSource;

    public List<AudioClip> SFX = new List<AudioClip>();
    public List<AudioClip> Music = new List<AudioClip>();

    private bool isPlayingCalmMusic;

    private void Awake()
    {
        instance = this;
    }

    public void PlayCalmMusic(int I)
    {
        isPlayingCalmMusic = true;
        calmMusicSource.clip = Music[I];
        calmMusicSource.Play();
    }

    public void PlayBattleMusic(int I)
    {
        isPlayingCalmMusic = false;
        battleMusicSource.clip = Music[I];
        battleMusicSource.Play();
    }

    public void ChangeMusic(int I)
    {
        float timeToFade = 0.25f;
        float timeElapsed = 0f;
        if()
        while (timeElapsed < timeToFade)
        {
            calmMusicSource.volume = Mathf.Lerp(1f, 0f, timeElapsed / timeToFade);
            battleMusicSource.volume = Mathf.Lerp(0f, 1f, timeElapsed / timeToFade);

            timeElapsed += Time.deltaTime;
        }
        calmMusicSource.clip = Music[I];
        calmMusicSource.volume = 1;
    }

    public void PlaySFX(int I)
    {
        sfxSource.clip = SFX[I];
        sfxSource.Play();
    }

    public void StopMusic() => calmMusicSource.Stop();
    public void StopSFX() => sfxSource.Stop();
}
