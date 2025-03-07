using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcadeUI : MonoBehaviour
{
    private Image _fade;
    public static ArcadeUI Instance;
    private void Awake()
    {
        Instance = this;
        _fade = transform.Find("Fade").GetComponent<Image>();
        _fade.DOColor(new Color(0, 0, 0), 0);
        _fade.material.SetFloat("_DissolveAmount", 0);
        _fade.gameObject.SetActive(true);
    }
    public void FadeIn()
    {
        _fade.DOColor(new Color(0, 0, 23 / 255), 1).OnComplete(()=>
        {
            _fade.material.DOFloat(2, "_DissolveAmount", 1).OnComplete(() => _fade.gameObject.SetActive(false));

        });
    }
    public void FadeOut()
    {
            _fade.gameObject.SetActive(true);
        _fade.material.DOFloat(0, "_DissolveAmount", 1).OnComplete(() =>
        {
            _fade.DOColor(new Color(0, 0, 0), .5f);
        });

    }
}
