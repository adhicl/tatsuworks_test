using System;
using DefaultNamespace;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public SkillConfig skillConfig;
    [SerializeField] private GameObject obHero;
    [SerializeField] private GameObject obCreep;
    
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject lineProjectilePrefab;
    [SerializeField] private GameObject areaDamagePrefab;
    
    public static GameController Instance { get; private set; }

    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    private double totalDamage;
    private double lastDamage;

    private int abilityLevel = 1;

    public delegate void ChangeAbility();
    public ChangeAbility changeAbility;

    public delegate void ChangeCooldown(float cooldown);
    public ChangeCooldown changeCooldown;

    public delegate void ChangeTotalDamage(double damage);
    public ChangeTotalDamage changetotalDamage;
    
    public delegate void ChangeLastDamage(double damage);
    public ChangeLastDamage changeLastDamage;
    
    public delegate void ChangeLevel(int levelNumber);
    public ChangeLevel changeLevel;

    public void DoUpgradeAbility()
    {
        Debug.Log("Upgradeability");
        abilityLevel += 1;
        if (abilityLevel > 5)
        {
            abilityLevel = 5;
        }
        changeLevel?.Invoke(abilityLevel);
    }
    
    public void DoCastAbililty()
    {
        if (timerCastAbilty > 0)
        {
            Debug.Log("Ability is not ready yet");
            return;
        }
        
        if (skillConfig != null)
        {
            DamageInfo newDamageInfo;
            newDamageInfo.damage = obHero.GetComponent<IAbleCast>().DoCastMagic(skillConfig, abilityLevel);
            newDamageInfo.damageType = skillConfig.damageType;
            newDamageInfo.talents = skillConfig.talents;
            
            timerCastAbilty = (float) skillConfig.cooldown;

            if (skillConfig.rangeType == RangeType.Single)
            {
                //create projectile prefab
                Vector3 projectilePos = obHero.transform.position;  //use hero position for now, can be changed later to sword / staff position if set in bone
                projectilePos.y = 1.5f;
                GameObject obj = Instantiate(projectilePrefab, projectilePos, Quaternion.identity);
                obj.GetComponent<Projectile>().SetTarget(obCreep.transform);
                obj.GetComponent<IAbleDamage>().SetDamageInfo(newDamageInfo);
            
                //create ability prefab
                GameObject obj2 = Instantiate(skillConfig.prefabAbility, obHero.transform.position, Quaternion.identity);
                obj2.transform.parent = obj.transform;
                obj2.transform.localPosition = Vector3.zero;
            }
            else if (skillConfig.rangeType == RangeType.Area)
            {
                GameObject obj = Instantiate(areaDamagePrefab, obCreep.transform.position, Quaternion.identity);
                obj.GetComponent<IAbleDamage>().SetDamageInfo(newDamageInfo);
            
                //create ability prefab
                GameObject obj2 = Instantiate(skillConfig.prefabAbility, obHero.transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
                obj2.transform.parent = obj.transform;
                obj2.transform.localScale = Vector3.one * 2f;
                obj2.transform.localPosition = Vector3.zero;
            }
            else if (skillConfig.rangeType == RangeType.Line)
            {
                //create projectile prefab
                Vector3 projectilePos = obHero.transform.position;  //use hero position for now, can be changed later to sword / staff position if set in bone
                GameObject obj = Instantiate(lineProjectilePrefab, projectilePos, Quaternion.identity);
                obj.GetComponent<LineProjectile>().SetTarget(obCreep.transform);
                obj.GetComponent<IAbleDamage>().SetDamageInfo(newDamageInfo);
            
                //create ability prefab
                GameObject obj2 = Instantiate(skillConfig.prefabAbility, obHero.transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
                obj2.transform.parent = obj.transform;
                obj2.transform.localPosition = Vector3.zero;
            }
            else if (skillConfig.rangeType == RangeType.Near)
            {
                GameObject obj = Instantiate(areaDamagePrefab, obHero.transform.position, Quaternion.identity);
                obj.GetComponent<IAbleDamage>().SetDamageInfo(newDamageInfo);
            
                //create ability prefab
                GameObject obj2 = Instantiate(skillConfig.prefabAbility, obHero.transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
                obj2.transform.parent = obj.transform;
                obj2.transform.localScale = Vector3.one * 5f;
                obj2.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void SetAbility(SkillConfig ability)
    {
        skillConfig = ability;
        abilityLevel = 1;
        changeAbility?.Invoke();
        changeLevel?.Invoke(abilityLevel);
    }

    private double timerCastAbilty = 0f;
    private void Update()
    {
        if (timerCastAbilty <= 0) return;
        if (timerCastAbilty > 0)
        {
            timerCastAbilty -= Time.deltaTime;
        }
        changeCooldown?.Invoke((float) (timerCastAbilty / skillConfig.cooldown));
    }

    public void AddDamage(double damage)
    {
        totalDamage += damage;
        changetotalDamage?.Invoke(totalDamage);

        lastDamage = damage;
        changeLastDamage?.Invoke(lastDamage);
    }
}