using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLinePlayerAnimator : MonoBehaviour
{
  private  Animator _anim;
    [SerializeField]
    private RuntimeAnimatorController[] _animCons;

    [SerializeField]
    private float[] _spritePosx;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
            _anim.runtimeAnimatorController = _animCons[PlayerDataManager.Instance.PlayerData.PlayerType];
        transform.localPosition = new Vector3(_spritePosx[PlayerDataManager.Instance.PlayerData.PlayerType], transform.localPosition.y,transform.localPosition.z);
    }
    public void Move()
    {
        _anim.SetBool("Move", true);
    }
    public void Idle()
    {
        _anim.SetBool("Move", false);
    }
}
