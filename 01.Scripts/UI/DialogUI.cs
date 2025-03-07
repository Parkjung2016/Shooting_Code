using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogUI : MonoBehaviour
{
    public static DialogUI Instance;
    private GameObject _dialogGroup;

    private TextMeshProUGUI _dialogText;
    private TextMeshProUGUI _npcNameText;

    private CanvasGroup _canvasGroup;
    public bool Dialoging = false;

    private Button _menuBtn;
    private Button _cancelBtn;
    private AudioSource _audioSource;
    public int CurrentNPC;
    public void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
        _menuBtn = transform.Find("Button Menu").GetComponent<Button>();
        _cancelBtn = transform.Find("Button Cancel").GetComponent<Button>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _dialogGroup = gameObject;
        _dialogText = transform.Find("DialogText").GetComponent<TextMeshProUGUI>();
        _npcNameText = transform.Find("NPCName").GetComponent<TextMeshProUGUI>();
        _canvasGroup.alpha = 0;
        HideDialog();
    }
    public void StartDialog(string npcName, string[] dialogs)
    {
        _audioSource.volume = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _menuBtn.gameObject.SetActive(CurrentNPC == 0);
        Dialoging = true;
        ShowDialog();
        _npcNameText.text = npcName;
        Dialog(0, dialogs);
    }
    public void ShowDialog()
    {
        Dialoging = true;
        _canvasGroup.DOFade(1, .5f);
    }
    public void EndDialog(bool sound = true)
    {
        _audioSource.volume = 0;
        if (sound)
        {
            SoundManager.Instance.ClickBtnAudio();

        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameManager_Lobby._instance._pC.IsMoveTrue = true;

        HideDialog();
    }
    void Dialog(int index, string[] dialogs)
    {
        if (Dialoging)
        {


            _dialogText.text = "";
            _audioSource.Play();
            Sequence seq = DOTween.Sequence();
            seq.Append(_dialogText.DOText(dialogs[index], .6f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _audioSource.Stop();
            }));
            seq.AppendInterval(.8f);
            seq.AppendCallback(() =>
            {
                if (index + 1 < dialogs.Length)
                    Dialog(index + 1, dialogs);
                seq.Kill();

            });
        }
    }
    private void Update()
    {
        if (_canvasGroup.alpha >= .99f)
        {
            if (Input.GetKeyDown(KeyCode.Return) && CurrentNPC == 0)
            {
                ExecuteEvents.Execute(_menuBtn.gameObject, new BaseEventData(UIManager_Lobby.Instance.EventSystem), ExecuteEvents.submitHandler);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExecuteEvents.Execute(_cancelBtn.gameObject, new BaseEventData(UIManager_Lobby.Instance.EventSystem), ExecuteEvents.submitHandler);
            }
        }
    }
    public void ClickMenuBtn()
    {
        GameManager_Lobby._instance._pC.IsMoveTrue = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        HideDialog();
        UIManager_Lobby.Instance.EnablePlayerInfo(false);
        GameManager_Lobby._instance._pC._rb.velocity = Vector3.zero;
        CameraManager_Lobby.Instance.ChangeCam(1, () =>
        {
            MenuUI_Lobby.Instance.ShowMenu();
        });
        SoundManager.Instance.ClickBtnAudio();
    }
    public void HideDialog()
    {
        _canvasGroup.DOFade(0, .5f).OnComplete(() => Dialoging = false);
    }
}
