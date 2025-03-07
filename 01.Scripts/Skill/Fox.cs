using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{
    private SpriteRenderer _renderer;
    [SerializeField, ColorUsage(true,true)]
    private Color _changeColor;


    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();

        _renderer.DOFade(0, 0);
    }
   public void Effect()
    {
        gameObject.SetActive(true);
        Sequence seq = DOTween.Sequence();
        seq.Append(_renderer.DOFade(1, 1));
        seq.AppendInterval(.5f);
        seq.Append(_renderer.DOColor(Color.white, 1));
        seq.Join(_renderer.material.DOColor(_changeColor, "_Color",1));
        seq.Join(transform.DOScale(new Vector3(1f, 1f), 1));
        seq.AppendInterval(.1f);
        seq.AppendCallback(() =>
        {
            EnemyBase[] Enemies = FindObjectsOfType<EnemyBase>();
            for(int i=0; i<Enemies.Length;i++)
            {
                Enemies[i].ApplyDamage(10000);
            }
            Obstacle obs = FindObjectOfType<Obstacle>();
            if (obs.gameObject.activeSelf)
            {
                obs.ApplyDamage(10000);
            }
        });
        seq.Join(_renderer.DOFade(0, 1).OnComplete(()=>
        {
            seq.Rewind();
        }));

    }
}
