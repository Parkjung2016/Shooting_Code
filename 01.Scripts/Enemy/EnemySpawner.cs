using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [SerializeField] private float[] _enemyActiveTime;

    private float _rightCamPos;

    [HideInInspector] public int[] BulletEnemyCount;

    public bool CanSpawnEwnemy;

    private int _maxEnemyCount = 3;

    protected override void Awake()
    {
        base.Awake();

        float sizeX = Camera.main.orthographicSize * Screen.width / Screen.height;
        _rightCamPos = sizeX + Camera.main.gameObject.transform.position.x;
        _maxEnemyCount = 3;
    }

    private void Start()
    {
        CanSpawnEwnemy = true;
        BulletEnemyCount = new int[PoolManager.Instance.EnemyNum()];
        for (int i = 0; i < BulletEnemyCount.Length; i++)
        {
            StartCoroutine(ActiveBulletEnemy(i));
        }
    }

    public void DecreaseEnemyTime()
    {
        for (int i = 0; i < _enemyActiveTime.Length; i++)
        {
            _enemyActiveTime[i] = Mathf.Max(1, _enemyActiveTime[i] - .5f);
        }
            _maxEnemyCount = Mathf.Clamp(_maxEnemyCount + 3, 0, 15);
    }

    private IEnumerator ActiveBulletEnemy(int index)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
        while (true)
        {
            if (index >= 4)
            {
                yield return new WaitUntil(() => GameManager._instance.AppearBossLevel >= 1);
            }

            yield return new WaitUntil(() => CanSpawnEwnemy);
            if (BulletEnemyCount[index] >= _maxEnemyCount)
            {
                yield return new WaitUntil(() => BulletEnemyCount[index] < _maxEnemyCount);
                yield return new WaitForSeconds(
                    _enemyActiveTime[index]);
            }

            BulletEnemyCount[index]++;
            EnemyBase enemyBase = PoolManager.Instance.Pop("Enemy" + (index + 1)) as EnemyBase;
            enemyBase._enemyIndex = index;
            enemyBase.GetComponent<SpriteRenderer>().sortingOrder = BulletEnemyCount[index];
            enemyBase.transform.position = new Vector3(UnityEngine.Random.Range(-_rightCamPos, _rightCamPos),
                Camera.main.orthographicSize * 2 + .3f);
            if (enemyBase is Enemy_Speed)
            {
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(_enemyActiveTime[index] ,
                _enemyActiveTime[index]+2f));
        }
    }
}