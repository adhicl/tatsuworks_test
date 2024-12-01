using System;
using UnityEngine;

public class AreaDamage : MonoBehaviour, IDestructible, IAbleDamage
{
    private DamageInfo damageInfo;
    protected float timerArea = 0f;
    [SerializeField] private float lifeTime;

    private void Update()
    {
        timerArea += Time.deltaTime;
        if (timerArea >= lifeTime)
        {
            DoDestroy();
        }
    }

    public void DoDestroy()
    {
        Destroy(this.gameObject);
    }

    public void SetDamageInfo(DamageInfo damageInfo)
    {
        this.damageInfo = damageInfo;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (damageInfo.damageType == DamageType.Heal)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<IDamageable>().Damage(damageInfo);
            }

        }
        else
        {
            if (other.CompareTag("enemy"))
            {
                other.GetComponent<IDamageable>().Damage(damageInfo);
            }

        }
    }
}