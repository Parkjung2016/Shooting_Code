using DG.Tweening;
using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : PlayerControllerBase
{



    [SerializeField]
    private float _fireRate;

    private Animator _engineEffectAnimator;
    private Animator _weaponAnimator;




    private Transform[] _firePoses;



    [SerializeField]
    private Sprite[] _damagedSprites;
    private int _attackPosIdx = 0;




    private SpriteRenderer _baseSpriteRenderer;
    private SpriteRenderer _engineSpriteRenderer;
    private SpriteRenderer _engineEffectSpriteRenderer;
    private SpriteRenderer _weaponSpriteRenderer;


    public int bulletIndex;


    private AudioSource _engineAudioSource;
    [SerializeField]
    private AudioClip[] _engineSFX;

    [SerializeField]
    private Material[] itemMats;

    protected override  void Awake()
    {
        base.Awake();
        Transform Sprite = transform.Find("Sprite").transform;

        PlayerData data = PlayerDataManager.Instance.PlayerData;


        _baseSpriteRenderer = Sprite.Find("Base").GetComponent<SpriteRenderer>();
        _engineSpriteRenderer = Sprite.Find("Engine").GetComponent<SpriteRenderer>();
        _weaponSpriteRenderer = Sprite.Find("Weapon").GetComponent<SpriteRenderer>();
        _engineEffectSpriteRenderer = Sprite.Find("EngineEffects").GetComponent<SpriteRenderer>();

        _engineEffectAnimator = Sprite.Find("EngineEffects").GetComponent<Animator>();
        _firePoses = new Transform[data.CurrentWeapon.FirePoses.Length];
        _weaponAnimator = _weaponSpriteRenderer.GetComponent<Animator>();
        _firePoses[0] = Sprite.Find("Weapon/FirePos").transform;
        _firePoses[0].localPosition = data.CurrentWeapon.FirePoses[0];
        if (data.CurrentWeapon.FirePoses.Length == 2)
        {
            _firePoses[1] = Sprite.Find("Weapon/FirePos2").transform;
            _firePoses[1].localPosition = data.CurrentWeapon.FirePoses[1];
        }
        string engineEffectResourceName = "Animation/Player/EngineEffect/" + data.CurrentEngine.spritePath.Replace("Image/PNGs/", "") + "EffectAnimator";
        string weaponResourceName = "Animation/Player/Weapon/" + data.CurrentWeapon.spritePath.Replace("Image/PNGs/", "") + "Animator";
        _engineEffectAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(engineEffectResourceName);
        _weaponAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(weaponResourceName);
        _engineSpriteRenderer.sprite = Resources.Load<Sprite>(data.CurrentEngine.spritePath);
        if(PlayerDataManager.Instance.PlayerData.CurrentWeapon.Level>= 5)
        {
            _weaponSpriteRenderer.material = itemMats[1];
        }
        if (PlayerDataManager.Instance.PlayerData.CurrentEngine.Level >= 5)
        {
            _engineSpriteRenderer.material = itemMats[1];
        }
        _fireRate = PlayerDataManager.Instance.PlayerData.FireRate;
        _weaponAnimator.SetFloat("AttackSpeed", data.CurrentWeapon.Values[1]);
        _engineAudioSource = _engineSpriteRenderer.GetComponent<AudioSource>();
        _engineAudioSource.clip = _engineSFX[PlayerDataManager.Instance.PlayerData.CurrentEngine.Index];
        _engineAudioSource.Play();

        if(PlayerDataManager.Instance.PlayerData.CurrentWeapon.MaxBulletType == BulletType.Lightning)
        {
            _weaponAnimator.speed = .75f;
        }
    }

    protected override void ChangeDamagedBaseSprite()
    {
        if (HP <= _maxHP * 20 / 100)
        {
            _baseSpriteRenderer.sprite = _damagedSprites[3];
        }
        else if (HP <= _maxHP * 50 / 100)
        {
            _baseSpriteRenderer.sprite = _damagedSprites[2];
        }

        else if (HP <= _maxHP * 70 / 100)
        {
            _baseSpriteRenderer.sprite = _damagedSprites[1];
        }
        else
        {
            _baseSpriteRenderer.sprite = _damagedSprites[0];
        }
    }


    protected override void Update()
    {
        base.Update();
        if (Death) return;
        Anim();
    }
    protected override void CheckDeath()
    {
        base.CheckDeath();
        if (HP <= 0 && !Death)
        {

            StopAllCoroutines();
            EnemySpawner._instance.StopAllCoroutines();
            ObstacleSpawner._instance.StopAllCoroutines();
            Sequence seq = DOTween.Sequence();
            _weaponSpriteRenderer.material = itemMats[0];
            _engineSpriteRenderer.material = itemMats[0];
            seq.Append(_baseSpriteRenderer.material.DOFloat(2, "_DissolveAmount", 3));
            seq.Join(_engineSpriteRenderer.material.DOFloat(2, "_DissolveAmount", 3));
            seq.Join(_weaponSpriteRenderer.material.DOFloat(2, "_DissolveAmount", 3));
            seq.Join(_engineEffectSpriteRenderer.material.DOFloat(2, "_DissolveAmount", 3));
            seq.AppendCallback(() =>
            {
                GameManager._instance.LoadLobbyScene();
            });

            Death = true;
        }

    }


    private void Anim()
    {
        if (Death) return;
        _engineEffectAnimator.SetBool("Move", Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
    }
    protected override IEnumerator CheckInputAttackKey()
    {
        while (true)
        {
            if (Input.GetButton("Attack"))
            {
                _weaponAnimator.SetBool("Attack", true);
                if (PlayerDataManager.Instance.PlayerData.CurrentWeapon.Level >= 5)
                {
                    switch (PlayerDataManager.Instance.PlayerData.CurrentWeapon.MaxBulletType)
                    {
                        case BulletType.Normal:
                            Attack();
                            yield return new WaitForSeconds(.1f);
                            break;
                    }

                }
            }
            else
                _weaponAnimator.SetBool("Attack", false);
            yield return null;
        }
    }

    public  override void Attack()
    {
        base.Attack();
        if (Death) return;
        if (PlayerDataManager.Instance.PlayerData.CurrentWeapon.ItemName != "гнгнгн") bulletIndex = 0;
        Bullet bullet = PoolManager.Instance.Pop("PlayerBullet") as Bullet;
        bullet.transform.position = _firePoses[_attackPosIdx].position;
        EazySoundManager.PlaySound(_currentWeaponSFX,.6f);
        if (_firePoses.Length == 2)
            _attackPosIdx = _attackPosIdx == 0 ? 1 : 0;
        bullet.Dir = Vector2.up;

        if (PlayerDataManager.Instance.PlayerData.CurrentWeapon.Level >= 5)
        {
            switch (PlayerDataManager.Instance.PlayerData.CurrentWeapon.MaxBulletType)
            {
                case BulletType.Follow:

                    FindNearByEnemy();
                    if (FoundObjects.Count - 1 >= bulletIndex && FoundObjects[bulletIndex] != null  && FoundObjects[bulletIndex].gameObject.activeSelf)
                        bullet.FollowTarget = FoundObjects[bulletIndex].transform;

                    bulletIndex++;
                    break;
            }

        }
    }



}
