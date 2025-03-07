using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Hellmade.Sound;
using Random = UnityEngine.Random;

public class Enemy_Attack : EnemyBase
{
    [SerializeField]
    private float _fireRate;

    private Transform[] _fireTrans;

    private PoolableMono _currentBullet;

    [SerializeField]
    private BehaviourType _moveType;
    [SerializeField]
    private BehaviourType _beginMoveType;
    [SerializeField]
    private BehaviourType _attackType;
    Sequence seq;

    [SerializeField]
    private string _bulletName;

    [SerializeField]
    private GameObject _shadowPrefab;
    private Transform _shadow;

    [SerializeField]
    private AudioClip _attackAudio;
    [SerializeField]
    private AudioClip _attackReadyAudio;
    [SerializeField]
    private float _attackVolume;

    private int _id;
    public override void Init()
    {

        base.Init();
        if(_shadow != null)
        _shadow.gameObject.SetActive(true);
        BeginSeq();
    }

    protected override void Awake()
    {
        base.Awake();
        if (_shadowPrefab != null)
        {
            _shadow = Instantiate(_shadowPrefab, GameManager._instance.transform).transform;
            _shadow.gameObject.SetActive(false);
        }
        Transform firePos2 = transform.Find("FirePos2");
        Transform firePos3 = transform.Find("FirePos3");
        if (firePos3 != null)
        {
            _fireTrans = new Transform[4];
            _fireTrans[0] = transform.Find("FirePos");
            _fireTrans[1] = firePos2;
            _fireTrans[2] = firePos3;
            _fireTrans[3] = transform.Find("FirePos4");
        }
        else if (firePos2 != null)
        {
            _fireTrans = new Transform[2];
            _fireTrans[0] = transform.Find("FirePos");
            _fireTrans[1] = firePos2;

        }
        else
            _fireTrans = new Transform[1];
        _fireTrans[0] = transform.Find("FirePos");
    }

