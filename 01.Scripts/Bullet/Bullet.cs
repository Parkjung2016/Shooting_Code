using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Hellmade.Sound;
using UnityEngine;
public enum BulletType
{
    Boom,
    Normal,
    Follow,
    Lightning
}
public class Bullet : PoolableMono
{

    public float _speed;
    public BulletType _bulletType;
    [HideInInspector]
    public Vector2 Dir;
    [SerializeField]
    private GameObject _hitEffect;

    [SerializeField]
    private float _damage;

    public Transform FollowTarget = null;
    public Vector2  addLookAtVec;
    private float _hideOrthographicSize;
    [SerializeField]
    private AudioClip _hitAudioClip;
    private void Awake()
    {
        _hideOrthographicSize = Camera.main.orthographicSize * 2;

    }
    public void SetValue(float speed, float damage, GameObject hitEffect)
    {
        _speed = speed;
        _damage = damage;
        _hitEffect = hitEffect;
    }
    public void MoveAndFollow(Transform target)
    {
           FollowTarget = null;
        Sequence seq2 = DOTween.Sequence();
        seq2.Append(transform.DOMoveY(transform.position.y + 2, 1).SetEase(Ease.Linear));
        seq2.AppendInterval(.5f);
        seq2.AppendCallback(() =>
        {
           FollowTarget = target;
           _speed = 7;
         addLookAtVec = new Vector2(1, 1);
            if(gameObject.activeSelf)
        StartCoroutine(   CallPushThis(1.5f));
            seq2.Kill();
        });
    }
    private void Update()
    {
        if (FollowTarget != null)
        {
            if (FollowTarget.gameObject.activeSelf)
            {
                Dir = (FollowTarget.position - transform.position).normalized;
                transform.right = Dir+ addLookAtVec;
            }
            else
            {
                Dir = Vector2.up;
            }
        }
        else
        {

        }
        transform.position += (_speed * (Vector3)Dir * Time.deltaTime);
        if (transform.position.y >= _hideOrthographicSize + .4f || transform.position.y <= -_hideOrthographicSize + .4f)
        {
            PushThis();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            enemy.ApplyDamage(_damage);
            EazySoundManager.PlaySound(_hitAudioClip, .6f);
            if (PlayerDataManager.Instance.PlayerData.CurrentWeapon.Level >= 5)
                switch (PlayerDataManager.Instance.PlayerData.CurrentWeapon.MaxBulletType)
                {
                    case BulletType.Boom:
                        GameManager._instance._pC.HP += 20;
                        break;
                    case BulletType.Lightning:
                        EnemyBase[] enemyBases = FindObjectsOfType<EnemyBase>();
                        for (int i = 0; i < enemyBases.Length; i++)
                        {
                            if (enemyBases[i] == enemy) continue;
                            enemyBases[i].ApplyDamage(_damage*15);
                            Instantiate(_hitEffect, transform.position, _hitEffect.transform.rotation);
                        }
                        break;
                    case BulletType.Follow:
                        CameraManager.Instance.Noise(5, .5f);
                        //TimeController.Instance.SetTimeFreeze(0, 0, .2f);
                        break;

                }
            PushThis();
            Instantiate(_hitEffect, transform.position, _hitEffect.transform.rotation);
        }
        if (collision.CompareTag("Player"))
        {
            EazySoundManager.PlaySound(_hitAudioClip,.6f);
            PushThis();
            Instantiate(_hitEffect, transform.position, _hitEffect.transform.rotation);
            collision.GetComponent<PlayerControllerBase>().ApplyDamage(_damage);
        }
        if (collision.CompareTag("Obstacle"))
        {
            PushThis();

            Instantiate(_hitEffect, transform.position, _hitEffect.transform.rotation);
            collision.GetComponent<Obstacle>().ApplyDamage(_damage);
        }
        if(collision.CompareTag("Shield"))
        {
            PushThis();
        }
    }
    public void PushThis()
    {
        PoolManager.Instance.Push(this);
    }
    public IEnumerator CallPushThis(float time)
    {
        yield return new WaitForSeconds(time);
        EazySoundManager.PlaySound(_hitAudioClip, .6f);
        Instantiate(_hitEffect, transform.position, _hitEffect.transform.rotation);
        PushThis();
    }
    public override void Init()
    {
    }
}
