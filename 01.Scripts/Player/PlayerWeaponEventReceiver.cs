using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponEventReceiver : MonoBehaviour
{
    PlayerController _pC;
    private void Awake()
    {
        _pC = GetComponentInParent<PlayerController>();
    }
    public void CallAttack()
    {
        if (PlayerDataManager.Instance.PlayerData.CurrentWeapon.Level >= 5)
        {
            if(PlayerDataManager.Instance.PlayerData.CurrentWeapon.MaxBulletType != BulletType.Normal)
        SendMessageUpwards("Attack", SendMessageOptions.DontRequireReceiver);

        }
        else
        SendMessageUpwards("Attack", SendMessageOptions.DontRequireReceiver);
    }
    public void StartAttack()
    {
        _pC.bulletIndex = 0;
    }
}
