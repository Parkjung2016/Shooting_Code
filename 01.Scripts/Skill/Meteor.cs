using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float _speed;
    public Vector2 Dir = new Vector2(.5f, -.5f);

    [SerializeField]
    private AudioClip _hitSFX;
    void Update()
    {
        transform.Translate(Dir * _speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBase enemybase = collision.GetComponent<EnemyBase>();
        if (enemybase != null )
        {
            EazySoundManager.PlaySound(_hitSFX,1f);
            enemybase.ApplyDamage(PlayerDataManager.Instance.PlayerData.CurrentEngine.SkillDamage);
            GetComponent<Animator>().Play("End");
        }
    }
    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
