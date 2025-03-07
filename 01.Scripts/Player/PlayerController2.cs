using DG.Tweening;
using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController2 : PlayerControllerBase
{
    private float _attackRate;
    private bool _attacking;
    private Animator _anim;
    private GameObject _swordCol;
    protected SpriteRenderer _playerSpriteRenderer;


    public bool Skilling;


    protected override void Awake()
    {
        base.Awake();
        Transform Sprite = transform.Find("Sprite").transform;

        _playerSpriteRenderer = Sprite.Find("Player").GetComponent<SpriteRenderer>();


        _anim = transform.Find("Sword"). GetComponent<Animator>();

        _attackRate = PlayerDataManager.Instance.PlayerData.CurrentWeapon.Values[1];
        _swordCol = transform.Find("Sword/COl").gameObject;


    }
    protected override void CheckDeath()
    {
        if (HP <= 0 && !Death)
        {

            StopAllCoroutines();
            EnemySpawner._instance.StopAllCoroutines();
            ObstacleSpawner._instance.StopAllCoroutines();
            Sequence seq = DOTween.Sequence();

            seq.Append(_playerSpriteRenderer.material.DOFloat(2, "_DissolveAmount", 3));
            seq.AppendCallback(() =>
            {
                GameManager._instance.LoadLobbyScene();
            });

            Death = true;
        }

    }
    public void Skill()
    {
        _anim.Play("Skill");
        Skilling = true;
    }
    protected override IEnumerator CheckInputAttackKey()
    {

        while (true)
        {
            if (!Skilling)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {

                    AttackType(0);

                    yield return new WaitForSeconds(_attackRate);

                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    AttackType(1);
                    yield return new WaitForSeconds(_attackRate);

                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    AttackType(2);
                    yield return new WaitForSeconds(_attackRate);

                }
            }
            yield return null;
        }
    }
    private void AttackType(int type)
    {
        if (Death || _attacking) return;
        _attacking = true;
        switch (type)
        {
            case 0:
                _anim.Play("Attack");
                break;
            case 1:
                _anim.Play("Attack2");
                break;
            case 2:
                _anim.Play("Attack3");
                break;
        }
    }
    public void ResetAttack()
    {
        _attacking = false;
    }

}

