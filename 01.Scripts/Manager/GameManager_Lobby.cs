using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_Lobby : Singleton<GameManager_Lobby>
{
    private SpriteRenderer _engineSpriteRenderer;
    private SpriteRenderer _attackSpriteRenderer;
    private SpriteRenderer _baseSpriteRenderer;
    public PlayerController_Lobby _pC;
    private Image _fade;

    [HideInInspector]
    public bool _isAracdeMachine;
    [HideInInspector]
    public bool _isVideo;
    [SerializeField]
    private GameObject _canvas;

    [SerializeField]
    private GameObject[] _playerPrefabs;
    [SerializeField]
    private Transform _startPlayerTrans;

    [SerializeField]
    private Sprite[] _playerSprites;

    [SerializeField]
    private Vector2[] _playerImgPos;
    [SerializeField]
    private Vector2[] _playerImgScale;

    [SerializeField]
    private GameObject[] _attackKeyGuides;

    [SerializeField]
    private Material[] itemMats;
    protected override void Awake()
    {
        base.Awake();
        CreateCameraManager();
        CreateUIManager();
        Transform Player = Instantiate(_playerPrefabs[PlayerDataManager.Instance.PlayerData.PlayerType], _startPlayerTrans.position, Quaternion.identity).transform;

        CameraManager_Lobby.Instance.SetPlayerCam(Player);

        _canvas = GameObject.Find("Canvas");
        _fade = _canvas.transform.Find("Fade").GetComponent<Image>();

        Transform ship = GameObject.Find("Ship").transform;
        _engineSpriteRenderer = ship.Find("Engine").GetComponent<SpriteRenderer>();
        _attackSpriteRenderer = ship.Find("Weapon").GetComponent<SpriteRenderer>();
        _baseSpriteRenderer = ship.Find("Base").GetComponent<SpriteRenderer>();
        _fade.gameObject.SetActive(true);
        _fade.DOFade(1, 0);
        _fade.DOFade(0, 1).OnComplete(() => _fade.gameObject.SetActive(false));
        EnableCanvas(true);
        _pC = Player.GetComponent<PlayerController_Lobby>();

        if (PlayerDataManager.Instance.PlayerData.PlayerType == 0)
        {
            _attackKeyGuides[0].SetActive(true);
            _attackKeyGuides[1].SetActive(false);
        }
        else
        {
            _attackKeyGuides[1].SetActive(true);
            _attackKeyGuides[0].SetActive(false);
        }
    }
    private void Start()
    {
        if (PlayerDataManager.Instance.PlayerData.PlayerType == 0)
        {


            if (PlayerDataManager.Instance.PlayerData.CurrentEngine == null || PlayerDataManager.Instance.PlayerData.CurrentEngine.ItemName == "")
            {
                ShopUI.Instance._currentSelectedItemData = ShopUI.Instance.transform.Find("EngineScrollView/Viewport/Content/EngineBaseItem").GetComponent<Item>();
                ShopUI.Instance.BuyorEquipItem(true, false);
                ShopUI.Instance.BuyorEquipItem(true, false);
                ShopUI.Instance._currentSelectedItemData = ShopUI.Instance.transform.Find("WeaponScrolls/WeaponsScrollView/Viewport/Content/WeaponBaseItem").GetComponent<Item>();
                ShopUI.Instance.BuyorEquipItem(true, false);
                ShopUI.Instance.BuyorEquipItem(true, false);
            }
            else
            {
                Item[] items = FindObjectsOfType<Item>();
                for (int i = 0; i < items.Length; i++)
                {
                    for (int j = 0; j < PlayerDataManager.Instance.PlayerData.EarnedEngine.Count; j++)
                        if (items[i].ItemData.ItemName == PlayerDataManager.Instance.PlayerData.EarnedEngine[j].ItemName)
                        {
                            ItemData item = items[i].ItemData;
                            items[i].ItemData = PlayerDataManager.Instance.PlayerData.EarnedEngine[j];
                            items[i].ItemData.Index = item.Index;
                            items[i].ItemData.skillType = item.skillType;
                            items[i].ItemData.SkillCoolTime = item.SkillCoolTime;
                            items[i].ItemData.SkillInfo = item.SkillInfo;
                            items[i].ItemData.SkillName = item.SkillName;
                            PlayerDataManager.Instance.PlayerData.EarnedEngine[j].skillType = item.skillType;
                            PlayerDataManager.Instance.PlayerData.EarnedEngine[j].SkillCoolTime = item.SkillCoolTime;
                            PlayerDataManager.Instance.PlayerData.EarnedEngine[j].SkillInfo = item.SkillInfo;
                            PlayerDataManager.Instance.PlayerData.EarnedEngine[j].SkillName = item.SkillName;
                            items[i].Refresh();
                        }
                    for (int j = 0; j < PlayerDataManager.Instance.PlayerData.EarnedWeapon.Count; j++)
                        if (items[i].ItemData.ItemName == PlayerDataManager.Instance.PlayerData.EarnedWeapon[j].ItemName)
                        {
                            ItemData item = items[i].ItemData;
                            items[i].ItemData = PlayerDataManager.Instance.PlayerData.EarnedWeapon[j];
                            items[i].ItemData.Index = item.Index;
                            items[i].ItemData.skillType = item.skillType;
                            items[i].ItemData.SkillCoolTime = item.SkillCoolTime;
                            items[i].ItemData.SkillInfo = item.SkillInfo;
                            items[i].ItemData.SkillName = item.SkillName;
                            PlayerDataManager.Instance.PlayerData.EarnedWeapon[j].skillType = item.skillType;
                            PlayerDataManager.Instance.PlayerData.EarnedWeapon[j].SkillCoolTime = item.SkillCoolTime;
                            PlayerDataManager.Instance.PlayerData.EarnedWeapon[j].SkillInfo = item.SkillInfo;
                            PlayerDataManager.Instance.PlayerData.EarnedWeapon[j].SkillName = item.SkillName;
                            items[i].Refresh();
                        }
                    for (int j = 0; j < PlayerDataManager.Instance.PlayerData.EarnedEngine.Count; j++)
                    {
                        if (PlayerDataManager.Instance.PlayerData.EarnedEngine[j].ItemName == PlayerDataManager.Instance.PlayerData.CurrentEngine.ItemName)
                        {
                            ItemData itemdata = PlayerDataManager.Instance.PlayerData.EarnedEngine[j];
                            PlayerDataManager.Instance.PlayerData.CurrentEngine.skillType = itemdata.skillType;
                            PlayerDataManager.Instance.PlayerData.CurrentEngine.SkillCoolTime = itemdata.SkillCoolTime;
                            PlayerDataManager.Instance.PlayerData.CurrentEngine.SkillInfo = itemdata.SkillInfo;
                            PlayerDataManager.Instance.PlayerData.CurrentEngine.SkillName = itemdata.SkillName;
                        }

                    }
                    for (int j = 0; j < PlayerDataManager.Instance.PlayerData.EarnedWeapon.Count; j++)
                        if (PlayerDataManager.Instance.PlayerData.EarnedWeapon[j].ItemName == PlayerDataManager.Instance.PlayerData.CurrentWeapon.ItemName)
                        {
                            ItemData itemdata = PlayerDataManager.Instance.PlayerData.EarnedWeapon[j];
                            PlayerDataManager.Instance.PlayerData.CurrentWeapon.skillType = itemdata.skillType;
                            PlayerDataManager.Instance.PlayerData.CurrentWeapon.SkillCoolTime = itemdata.SkillCoolTime;
                            PlayerDataManager.Instance.PlayerData.CurrentWeapon.SkillInfo = itemdata.SkillInfo;
                            PlayerDataManager.Instance.PlayerData.CurrentWeapon.SkillName = itemdata.SkillName;
                        }
                }
                ChangeShipSprite(PlayerDataManager.Instance.PlayerData.CurrentWeapon);
                ChangeShipSprite(PlayerDataManager.Instance.PlayerData.CurrentEngine);
                InfinityAdvetureUI.Instance.Refresh();



            }
        }
        else
        {
            if (PlayerDataManager.Instance.PlayerData.CurrentWeapon == null || PlayerDataManager.Instance.PlayerData.CurrentWeapon.ItemName == "")
            {
                ShopUI.Instance._currentSelectedItemData = ShopUI.Instance.transform.Find("WeaponScrolls/WeaponsScrollView (1)/Viewport/Content/WeaponCodeZero02").GetComponent<Item>();
                ShopUI.Instance.BuyorEquipItem(true, false);
                ShopUI.Instance.BuyorEquipItem(true, false);
            }
            else
            {
                Item[] items = FindObjectsOfType<Item>();
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].ItemData.ItemName == "코드제로02")
                    {
                        ItemData item = items[i].ItemData;
                        items[i].ItemData = PlayerDataManager.Instance.PlayerData.CurrentWeapon;
                        items[i].ItemData.Index = item.Index;
                        items[i].ItemData.skillType = item.skillType;
                        items[i].ItemData.SkillCoolTime = item.SkillCoolTime;
                        items[i].ItemData.SkillInfo = item.SkillInfo;
                        items[i].ItemData.SkillName = item.SkillName;
                        items[i].Refresh();
                        break;
                    }
                }
            }
            if(PlayerDataManager.Instance.PlayerData.MaxHP == 0)
            {
                PlayerDataManager.Instance.PlayerData.MaxHP = 100;
                PlayerDataManager.Instance.PlayerData.MoveSpeed = 6;
            }
            ChangeShipSprite(null);
            InfinityAdvetureUI.Instance.Refresh();
        }
        MenuUI_Lobby.Instance.EnableGroup(100);
        MenuUI_Lobby.Instance.gameObject.SetActive(false);
        UIManager_Lobby.Instance.SetPlayerInfo();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void EnableCanvas(bool enable)
    {
        _canvas.SetActive(enable);
    }
    public void FadeOut()
    {
        _fade.DOFade(0, 1).OnComplete(() => _fade.gameObject.SetActive(false));
    }
    public void FadeIn(Action action = null)
    {
        _fade.gameObject.SetActive(true);
        _fade.DOFade(1, 1).SetUpdate(true).OnComplete(() => action?.Invoke());
    }
    private void CreateUIManager()
    {
        GameObject obj = new GameObject("UIManager");
        UIManager_Lobby.Instance = obj.AddComponent<UIManager_Lobby>();
        obj.transform.parent = transform;
        obj.gameObject.SetActive(true);
        UIManager_Lobby.Instance.Init(GameObject.Find("Canvas").GetComponent<Canvas>().transform, _playerSprites, _playerImgPos, _playerImgScale);
    }
    public void SetpC(PlayerController_Lobby pC)
    {
        _pC = pC;
    }
    public void ChangeShipSprite(ItemData itemData)
    {
        if (itemData == null)
        {
            _attackSpriteRenderer.gameObject.SetActive(false);
            _engineSpriteRenderer.gameObject.SetActive(false);
            _baseSpriteRenderer.gameObject.SetActive(false);
        }
        else
        {

            Sprite[] sprites = Resources.LoadAll<Sprite>(itemData.spritePath);
            if (itemData.itemType == ItemType.WEAPON)
                _attackSpriteRenderer.sprite = sprites[0];
            else
            {
                _engineSpriteRenderer.sprite = Resources.Load<Sprite>(itemData.spritePath);

            }
            if (PlayerDataManager.Instance.PlayerData.PlayerType == 0)
            {
                if (PlayerDataManager.Instance.PlayerData.CurrentEngine.level >= 5)
                {
                    _engineSpriteRenderer.material = itemMats[1];
                }
                else
                {
                    _engineSpriteRenderer.material = itemMats[0];
                }

            }
            if (PlayerDataManager.Instance.PlayerData.CurrentWeapon.level >= 5)
            {
                _attackSpriteRenderer.material = itemMats[1];
            }
            else
            {
                _attackSpriteRenderer.material = itemMats[0];

            }
        }
    }
    private void CreateCameraManager()
    {
        GameObject obj = new GameObject("CameraManager");
        CameraManager_Lobby.Instance = obj.AddComponent<CameraManager_Lobby>();
        obj.transform.parent = transform;
        CameraManager_Lobby.Instance.Init();
    }
}
