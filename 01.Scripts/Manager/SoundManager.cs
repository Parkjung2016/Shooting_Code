using DG.Tweening;
using Hellmade.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _errorAudioClip;
    [SerializeField]
    private AudioClip _successAudioClip;
    [SerializeField]
    private AudioClip _ClickBtnAudioClip;
    private AudioSource[] audios;
    private float[] audiosVolume;
    private AudioSource _dialogAudiSource;


    private float[] _eazyVolumes;
    private void Awake()
    {
        SoundManager[] sms = FindObjectsOfType<SoundManager>();
        if (sms.Length == 2)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        _audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += LoadScene;
    }

    private void LoadScene(Scene arg0, LoadSceneMode arg1)
    {
        if(UIManager.Instance != null)
        {
            _dialogAudiSource = UIManager.Instance._dialogAudioSource;
        }
        audios = FindObjectsOfType<AudioSource>();
        audiosVolume = new float[audios.Length];

        for (int i = 0; i < audios.Length; i++)
        {
            audiosVolume[i] = audios[i].volume;
        }
        _audioSource.volume = 1;
    }

    public void ErrorAudio()
    {
        _audioSource.PlayOneShot(_errorAudioClip);
    }
    public void FadeSound(float volume,float time = 1)
    {
        EazySoundManager.GlobalSoundsVolume = volume;


        for (int i = 0; i < audios.Length; i++)
        {
            audios[i].DOFade(volume== 1 ? audiosVolume[i]:0, time).SetUpdate(true);
        }
            if(_dialogAudiSource != null)
            _dialogAudiSource.DOFade(volume == 1 ? 1 : 0, time).SetUpdate(true);
       Obstacle obs=  FindObjectOfType<Obstacle>();
            if(obs != null)
        {
            obs.GetComponent<AudioSource>().DOFade(volume == 1 ? .6f : 0, time).SetUpdate(true);
        }
        _audioSource.volume = 1;
    }

    public void SuccessAudio()
    {

        _audioSource.PlayOneShot(_successAudioClip);
    }
    public void ClickBtnAudio()
    {
        _audioSource.PlayOneShot(_ClickBtnAudioClip);
    }
    public void PlayAudioWithOneShot(AudioClip audioClip,float volume= 1)
    {
        _audioSource.PlayOneShot(audioClip, volume);
    }
}
