using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : Singleton<ObstacleSpawner>
{
    [SerializeField]
    private float[] _obstacleActiveTime;

    private float _rightCamPos;

    [HideInInspector]
    public int[] ObstacleCount;


    protected override void Awake()
    {
        base.Awake();

        float sizeX = Camera.main.orthographicSize * Screen.width / Screen.height;
        _rightCamPos = sizeX + Camera.main.gameObject.transform.position.x;
    }
    private void Start()
    {
        ObstacleCount = new int[PoolManager.Instance.ObstacleNum()];
        for (int i = 0; i < ObstacleCount.Length; i++)
        {
            StartCoroutine(ActiveObstacle(i));
        }


    }

    private IEnumerator ActiveObstacle(int index)
    {
        while (true)
        {
            while (ObstacleCount[index] >= 3)
            {
                yield return null;
            }
            yield return new WaitForSeconds(Random.Range(_obstacleActiveTime[index] + 2f, _obstacleActiveTime[index]));
            if (GameManager._instance.ApperingBoss)
            {
                for (int i = 0; i < Random.Range(1, GameManager._instance.AppearBossLevel ==2 ? 4 : 3); i++)
                {
                    SpawnObstacle(index);


                }
            }
            else
                SpawnObstacle(index);
        }
    }

    void SpawnObstacle(int index)
    {
        ObstacleCount[index]++;
        Obstacle obstacle = PoolManager.Instance.Pop("Obstacle" + (index + 1)) as Obstacle;
        obstacle._obstacleIndex = index;
        obstacle.transform.position = new Vector3(UnityEngine.Random.Range(-_rightCamPos, _rightCamPos), Camera.main.orthographicSize * 2 + .3f);
    }

}
