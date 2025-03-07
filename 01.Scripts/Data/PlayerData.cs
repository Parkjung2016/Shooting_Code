using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;


[Serializable]
public class PlayerData
{
    public int[] HighScore = new int[2];
    public float MaxHP;
    public float FireRate;
    public float MoveSpeed;
    public float Damage;
    public float MaxMana;
    public bool SkipTrue;
    public int Gold;
    public int PlayerType=99;
    public List<ItemData> EarnedEngine = new List<ItemData>();
    public List<ItemData> EarnedWeapon = new List<ItemData>();

    public ItemData CurrentEngine ;
    public ItemData CurrentWeapon;
    public bool Pet;
    public bool Begin = false;

}

[Serializable]
public class Data
{
    public PlayerData[] datas = new PlayerData[2];
    public string PlayerName;
    public bool Remove = true;
}
