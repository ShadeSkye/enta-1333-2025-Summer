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
        calmMusicSource.clip = Music[0];
        battleMusicSource.clip = Music[1];
    }

    public void PlayCalmMusic()
    {
        isPlayingCalmMusic = true;
        calmMusicSource.Play();
    }

    public void PlayBattleMusic()
    {
        isPlayingCalmMusic = false;
        battleMusicSource.Play();
    }

    public void ChangeMusic()
    {
        float timeToFade = 1f;
        float timeElapsed = 0f;
        if (isPlayingCalmMusic == true)
        {
            PlayBattleMusic();
            while (timeElapsed < timeToFade)
            {
                calmMusicSource.volume = Mathf.Lerp(1f, 0f, timeElapsed / timeToFade);
                battleMusicSource.volume = Mathf.Lerp(0f, 1f, timeElapsed / timeToFade);

                timeElapsed += Time.deltaTime;
            }
        }
        else if (isPlayingCalmMusic == false)
        {
            PlayCalmMusic();
            while (timeElapsed < timeToFade)
            {
                battleMusicSource.volume = Mathf.Lerp(1f, 0f, timeElapsed / timeToFade);
                calmMusicSource.volume = Mathf.Lerp(0f, 1f, timeElapsed / timeToFade);

                timeElapsed += Time.deltaTime;
            }
        }

    }

    public void PlaySFX(int I)
    {
        sfxSource.clip = SFX[I];
        sfxSource.Play();
    }

    public void StopMusic() => calmMusicSource.Stop();
    public void StopSFX() => sfxSource.Stop();
}
