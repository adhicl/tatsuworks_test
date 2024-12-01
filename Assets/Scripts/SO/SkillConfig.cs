using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Skill", menuName = "Gameplay/Skill", order = 0)]
public class SkillConfig : ScriptableObject
{
    public string skillName;
    public string description;
    public double manaCost;
    public double cooldown;
    
    public double damage;
    public DamageType damageType;
    public RangeType rangeType;
    public Talents[] talents;

    public Sprite iconAbility;
    public GameObject prefabAbility;
}
