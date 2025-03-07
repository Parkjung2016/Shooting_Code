using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Laser : PoolableMono
{
  private  ParticleSystem _ps;
    private PlayerControllerBase _pC;

    [SerializeField]
    private float _damage;
    [SerializeField]
    private AudioClip _hitAudioClip;
    private void Awake()
    {
        _ps = transform.GetComponent<ParticleSystem>();
        _pC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerBase>();
        _ps.trigger.AddCollider(_pC.GetComponent<Collider2D>());
    }
    public override void Init()
    {
        Invoke("Hide", 1.5f);
    }
    private void Hide()
    {
        PoolManager.Instance.Push(this);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            EazySoundManager.PlaySound(_hitAudioClip);
            _pC.ApplyDamage(_damage, 3, .7f);
        }
    }
}