    private void Update()
    {
        if (_shadow != null)
        {
            _shadow.position = transform.position + new Vector3(0, -1.05f, 0);
            _shadow.rotation = transform.rotation;
        }
    }
    private void BeginSeq()
    {
        if (_death) return;
        Sequence seq = DOTween.Sequence();
        switch (_beginMoveType)
        {
            case BehaviourType.ONE:
                seq.Append(transform.DOMoveY(Camera.main.orthographicSize - 1.5f, 1f));
                seq.AppendInterval(.3f);
                seq.AppendCallback(() =>
                {
                    _trueDamaged = true;
                    Move();
                    seq.Kill();
                });
                break;
            case BehaviourType.TWO:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                seq.AppendCallback(() => transform.position = new Vector3(transform.position.x, -Camera.main.orthographicSize * 2 + .4f));
                transform.localScale = new Vector3(.2f, .2f);
                seq.Append(transform.DOMoveY(Camera.main.orthographicSize - 1.5f, 1f));
                seq.Join(transform.DOScale(new Vector3(1, 1, 1), 1));
                seq.Append(transform.DORotate(new Vector3(0, 0, 180), .5f));
                seq.AppendInterval(.3f);
                seq.AppendCallback(() =>
                {
                    _trueDamaged = true;
                    Move();
                    seq.Kill();
                });
                break;
            case BehaviourType.THREE:
                seq.AppendCallback(() => transform.position = new Vector3(transform.position.x, -Camera.main.orthographicSize * 2 + .4f));
                transform.localScale = new Vector3(.2f, .2f);
                seq.Append(transform.DOMoveY(Camera.main.orthographicSize - 1.5f, 1f));
                seq.Join(transform.DOScale(new Vector3(1, 1, 1), 1));
                seq.AppendInterval(1.2f);
                seq.AppendCallback(() =>
                {
                    _trueDamaged = true;
                    Move();
                    seq.Kill();
                });
                break;
            case BehaviourType.FOUR:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                seq.AppendCallback(() => transform.position = new Vector3(12.4f, UnityEngine.Random.Range(2.27f, 3.7f)));
                seq.Append(transform.DOMoveX(UnityEngine.Random.Range(-3f, 3f), 1));
                seq.Join(transform.DORotate(new Vector3(0, 0, 180), 1));
                seq.AppendInterval(.5f);
                seq.AppendCallback(() =>
                {
                    _trueDamaged = true;
                    Move();
                    seq.Kill();
                });
                break;
            case BehaviourType.FIVE:
                float minX  = Camera.main.ViewportToWorldPoint(new Vector3(0, 0)).x+.4f;
                float maxX= Camera.main.ViewportToWorldPoint(new Vector3(1, 1)).x-.4f;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                transform.position = new Vector3(Random.Range(minX,maxX), 7, 0);
                seq.Append(transform.DOMoveY(UnityEngine.Random.Range(2,4), 1));
                seq.AppendInterval(.5f);
                seq.AppendCallback(() =>
                {
                    _trueDamaged = true;
                    Move();
                    seq.Kill();
                });
                break;
        }

    }
    private void Move()
    {
        if (Player.Death || _death)
            return;
        seq = DOTween.Sequence();
        switch (_moveType)
        {
            case BehaviourType.ONE:
                if(_attackReadyAudio != null)
                {
                _id = EazySoundManager.PlaySound(_attackReadyAudio, _attackVolume);

                }
                seq = DOTween.Sequence();
                seq.Append(transform.DOMoveX(Player.transform.position.x, 1));
                seq.AppendCallback(() =>
                {

                    StartCoroutine(Attack());
                    });
                seq.AppendInterval(UnityEngine.Random.Range(2, _fireRate));
                seq.AppendCallback(() =>
                {
                    seq.Kill();
                    Move();

                });
                break;
            case BehaviourType.TWO:
                if (gameObject.activeSelf)
                    StartCoroutine(Attack(z =>
                    {
                        StopAllCoroutines();
                        Sequence seq = DOTween.Sequence();
                        seq.Append(transform.DOMoveY((z==180 ? -1 : 1)* Camera.main.orthographicSize * 2 - .3f, 1));
                        seq.AppendCallback(() =>
                        {
                            PoolManager.Instance.Push(this);
                            EnemySpawner._instance.BulletEnemyCount[_enemyIndex]--;
                        });

                    }));
                break;
            case BehaviourType.THREE:
                Coroutine coroutine = null;
                if (transform.position.y < Player.transform.position.y)
                {
                    seq.Append(transform.DOMoveY(UnityEngine.Random.Range(-2.6f, -1.15f), 1));
                    seq.Append(transform.DORotate(Vector3.zero, .5f, RotateMode.FastBeyond360));
                }
                else
                {

                    seq.Append(transform.DOMoveY(UnityEngine.Random.Range(1.5f, 3.6f), 1));
                    seq.Append(transform.DORotate(new Vector3(0, 0, 180), .5f, RotateMode.FastBeyond360));
                }
                seq.Append(transform.DOMoveX(-8.11f, 1)).OnStart(() => coroutine = StartCoroutine(Attack())).OnComplete(() => StopCoroutine(coroutine));
                seq.Join(transform.DOMoveY(UnityEngine.Random.Range(2, 4), 1));
                seq.AppendInterval(.3f);
                seq.Append(transform.DOMoveX(8.11f, 1)).OnStart(() => coroutine = StartCoroutine(Attack())).OnComplete(() => StopCoroutine(coroutine));
                seq.Join(transform.DOMoveY(UnityEngine.Random.Range(2, 4), 1));
                seq.AppendInterval(1.5f);
                seq.AppendCallback(() =>
                {
                    Move();
                });
                break;
            case BehaviourType.FOUR:
                seq.AppendCallback(() => StartCoroutine(Attack()));
                seq.AppendInterval(6f);
                seq.AppendCallback(() => Move());
                break;
            case BehaviourType.FIVE:
                seq.AppendCallback(() => StartCoroutine(Attack()));
                seq.AppendInterval(2f);
                seq.AppendCallback(() => Move());
                break;
        }


    }
    private void OnDisable()
    {
        seq.Kill();
    }
    public void CallAttack()
    {
        if (_death) return;
        float rotZ = 0f;
        if (transform.position.y < Player.transform.position.y)
        {
            rotZ = 0;
        }
        else
        {

            rotZ = 180;
        }
        for (int i = 0; i < _fireTrans.Length; i++)
        {


            _currentBullet = PoolManager.Instance.Pop(_bulletName);
            _currentBullet.transform.position = _fireTrans[i].position;
            _currentBullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotZ));
            if (_currentBullet is Bullet)
            {
                Bullet bullet = (_currentBullet as Bullet);
                EazySoundManager.PlaySound(_attackAudio);
                bullet._speed = 13;
                bullet.FollowTarget = null;
                bullet.Dir = rotZ == 180 ? Vector2.down : Vector2.up;
            }
        }
    }

    private IEnumerator Attack(Action<float> action = null)
    {
        if (Player.Death)
            StopAllCoroutines();
        _anim.Play("Attack");
        Sequence seq = DOTween.Sequence();
        float rot = 0f;
        switch (_attackType)
        {
            case BehaviourType.ONE:


                if (transform.position.y < Player.transform.position.y)
                {

                    seq.Append(transform.DOMoveY(UnityEngine.Random.Range(-2.6f, -1.15f), 1));
                    seq.Append(transform.DORotate(Vector3.zero, .5f, RotateMode.FastBeyond360));
                    rot = 180;
                }
                else
                {
                    seq.Append(transform.DOMoveY(UnityEngine.Random.Range(1.5f, 3.6f), 1));
                    seq.Append(transform.DORotate(new Vector3(0, 0, 180), .5f, RotateMode.FastBeyond360));

                    rot = 0;
                }
                seq.AppendCallback(() =>
                {
                    _currentBullet = PoolManager.Instance.Pop(_bulletName);
                    if (_attackReadyAudio != null)
                    {
                    EazySoundManager.GetSoundAudio(_id).Stop();

                    }
                        EazySoundManager.PlaySound(_attackAudio, _attackVolume);
                    if (_currentBullet is Bullet)
                    {
                        Bullet bullet = (_currentBullet as Bullet);
                        bullet._speed = 13;
                       bullet.FollowTarget = null;
                    }
                    _currentBullet.transform.position = _fireTrans[0].position;
                    _currentBullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));
                });
                break;
            case BehaviourType.TWO:

                if (transform.position.y < Player.transform.position.y)
                {
                    rot = 0;

                    seq.Append(transform.DOMoveY(UnityEngine.Random.Range(-2.6f, -1.15f), 1));
                    seq.Append(transform.DORotate(Vector3.zero, .5f, RotateMode.FastBeyond360));
                }
                else
                {

                    rot = 180;
                    seq.Append(transform.DOMoveY(UnityEngine.Random.Range(1.5f, 3.6f), 1));
                    seq.Append(transform.DORotate(new Vector3(0, 0, 180), .5f, RotateMode.FastBeyond360));
                }
                seq.AppendCallback(() =>
                {
                    Sequence seq2 = DOTween.Sequence();
                    seq2.AppendCallback(() =>
                    {

                        _currentBullet = PoolManager.Instance.Pop(_bulletName);
                        _currentBullet.transform.position = _fireTrans[0].position;
                        _currentBullet.transform.rotation = Quaternion.Euler(new Vector3(rot, _currentBullet.transform.rotation.y));
                        if (_currentBullet is Bullet)
                        {
                            Bullet bullet = (_currentBullet as Bullet);
                            EazySoundManager.PlaySound(_attackAudio, _attackVolume);
                            bullet.FollowTarget = null;
                            bullet._speed = 13;
                            bullet.Dir = rot == 180 ? Vector2.down : Vector2.up;
                        }
                    });
                    seq2.AppendInterval(.1f);
                    seq2.AppendCallback(() =>
                    {

                        _currentBullet = PoolManager.Instance.Pop(_bulletName);
                        _currentBullet.transform.position = _fireTrans[0].position;
                        _currentBullet.transform.rotation = Quaternion.Euler(new Vector3(rot, _currentBullet.transform.rotation.y));
                        if (_currentBullet is Bullet)
                        {
                            Bullet bullet = (_currentBullet as Bullet);
                            EazySoundManager.PlaySound(_attackAudio, _attackVolume);
                            bullet.FollowTarget = null;
                            bullet._speed = 13;
                            bullet.Dir = rot == 180 ? Vector2.down : Vector2.up;
                        }
                    });
                    seq2.AppendInterval(.1f);
                    seq2.AppendCallback(() =>
                    {

                        _currentBullet = PoolManager.Instance.Pop(_bulletName);
                        _currentBullet.transform.position = _fireTrans[0].position;
                        _currentBullet.transform.rotation = Quaternion.Euler(new Vector3(rot, _currentBullet.transform.rotation.y));
                        if (_currentBullet is Bullet)
                        {
                            Bullet bullet = (_currentBullet as Bullet);
                            EazySoundManager.PlaySound(_attackAudio, _attackVolume);
                            bullet.FollowTarget = null;
                            bullet._speed = 13;
                            bullet.Dir = rot == 180 ? Vector2.down : Vector2.up;
                        }
                    });
                    seq2.AppendInterval(.1f);
                    seq2.AppendCallback(() =>
                    {

                        _currentBullet = PoolManager.Instance.Pop(_bulletName);
                        _currentBullet.transform.position = _fireTrans[0].position;
                        _currentBullet.transform.rotation = Quaternion.Euler(new Vector3(rot, _currentBullet.transform.rotation.y));
                        if (_currentBullet is Bullet)
                        {
                            Bullet bullet = (_currentBullet as Bullet);
                            EazySoundManager.PlaySound(_attackAudio, _attackVolume);
                            bullet.FollowTarget = null;
                            bullet._speed = 13;
                            bullet.Dir = rot == 180 ? Vector2.down : Vector2.up;
                        }
                    });
                    seq2.AppendInterval(.4f);
                    seq2.AppendCallback(() =>
                    {
                        action?.Invoke(rot);
                        seq2.Kill();
                    });

                });
                break;
            case BehaviourType.THREE:

                break;
            case BehaviourType.FOUR:
                for (int i = 0; i < _fireTrans.Length; i++)
                {
                    _currentBullet = PoolManager.Instance.Pop(_bulletName);
                    _currentBullet.transform.position = _fireTrans[i].position;
                    _currentBullet.transform.rotation = Quaternion.identity ;
                    if (_currentBullet is Bullet)
                    {
                        Bullet bullet = (_currentBullet as Bullet);
                        EazySoundManager.PlaySound(_attackAudio, _attackVolume);
                        bullet.Dir = Vector2.zero;
                        bullet.MoveAndFollow(Player.transform);
                    }
                }
                break;
            case BehaviourType.FIVE:
                for (int i = 0; i < 20; i++)
                {
                   Bullet bullet =  PoolManager.Instance.Pop("BomberBullet") as Bullet;
                   bullet.Dir = new Vector2(.5f, -.5f);
                   bullet.transform.rotation = Quaternion.Euler(0, 0, 160);
                   bullet.transform.position = new Vector3(transform.position.x-.25f, transform.position.y);
                }
                for (int i = 0; i < 20; i++)
                {
                    Bullet bullet =  PoolManager.Instance.Pop("BomberBullet") as Bullet;
                    bullet.Dir = new Vector2(-.5f, -.5f);
                    bullet.transform.rotation = Quaternion.Euler(0, 0, -160);
                   bullet.transform.position = new Vector3(transform.position.x+.25f, transform.position.y);
                }
                break;
        }
        yield return null;

    }
    protected override void OnDeath()
    {
        StopAllCoroutines();
        seq.Kill();
        base.OnDeath();
        if(_shadow != null)
        _shadow.gameObject.SetActive(false);
    }


}
