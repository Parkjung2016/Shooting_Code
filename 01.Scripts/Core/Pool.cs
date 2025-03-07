using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Pool<T> where T : PoolableMono
{
    //PoolableMono�� ��ӹ��� ��� �ֵ��� T�� �� �� �ִ�.
    private Stack<T> _pool = new Stack<T>();
    private T _prefab;
    private Transform _parent;

    public Pool(T prefab, Transform parent, int count)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < count; i++)
        {
            T obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.name = obj.gameObject.name.Replace("(Clone)", "");
            if(obj.gameObject.name == "PlayerBullet")
            {
                Bullet bullet = obj as Bullet;
                string path = "Animation/Player/Bullet/" + PlayerDataManager.Instance.PlayerData.CurrentWeapon.spritePath.Replace("Image/PNGs/", "") + "Animator";
                string effectpath = "Prefab/"+ PlayerDataManager.Instance.PlayerData.CurrentWeapon.spritePath.Replace("Image/PNGs/", "") + "HitEffect";
                bullet.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(path);
                PlayerData data = PlayerDataManager.Instance.PlayerData;
                bullet.SetValue(8, data.Damage,Resources.Load<GameObject>(effectpath));
            }
            obj.gameObject.SetActive(false);
            _pool.Push(obj);
        }
    }

    public T Pop()
    {
        T obj = null;

        if (_pool.Count <= 0) //Ǯ�� ���� ������ ���� �ٽ��
        {
            obj = GameObject.Instantiate(_prefab, _parent);
            obj.gameObject.name = obj.gameObject.name.Replace("(Clone)", "");
        }
        else  //Ǯ�� ������ �־�
        {
            obj = _pool.Pop();
            obj.gameObject.SetActive(true); //��Ƽ�길 �Ѽ� ������
        }
        return obj;
    }


    public void Push(T obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Push(obj);
    }

}