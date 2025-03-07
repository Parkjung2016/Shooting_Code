using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    private Button _player1SelectBtn;
    private Button _player2SelectBtn;

    private int _currentPlayerSelected;

    private Transform _group;

    private Text _playerNameText;
    private Text _playerInfoText;

    [SerializeField]
    private string[] _playerNames;
    [SerializeField]
    private string[] _playerInfos;

    private GameObject _selectBtn;

    private EventSystem _eventSystem;

    private Image _fade;
    private bool _selectTrue;

    [SerializeField]
    private GameObject[] containDatas;
    private void Awake()
    {
        _group = transform.Find("Group");
        _playerNameText = _group.Find("Name").GetComponent<Text>();
        _playerInfoText = _group.Find("Info").GetComponent<Text>();
        _player1SelectBtn = transform.Find("Player1/Button Accept").GetComponent<Button>();
        _player2SelectBtn = transform.Find("Player2/Button Accept").GetComponent<Button>();
        _group.gameObject.SetActive(false);
        _selectBtn = _group.Find("Button Accept").gameObject;
        _eventSystem = FindObjectOfType<EventSystem>();
        _fade = transform.Find("Fade").GetComponent<Image>();
        _fade.DOFade(0,0);
        _fade.gameObject.SetActive(false);
        for(int i=0; i<2;i++)
        containDatas[i].SetActive(PlayerDataManager.Instance.SavePlayerData.datas[i].MaxHP != 0);
    }
    private void Update()
    {
        if (_fade.color.a > 0) return;
        if (Input.GetKeyDown(KeyCode.LeftArrow) && _currentPlayerSelected != 0)
        {
            _currentPlayerSelected = 0;
            _player1SelectBtn.Select();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && _currentPlayerSelected != 1)
        {
            _currentPlayerSelected = 1;
            _player2SelectBtn.Select();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (_group.gameObject.activeSelf && _selectTrue)
                ExecuteEvents.Execute(_selectBtn, new BaseEventData(_eventSystem), ExecuteEvents.submitHandler);
            else EnablePlayerInfo(_currentPlayerSelected);
        }
    }
    public void EnablePlayerInfo(int num)
    {
        SoundManager.Instance.ClickBtnAudio();
        StartCoroutine(SelectTrue());
        _group.gameObject.SetActive(true);
        _currentPlayerSelected = num;
        _playerNameText.text = "<" + _playerNames[num] + ">";
        _playerInfoText.text = _playerInfos[num];
    }
    IEnumerator SelectTrue()
    {

        _selectTrue = false;
        yield return new WaitForSeconds(.1f);
        _selectTrue = true;
    }
    public void SelectPlayer()
    {
        SoundManager.Instance.ClickBtnAudio();
        PlayerDataManager.Instance.LoadData(_currentPlayerSelected);
        if (!PlayerDataManager.Instance.PlayerData.Begin)
        {
            PlayerDataManager.Instance.PlayerData.Begin = true;
            PlayerDataManager.Instance.PlayerData.PlayerType = _currentPlayerSelected;
        PlayerDataManager.Instance.SaveData();
        }
        _fade.gameObject.SetActive(true);
        _fade.DOFade(1, 1).OnComplete(() =>
            {
                // PlayerDataManager.Instance.LoadData(_currentPlayerSelected);
                LoadingSceneManager.LoadScene("Lobby");
            });
    }
}
