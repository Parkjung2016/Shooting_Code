using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public static PauseUI Instance;
    private CanvasGroup _canvasGroup;
    public bool Paused;
    private Animator[] npc;
    private DepthOfField _dof;
    private Button _resumeBtn;
    private Button _titleBtn;
    private Button _mainBtn;
    private GameObject _exitBtn;
    private GameObject _returnBtn;

    private GameObject _checkExit;
    private void Awake()
    {
        Transform checkexit = transform.Find("CheckExit");
        if (checkexit != null)
        {
            _checkExit = checkexit.gameObject;
            _exitBtn = _checkExit.transform.Find("Exit").gameObject;
            _returnBtn = _checkExit.transform.Find("Return").gameObject;
            _checkExit.SetActive(false);
        }
        if (FindObjectOfType<NPC>() != null)
        {
            NPC[] _npc = FindObjectsOfType<NPC>();
            npc = new Animator[_npc.Length];
            for (int i = 0; i < _npc.Length; i++)
            {
                npc[i] = _npc[i].GetComponent<Animator>();
            }

        }

        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        gameObject.SetActive(false);
        Instance = this;

        _titleBtn = transform.Find("CanvasGroup/TitleBtn").GetComponent<Button>();
        if (GameManager._instance != null)
            _mainBtn = transform.Find("CanvasGroup/MainBtn").GetComponent<Button>();
        _resumeBtn = transform.Find("CanvasGroup/ResumeBtn").GetComponent<Button>();
        if (transform.Find("CanvasGroup/MainBtn") != null)
            _mainBtn = transform.Find("CanvasGroup/MainBtn").GetComponent<Button>();
        FindObjectOfType<Volume>().profile.TryGet<DepthOfField>(out _dof);
        if (GameManager_Lobby._instance != null)
            _dof.focalLength.value = 42;
        else
        {
            _dof.focalLength.value = 0;

        }
        Paused = false;

    }
    public void EnableUI()
    {
        Paused = true;
        //SoundManager.Instance.ClickBtnAudio();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SoundManager.Instance.FadeSound(0);
        Time.timeScale = 0;
        if (GameManager_Lobby._instance != null)
        {


            GameManager_Lobby._instance._pC._animator.enabled = false;
            GameManager_Lobby._instance._pC._rb.velocity = Vector3.zero;
            for (int i = 0; i < npc.Length; i++)
            {
                npc[i].enabled = false;
            }
        }
        gameObject.SetActive(true);
        if (GameManager_Lobby._instance != null)
            DOTween.To(() => _dof.focalLength.value, x => _dof.focalLength.value = x, 100, 1).SetUpdate(true);
        else
        {
            DOTween.To(() => _dof.focalLength.value, x => _dof.focalLength.value = x, 42, 1).SetUpdate(true);

        }
        if (_mainBtn != null) _mainBtn.Select();
        _canvasGroup.DOFade(1, 1).SetUpdate(true);
    }
    private void Update()
    {
        if (_canvasGroup.alpha >= .99f)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_checkExit == null || _checkExit != null && !_checkExit.activeSelf)
                    ExecuteEvents.Execute(_resumeBtn.gameObject, new BaseEventData(UIManager_Lobby.Instance == null ? UIManager.Instance.EventSystem : UIManager_Lobby.Instance.EventSystem), ExecuteEvents.submitHandler);
                else
                    ExecuteEvents.Execute(_returnBtn, new BaseEventData(UIManager_Lobby.Instance == null ? UIManager.Instance.EventSystem : UIManager_Lobby.Instance.EventSystem), ExecuteEvents.submitHandler);

            }


            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (_checkExit == null || _checkExit != null && !_checkExit.activeSelf)
                {

                    if (_mainBtn == null)
                        ExecuteEvents.Execute(_titleBtn.gameObject, new BaseEventData(UIManager_Lobby.Instance == null ? UIManager.Instance.EventSystem : UIManager_Lobby.Instance.EventSystem), ExecuteEvents.submitHandler);
                }
                else
                    ExecuteEvents.Execute(_exitBtn.gameObject, new BaseEventData(UIManager_Lobby.Instance == null ? UIManager.Instance.EventSystem : UIManager_Lobby.Instance.EventSystem), ExecuteEvents.submitHandler);

                //else
                //{
                //    ExecuteEvents.Execute(_mainBtn.gameObject, new BaseEventData(UIManager_Lobby.Instance == null ? UIManager.Instance.EventSystem : UIManager_Lobby.Instance.EventSystem), ExecuteEvents.submitHandler);
                //}
            }
        }
    }
    public void ShowExitCheck()
    {

        if (_canvasGroup.alpha < .99f) return;
        SoundManager.Instance.ClickBtnAudio();
        _checkExit.SetActive(true);
    }
    public void ExitBtn()
    {
        SoundManager.Instance.ClickBtnAudio();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void ReturnBtn()
    {
        SoundManager.Instance.ClickBtnAudio();
        _checkExit.SetActive(false);
    }

    public void ResumeBtnClick()
    {
        
        SoundManager.Instance.ClickBtnAudio();
        SoundManager.Instance.FadeSound(1);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (GameManager_Lobby._instance != null)
            DOTween.To(() => _dof.focalLength.value, x => _dof.focalLength.value = x, 42, 1).SetUpdate(true);
        else
        {
            DOTween.To(() => _dof.focalLength.value, x => _dof.focalLength.value = x, 0, 1).SetUpdate(true);

        }
        _canvasGroup.DOFade(0, 1).SetUpdate(true).OnComplete(() =>
        {
            if (GameManager_Lobby._instance != null)
            {


                GameManager_Lobby._instance._pC._animator.enabled = true;
                for (int i = 0; i < npc.Length; i++)
                {
                    npc[i].enabled = true;
                }
            }
            Time.timeScale = 1;
            gameObject.SetActive(false);
            Paused = false;
        });
    }
    public void MainBtnClick()
    {
        if (_canvasGroup.alpha < .99f) return;

        //Time.timeScale = 1;
        SoundManager.Instance.ClickBtnAudio();
        DOTween.KillAll();
        UIManager.Instance.FadeIn(action: () =>
        {
            Time.timeScale = 1;
            LoadingSceneManager.LoadScene("Lobby");
        });
    }
    public void TitleBtnClick()
    {
        if (_canvasGroup.alpha < .99f) return;

        //Time.timeScale = 1;
        SoundManager.Instance.ClickBtnAudio();
        DOTween.KillAll();
        if (GameManager_Lobby._instance == null)
            UIManager.Instance.FadeIn(action: () =>
            {
                PlayerDataManager.Instance.SaveData();
                PlayerDataManager.Instance.PlayerData = new PlayerData();
                Time.timeScale = 1;
                SceneManager.LoadScene("Title");
            });
        else
        {
            GameManager_Lobby._instance.FadeIn(() =>
            {
                PlayerDataManager.Instance.SaveData();
                PlayerDataManager.Instance.PlayerData = new PlayerData();
                Time.timeScale = 1;
                SceneManager.LoadScene("Title");

            });
        }
    }
}
