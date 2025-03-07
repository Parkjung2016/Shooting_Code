using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraManager_Lobby : MonoBehaviour
{
    public static CameraManager_Lobby Instance;
    private CinemachineVirtualCamera[] vCams;
  private  CinemachineBrain _vBrain;
    public void Init()
    {
        _vBrain= Camera.main.GetComponent<CinemachineBrain>();
        vCams = new CinemachineVirtualCamera[4];
        for (int i =1; i<= 4;i++)
        {
            vCams[i-1] = GameObject.Find("CM vcam" + i).GetComponent< CinemachineVirtualCamera>();
        }
        ChangeCam(0);
    }
    public void ChangeCam(byte index,Action action = null)
    {
        for(int i =0; i< vCams.Length;i++)
        {
            vCams[i].gameObject. SetActive(false);
        }
        vCams[index].gameObject. SetActive(true);
        StartCoroutine( CallAction(_vBrain.m_DefaultBlend.m_Time,action));
    }
    public void SetPlayerCam(Transform player)
    {
        vCams[0].Follow = player.transform;
        vCams[0].LookAt = player.transform;
    }
    IEnumerator CallAction(float Time,Action action)
    {
        yield return new WaitForSeconds(Time);
        action?.Invoke();
    }
}
