
using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;
using DG.Tweening;


public class CameraManager
{
  public static  CameraManager Instance;
    CinemachineVirtualCamera _vMainCam;
    CinemachineBasicMultiChannelPerlin _perlin;

    Sequence seq;
    public void Init()
    {
        _vMainCam = GameObject.Find("CM vMainCam").GetComponent<CinemachineVirtualCamera>();
        _perlin = _vMainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _perlin.m_AmplitudeGain = 0;
    }
    public void Noise(float AmplitudeGain,float time)
    {
        if (seq != null && seq.IsActive())
            seq.Kill();
   _perlin.m_AmplitudeGain = AmplitudeGain;
        seq= DOTween.Sequence();
        seq.AppendInterval(time);
        seq.Append(DOTween.To(() => _perlin.m_AmplitudeGain, x => _perlin.m_AmplitudeGain = x, .1f  , .5f));
    }

}
