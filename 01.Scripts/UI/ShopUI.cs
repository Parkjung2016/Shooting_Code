using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;
    private Text _itemName;
    private Text[] _datasText = new Text[2];
    private Image _itemIcon;

    private Transform _itemInfoBG;

    [SerializeField]
    private Sprite[] BtnImgs;

    private Image _itemBuyBtnImg;
    private Text _itemBuyText;
    private Image _upgradeBtnImg;

    public Item _currentSelectedItemData;

    private Text _goldText;

    private TextMeshProUGUI _errorText;

    private Text _priceText;

    private Text _buyorUpgradeText;

    Coroutine _appearErrorCor;
    private Text _itemLevelText;

    private Item[] _itmes;

    private Button _itemBuyBtn;
    private Button _upgradeBtn;
    private Text _skillNameText;
    private Text _skillInfoText;
    private Image _skillIconImg;
    [SerializeField]
    private Sprite[] _skillSprites;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _equipSound;

    [SerializeField]
    private GameObject[] _player1ScrollViews;
    [SerializeField]
    private GameObject _player2ScrollView;

    public Material[] ItemMats;
    private GameObject _petGroup;

    private Image _petBuyBtn;
    private TextMeshProUGUI _petBuyError;

    private void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
        _itemInfoBG = transform.Find("ItemInfoBG").transform;
        _itemBuyBtnImg = _itemInfoBG.Find("BtnGroup/Buy Btn").GetComponent<Image>();
        _itemBuyText = _itemBuyBtnImg.transform.Find("Text").GetComponent<Text>();
        _goldText = transform.transform.Find("Gold/Value").GetComponent<Text>();
        _upgradeBtnImg = _itemInfoBG.Find("BtnGroup/Upgrade Btn").GetComponent<Image>();
        _upgradeBtn = _upgradeBtnImg.GetComponent<Button>();
        _itemBuyBtn = _itemBuyBtnImg.GetComponent<Button>();
        _itemName = _itemInfoBG.Find("Name").GetComponent<Text>();
        _itemIcon = _itemInfoBG.Find("Item/Icon/Image/ItemIcon").GetComponent<Image>();
        _datasText[0] = _itemInfoBG.Find("Data").GetComponent<Text>();
        _datasText[1] = _itemInfoBG.Find("Data2").GetComponent<Text>();
        _petGroup = transform.Find("Pet").gameObject;
        _errorText = _itemInfoBG.Find("Error").GetComponent<TextMeshProUGUI>();
        _priceText = _itemInfoBG.Find("Gold/Value").GetComponent<Text>();
        _buyorUpgradeText = _itemInfoBG.Find("Gold/What").GetComponent<Text>();
        _itemLevelText = _itemInfoBG.Find("Item/Level").GetComponent<Text>();
        _petBuyBtn = _petGroup.transform.Find("Buy Btn").GetComponent<Image>();
        _itemLevelText.gameObject.SetActive(false);
        _itmes = FindObjectsOfType<Item>();
        _skillIconImg = _itemInfoBG.Find("SkillIcon").GetComponent<Image>();
        _skillNameText = _skillIconImg.transform.Find("SkillName").GetComponent<Text>();
        _skillInfoText = _skillIconImg.transform.Find("SkillInfo").GetComponent<Text>();
        _player1ScrollViews = new GameObject[2];
        _player1ScrollViews[0] = transform.Find("WeaponScrolls/WeaponsScrollView").gameObject;
        _player1ScrollViews[1] = transform.Find("EngineScrollView").gameObject;
        _player2ScrollView = transform.Find("WeaponScrolls/WeaponsScrollView (1)").gameObject;
        _petBuyError = _petGroup.transform.Find("Error").GetComponent<TextMeshProUGUI>();
        if (PlayerDataManager.Instance.PlayerData.PlayerType == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                _player1ScrollViews[i].SetActive(true);
            }
            _player2ScrollView.SetActive(false);
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                _player1ScrollViews[i].SetActive(false);
            }
            _player2ScrollView.SetActive(true);
        }
        _petBuyError.gameObject.SetActive(false);
    }
    private void Start()
    {
        //if (PlayerDataManager.Instance.PlayerData.PlayerName=="")
        //{
        //    _currentSelectedItemData = transform.Find("EngineScrollView/Viewport/Content/EngineBaseItem").GetComponent<Item>();
        //    BuyorEquipItem(true);
        //    BuyorEquipItem(true);
        //    _currentSelectedItemData = transform.Find("WeaponsScrollView/Viewport/Content/WeaponBaseItem").GetComponent<Item>();
        //    BuyorEquipItem(true);
        //    BuyorEquipItem(true);
        //}
        //else
        //{
        //    Item[] items = FindObjectsOfType<Item>();
        //    for (int i = 0; i < items.Length; i++)
        //    {
        //        for (int j = 0; j < PlayerDataManager.Instance.PlayerData.EarnedEngine.Count; j++)
        //            if (items[i].ItemData.ItemName == PlayerDataManager.Instance.PlayerData.EarnedEngine[j].ItemName)
        //            {
        //                items[i].ItemData = PlayerDataManager.Instance.PlayerData.EarnedEngine[j];
        //                items[i].Refresh();
        //            }
        //        for (int j = 0; j < PlayerDataManager.Instance.PlayerData.EarnedWeapon.Count; j++)
        //            if (items[i].ItemData.ItemName == PlayerDataManager.Instance.PlayerData.EarnedWeapon[j].ItemName)
        //            {
        //                items[i].ItemData = PlayerDataManager.Instance.PlayerData.EarnedWeapon[j];
        //                items[i].Refresh();
        //            }
        //    }
        //    GameManager_Lobby._instance.ChangeShipSprite(PlayerDataManager.Instance.PlayerData.CurrentWeapon);
        //    GameManager_Lobby._instance.ChangeShipSprite(PlayerDataManager.Instance.PlayerData.CurrentEngine);
        //    InfinityAdvetureUI.Instance.Refresh();
        //    //print(Array.Find(items, x => x.ItemData.ItemName == PlayerDataManager.Instance.PlayerData.CurrentEngine.ItemName));
        //    //Item item = Array.Find(items, x => x.ItemData.ItemName == PlayerDataManager.Instance.PlayerData.CurrentEngine.ItemName);
        //    //item.ItemData = PlayerDataManager.Instance.PlayerData.CurrentEngine;
        //    //item.Refresh();
        //    //item = Array.Find(items, x => x.ItemData.ItemName == PlayerDataManager.Instance.PlayerData.CurrentWeapon.ItemName);
        //    //item.ItemData = PlayerDataManager.Instance.PlayerData.CurrentWeapon;
        //    //item.Refresh();


        //}
        _itemInfoBG.gameObject.SetActive(false);
        SetGoldText();
        _errorText.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        _errorText.gameObject.SetActive(false);
        _petBuyError.gameObject.SetActive(false);
        CheckPet();
    }
    public void CheckPet()
    {
            _petGroup.SetActive(!PlayerDataManager.Instance.PlayerData.Pet);
        if (!PlayerDataManager.Instance.PlayerData.Pet)
        {
            _petBuyBtn.sprite = PlayerDataManager.Instance.PlayerData.Gold >= 3000 ? BtnImgs[0] : BtnImgs[1];
        }
    }
    public void SetGoldText()
    {
        _goldText.text = PlayerDataManager.Instance.PlayerData.Gold.ToString();
    }
    public void ShowItemInfo(Item itemData)
    {
        _currentSelectedItemData = itemData;
        _itemInfoBG.gameObject.SetActive(true);
        if (checkContainsItem(itemData.ItemData))
        {
            _itemBuyBtnImg.sprite = BtnImgs[0];
            _itemBuyBtnImg.gameObject.SetActive(
                 !checkiscurrentEquipedItem(itemData.ItemData));
            _itemBuyText.text = "장착하기";
            _upgradeBtn.gameObject.SetActive(true);
            _priceText.text = itemData.ItemData.ReinforcePrice.ToString();
            _buyorUpgradeText.text = "진화";
            _upgradeBtnImg.sprite = itemData.ItemData.ReinforcePrice <= PlayerDataManager.Instance.PlayerData.Gold ? BtnImgs[0] : BtnImgs[1];


            if (_currentSelectedItemData.ItemData.Level == 0)
            {

                _itemIcon.material = ItemMats[0];
                _itemLevelText.gameObject.SetActive(false);
            }
            else
            {
                if (_currentSelectedItemData.ItemData.Level >= 5)
                {
                    _itemIcon.material = ItemMats[1];
                }
                else
                    _itemIcon.material = ItemMats[0];

                _itemLevelText.gameObject.SetActive(true);
                _itemLevelText.text = "+" + _currentSelectedItemData.ItemData.Level.ToString();

            }

        }
        else
        {
            _itemIcon.material = ItemMats[0];
            _itemBuyBtnImg.gameObject.SetActive(true);
            _itemBuyText.text = "구매하기";
            _buyorUpgradeText.text = "구매";
            _priceText.text = itemData.ItemData.Price.ToString();
            _itemBuyBtnImg.sprite = itemData.ItemData.Price <= PlayerDataManager.Instance.PlayerData.Gold ? BtnImgs[0] : BtnImgs[1];
            _upgradeBtn.gameObject.SetActive(false);

            _itemLevelText.gameObject.SetActive(false);
        }
        if (itemData.ItemData.itemType == ItemType.ENGINE || PlayerDataManager.Instance.PlayerData.PlayerType == 1)
        {
            _skillIconImg.enabled = true;
            _skillIconImg.gameObject.SetActive(true);
            _skillNameText.gameObject.SetActive(true);
            _skillIconImg.sprite = _skillSprites[(byte)itemData.ItemData.skillType];
            _skillNameText.text = itemData.ItemData.SkillName;
        }
        else
        {

            _skillIconImg.enabled = false;
            _skillNameText.gameObject.SetActive(false);
        }
        _skillInfoText.text = itemData.ItemData.SkillInfo;
        if (_itemBuyBtnImg.gameObject.activeSelf)
        {
            _itemBuyBtn.Select();
        }
        else
            _upgradeBtn.Select();
        _itemName.text = itemData.ItemData.ItemName;
        Sprite[] sprites = Resources.LoadAll<Sprite>(itemData.ItemData.spritePath);
        if (sprites.Length != 0)
            _itemIcon.sprite = sprites[0];
        else
            _itemIcon.sprite = Resources.Load<Sprite>(itemData.ItemData.spritePath);
        for (int i = 0; i < 2; i++)
        {
            _datasText[i].text = $"{itemData.ItemData.Datas[i]} : {itemData.ItemData.Values[i]}";
        }
    }

    public void BuyorEquipItemBtn()
    {

        BuyorEquipItem();
    }
    public void BuyPet()
    {
        SoundManager.Instance.ClickBtnAudio();
        if (PlayerDataManager.Instance.PlayerData.Gold >= 3000)
        {
            PlayerDataManager.Instance.PlayerData.Pet = true;
            SoundManager.Instance.SuccessAudio();
            CheckPet();
        }
        else
        {
            StartCoroutine(AppearErrorText(_petBuyError, "<shake>돈이 부족합니다!</shake>", Color.red));
            SoundManager.Instance.ErrorAudio();
        }
    }
    public void BuyorEquipItem(bool cheat = false, bool audio = false)
    {
        if (audio)
            SoundManager.Instance.ClickBtnAudio();
        if (checkContainsItem(_currentSelectedItemData.ItemData))
        {
            _audioSource.PlayOneShot(_equipSound);
            switch (_currentSelectedItemData.ItemData.itemType)
            {
                case ItemType.BASE:
                    break;
                case ItemType.WEAPON:
                    PlayerDataManager.Instance.PlayerData.CurrentWeapon = _currentSelectedItemData.ItemData;
                    PlayerDataManager.Instance.PlayerData.Damage = _currentSelectedItemData.ItemData.Values[0];
                    PlayerDataManager.Instance.PlayerData.FireRate = _currentSelectedItemData.ItemData.Values[1];
                    break;
                case ItemType.ENGINE:
                    PlayerDataManager.Instance.PlayerData.CurrentEngine = _currentSelectedItemData.ItemData;
                    PlayerDataManager.Instance.PlayerData.MaxHP = _currentSelectedItemData.ItemData.Values[0];
                    PlayerDataManager.Instance.PlayerData.MoveSpeed = _currentSelectedItemData.ItemData.Values[1];
                    break;
            }
            for (int i = 0; i < _itmes.Length; i++)
            {
                //if (PlayerDataManager.Instance.PlayerData.PlayerType == 0 /*&& _itmes[i].ItemData.ItemName == "코드제로02"*/) continue;
                _itmes[i].Refresh();
            }
            _currentSelectedItemData.Refresh();
            UIManager_Lobby.Instance.SetPlayerInfo();
            GameManager_Lobby._instance.ChangeShipSprite(_currentSelectedItemData.ItemData);
            InfinityAdvetureUI.Instance.Refresh();
        }
        else
        {
            if (!cheat)
            {
                if (_currentSelectedItemData.ItemData.Price <= PlayerDataManager.Instance.PlayerData.Gold)
                {
                    PlayerDataManager.Instance.PlayerData.Gold -= _currentSelectedItemData.ItemData.Price;
                    SoundManager.Instance.SuccessAudio();
                    SetGoldText();
                    switch (_currentSelectedItemData.ItemData.itemType)
                    {
                        case ItemType.BASE:
                            break;
                        case ItemType.WEAPON:
                            PlayerDataManager.Instance.PlayerData.EarnedWeapon.Add(_currentSelectedItemData.ItemData);
                            break;
                        case ItemType.ENGINE:
                            PlayerDataManager.Instance.PlayerData.EarnedEngine.Add(_currentSelectedItemData.ItemData);
                            break;
                    }
                }
                else
                {
                    StartCoroutine(AppearErrorText(_errorText, "<shake>돈이 부족합니다!</shake>", Color.red));
                    SoundManager.Instance.ErrorAudio();
                }
            }
            else
            {
                switch (_currentSelectedItemData.ItemData.itemType)
                {
                    case ItemType.BASE:
                        break;
                    case ItemType.WEAPON:
                        PlayerDataManager.Instance.PlayerData.EarnedWeapon.Add(_currentSelectedItemData.ItemData);
                        break;
                    case ItemType.ENGINE:
                        PlayerDataManager.Instance.PlayerData.EarnedEngine.Add(_currentSelectedItemData.ItemData);
                        break;
                }
            }
        }
        ShowItemInfo(_currentSelectedItemData);
    }

    public void Upgrade()
    {
        if (_currentSelectedItemData != null)
        {
            SoundManager.Instance.ClickBtnAudio();
            if (_currentSelectedItemData.ItemData.Level < 5)
            {
                if (_currentSelectedItemData.ItemData.ReinforcePrice <= PlayerDataManager.Instance.PlayerData.Gold)
                {
                    PlayerDataManager.Instance.PlayerData.Gold -= _currentSelectedItemData.ItemData.ReinforcePrice;
                    SetGoldText();
                    _currentSelectedItemData.ItemData.ReinforcePrice += _currentSelectedItemData.ItemData.AddReinforcePrice;
                    _currentSelectedItemData.ItemData.Level++;
                    _currentSelectedItemData.ItemData.Values[0] += _currentSelectedItemData.ItemData.UpgradeValue0;
                    _currentSelectedItemData.ItemData.Values[1] += _currentSelectedItemData.ItemData.UpgradeValue1;
                    if (PlayerDataManager.Instance.PlayerData.CurrentWeapon.ItemName == _currentSelectedItemData.ItemData.ItemName)
                    {
                        PlayerDataManager.Instance.PlayerData.CurrentWeapon.Level = _currentSelectedItemData.ItemData.Level;
                        PlayerDataManager.Instance.PlayerData.CurrentWeapon.Values[0] = _currentSelectedItemData.ItemData.Values[0];
                        PlayerDataManager.Instance.PlayerData.CurrentWeapon.Values[1] = _currentSelectedItemData.ItemData.Values[1];


                    }
                    else if (PlayerDataManager.Instance.PlayerData.CurrentEngine.ItemName == _currentSelectedItemData.ItemData.ItemName)
                    {
                        PlayerDataManager.Instance.PlayerData.CurrentEngine.Level = _currentSelectedItemData.ItemData.Level;
                        PlayerDataManager.Instance.PlayerData.CurrentEngine.Values[0] = _currentSelectedItemData.ItemData.Values[0];
                        PlayerDataManager.Instance.PlayerData.CurrentEngine.Values[1] = _currentSelectedItemData.ItemData.Values[1];
                    }
                    CallAppearErrorText("<bounce>진화에 성공하였습니다!</bounce>", Color.yellow);
                    SoundManager.Instance.SuccessAudio();
                    if (_currentSelectedItemData.ItemData.Level == 0)
                        _itemLevelText.gameObject.SetActive(false);
                    else
                    {
                        _itemLevelText.gameObject.SetActive(true);
                        _itemLevelText.text = _currentSelectedItemData.ItemData.Level.ToString();

                    }
                    _currentSelectedItemData.Refresh();
                    GameManager_Lobby._instance.ChangeShipSprite(_currentSelectedItemData.ItemData);
                    InfinityAdvetureUI.Instance.Refresh();
                    ShowItemInfo(_currentSelectedItemData);

                }
                else
                {
                    StartCoroutine(AppearErrorText(_errorText,"<shake>돈이 부족합니다!</shake>", Color.red));
                    SoundManager.Instance.ErrorAudio();
                }
            }
            else
            {
                CallAppearErrorText("<shake>최대 진화에 도달하였습니다!</shake>", Color.red);
                SoundManager.Instance.ErrorAudio();
            }
        }
    }
    void CallAppearErrorText(string text, Color color)
    {
        if (_appearErrorCor != null)
            StopCoroutine(_appearErrorCor);
        _appearErrorCor = StartCoroutine(AppearErrorText(_errorText,text, color));
    }
    IEnumerator AppearErrorText(TextMeshProUGUI text, string Text, Color color)
    {
        text.gameObject.SetActive(true);
        text.text = Text;
        text.color = color;
        yield return new WaitForSeconds(1);
        text.gameObject.SetActive(false);
    }
    bool checkContainsItem(ItemData itemData)
    {
        return PlayerDataManager.Instance.PlayerData.EarnedEngine.Exists(x => x.ItemName == itemData.ItemName) ||
            PlayerDataManager.Instance.PlayerData.EarnedWeapon.Exists(x => x.ItemName == itemData.ItemName);
    }
    bool checkiscurrentEquipedItem(ItemData itemData)
    {
        return
                 PlayerDataManager.Instance.PlayerData.CurrentEngine != null && PlayerDataManager.Instance.PlayerData.CurrentEngine.ItemName == itemData.ItemName ||
           PlayerDataManager.Instance.PlayerData.CurrentWeapon != null && PlayerDataManager.Instance.PlayerData.CurrentWeapon.ItemName == itemData.ItemName;
    }
}
