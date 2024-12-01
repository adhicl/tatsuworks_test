using System;
using UnityEngine;

public class Creep : Enemy, IDamageable
{
    private Animator animator;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Damage(DamageInfo damageInfo)
    {
        double damage = damageInfo.damage;
        SpawnHit();
        
        //calculate damage reduction
        if (damageInfo.damageType == DamageType.Physical)
        {
            double reduceArmor = damage * ((this.Armor / 100) / 2) ;
            double reduceEvade = damage * ((this.Evasion / 100) / 2);
            damage -= reduceEvade - reduceArmor;
        }
        else if (damageInfo.damageType == DamageType.Magical)
        {
            double reduceMagicResistance = damage * ((this.MagicResistance / 100) / 2);
            damage -= reduceMagicResistance;
        }
        
        //reduce health
        this.CurHealth -= damage;
        if (this.CurHealth <= 0)
        {
            this.CurHealth = 0;
            Die();
        }

        //show health
        float healthBarSize = (float) (this.CurHealth / this.MaxHealth);
        healthBarImage.fillAmount = healthBarSize;
        
        //show damage
        GameController.Instance.AddDamage(damage);
        
        //check for talents
        for (int i = 0; i < damageInfo.talents.Length; i++)
        {
            Talents currentTalents = damageInfo.talents[i];
            switch (currentTalents.type)
            {
                case TalentType.Poison:
                    poisonPerTick = currentTalents.value;
                    timerPoison = 3f;   //poison is per 3 second
                    lastTimerPoison = 3f;
                    isPoisoned = true;
                    poisonEffect.SetActive(true);
                    break;
                case TalentType.Slow:
                    timerSlow = (float) currentTalents.value;
                    isSlowed = true;
                    slowEffect.SetActive(true);
                    break;
                case TalentType.Stun:
                    timerStun = (float) currentTalents.value;
                    isStunned = true;
                    stunEffect.SetActive(true);
                    break;
            }
        }
    }

    private void PoisonDamage()
    {
        this.CurHealth -= poisonPerTick;
        SpawnHit();
            
        if (this.CurHealth <= 0)
        {
            this.CurHealth = 0;
            Die();
        }

        //show health
        float healthBarSize = (float) (this.CurHealth / this.MaxHealth);
        healthBarImage.fillAmount = healthBarSize;
        
        //show damage
        GameController.Instance.AddDamage(poisonPerTick);
    }

    public void SpawnHit()
    {
        Vector3 position = this.transform.position;
        position.x -= 0.2f;
        position.y += 1.6f;
        Instantiate(hitEffect, position, Quaternion.identity);
    }

    public void Die()
    {
        this.animator.SetTrigger("Die");
        timerRevive = 2f;
        //reset
        
        stunEffect.SetActive(false);
        slowEffect.SetActive(false);
        poisonEffect.SetActive(false);
        timerStun = 0f;
        timerPoison = 0f;
        timerSlow = 0f;
    }

    private float timerRevive = 0f;

    private void Update()
    {
        if (timerRevive > 0)
        {
            timerRevive -= Time.deltaTime;
            if (timerRevive <= 0)
            {
                this.animator.SetTrigger("Revive");
                this.CurHealth = this.MaxHealth;
                
                //show health
                float healthBarSize = (float) (this.CurHealth / this.MaxHealth);
                healthBarImage.fillAmount = healthBarSize;
            }

            return;
        }

        if (isSlowed)
        {
            timerSlow -= Time.deltaTime;
            if (timerSlow <= 0)
            {
                isSlowed = false;
                slowEffect.SetActive(false);
            }
        }
        
        if (isStunned)
        {
            timerStun -= Time.deltaTime;
            if (timerStun <= 0)
            {
                isStunned = false;
                stunEffect.SetActive(false);
            }
        }
        
        if (isPoisoned)
        {
            timerPoison -= Time.deltaTime;
            if (timerPoison <= (lastTimerPoison - 1f))
            {
                PoisonDamage();
                lastTimerPoison -= 1f;
            }
            if (timerPoison <= 0)
            {
                isPoisoned = false;
                poisonEffect.SetActive(false);
            }
        }
    }
}