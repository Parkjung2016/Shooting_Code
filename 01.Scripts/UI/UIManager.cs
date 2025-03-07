using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Image _hpProgressBar;
    private Image _manaProgressBar;

    private Text _hpProgressText;
    private Text _manaProgressText;
    private Text _goldText;

    private Image _damagedImg;

    private Sequence seq;
    private TextMeshProUGUI _dialogText;

    private Text _playerNameText;
    private Text _playerClassText;

    private Image _fade;

    private Text _scoreText;

    private CanvasGroup _deathcCanvasGroup;
    private TextMeshProUGUI _highScoreText;
    private Text _currentScoreText;


    [HideInInspector]
    public Image _skillImg;
    [HideInInspector]
    public Image _skillCoolTimeImg;
    public EventSystem EventSystem;

    [HideInInspector]
    public AudioSource _dialogAudioSource;

    private AudioSource _deathAudioSource;

    private Image _playerImg;
    private Sprite[] _playerSprites;

    private Vector2[] _playerImgPos;
    private Vector2[] _playerImgScale;

    public void Init(Transform canvasTrm, Sprite[] playerSprites, Vector2[] playerImgPos, Vector2[] playerImgScale)
    {
        Transform Unitframe = canvasTrm.Find("Unitframe").transform;
        EventSystem = FindObjectOfType<EventSystem>();
        _hpProgressBar = Unitframe.Find("Bars/Health/Filled").GetComponent<Image>();
        _manaProgressBar = Unitframe.Find("Bars/Mana/Filled").GetComponent<Image>();

        _dialogText = canvasTrm.Find("DialogBG/DialogText").GetComponent<TextMeshProUGUI>();
        _scoreText = canvasTrm.Find("Score/Score").GetComponent<Text>();
        _hpProgressText = Unitframe.Find("Bars/Health/Text").GetComponent<Text>();
        _manaProgressText = Unitframe.Find("Bars/Mana/Text").GetComponent<Text>();
        _goldText = canvasTrm.Find("Gold/Value").GetComponent<Text>();
        _playerClassText = Unitframe.Find("Header/Class").GetComponent<Text>();
        Transform Mask = canvasTrm.Find("SkillIconBG/SkillIcon/Mask/");
        _skillImg = Mask.Find("Icon (1)").GetComponent<Image>();
        _skillCoolTimeImg = Mask.Find("CoolTime").GetComponent<Image>();
        _playerNameText = Unitframe.Find("Header/Name").GetComponent<Text>();
        _damagedImg = canvasTrm.Find("Damaged").GetComponent<Image>();
        _damagedImg.DOFade(0, 0);
        _fade = canvasTrm.Find("Fade").GetComponent<Image>();
        _fade.DOFade(0, 0);
        _fade.gameObject.SetActive(false);
        _dialogText.transform.parent.gameObject.SetActive(false);
        _deathcCanvasGroup = canvasTrm.Find("Death").GetComponent<CanvasGroup>();
        _highScoreText= _deathcCanvasGroup.transform.Find("HighScore").GetComponent<TextMeshProUGUI>();
        _currentScoreText = _deathcCanvasGroup.transform.Find("CurrentScore").GetComponent<Text>();
        _deathcCanvasGroup.alpha = 0;
        _skillCoolTimeImg.DOFillAmount(0, 0);
        _deathAudioSource = canvasTrm.Find("Death").GetComponent<AudioSource>();

        _dialogAudioSource = _dialogText.GetComponent<AudioSource>();

        _playerImg = Unitframe.Find("Character/Mask/Current Picture (set your image there)").GetComponent<Image>();
        _playerImgPos = playerImgPos;
        _playerImgScale = playerImgScale;
        _playerSprites = playerSprites;
    }
    public void ChangeSkillImg(Sprite img)
    {
        _skillImg.sprite =img;
    }
    public void CoolTimeSkill(float time,Action action)
    {

       _skillCoolTimeImg.fillAmount = 1;
        _skillCoolTimeImg.DOFillAmount(0, time).OnComplete(() =>
            {
                action?.Invoke();
            });
    }

    private void Start()
    {
        _goldText.text = PlayerDataManager.Instance.PlayerData.Gold.ToString();
        _playerNameText.text = PlayerDataManager.Instance.SavePlayerData.PlayerName;
        if (PlayerDataManager.Instance.PlayerData.PlayerType == 0)
            _playerClassText.text = PlayerDataManager.Instance.PlayerData.CurrentEngine.ItemName;
        else
            _playerClassText.text = "";
        int index = PlayerDataManager.Instance.PlayerData.PlayerType;

        _playerImg.sprite = _playerSprites[index];
        _playerImg.rectTransform.anchoredPosition = _playerImgPos[index];
        _playerImg.rectTransform.localScale = _playerImgScale[index];
    }
    public void SetScoreText(int score)
    {
        _scoreText.text = "점수 : " +score.ToString();
    }
    public void VisibleDeath(int currentScore,float time = 1)
    {
        _deathAudioSource.volume = 1;
        _deathAudioSource.Play();
        _deathcCanvasGroup.gameObject.SetActive(true);
        _deathcCanvasGroup.DOFade(1, time);
        _currentScoreText.text = "현재 점수 -> "+ currentScore.ToString();
        _highScoreText.text ="최고 점수 -> "+ PlayerDataManager.Instance.PlayerData.HighScore[PlayerDataManager.Instance.MapIndex].ToString();
        if (currentScore > PlayerDataManager.Instance.PlayerData.HighScore[PlayerDataManager.Instance.MapIndex])
        {
                PlayerDataManager.Instance.PlayerData.HighScore[PlayerDataManager.Instance.MapIndex] = currentScore;
        StartCoroutine(ChangeBestScore(currentScore));
        }
    }
    IEnumerator ChangeBestScore(int currentScore)
    {
        yield return new WaitForSeconds(1);
        _highScoreText.GetComponent<AudioSource>().Play();
        _highScoreText.text = "";
        _highScoreText.text = "최고 점수 -> " + currentScore;
    }
    public void HideDeath(Action action = null)
    {

        _deathcCanvasGroup.DOFade(0, 1).OnComplete(()=>action?.Invoke());
    }
    public void FadeIn(float time= 1f,Action action= null)
    {
        _fade.gameObject.SetActive(true);
        SoundManager.Instance.FadeSound(0);
        _fade.DOFade(1, time).SetUpdate(true). OnComplete(() => action?.Invoke());

    }
    public void Damage(float time = .5f)
    {
        if (seq != null && seq.IsActive())
            seq.Kill();
        seq = DOTween.Sequence();
        seq.Append(_damagedImg.DOFade(.1f, 0));
        seq.AppendInterval(time);
        seq.Append(_damagedImg.DOFade(0, 1));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)  && !PauseUI.Instance.gameObject.activeSelf )
        {
            PauseUI.Instance.EnableUI();
        }


    }

    public void SetHpProgressBar(float currentHP,float maxHP,float duration=1)
    {
        _hpProgressBar.DOFillAmount(currentHP / maxHP, duration);
        _hpProgressText.text = $"{currentHP}/{maxHP}";
    }
    public void SetManaProgressBar(float currentMana,float maxMana)
    {
        _manaProgressBar.DOFillAmount(currentMana / maxMana, 1);
        _manaProgressText.text = $"{currentMana}/{maxMana}";
    }
    public IEnumerator SetGoldText()
    {
        int i = int.Parse(_goldText.text);
        while (i != PlayerDataManager.Instance.PlayerData.Gold+1)
        {
            _goldText.text = i.ToString();
            i++;
            yield return new WaitForSeconds(.01f);

        }
    }
    public void Typingsoliloquy(string text,Action action = null)
    {
        _dialogText.text = "";
        _dialogText.transform.parent.gameObject.SetActive(true);
        Sequence seq = DOTween.Sequence();
        _dialogAudioSource.Play();
        seq.Append(_dialogText.DOText(text, 1).OnComplete(()=> _dialogAudioSource.Stop()));
        seq.AppendInterval(1);
        seq.AppendCallback(() => _dialogText.transform.parent.gameObject.SetActive(false));
        seq.AppendCallback(() => action?.Invoke());
    }
}