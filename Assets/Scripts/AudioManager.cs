using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Static Instance
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    //instance = new GameObject("Spawned AudioManager", typeof(AudioManager)).GetComponent<AudioManager>();
                }
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    #endregion

    #region Fields
    private AudioSource engineSound;
    //private AudioSource musicSource2;
    private AudioSource sfxSource;
    private bool firstMusicSourceIsPlaying;

    #endregion

    private void Awake()
    {
        //make sure to don't destroy this instance
        DontDestroyOnLoad(this.gameObject);

        //create audio sources and save as reference
        engineSound = gameObject.AddComponent<AudioSource>();
        //musicSource2 = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        //Loop the music tracks
        engineSound.loop = true;
        //musicSource2.loop = true;
    }


    public void PlayMusic(AudioClip audioClip)
    {
        //Determine wich source is playing
        AudioSource activeSource = engineSound;//(firstMusicSourceIsPlaying) ? musicSource : musicSource2;

        activeSource.clip = audioClip;
        activeSource.volume = 1;
        activeSource.Play();
    }

    public void PlayMusicWithFade(AudioClip newClip, float transitionTime = 1.0f)
    {
        //Determine wich source is playing
        AudioSource activeSource = engineSound; //(firstMusicSourceIsPlaying) ? musicSource : musicSource2;

        StartCoroutine(UpdateMusicWithfade(activeSource, newClip, transitionTime));
    }

    public void PlayMusicWithCrossFade(AudioClip newClip, float transitionTime = 1.0f)
    {
        //Determine wich source is playing
        AudioSource activeSource = engineSound;//(firstMusicSourceIsPlaying) ? musicSource : musicSource2;
        AudioSource newSource = engineSound;//(firstMusicSourceIsPlaying) ? musicSource2 : musicSource;

        //Swap the source
        firstMusicSourceIsPlaying = !firstMusicSourceIsPlaying;

        //Set the fields of the audio source, then start the coroutine to crossfade
        newSource.clip = newClip;
        newSource.Play();
        StartCoroutine(UpdateMusicWithCrossFade(activeSource, newSource, transitionTime));
    }

    public void StopMusic(AudioClip audioClip)
    {
        engineSound.Stop();



    }

    private IEnumerator UpdateMusicWithfade(AudioSource activeSource, AudioClip newClip, float transitionTime)
    {
        //Make sure the source is active and playing
        if (!activeSource.isPlaying)
        {
            activeSource.Play();
        }
        float t = 0.0f;

        //Fade out
        for (t = 0; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (1 - (t / transitionTime));
            yield return null;
        }

        activeSource.Stop();
        activeSource.clip = newClip;
        activeSource.Play();

        //Fade in
        for (t = 0; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (t / transitionTime);
            yield return null;
        }
    }
    private IEnumerator UpdateMusicWithCrossFade(AudioSource original, AudioSource newSource, float transitionTime)
    {
        float t = 0.0f;

        for (t = 0; t < transitionTime; t += Time.deltaTime)
        {
            original.volume = (1 - (t / transitionTime));
            newSource.volume = (t / transitionTime);
            yield return null;
        }

        original.Stop();
    }

    public void PlaySFX(AudioClip audioClip)
    {
        sfxSource.PlayOneShot(audioClip);
    }

    public void PlaySFX(AudioClip audioClip, float volume)
    {
        sfxSource.PlayOneShot(audioClip, volume);
    }

    public void SetMusicVolume(float volume)
    {
        engineSound.volume = volume;
        //musicSource2.volume = volume;
    }
    public void SetMusicPitch(float pitch)
    {
        engineSound.pitch = pitch;
        //musicSource2.volume = volume;
    }
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
    public void SetSFXPitch(float pitch)
    {
        sfxSource.pitch = pitch;
        //musicSource2.volume = volume;
    }
}
