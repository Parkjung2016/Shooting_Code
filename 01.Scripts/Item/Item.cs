using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Item : MonoBehaviour
{
    public ItemData ItemData;

    private GameObject _clickImg;

    [SerializeField]
    private Text _levelText;

    [SerializeField]
    private GameObject EquipedImg;

    [SerializeField]
    private Image _itemImg;

    private void Awake()
    {
        _clickImg = transform.Find("Active").gameObject;
        //_itemImg = transform.Find("Icon/Image/ItemIcon").GetComponent<Image>();
        Sprite img = _itemImg.sprite;

        ItemData.spritePath = "Image/PNGs/" + img.name.Replace("_0", "");
        //_levelText = transform.Find("Level").GetComponent<Text>();
        //EquipedImg = transform.Find("Equiped").gameObject;

        Refresh();
    }
    public void ClickBtn()
    {
        SoundManager.Instance.ClickBtnAudio();
        Item[] items = FindObjectsOfType<Item>();
        for (int i = 0; i < items.Length; i++)
        {
            items[i].Hide();
        }
        _clickImg.SetActive(true);
        ShopUI.Instance.ShowItemInfo(this);
    }
    public void Refresh()
    {
        EquipedImg.SetActive(false);
        if (ItemData.Level == 0)
            _levelText.gameObject.SetActive(false);
        else
        {
            if(ItemData.Level>=5)
            {
                _itemImg.material = ShopUI.Instance.ItemMats[1];
            }
            else
            {
                _itemImg.material = ShopUI.Instance.ItemMats[0];

            }
            _levelText.gameObject.SetActive(true);
            _levelText.text = "+" + ItemData.Level.ToString();
        }
        if ((PlayerDataManager.Instance.PlayerData.PlayerType == 1 && PlayerDataManager.Instance.PlayerData.CurrentWeapon == null) ||
            PlayerDataManager.Instance.PlayerData.PlayerType == 0 &&
            (ItemData.itemType == ItemType.ENGINE && PlayerDataManager.Instance.PlayerData.CurrentEngine == null ||
            ItemData.itemType == ItemType.WEAPON && PlayerDataManager.Instance.PlayerData.CurrentWeapon == null))
            EquipedImg.SetActive(false);
        else
        {
            if (PlayerDataManager.Instance.PlayerData.PlayerType == 1)
            {
                EquipedImg.SetActive(
              ItemData.itemType == ItemType.WEAPON && ItemData.ItemName == PlayerDataManager.Instance.PlayerData.CurrentWeapon.ItemName);
            }
            else
            {
            EquipedImg.SetActive(ItemData.itemType == ItemType.ENGINE && ItemData.ItemName == PlayerDataManager.Instance.PlayerData.CurrentEngine.ItemName
                || ItemData.itemType == ItemType.WEAPON && ItemData.ItemName == PlayerDataManager.Instance.PlayerData.CurrentWeapon.ItemName);

            }
        }

    }
    public void Hide()
    {
        _clickImg.SetActive(false);
    }

}
