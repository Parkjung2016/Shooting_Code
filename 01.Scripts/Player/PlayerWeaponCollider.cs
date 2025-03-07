using DG.Tweening;
using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerWeaponCollider : MonoBehaviour
{
    private float _damage;
    [SerializeField]
    private GameObject _hitEffect;
    [SerializeField]
    private AudioClip _hitAudioClip;
    [SerializeField]
    private AudioClip _protectAudioClip;
    private PlayerWeapon _weapon;

    private void Awake()
    {
        _damage = PlayerDataManager.Instance.PlayerData.Damage;

        _weapon= GetComponentInParent<PlayerWeapon>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            enemy.ApplyDamage(_damage);
            if(_weapon.Blood &&! GameManager._instance._pC.Death)
            {
                _weapon._player.HP += 10;
                _weapon._player.HpUpSFX();
            }
            CameraManager.Instance.Noise(4, .5f);
            EazySoundManager.PlaySound(_hitAudioClip, .6f);
            Instantiate(_hitEffect, transform.position, _hitEffect.transform.rotation);
        }

        if (collision.CompareTag("Obstacle"))
        {
            CameraManager.Instance.Noise(2, .5f);
            EazySoundManager.PlaySound(_protectAudioClip, .6f);
            Instantiate(_hitEffect, transform.position, _hitEffect.transform.rotation); 
            collision.GetComponent<Obstacle>().ApplyDamage(_damage);
        }
        if (collision.CompareTag("EnemyBullet"))
        {
            CameraManager.Instance.Noise(2, .5f);
            EazySoundManager.PlaySound(_protectAudioClip, .6f);
            Instantiate(_hitEffect, transform.position, _hitEffect.transform.rotation);
            collision.GetComponent<Bullet>().PushThis();
        }
    }

}
