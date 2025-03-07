using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MapSelect : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    private Vector3 _originScale;

    public bool IsPointerEntered;
    [SerializeField]
    private int _mapIndex;
    private Text _highScoreText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!IsPointerEntered && InfinityAdvetureUI.Instance._selectMapGroup.alpha >=.8f)
        {
            InfinityAdvetureUI.Instance._currentMapSelect = _mapIndex;
            IsPointerEntered = true;
        transform.DOScale(Vector3.one * .63f,1);

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(IsPointerEntered)
        {
            InfinityAdvetureUI.Instance._currentMapSelect = 99;
           IsPointerEntered = false;
        transform.DOScale(_originScale,1);
        }
    }
    public void Refresh()
    {
        _highScoreText.text = "최고 점수 : "+PlayerDataManager.Instance.PlayerData.HighScore[_mapIndex];
    }
    private void Awake()
    {
        _highScoreText = transform.Find("HighScore").GetComponent<Text>();
        _originScale = transform.localScale;
        Refresh();
    }

    private void OnEnable()
    {
        transform.localScale = _originScale;
    }
}
