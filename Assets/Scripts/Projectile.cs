using System;
using UnityEngine;

public class Projectile : MonoBehaviour, IDestructible, IAbleDamage
{
    [SerializeField] private float speed;

    private Transform targetTransform;
    private DamageInfo damageInfo;
    
    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }

    private void Update()
    {
        this.transform.LookAt(targetTransform);
        Vector3 movement = this.transform.forward * this.speed * Time.deltaTime;
        movement.y = 0f;
        this.transform.position += movement;
    }

    public void SetDamageInfo(DamageInfo damageInfo)
    {
        this.damageInfo = damageInfo;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
        {
            DoDestroy();
            other.GetComponent<IDamageable>().Damage(damageInfo);
        }
    }

    public void DoDestroy()
    {
        Destroy(this.gameObject);
    }
}