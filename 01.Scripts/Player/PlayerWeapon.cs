using DG.Tweening;
using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private ParticleSystem _teleportParticle;
    private ParticleSystem _trail;
    private ParticleSystem _particles;
    public PlayerController2 _player;
    private GameObject _col;
    [SerializeField]
    private AudioClip _slashAudioClip;
    public bool Blood;
    [SerializeField]
    private Material mat;
    private void Awake()
    {
        _col = transform.Find("COl").gameObject;
        _player = transform.parent.GetComponent<PlayerController2>();
        _teleportParticle = transform.Find("TeleportParticle").GetComponent<ParticleSystem>();
        _trail = transform.Find("Trails").GetComponent<ParticleSystem>();
        _particles = transform.Find("Particles").GetComponent<ParticleSystem>();
        DisableSwordCol();

        if (PlayerDataManager.Instance.PlayerData.CurrentWeapon.Level >= 5)
        {
            GetComponent<SpriteRenderer>().material = mat;
        }
    }
    public void PlaySlashAudio()
    {
        EazySoundManager.PlaySound(_slashAudioClip, .7f);
    }
    public void EnableSwordCol()
    {
        _trail.Play();
        _particles.Play();
        _col.SetActive(true);

    }
    public void EnableBlood()
    {
        Blood = true;
    }
    public void DisableBlood()
    {
        Blood = false;
    }
    public void DisableSwordCol()
    {
        _trail.Stop();
        _particles.Stop();
        _col.SetActive(false);
    }
    public void TeleportParticle()
    {
        _teleportParticle.Play();
    }
    public void NullParent()
    {
        transform.SetParent(null);
    }
    public void SetParent()
    {
        transform.SetParent(_player.transform);
        _player.Skilling = false;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(new Vector3(-0.629f, -0.06f), 4));
        seq.Join(transform.DORotate(new Vector3(0, 0, 315), 4));
    }
    public void ResetAttack()
    {
        _player.ResetAttack();
    }
}
