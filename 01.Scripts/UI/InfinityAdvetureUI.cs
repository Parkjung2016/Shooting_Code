using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System;

public class InfinityAdvetureUI : MonoBehaviour
{
    public static InfinityAdvetureUI Instance;
    private TextMeshProUGUI _fireRateText;
    private TextMeshProUGUI _hpText;
    private TextMeshProUGUI _powerText;
    private TextMeshProUGUI _speedText;
    private Image _engineImg;
    private Image _weaponImg;
    private GameObject _baseobj;

    public CanvasGroup _selectMapGroup;

    Action<int> action = null;

    public bool MapSelectEnable;
    public int _currentMapSelect;
    private MapSelect[] _mapSelects;
    private Button[] _mapSelectBtns;
    public Sequence seq;

    [SerializeField]
    private Material[] itemMats;
    private void Awake()
    {
        Instance = this;
        Transform Ship = transform.Find("Ship").transform;
        _fireRateText = Ship.Find("FireRate").GetComponent<TextMeshProUGUI>();
        _hpText = Ship.Find("HP").GetComponent<TextMeshProUGUI>();
        _powerText = Ship.Find("Power").GetComponent<TextMeshProUGUI>();
        _selectMapGroup = transform.Find("MapSelect").GetComponent<CanvasGroup>();
        _speedText = Ship.Find("Speed").GetComponent<TextMeshProUGUI>();
        _engineImg = Ship.Find("ShipEngine").GetComponent<Image>();
        _weaponImg = Ship.Find("ShipWeapon").GetComponent<Image>();
        _baseobj = Ship.Find("ShipBase").gameObject;
        _mapSelects = GetComponentsInChildren<MapSelect>();
        _mapSelectBtns = new Button[_mapSelects.Length];
        for (int i = 0; i < _mapSelectBtns.Length; i++)
        {
            _mapSelectBtns[i] = _mapSelects[i].transform.Find("SelectBtn").GetComponent<Button>();
        }
        _selectMapGroup.alpha = 0;
        _selectMapGroup.gameObject.SetActive(false);
        if (PlayerDataManager.Instance.PlayerData.PlayerType == 1)
        {
            seq = DOTween.Sequence();
            seq.Append(_weaponImg.rectTransform.DOAnchorPosY(8.41f, 1.3f).SetEase(Ease.OutSine));
            seq.Append(_weaponImg.rectTransform.DOAnchorPosY(-9.4f, 1).SetEase(Ease.InOutSine));
            seq.SetLoops(-1);
        }
    }
    private void Update()
    {
        if (MapSelectEnable)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) && _currentMapSelect != 1)
            {
                _currentMapSelect = 1;

                _mapSelects[0].OnPointerExit(null);
                _mapSelects[1].OnPointerEnter(null);
                _mapSelectBtns[1].Select();

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && _currentMapSelect != 0)
            {
                _currentMapSelect = 0;
                _mapSelects[1].OnPointerExit(null);
                _mapSelects[0].OnPointerEnter(null);
                _mapSelectBtns[0].Select();
            }
            //if(Input.GetKeyDown(KeyCode.Return))
            //{
            //    if(_currentMapSelect== 0 )
            //    {
            //        ExecuteEvents.Execute(_mapSelects[0].transform.Find("SelectBtn").gameObject, new BaseEventData(UIManager_Lobby.Instance.EventSystem), ExecuteEvents.submitHandler);
            //    }
            //  else  if (_currentMapSelect == 1)
            //    {
            //        ExecuteEvents.Execute(_mapSelects[1].transform.Find("SelectBtn").gameObject, new BaseEventData(UIManager_Lobby.Instance.EventSystem), ExecuteEvents.submitHandler);
            //    }
            //}
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                hideMapSelectGroup();

            }
        }
    }
    public void Refresh()
    {
        if (PlayerDataManager.Instance.PlayerData.CurrentWeapon != null)
        {
            if (PlayerDataManager.Instance.PlayerData.PlayerType == 0)
            {
                Sprite[] sprites = Resources.LoadAll<Sprite>(PlayerDataManager.Instance.PlayerData.CurrentWeapon.spritePath);
                _weaponImg.sprite = sprites[0];

            }
            else
            {
                _weaponImg.sprite = Resources.Load<Sprite>("Image/PNGs/Gear_Show");
                _weaponImg.rectTransform.anchoredPosition = new Vector2(-44.5f, -9.4f);
                _weaponImg.transform.rotation = Quaternion.Euler(0, 0, -45f);
                _weaponImg.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);


            }
            if (PlayerDataManager.Instance.PlayerData.CurrentWeapon.level >= 5)
            {
                _weaponImg.material = itemMats[1];
            }
            else
            {
                _weaponImg.material = itemMats[0];

            }
            _fireRateText.text = "공격 속도 : " + PlayerDataManager.Instance.PlayerData.FireRate.ToString();
            _powerText.text = "공격력 : " + PlayerDataManager.Instance.PlayerData.Damage.ToString();
        }
        _speedText.text = "이동 속도 : " + PlayerDataManager.Instance.PlayerData.MoveSpeed.ToString();
        _hpText.text = "체력 : " + PlayerDataManager.Instance.PlayerData.MaxHP.ToString();
        if (PlayerDataManager.Instance.PlayerData.PlayerType == 0)
        {

            _engineImg.sprite = Resources.Load<Sprite>(PlayerDataManager.Instance.PlayerData.CurrentEngine.spritePath);
            if (PlayerDataManager.Instance.PlayerData.CurrentEngine.level >= 5)
            {
                _engineImg.material = itemMats[1];
            }
            else
            {
                _engineImg.material = itemMats[0];

            }
        }
        else
        {
            if (PlayerDataManager.Instance.PlayerData.CurrentWeapon.level >= 5)
            {
                _weaponImg.material = itemMats[1];
            }
            else
            {
                _weaponImg.material = itemMats[0];

            }
            _baseobj.SetActive(false);
            _engineImg.sprite = Resources.Load<Sprite>("Image/PNGs/Player2");

        }
    }
    public void EnableMapselectGroup(Action<int> action)
    {
        if (MapSelectEnable) return;
        this.action = action;
        MapSelectEnable = true;
        _selectMapGroup.gameObject.SetActive(true);
        _selectMapGroup.DOFade(1, 1);

        for(int i =0;i <2;i++)
        {
            _mapSelects[i].Refresh();
        }

    }
    public void CallHideMapSelectGroup()
    {
        hideMapSelectGroup();
    }
    public void hideMapSelectGroup(float time = 1, bool Audio = true)
    {
        if (Audio)
            SoundManager.Instance.ClickBtnAudio();

        MapSelectEnable = false;
        for (int i = 0; i < _mapSelects.Length; i++)
        {
            _mapSelects[i].OnPointerExit(null);
            _mapSelects[i].IsPointerEntered = false;
        }
        MenuUI_Lobby.Instance._infinityBtn.Select();
        _selectMapGroup.DOFade(0, 1).OnComplete(() => _selectMapGroup.gameObject.SetActive(false));
    }
    public void SelectMap(int index)
    {
        if (_selectMapGroup.alpha >= .9f)
        {
            SoundManager.Instance.ClickBtnAudio();
            PlayerDataManager.Instance.MapIndex = index;
            MapSelectEnable = false;
            action?.Invoke(index);

        }
    }

}
