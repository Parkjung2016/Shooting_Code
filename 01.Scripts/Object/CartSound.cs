using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartSound : MonoBehaviour
{
    private AudioSource _audioSource;
    private Rigidbody _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        _audioSource.volume = Mathf.Clamp(_rb.velocity.sqrMagnitude, 0, 1);
    }

}
