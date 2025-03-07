using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy_Speed : EnemyBase
{
    [SerializeField]
    private BehaviourType _moveType;

    public override void Init()
    {
        base.Init();
        Move();
    }
    protected override void Awake()
    {
        base.Awake();

    }
    void Move()
    {
        _trueDamaged = true;
        Sequence seq = DOTween.Sequence();
        switch (_moveType)
        {
            case BehaviourType.ONE:
                float z = 180;
                if (transform.position.y < Player.transform.position.y)
                {

                    z = 0;
                }
                else
                {

                    z = 180;
                }
                var dir = Player.transform. position - transform.position;
                float rotationZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion angleAxis = Quaternion.AngleAxis(rotationZ - 90f, Vector3.forward);
                seq.Append(transform.DOMove(Player.transform.position, 1));
                seq.Join(transform.DORotateQuaternion(angleAxis, 1f));
                seq.Append(transform.DORotate(new Vector3(0, 0, z),.5f));
                seq.AppendInterval(.5f);
                seq.Append(transform.DOMoveY((z== 180 ?     1 : 1)*Camera.main.orthographicSize * 2 + .3f, 1));
                seq.AppendCallback(() =>
                {
                    EnemySpawner._instance.BulletEnemyCount[_enemyIndex]--;
                    PoolManager.Instance.Push(this);
                });
                break;
            case BehaviourType.TWO:
                break;
            case BehaviourType.THREE:
                break;
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();

    }
}
