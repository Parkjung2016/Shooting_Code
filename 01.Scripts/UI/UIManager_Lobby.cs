using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager_Lobby : MonoBehaviour
{
    public static UIManager_Lobby Instance;

    private Text _playerNameText;
    private Text _playerClassText;
    private Transform _playerInfo;
    public EventSystem EventSystem;

    private Image _skipProgress;
    private Text _skipText;
    private CanvasGroup _skipProgressBar;
    public bool _skipTrue;
    private bool _skipCanceling;

    private RectTransform _mapInfo;

    private Image _playerImg;
    private Sprite[] _playerSprites;

    private Vector2[] _playerImgPos;
    private Vector2[] _playerImgScale;

    public Transform trm;
    //2.3f
    public void Init(Transform trm, Sprite[] playerSprites, Vector2[] playerImgPos,Vector2[] playerImgScale)
    {
      this.  trm = trm;
        _skipTrue = PlayerDataManager.Instance.PlayerData.SkipTrue;
        _playerInfo = trm.Find("Unitframe");
        _skipText = trm.Find("SkipBeginTimeLine/SkipToPressKey").GetComponent<Text>();
        _skipProgressBar = trm.Find("SkipBeginTimeLine/Progress Bar (Horizontal)").GetComponent<CanvasGroup>();
        _skipProgress = _skipProgressBar.transform.GetChild(0).GetComponent<Image>();
        Transform PlayerHeader = _playerInfo.Find("Header");
        EventSystem = FindObjectOfType<EventSystem>();
        _playerNameText = PlayerHeader.Find("Name").GetComponent<Text>();
        _playerClassText = PlayerHeader.Find("Class").GetComponent<Text>();
        gameObject.SetActive(true);
        _playerImg = _playerInfo.Find("Character/Mask/Current Picture (set your image there)").GetComponent<Image>();
        _playerImgPos = playerImgPos;
        _playerImgScale = playerImgScale;
        _playerSprites = playerSprites;
        FalseSkip(0);
        if(_skipTrue)
        {
            _skipText.DOFade(1, 0);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(.6f);
            seq.Append(_skipText.DOFade(0, 1));
        }
        _mapInfo = trm.Find("MapInfo/BG").GetComponent<RectTransform>();
        _mapInfo.anchoredPosition = new Vector3(0, 200, 0);
    }
    private void Awake()
    {
        //_skipTrue = true;
    }
    private void Start()
    {
        int index = PlayerDataManager.Instance.PlayerData.PlayerType;
        _playerImg.sprite = _playerSprites[index];
        _playerImg.rectTransform.anchoredPosition = _playerImgPos[index];
        _playerImg.rectTransform.localScale = _playerImgScale[index];
    }
    float time = 0;
    public void EnablePlayerInfo(bool enable)
    {
        _playerInfo.gameObject.SetActive(enable);
    }
    public void ShowMapInfo()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_mapInfo.DOAnchorPosY(-77, .5f).SetEase(Ease.InQuad));
        seq.AppendInterval(2f);
        seq.Append(_mapInfo.DOAnchorPosY(200, 1).SetEase(Ease.OutQuad));

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&&!GameManager_Lobby._instance._isVideo&& !DialogUI.Instance.Dialoging && _playerInfo.gameObject.activeSelf && !PauseUI.Instance.gameObject.activeSelf && !MenuUI_Lobby.Instance.gameObject.activeSelf)
        {

            PauseUI.Instance.EnableUI();
        }

        if (Input.GetKey(KeyCode.Space) && _skipTrue && !GameManager_Lobby._instance._pC.IsMoveTrue)
        {
            time += Time.deltaTime;
            _skipProgress.fillAmount = time;
            _skipProgressBar.alpha = 1;
            _skipText.DOFade(1, 0);
            if(_skipProgress.fillAmount >= .99f)
            {
                _skipTrue = false;
                TimeLineManager.Instance.SkipTimeLine();
                FalseSkip();
            }
        }
        if (Input.GetKeyUp(KeyCode.Space) && !GameManager_Lobby._instance._pC.IsMoveTrue && !_skipCanceling)
        {
            _skipCanceling = true;

            Invoke("CallFalseSkip", 1);
        }
    }
    private void CallFalseSkip()
    {
        FalseSkip();
    }
   public void FalseSkip(float time= 1)
    {

        this.time = 0;
        _skipProgressBar.DOFade(0, time).OnComplete(() =>
        {
            _skipProgress.fillAmount = 0;
            _skipCanceling = false;
        }) ;
        _skipText.DOFade(0, time);
    }
    public void SetPlayerInfo()
    {
        _playerNameText.text = PlayerDataManager.Instance.SavePlayerData.PlayerName;
        if(PlayerDataManager.Instance.PlayerData.CurrentEngine != null)
        _playerClassText.text = PlayerDataManager.Instance.PlayerData.CurrentEngine.ItemName;
        else
        _playerClassText.text = "";

    }
}
