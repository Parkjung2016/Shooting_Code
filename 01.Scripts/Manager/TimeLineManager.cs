using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class TimeLineManager : MonoBehaviour
{
   public static TimeLineManager Instance;

    private PlayableDirector _playableDirector;

    [SerializeField]
    private TimelineAsset[] _timelineAsset;

    private ColorAdjustments _colorAdjustments;

    public int Sceneindex;
    private void Awake()
    {
        Volume volume = FindAnyObjectByType<Volume>();
        volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments);
        _playableDirector = GetComponent<PlayableDirector>();
        Instance = this;

    }
    private void Start()
    {

        PlayEnterLobbyTimeLine();
    }
    public void SkipTimeLine()
    {
        _playableDirector.time = 6;
    }
    public void PlayEnterLobbyTimeLine()
    {
        _playableDirector.playableAsset = _timelineAsset[1];
       TimelineAsset timelineAsset = (TimelineAsset)_playableDirector.playableAsset;
        // iterate through tracks and map the objects appropriately
        foreach(var track in timelineAsset.GetOutputTracks())
        {
            if (track.name == "Activation Track (1)")
            {
                _playableDirector.SetGenericBinding(track, GameManager_Lobby._instance._pC.gameObject);
            }

        }
        _playableDirector.Play();
    }
    public void PlayAdventureTimeLine()
    {
        _playableDirector.playableAsset = _timelineAsset[0];

        _playableDirector.Play();
    }
    public void OpenInGameScene()
    {
        SoundManager.Instance.FadeSound(0,2);
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => _colorAdjustments.postExposure.value, x => _colorAdjustments.postExposure.value = x, -30, 4).SetEase(Ease.Linear));
        seq.AppendCallback(() => LoadingSceneManager.LoadScene("InGame"+ Sceneindex));
    }

    public void MoveStop()
    {
        GameManager_Lobby._instance._pC.IsMoveTrue = false;
    }
    public void MoveAble()
    {
        GameManager_Lobby._instance._pC.IsMoveTrue = true;
        UIManager_Lobby.Instance.FalseSkip(0);
        PlayerDataManager.Instance.PlayerData.SkipTrue = true;
        UIManager_Lobby.Instance.ShowMapInfo();
        _playableDirector.Stop();
    }
    public void FalseSkip()
    {
        UIManager_Lobby.Instance._skipTrue = false;
        UIManager_Lobby.Instance.FalseSkip();

    }
}
