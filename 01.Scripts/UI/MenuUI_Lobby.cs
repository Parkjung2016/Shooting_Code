using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuUI_Lobby : MonoBehaviour
{
    public static MenuUI_Lobby Instance;
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private GameObject[] _groups;
    private GameObject _closeBtn;
    [HideInInspector]
    public Button _infinityBtn;
    private GameObject _shopBtn;
    private void Awake()
    {
        Instance = this;
        _closeBtn = transform.Find("BtnBG/Button Close").gameObject;
        _infinityBtn = transform.Find("BtnBG/LayOutGroup/InfinityAdventureBtn").GetComponent<Button>();
        _shopBtn = transform.Find("BtnBG/LayOutGroup/Shop").gameObject;
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;


    }
    public void ShowMenu()
    {

        _infinityBtn.Select();
        gameObject.SetActive(true);
        _canvasGroup.DOFade(1, .5f);
        GameManager_Lobby._instance._pC._rb.velocity = Vector3.zero;
    }
    private void Start()
    {

    }
    public void Update()
    {
        if (_canvasGroup.alpha == 1 && Input.GetKeyDown(KeyCode.Escape) && !InfinityAdvetureUI.Instance.MapSelectEnable)
        {
            ExecuteEvents.Execute(_closeBtn.gameObject, new BaseEventData(UIManager_Lobby.Instance.EventSystem), ExecuteEvents.submitHandler);
        }
    }
    public void HideMenu()
    {
        if (InfinityAdvetureUI.Instance.MapSelectEnable) return;
        UIManager_Lobby.Instance.EnablePlayerInfo(true);
        _canvasGroup.DOFade(0, .5f).OnComplete(() =>
        {

            gameObject.SetActive(false);
            EnableGroup(100);
        });
        InfinityAdvetureUI.Instance.hideMapSelectGroup(0);
        DialogUI.Instance.ShowDialog();
        GameManager_Lobby._instance._pC.IsMoveTrue = true;
        CameraManager_Lobby.Instance.ChangeCam(0);
    }
    public void EnableGroup(int index)
    {
        if (InfinityAdvetureUI.Instance.MapSelectEnable) return;
        if (index != 100)
            SoundManager.Instance.ClickBtnAudio();
        for (int i = 0; i < _groups.Length; i++)
        {
            _groups[i].SetActive(false);
        }
        if (index < _groups.Length)
            _groups[index]?.SetActive(true);

        if (index != 0)
        {
            InfinityAdvetureUI.Instance.hideMapSelectGroup(0, index == 100 ? false : true);
        }
    }
    public void StartAdventure()
    {
        SoundManager.Instance.ClickBtnAudio();
        InfinityAdvetureUI.Instance.EnableMapselectGroup((x) =>
        {
            TimeLineManager.Instance.Sceneindex = x;
            HideMenu();
            InfinityAdvetureUI.Instance.seq.Kill();
            //SoundManager.Instance.FadeSound(0);
            DialogUI.Instance.HideDialog();
            GameManager_Lobby._instance._pC.gameObject.SetActive(false);
            TimeLineManager.Instance.PlayAdventureTimeLine();

        });
    }

}
