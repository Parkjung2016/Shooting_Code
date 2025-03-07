using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootStep : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _footSteps;
    private AudioSource _audioSource;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void FootStep()
    {
        _audioSource.PlayOneShot(_footSteps[Random.Range(0,_footSteps.Length)]);
    }
}
