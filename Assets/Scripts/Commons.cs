using System;

public enum DamageType
{
    Physical,Magical,Pure,Heal
}

public enum RangeType
{
    Single,Line,Near,Area
}

public enum TalentType
{
    ScaleStrength, ScaleMagic, Poison, Stun, Slow, 
}

[Serializable]
public struct Talents
{
    public TalentType type;
    public double value;
}

public struct DamageInfo
{
    public double damage;
    public DamageType damageType;
    public Talents[] talents;
}