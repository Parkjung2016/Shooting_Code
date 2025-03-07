using DG.Tweening;
using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Boss : EnemyBase
{
    [SerializeField]
    private float _fireRate;

    [SerializeField]
    BehaviourType _attackType;


    private PlayerControllerBase _pC;
    private float weightAngle;
    private bool _skilling;
    [SerializeField]
    private AudioClip _fireClip;

    [SerializeField]
    private AudioClip _dashClip;
    [SerializeField]
    private AudioClip _dashEndClip;
    protected override void Awake()
    {
        base.Awake();
        _pC = GameManager._instance._pC;
        Init();
        HP += PlayerDataManager.Instance.PlayerData.CurrentWeapon.Level >= 5 ? 200 : 0;
    }
    private void Start()
    {
        BeginSeq();
        HP += PlayerDataManager.Instance.PlayerData.CurrentWeapon.Values[0] * 1.5f;
    }
    private void BeginSeq()
    {
        if (_death) return;
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMoveY(Camera.main.orthographicSize - 3f, 1f));
        seq.AppendInterval(.3f);
        seq.AppendCallback(() =>
        {
            _trueDamaged = true;
            AttackCoroutine = Attack();
            StartCoroutine(AttackCoroutine);
            seq.Kill();
        });
    }
    IEnumerator AttackCoroutine;
    IEnumerator Attack()
    {
        while (true)
        {
            if (_pC.Death) StopAllCoroutines();
            switch (_attackType)
            {
                case BehaviourType.ONE:
                    CircleAttack();
                    break;
                case BehaviourType.TWO:
                    DashAttack();
                    StopCoroutine(AttackCoroutine);
                    break;
            }
            yield return new WaitForSeconds(_fireRate);
        }
    }
    protected override void Collision(PlayerControllerBase pC, float dam, float Amp, float time)
    {
        if (_skilling)
        {
            base.Collision(pC, 30, 5, .7f);

        }
        else
            base.Collision(pC, _colDam, 3, .6f);
    }
    private void CircleAttack()
    {
        if (_pC.Death) return;
        int Count = 30;
        int intervalAngle = 360 / Count;

        //for (int fireAngle = 30; fireAngle < 330; fireAngle += 10)
        //{
        //    Bullet bullet = PoolManager.Instance.Pop("BossBullet") as Bullet;
        //    Vector2 direction = new Vector2(Mathf.Cos(fireAngle * Mathf.Deg2Rad), Mathf.Sin(fireAngle * Mathf.Deg2Rad));

        //    bullet.transform.position = transform.position;
        //    bullet.transform.right = direction;
        //    bullet.Dir = direction;
        //    print(bullet.Dir);
        //}

            EazySoundManager.PlaySound(_fireClip,.5f);
        for (int i = 0; i < Count; ++i)
        {

            Bullet bullet = PoolManager.Instance.Pop("BossBullet") as Bullet;
        
            bullet.transform.position = transform.position;
            float angle = weightAngle + intervalAngle * i;
            float x = Mathf.Cos(angle * Mathf.PI / 180.0f);
            float y = Mathf.Sin(angle * Mathf.PI / 180.0f);
            Vector2 Dir = new Vector2(x, y);
            //float dirRot = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
            bullet.Dir = Dir;
            bullet.transform.right = Dir;

        }
        ++weightAngle;


    }
    private void DashAttack()
    {
        if (_pC.Death) return;
        int index = Random.Range(0, 2);

        Sequence seq = DOTween.Sequence();
        float y = transform.position.y;
        switch (index)
        {
            case 0:
                _skilling = true;
                seq.Append(transform.DOMoveY(-Camera.main.orthographicSize + 3, 1).SetEase(Ease.InQuart));
                  seq.AppendCallback(()=>
                  {
                      CameraManager.Instance.Noise(5, .3f);
                      EazySoundManager.PlaySound(_dashEndClip);
                  });
                EazySoundManager.PlaySound(_dashClip);
                seq.AppendInterval(.5f);
                seq.AppendCallback(() => EazySoundManager.PlaySound(_dashClip));
                seq.Append(transform.DOMoveY(y, 1).SetEase(Ease.Flash));
                seq.AppendCallback(() =>
                {
                    _skilling = false;
                    StartCoroutine(AttackCoroutine);
                });
                break;
            case 1:
                _skilling = true;
                seq.AppendCallback(() => EazySoundManager.PlaySound(_dashClip));
                seq.Append(transform.DOMoveY(Camera.main.orthographicSize + 3, 1).SetEase(Ease.InOutSine));
                seq.Append(transform.DOMoveX(Player.transform.position.x, 1));
                seq.AppendInterval(.5f);
                seq.AppendCallback(() => EazySoundManager.PlaySound(_dashClip));
                seq.Append(transform.DOMoveY(-Camera.main.orthographicSize + 3, .8f).SetEase(Ease.InQuart));
                seq.AppendCallback(() =>
                {
                    CameraManager.Instance.Noise(5, .3f);
                    EazySoundManager.PlaySound(_dashEndClip);
                });
                seq.AppendInterval(.5f);
                seq.Append(transform.DOMoveY(y, 1).SetEase(Ease.Flash));
                seq.AppendCallback(() =>
                {
                    _skilling = false;
                    StartCoroutine(AttackCoroutine);
                });
                break;
        }



    }
    protected override void OnDeath()
    {
        base.OnDeath();
        GameManager._instance.DeathBoss();
        Destroy(gameObject);
    }
}
