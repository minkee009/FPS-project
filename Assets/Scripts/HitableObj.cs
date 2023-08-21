using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitableObj : MonoBehaviour
{
    public float Hp => _hp;

    public float maxHp;

    public UnityAction OnHit;
    public UnityAction OnDie;
    
    public GameObject hitEffect;
    public GameObject deathEffect;

    public bool setDestroy = true;

    float _hp;

    bool _massageTrigger;

    private void OnDisable()
    {
        _massageTrigger = false;
    }

    private void Start()
    {
        _hp = maxHp;
    }

    private void Update()
    {
        if (_hp <= 0f)
        {
            Die();
        }
    }

    public void IncOrDecHp(float value)
    {
        _hp = Mathf.Clamp(_hp + value, 0f, maxHp);
    }

    public void Hit(float value, GameObject from, bool playHitEffect = true)
    {
        if (_massageTrigger) return ;
        OnHit?.Invoke();

        if (playHitEffect && hitEffect != null)
        {
            var effect = Instantiate(hitEffect);
            effect.transform.position = from.transform.position;
        }

        IncOrDecHp(value);

        Debug.Log(gameObject.name + "이(가) 히트판정을 받음 | 다음 오브젝트가 히트시도 : " + from.name);
    }

    public void Die()
    {
        if (_massageTrigger) return;
        OnDie?.Invoke();

        if(deathEffect != null)
        {
            var effect = Instantiate(deathEffect);
            effect.transform.position = transform.position;
        }

        if(setDestroy) Destroy(gameObject);
        _massageTrigger = true;

        Debug.Log(gameObject.name + "이(가) 죽었음");
    }
}
