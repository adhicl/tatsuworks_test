using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected double MaxHealth = 1000;
    [SerializeField] protected double CurHealth = 1000;
    [SerializeField] protected double Armor = 50;
    [SerializeField] protected double Evasion = 50;
    [SerializeField] protected double MagicResistance = 50;

    protected bool isStunned;
    protected bool isSlowed;
    protected bool isPoisoned;

    protected float timerStun;
    protected float timerSlow;
    protected float timerPoison;
    protected float lastTimerPoison;
    protected double poisonPerTick;

    [SerializeField] protected Image healthBarImage;
    
    [SerializeField] protected GameObject stunEffect;
    [SerializeField] protected GameObject slowEffect;
    [SerializeField] protected GameObject poisonEffect;
    
    [SerializeField] protected GameObject hitEffect;
}