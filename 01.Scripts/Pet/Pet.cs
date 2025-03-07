using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    private Animator _anim;

    [SerializeField]
    private float _attackDelay;

    private ParticleSystem _teleport;

    private PlayerControllerBase _pC;

    private bool _following;
    private int prevBehaviour = -2;

    private Collider2D attackCol;
    private void Awake()
    {
        _teleport = transform.Find("Teleport").GetComponent<ParticleSystem>();
        _anim = GetComponent<Animator>();
        attackCol = GetComponent<Collider2D>();
        _pC = FindObjectOfType<PlayerControllerBase>();
        attackCol.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        WaitForSeconds ws = new WaitForSeconds(Random.Range( _attackDelay,_attackDelay+2));

        yield return ws;
        int index = Random.Range(0, 4);
        while (prevBehaviour == index)
        {
            index = Random.Range(0, 3);
            yield return null;
        }
        if (GameManager._instance.ApperingBoss) index = 3;
        prevBehaviour = index;
        Sequence seq = DOTween.Sequence();
        switch (index)
        {
            case 0:
                _anim.Play("Attack0");
                float y = transform.position.y;
                attackCol.enabled = true;
                seq.Append(transform.DOMoveY(4, 2).SetEase(Ease.Linear));
                seq.Join(transform.DORotate(new Vector3(0, 0, 0), 1));
                seq.AppendInterval(.5f);
                seq.Append(transform.DOMoveY(y, 2).SetEase(Ease.Linear));
                seq.Join(transform.DORotate(new Vector3(0, 0, 180), 1));
                seq.OnComplete(() =>
                {
                    attackCol.enabled = false;
                    seq.Join(transform.DORotate(new Vector3(0, 0, 0), 1));
                    _anim.Play("Idle");
                    StartCoroutine(Attack());
                });

                break;
            case 1:
                EnemyBase[] enemybases = FindObjectsOfType<EnemyBase>();
                int prevIndex = -2;
                for (int i = 0; i < enemybases.Length * .5f; i++)
                {
                    if (enemybases[i] as Boss !=null ) continue;
                    yield return new WaitForSeconds(.1f);
                    attackCol.enabled = true;
                    int idx = Random.Range(0, enemybases.Length);
                    while (!enemybases[idx].gameObject.activeSelf || prevIndex == idx)
                    {
                        idx = Random.Range(0, enemybases.Length);
                        yield return null;
                    }
                    _teleport.Play();
                    prevIndex = idx;
                    transform.position = enemybases[Random.Range(0, enemybases.Length)].transform.position;
                    enemybases[Random.Range(0, enemybases.Length)].ApplyDamage(555);
                    _anim.Play("Attack 1");
                    yield return new WaitForSeconds(.3f);
                    attackCol.enabled = false;
                    yield return new WaitForSeconds(.7f);
                }
                StartCoroutine(Attack());
                break;
            case 2:
                _following = true;
                _pC.EnableCol(false);
                yield return new WaitForSeconds(3.5f);
                _pC.EnableCol(true);
                _following = false;
                StartCoroutine(Attack());
                break;
            case 3:

                if (_pC.HP <= _pC._maxHP && !_pC.Death)
                {
                    _pC.HP += 15;
                    _pC.HpUpSFX();

                }
                StartCoroutine(Attack());
                break;
        }
    }
    private void Update()
    {
        if (_following)
        {
            transform.position = Vector3.MoveTowards(transform.position, _pC.transform.position + new Vector3(0, 1), Time.deltaTime * 10f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.GetComponent<Boss>() != null) return;
            EnemyBase _enemy = collision.GetComponent<EnemyBase>();
            collision.GetComponent<EnemyBase>().ApplyDamage(555);
        }
    }
}
