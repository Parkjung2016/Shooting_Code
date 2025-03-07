using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : PoolableMono
{
    [SerializeField]
    private BehaviourType _moveType;

    [SerializeField]
    private float Damage;

    public int _obstacleIndex;

    private float _hp;
    [SerializeField]
    private float _maxHP;
    [SerializeField]
    private GameObject _destroyParticle;
    [SerializeField]
    private GameObject _hitEffect;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _hitAudioClip;
    private float HP
    {
        set {
            _hp = value;
        if(_hp <=0)
            {
                SpawnDestroyParticle();
                PoolManager.Instance.Push(this);
            }
        }
        get => _hp;
    }
    private void SpawnDestroyParticle()
    {
        Instantiate(_destroyParticle,transform.position,Quaternion.identity);
    }
    public override void Init()
    {
        _hp = _maxHP;
        switch(_moveType)
        {
            case BehaviourType.ONE:
                Sequence seq = DOTween.Sequence();
                seq.Append(transform.DOMoveY((-Camera.main.orthographicSize * 2) + .4f, 5)).OnComplete(()=>Hide());
                seq.Join(transform.DORotate(new Vector3(0, 0, 1080), 3, RotateMode.FastBeyond360));
                break;
            case BehaviourType.TWO:
                break;
            case BehaviourType.THREE:
                break;
        }
    }
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void ApplyDamage(float Dam)
    {
        HP -= Dam;
    }
    void Hide()
    {
        ObstacleSpawner._instance.ObstacleCount[_obstacleIndex]--;
        PoolManager.Instance.Push(this);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !collision.GetComponent<PlayerControllerBase>().Death)
        {
            SoundManager.Instance.PlayAudioWithOneShot(_hitAudioClip);
            Instantiate(_hitEffect, transform.position, _hitEffect.transform.rotation);
            Instantiate(_destroyParticle, transform.position, _destroyParticle.transform.rotation);
            collision.GetComponent<PlayerControllerBase>().ApplyDamage(Damage,4f,.7f);
            Hide();
        }
    }
}
