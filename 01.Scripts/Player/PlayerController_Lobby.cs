using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController_Lobby : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    public Rigidbody _rb;
    public Animator _animator;

    private float _hor;

    [SerializeField]
    private float _interactionDir;
    private float _ver;

    [SerializeField]
    private Transform _arcade_machine;

    [SerializeField]
    private float _arcade_machineDis;
    [SerializeField]
    private float _videoDis;

    [HideInInspector]
    public bool IsMoveTrue;


    private Transform _videoTrans;



    private void Awake()
    {
        _videoTrans = GameObject.Find("Video").transform;
        //_arcade_machine = FindObjectOfType<ArcadeUI>().transform.parent.transform;
        IsMoveTrue = true;
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();

    }
    private void Update()
    {
        if (PauseUI.Instance.Paused) return;
        Move();
        MoveAnim();
        if (Vector3.Distance(transform.position, _videoTrans.position) <= _videoDis && !GameManager_Lobby._instance._isVideo)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                IsMoveTrue = false;
                GameManager_Lobby._instance._isVideo = true;
                _rb.velocity = Vector3.zero;
                UIManager_Lobby.Instance.trm.gameObject.SetActive(false);
                CameraManager_Lobby.Instance.ChangeCam(2, () =>
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    VideoUI.Instance.gameObject.SetActive(true);
                    //ArcadeUI.Instance.FadeIn();
                });
            }
        }
      
    }
    private void Move()
    {
        if (!IsMoveTrue)
        {
            _hor = 0;
            _ver = 0;
        return;
        }
        _hor = Input.GetAxisRaw("Horizontal");
        _ver = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(_hor, _rb.velocity.y, _ver).normalized;
        _rb.velocity = input * _speed;
        if (_hor > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        if (_hor < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    public void MoveAnim()
    {
        _animator.SetBool("Move", _hor != 0 || _ver != 0);
    }
}
