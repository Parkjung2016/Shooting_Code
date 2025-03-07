using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.EventSystems;
using System.Security.Cryptography;

public class Title_UI : MonoBehaviour
{
    private Image _fade;
    private bool _isStarted;
    private Image _makeBtnImg;
    [SerializeField]
    private Sprite[] _makeBtnSprites;

    private TextMeshProUGUI _errorText;

    private CanvasGroup _playerInfoGroup;

    private Coroutine _errorMakeNameCoroutine;

    private GameObject _anykeyPress;

    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _showMakeInfoAudioClip;
    [SerializeField]
    private AudioClip _typingNameAudioClip;
    [SerializeField]
    private AudioClip _enterAudioClip;

    private bool _startTrue;

    private GameObject _checkExitGroup;

    private EventSystem _eS;

    private GameObject _showCheckExitBtn;
    private GameObject _exitBtn;
    private GameObject _returnBtn;
    private void Awake()
    {
        _startTrue = false;
        _eS = FindObjectOfType<EventSystem>();
        _checkExitGroup = transform.Find("CheckExit").gameObject;
        _fade = transform.Find("Fade").GetComponent<Image>();
        _fade.DOFade(0, 0);
        _fade.gameObject.SetActive(false);
        _playerInfoGroup = transform.Find("MakeInfo").GetComponent<CanvasGroup>();
        _makeBtnImg = _playerInfoGroup.transform.Find("Button Accept").GetComponent<Image>();
        _errorText = _playerInfoGroup.transform.Find("ErrorText").GetComponent<TextMeshProUGUI>();
        _errorText.gameObject.SetActive(false);
        _anykeyPress = transform.Find("PressAnyKeyImg").gameObject;
        _playerInfoGroup.alpha = 0;
        _playerInfoGroup.gameObject.SetActive(false);
        _anykeyPress.SetActive(true);
        _audioSource = GetComponent<AudioSource>();
        DOTween.SetTweensCapacity(3000  , 3000);
        _showCheckExitBtn = transform.Find("ExitBtn").gameObject;
        _returnBtn = _checkExitGroup.transform.Find("Return").gameObject;
        _exitBtn = _checkExitGroup.transform.Find("Exit").gameObject;
        _checkExitGroup.SetActive(false);
    }
    private void Update()
    {
        if (!_startTrue) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_checkExitGroup.activeSelf)
            {
                ExecuteEvents.Execute(_showCheckExitBtn, new BaseEventData(_eS), ExecuteEvents.submitHandler);
            }
            else
                ExecuteEvents.Execute(_returnBtn, new BaseEventData(_eS), ExecuteEvents.submitHandler);
        }
        if (Input.GetKeyDown(KeyCode.Return) && _checkExitGroup.activeSelf)
            ExecuteEvents.Execute(_exitBtn, new BaseEventData(_eS), ExecuteEvents.submitHandler);
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !_isStarted)
        {
            PressBtn();
            _anykeyPress.SetActive(false);
            _isStarted = true;
        }

    }
    public void ShowCheckExit()
    {
        SoundManager.Instance.ClickBtnAudio();
        Time.timeScale = 0;
        _checkExitGroup.SetActive(true);
        AudioListener.volume = 0;
    }
    public void StartTrue()
    {
        _startTrue = true;

    }

    public void PressBtn()
    {
        bool loaded = PlayerDataManager.Instance.CheckData();
        PlayerDataManager.Instance.LoadData();

        if (PlayerDataManager.Instance.SavePlayerData.Remove)
        {
            PlayerDataManager.Instance.DeleteData();
            PlayerDataManager.Instance.PlayerData = new PlayerData();
            PlayerDataManager.Instance.SavePlayerData = new Data();
            PlayerDataManager.Instance.SavePlayerData.Remove = false;
            PlayerDataManager.Instance.SaveData();
            loaded = false;
        }
        if (!loaded || (loaded && (PlayerDataManager.Instance.SavePlayerData == null ||PlayerDataManager.Instance.SavePlayerData.PlayerName == "")))
        {
            _audioSource.PlayOneShot(_showMakeInfoAudioClip);
            _playerInfoGroup.gameObject.SetActive(true);
            _playerInfoGroup.DOFade(1, 1);
        }
        else
        {
            _audioSource.PlayOneShot(_enterAudioClip);
            _fade.gameObject.SetActive(true);
            Camera.main.GetComponent<AudioSource>().DOFade(0, 1);
            _fade.DOFade(1, 1).OnComplete(() =>
            {
                SceneManager.LoadScene("CharacterSelect");

            });
        }
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
        Time.timeScale = 1;
        AudioListener.volume = 1;
        _checkExitGroup.SetActive(false);

    }
    public void UpdateInputField(InputField inputField)
    {
        _audioSource.PlayOneShot(_typingNameAudioClip);
        if (inputField.text == "")
        {

            _makeBtnImg.sprite = _makeBtnSprites[1];
        }
        else
        {
            int a = 0;
            string check = Regex.Replace(inputField.text[0].ToString(), @"[ ^0-9a-zA-Z°¡-ÆR ]{1,10}", "", RegexOptions.Singleline);
            if (int.TryParse(inputField.text[0].ToString(), out a)
           || inputField.text[0] == ' ' || inputField.text[0].ToString().Equals(check))
            {
                _makeBtnImg.sprite = _makeBtnSprites[1];
            }
            else
            {
                _makeBtnImg.sprite = _makeBtnSprites[0];

            }
        }

    }

    IEnumerator ErrorMakeName(string error)
    {
        SoundManager.Instance.ErrorAudio();
        _errorText.gameObject.SetActive(true);
        _errorText.text = error;
        yield return new WaitForSeconds(1);
        _errorText.gameObject.SetActive(false);
    }
    public void MakeName(InputField inputField)
    {
        if (inputField.text == "")
        {
            if (_errorMakeNameCoroutine != null) StopCoroutine(_errorMakeNameCoroutine);
            _errorMakeNameCoroutine = StartCoroutine(ErrorMakeName("´Ð³×ÀÓÀº °ø¹éÀ¸·Î ¼³Á¤ ÇÒ ¼ö ¾ø½À´Ï´Ù!"));
        }
        else
        {
            int a = 0;
            string check = Regex.Replace(inputField.text[0].ToString(), @"[ ^0-9a-zA-Z°¡-ÆR ]{1,10}", "", RegexOptions.Singleline);
            if (int.TryParse(inputField.text[0].ToString(), out a))
            {
                if (_errorMakeNameCoroutine != null) StopCoroutine(_errorMakeNameCoroutine);
                _errorMakeNameCoroutine = StartCoroutine(ErrorMakeName("Ã¹ ±ÛÀÚ´Â ¼ýÀÚ·Î ½ÃÀÛ ÇÒ ¼ö ¾ø½À´Ï´Ù!"));
            }
            else if (inputField.text[0].ToString().Equals(check))
            {
                if (_errorMakeNameCoroutine != null) StopCoroutine(_errorMakeNameCoroutine);
                _errorMakeNameCoroutine = StartCoroutine(ErrorMakeName("Ã¹ ±ÛÀÚ´Â Æ¯¼ö¹®ÀÚ·Î ½ÃÀÛ ÇÒ ¼ö ¾ø½À´Ï´Ù!"));

            }
            else if (inputField.text[0] == ' ')
            {
                if (_errorMakeNameCoroutine != null) StopCoroutine(_errorMakeNameCoroutine);
                _errorMakeNameCoroutine = StartCoroutine(ErrorMakeName("Ã¹ ±ÛÀÚ´Â °ø¹éÀ¸·Î ½ÃÀÛ ÇÒ ¼ö ¾ø½À´Ï´Ù!"));

            }

            else
            {
                SoundManager.Instance.SuccessAudio();
                _fade.gameObject.SetActive(true);
                PlayerDataManager.Instance.SavePlayerData = new Data();
                PlayerDataManager.Instance.SavePlayerData .Remove = false;
                for (int i = 0; i <          PlayerDataManager.Instance.SavePlayerData.datas.Length; i++)
                {
                    PlayerDataManager.Instance.SavePlayerData.datas[i] = new PlayerData();
                }
                PlayerDataManager.Instance.SavePlayerData.PlayerName= inputField.text;
                PlayerDataManager.Instance.SaveData();
             _fade.DOFade(1, 1).OnComplete(() => SceneManager.LoadScene("CharacterSelect"));
            }
        }
    }
}
