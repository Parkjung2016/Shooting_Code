using DG.Tweening;
using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBase : MonoBehaviour
{
    [SerializeField]
    protected float _speed;



    protected Vector2 _minPos;
    protected Vector2 _maxPos;

    [SerializeField]
    protected float _clampAdd;


    protected float _hp;

    [SerializeField]
    private GameObject _petPrefab;
    public float HP
    {
        set
        {
            _hp = Mathf.Clamp(value, 0, _maxHP);
            UIManager.Instance.SetHpProgressBar(_hp, _maxHP);
            ChangeDamagedBaseSprite();
            CheckDeath();
        }
        get { return _hp; }
    }
    public float _maxHP;

    public bool Death;



    public List<EnemyBase> FoundObjects;
    public List<EnemyBase> enemy = new List<EnemyBase>();
    public float shortDis;



    protected Collider2D _col2D;

    protected GameObject _shieldEffect;

    [SerializeField]
    protected AudioClip[] _weaponSFX;
    protected AudioClip _currentWeaponSFX;
    [SerializeField]
    protected AudioClip[] _hitAudioClip;
    [SerializeField]
    private AudioClip _hpUpClip;
    protected virtual void Awake()
    {
        PlayerData data = PlayerDataManager.Instance.PlayerData;
        _col2D = GetComponent<Collider2D>();
        _shieldEffect = transform.Find("ShieldEffect").gameObject;
        _shieldEffect.SetActive(false);

        _minPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
        _maxPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 1));



        _maxHP = PlayerDataManager.Instance.PlayerData.MaxHP;
        _speed = PlayerDataManager.Instance.PlayerData.MoveSpeed;

        _currentWeaponSFX = _weaponSFX[PlayerDataManager.Instance.PlayerData.CurrentWeapon.Index];

        if (PlayerDataManager.Instance.PlayerData.Pet)
        {

            Instantiate(_petPrefab, transform.position-new Vector3(2.5f,0), _petPrefab.transform.rotation);
            Instantiate(_petPrefab, transform.position+new Vector3(2.5f,0), _petPrefab.transform.rotation);
        }

    }
    public void HpUpSFX()
    {
        EazySoundManager.PlaySound(_hpUpClip);
    }
    public void FindNearByEnemy()
    {
        FoundObjects = new List<EnemyBase>(GameObject.FindObjectsOfType<EnemyBase>());
        for (int i = 0; i < FoundObjects.Count; i++)
        {
            if (FoundObjects[i]._death)
            {
                FoundObjects.Remove(FoundObjects[i]);
            }
        }
    }
    protected virtual void ChangeDamagedBaseSprite()
    {

    }
    protected virtual void Start()
    {

        StartCoroutine(CheckInputAttackKey());
        _hp = _maxHP;
        UIManager.Instance.SetHpProgressBar(HP, _maxHP, 0);
    }

    protected virtual void Update()
    {
        if (Death) return;
        Move();
    }
    protected virtual void CheckDeath()
    {


    }
    public void EnableCol(bool enable)
    {
        _col2D.enabled = enable;
    }

    public IEnumerator Shiled()
    {
        EnableCol(false);
        _shieldEffect.SetActive(true);
        yield return new WaitForSeconds(5);
        EnableCol(true);
        GameManager._instance.SkillCoolTime();

    }
    private void Move()
    {
        float Hor = Input.GetAxisRaw("Horizontal");
        float Ver = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(Hor, Ver).normalized;

        Vector2 pos = (Vector2)transform.position + (input * _speed * Time.deltaTime);

        transform.position = new Vector2(Mathf.Clamp(pos.x, _minPos.x + _clampAdd, _maxPos.x - _clampAdd), Mathf.Clamp(pos.y, _minPos.y + _clampAdd, _maxPos.y - _clampAdd));
    }
   protected virtual  IEnumerator CheckInputAttackKey()
    {
        yield return null;
    }

    public virtual void Attack()
    {
        if (Death) return;
    }
    public void ApplyDamage(float damage, float noisevalue = 1.5f, float time = .6f)
    {
        if (Death) return;
        UIManager.Instance.Damage();
        CameraManager.Instance.Noise(noisevalue, time);
        HP -= damage;
    }

}
