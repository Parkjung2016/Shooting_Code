using DG.Tweening;
using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum BehaviourType
{
    ONE,
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX,
}
public class EnemyBase : PoolableMono
{



    private float _hp;
    [SerializeField]
    protected float _maxHP;

    public bool _death;

    protected Animator _anim;

    public int _enemyIndex;

    public float HP
    {
        set
        {
            _hp = value;
            if (_hp > 0)
            {

                SetHPProgress();
            }
            else
            {
                if(!_death)
                {

                    transform.DOKill();


                    OnDeath();
                }
            }
        }
        get { return _hp; }
    }

    [SerializeField]
    private Image _hpProgressBar;
    protected bool _trueDamaged;
    protected PlayerControllerBase Player;

    [SerializeField]
    private int _addGold;
    [SerializeField]
    private int _addScore;

    private Collider2D _col2D;
    [SerializeField]
    protected float _colDam;

    [SerializeField]
    private GameObject _destroyEffectPrefab;
    private GameObject _destroyEffect;

    [SerializeField]
    private AudioClip _hitAudioClip;
    [SerializeField]
    private AudioClip _destroyAudioClip;
    [SerializeField]
    protected float _colAmp = 3f;

    public int appearStage = 0;
    public override void Init()
    {
        _trueDamaged = false;
        _anim.Play("Move");
        _hp = _maxHP;
        if (_hpProgressBar != null)
            _hpProgressBar.fillAmount = 1;
        _death = false;
            _col2D.enabled = true;

        if (!EnemySpawner._instance.CanSpawnEwnemy && this as Boss == null)
        {
            AppearBoss();
        }
    }
    private void OnEnable()
    {
    }

    protected virtual void Awake()
    {
        _col2D =GetComponent<Collider2D>();
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerBase>();
        _anim = GetComponent<Animator>();
        _hpProgressBar = transform.Find("Canvas/HPProgress").GetComponent<Image>();
        if(_destroyEffectPrefab != null)
        {
            _destroyEffect= Instantiate(_destroyEffectPrefab, GameManager._instance.transform);
            _destroyEffect.gameObject.SetActive(false);
        }
    }
    public void ApplyDamage(float Dam)
    {
        HP -= Dam;
    }
    public void AppearBoss()
    {
        if (_hpProgressBar != null)
        transform.DOKill();
        StopAllCoroutines();
        _col2D.enabled = false;
        _death = true;

        Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMoveY(transform.position.y+ 2, 1));
        if(transform.position.x >0)
        {
            seq.Join(transform.DOMoveX(15, 2));
        }
        else
            seq.Join(transform.DOMoveX(-15, 2));
        seq.AppendCallback(() => {
            Pushthis();
            });
    }
    private void Update()
    {
        if (this as Boss != null) return;
        //switch(Level)
        //{
        //    case 1:
        //        if(Physics2D.OverlapCircle(transform.position, 2, LayerMask.GetMask("Player")) && !_isAvoided)
        //            {
        //            DOTween.Kill(this);
        //            _isAvoided = true;
        //            transform.DOMoveX(transform.position.x<=-8.3f ? Random.Range(1,3) : transform.position.x >= 8.3f? Random.Range(-1, -3) : Random.Range(-3, 3), .4f).OnComplete(()=> _isAvoided=false);
        //        }
        //        break;
        //}
    }
    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position, 3);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _trueDamaged &&!Player.Death && !_death)
        {
            EazySoundManager.PlaySound(_hitAudioClip);
            Collision(collision.GetComponent<PlayerControllerBase>(),_colDam, _colAmp, .6f);
        }

    }
    protected virtual void Collision(PlayerControllerBase pC, float dam, float Amp, float _time)
    {
        CameraManager.Instance.Noise(Amp, _time );
        ApplyDamage(15);
        pC.ApplyDamage(dam);
    }
    public void SetHPProgress()
    {
        _hpProgressBar.DOFillAmount(_hp / _maxHP, .6f);

    }

    protected virtual void OnDeath()
    {
        EazySoundManager.PlaySound(_destroyAudioClip);
        _death = true;
        _col2D.enabled = false;
        if(_destroyEffect != null)
        {
            _destroyEffect.SetActive(true);
            _destroyEffect.transform.position = transform.position;
        }
        StopAllCoroutines();
        transform.DOKill();
        _anim.Play("Death");
        PlayerDataManager.Instance.PlayerData.Gold += _addGold;
        UIManager.Instance.StartCoroutine(UIManager.Instance.SetGoldText());
        GameManager._instance.AddScore(_addScore);

    }

    public void Pushthis()
    {
        EnemySpawner._instance.BulletEnemyCount[_enemyIndex]--;
        PoolManager.Instance.Push(this);
    }

}
