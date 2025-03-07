using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    BASE,
    ENGINE,
    WEAPON
}
public enum SkillType
{
    METEOR,
    FOX,
    SHIELD,
    ARC,
    NULL
}
[Serializable]
public class ItemData
{
    public ItemType itemType;
    public BulletType MaxBulletType;
    public string ItemName;
    public string[] Datas;
    public float[] Values;
    public string spritePath;
    public Vector2[] FirePoses;
    public int Price;
    public int ReinforcePrice;
    public int level=0;
    public int AddReinforcePrice;
    public SkillType skillType;
    public float SkillCoolTime;
    public float SkillDamage;
    public string SkillInfo;
    public string SkillName;
    public int Index;
    public int Level
    {
        get => level;
        set { level = Mathf.Clamp(value, 0, 5); }
    }

    public float UpgradeValue0;
    public float UpgradeValue1;

}
