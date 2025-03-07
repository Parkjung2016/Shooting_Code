using Cinemachine;
using DG.Tweening;
using Hellmade.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PoolingListSO _poolListSO;

    [SerializeField] private string[] SoliloquyText;
    [SerializeField] private string[] Soliloquy2Text;
    private ColorAdjustments _colorAdjustments;

    private int _score;
    private int _appearBossScroe = 1000;
    private int _decreaseEnemyTimeScore = 0;

    [SerializeField] private GameObject _bossPrefab;

    [SerializeField] private Sprite[] _skillSprites;


    private SkillType _skillType;

    public bool IsSkillUsing;
    private float _skillCoolTime;
    [SerializeField] private GameObject[] _metroSkillPrefabs;
    private Fox _fox;
    private ARC _arc;

    [HideInInspector] public PlayerControllerBase _pC;
    private AudioSource _audioSource;
    private AudioSource _bgmAudioSource;

    [SerializeField] private AudioClip[] _skillClips;

    [SerializeField] private GameObject[] _playerPrefabs;
    private Transform _startPlayerPos;

    public bool ApperingBoss;

    public int AppearBossLevel;

    private ChannelMixer cM;

    public int Score
    {
        set
        {
            _score = value;
            UIManager.Instance.SetScoreText(_score);
            if (_score >= _appearBossScroe)
            {
                _appearBossScroe += 5000;
                EnemyBase[] enemybase = FindObjectsOfType<EnemyBase>();
                for (int i = 0; i < enemybase.Length; i++)
                {
                    enemybase[i].AppearBoss();
                }

                AppearBoss();
            }
        }
        get => _score;
    }

    [SerializeField] private Sprite[] _playerSprites;

    [SerializeField] private Vector2[] _playerImgPos;
    [SerializeField] private Vector2[] _playerImgScale;

    protected override void Awake()
    {
        base.Awake();

        _startPlayerPos = GameObject.Find("StartPlayerPos").transform;

        EazySoundManager.GlobalSoundsVolume = 1;
        _bgmAudioSource = Camera.main.GetComponent<AudioSource>();
        _audioSource = GetComponent<AudioSource>();
        _fox = FindObjectOfType<Fox>();
        _arc = FindObjectOfType<ARC>();

        Volume volume = FindObjectOfType<Volume>();
        volume.sharedProfile.TryGet<ColorAdjustments>(out _colorAdjustments);
        _fox.gameObject.SetActive(false);
        volume.sharedProfile.TryGet<ChannelMixer>(out cM);
        _pC = Instantiate(_playerPrefabs[PlayerDataManager.Instance.PlayerData.PlayerType], _startPlayerPos.position,
            Quaternion.identity).GetComponent<PlayerControllerBase>();
        if (PlayerDataManager.Instance.PlayerData.CurrentEngine != null)
            _skillType = PlayerDataManager.Instance.PlayerData.CurrentEngine.skillType;
        else
            _skillType = SkillType.NULL;
        _appearBossScroe = 5000;
        CreateUIManager();
        CreateTimeController();
        CreateCameraManager();
        CreatePoolManager();
        StartCoroutine(Soliloquy());
        Score = 0;
        _arc.gameObject.SetActive(false);
        if (PlayerDataManager.Instance.PlayerData.PlayerType == 0)
            _skillCoolTime = PlayerDataManager.Instance.PlayerData.CurrentEngine.SkillCoolTime;
        else
            _skillCoolTime = PlayerDataManager.Instance.PlayerData.CurrentWeapon.SkillCoolTime;


        // cM.redOutBlueIn.value = 111;
        // cM.redOutGreenIn.value =-43;
        // cM.redOutBlueIn.value = 12;
        // ;
        // cM.greenOutRedIn.value = 14;
        // cM.greenOutGreenIn.value = 116;
        // cM.greenOutBlueIn.value = 8;
        // ;
        // cM.blueOutRedIn.value = 37;
        // cM.blueOutGreenIn.value = 64;
        // cM.blueOutBlueIn.value = 55;
    }


    public void AddScore(int add)
    {
        _decreaseEnemyTimeScore += add;
        Score += add;
    }

    private void CreateTimeController()
    {
        TimeController.Instance = gameObject.AddComponent<TimeController>();
    }

    private void CreateCameraManager()
    {
        CameraManager.Instance = new CameraManager();
        CameraManager.Instance.Init();
    }

    private void CreateUIManager()
    {
        GameObject obj = new GameObject("UIManager");
        UIManager.Instance = obj.AddComponent<UIManager>();
        obj.transform.parent = transform;
        UIManager.Instance.Init(FindAnyObjectByType<Canvas>().transform, _playerSprites, _playerImgPos,
            _playerImgScale);
        if (PlayerDataManager.Instance.PlayerData.PlayerType == 0)
            UIManager.Instance.ChangeSkillImg(_skillSprites[(byte)_skillType]);
        else
            UIManager.Instance.ChangeSkillImg(_skillSprites[4]);
    }

    public void DeathBoss()
    {
        ApperingBoss = false;
        AppearBossLevel++;
        DOTween.To(() => cM.redOutRedIn.value, x => cM.redOutRedIn.value = x, Random.Range(70f, 140f), 1);
        DOTween.To(() => cM.redOutGreenIn.value, x => cM.redOutGreenIn.value = x, Random.Range(-100f, 54f), 1);
        DOTween.To(() => cM.redOutBlueIn.value, x => cM.redOutBlueIn.value = x, Random.Range(-44f, 39f), 1);

        DOTween.To(() => cM.greenOutRedIn.value, x => cM.greenOutRedIn.value = x, Random.Range(-15f, 71f), 1);
        DOTween.To(() => cM.greenOutGreenIn.value, x => cM.greenOutGreenIn.value = x, Random.Range(42f, 130f), 1);
        DOTween.To(() => cM.greenOutBlueIn.value, x => cM.greenOutBlueIn.value = x, Random.Range(-23f, 37f), 1);

        DOTween.To(() => cM.blueOutRedIn.value, x => cM.blueOutRedIn.value = x, Random.Range(-10f, 90f), 1);
        DOTween.To(() => cM.blueOutGreenIn.value, x => cM.blueOutGreenIn.value = x, Random.Range(23f, 101f), 1);
        DOTween.To(() => cM.blueOutBlueIn.value, x => cM.blueOutBlueIn.value = x, Random.Range(14f, 92f), 1);
        _audioSource.DOFade(1, 0).OnComplete(() => _audioSource.Pause());
        _bgmAudioSource.DOFade(0, 0);
        _bgmAudioSource.Play();
        _bgmAudioSource.DOFade(1, 0);
        CameraManager.Instance.Noise(7f, .8f);
        TimeController.Instance.SetTimeFreeze(0, 0, .4f);
        EnemySpawner._instance.CanSpawnEwnemy = true;
    }

    public void LoadLobbyScene()
    {
        UIManager.Instance.FadeIn(1, () =>
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(.1f);
            UIManager.Instance.VisibleDeath(Score);
            seq.AppendInterval(4f);
            seq.AppendCallback(() => UIManager.Instance.HideDeath(action: () => SceneManager.LoadScene("Lobby")));
        });
    }

    private void AppearBoss()
    {
        ApperingBoss = true;
        _bgmAudioSource.DOFade(0, 1).OnComplete(() => _bgmAudioSource.Pause());
        _audioSource.DOFade(0, 0);
        _audioSource.Play();
        _audioSource.DOFade(1, 0);
        EnemySpawner._instance.CanSpawnEwnemy = false;
        Instantiate(_bossPrefab, new Vector3(0, Camera.main.orthographicSize * 2 + 3), _bossPrefab.transform.rotation);
    }

    private void CreatePoolManager()
    {
        PoolManager.Instance = new PoolManager(transform);

        foreach (PoolingPair pair in _poolListSO.Pairs)
        {
            PoolManager.Instance.CreatePool(pair.Prefab, pair.Count);
        }
    }

    private void Update()
    {
        if (_decreaseEnemyTimeScore >= 1000)
        {
            EnemySpawner._instance.DecreaseEnemyTime();
            _decreaseEnemyTimeScore = 0;
        }

        if (Input.GetKeyDown(KeyCode.Q) && !IsSkillUsing && !_pC.Death)
        {
            UIManager.Instance._skillCoolTimeImg.gameObject.SetActive(true);

            bool Cooltime = true;
            IsSkillUsing = true;
            if (PlayerDataManager.Instance.PlayerData.PlayerType == 0)
            {
                switch (_skillType)
                {
                    case SkillType.METEOR:
                        for (int i = 0; i < 18; i++)
                        {
                            EazySoundManager.PlaySound(_skillClips[0]);
                            int dir = UnityEngine.Random.Range(0, 2);
                            Meteor obj =
                                Instantiate(_metroSkillPrefabs[UnityEngine.Random.Range(0, _metroSkillPrefabs.Length)],
                                    Vector3.zero, Quaternion.identity).GetComponent<Meteor>();
                            obj._speed = UnityEngine.Random.Range(6f, 10f);
                            if (dir == 0)
                            {
                                obj.transform.position = new Vector3(UnityEngine.Random.Range(-12.3f, -9.2f),
                                    UnityEngine.Random.Range(5, 12f));
                            }
                            else
                            {
                                obj.transform.position = new Vector3(UnityEngine.Random.Range(9.2f, 12.3f),
                                    UnityEngine.Random.Range(5, 12f));
                                obj.transform.localScale = new Vector3(-obj.transform.localScale.x,
                                    obj.transform.localScale.y);
                                obj.Dir = new Vector2(-.5f, -.5f);
                            }
                        }

                        break;
                    case SkillType.FOX:
                        EazySoundManager.PlaySound(_skillClips[1]);
                        _fox.Effect();
                        break;
                    case SkillType.SHIELD:
                        Cooltime = false;
                        EazySoundManager.PlaySound(_skillClips[2]);
                        _pC.StartCoroutine(_pC.Shiled());
                        break;
                    case SkillType.ARC:
                        EazySoundManager.PlaySound(_skillClips[3]);
                        _arc.Effect();
                        break;
                }
            }
            else
            {
                (_pC as PlayerController2).Skill();
            }

            if (Cooltime)
                SkillCoolTime();
        }
    }

    public void SkillCoolTime()
    {
        UIManager.Instance.CoolTimeSkill(_skillCoolTime, () => IsSkillUsing = false);
    }

    public void CallWaitingForAction(float second, Action action)
    {
        StartCoroutine(WaitingForAction(second, action));
    }

    IEnumerator WaitingForAction(float second, Action action)
    {
        yield return new WaitForSeconds(second);
        action();
    }

    IEnumerator Soliloquy()
    {
        WaitForSeconds wt = new WaitForSeconds(4);
        string[] texts;
        if (PlayerDataManager.Instance.PlayerData.PlayerType == 0)
            texts = SoliloquyText;
        else
            texts = Soliloquy2Text;
        while (true)
        {
            yield return wt;
            if (UnityEngine.Random.Range(0, 101) <= 20)
            {
                UIManager.Instance.Typingsoliloquy(texts[UnityEngine.Random.Range(0, texts.Length)]);
            }
        }
    }
}