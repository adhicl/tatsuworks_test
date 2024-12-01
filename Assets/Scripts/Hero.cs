using System;
using UnityEngine;

public class Hero : MonoBehaviour, IDamageable, IAbleCast
{
    [SerializeField] double MaxHealth = 5000;
    [SerializeField] double CurHealth = 5000;
    [SerializeField] double MaxMana = 2000;
    [SerializeField] double CurMana = 2000;
    [SerializeField] double PhysAttack = 50;
    [SerializeField] double MagicAttack = 50;
    
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Damage(DamageInfo damageInfo)
    {
        if (damageInfo.damageType == DamageType.Heal)
        {
            CurHealth -= damageInfo.damage;
            if (CurHealth > MaxHealth) CurHealth = MaxHealth;
        }
    }

    public void SpawnHit()
    {
        throw new NotImplementedException();
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }

    public double DoCastMagic(SkillConfig config, int level)
    {
        //if want to lock player from using if no mana
        //upgrade with level
        double manaCost = config.manaCost * Math.Pow(1.2, level); 
        //if (this.CurMana < manaCost) return;
        this.CurMana -= manaCost;
        if (this.CurMana < 0) this.CurMana = 0;

        //do animation
        animator.SetTrigger("cast");

        //calculate damage
        double damage = 0;
        if (config.damageType == DamageType.Physical)
        {
            damage += PhysAttack + config.damage;    
        }
        else if (config.damageType == DamageType.Magical)
        {
            damage += MagicAttack + config.damage;
        }
        else if (config.damageType == DamageType.Pure || config.damageType == DamageType.Heal)
        {
            damage += config.damage;
        }
        //upgrade with level
        damage *= Math.Pow(1.2, level);
        
        //check damage scaling from talents
        for (int i = 0; i < config.talents.Length; i++)
        {
            if (config.talents[i].type == TalentType.ScaleStrength)
            {
                damage += PhysAttack * (config.talents[i].value / 100);
            }
            else if (config.talents[i].type == TalentType.ScaleMagic)
            {
                damage += MagicAttack * (config.talents[i].value / 100);
            }
        }

        //healing reverse damage
        if (config.damageType == DamageType.Heal)
        {
            damage *= -1;
        }

        return damage;
    }
}