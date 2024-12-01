using System;
using UnityEngine;

public class LineProjectile : MonoBehaviour, IDestructible, IAbleDamage
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;

    private Transform targetTransform;
    private DamageInfo damageInfo;
    
    public void SetTarget(Transform target)
    {
        targetTransform = target;
        this.transform.LookAt(targetTransform);
    }

    private void Update()
    {
        //move straight with line
        Vector3 movement = this.transform.forward * this.speed * Time.deltaTime;
        this.transform.position += movement;
        
        //kill after certain time
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            DoDestroy();
        }
    }

    public void SetDamageInfo(DamageInfo damageInfo)
    {
        this.damageInfo = damageInfo;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
        {
            other.GetComponent<IDamageable>().Damage(damageInfo);
        }
    }

    public void DoDestroy()
    {
        Destroy(this.gameObject);
    }
}