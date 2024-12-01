using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Image iconAbility;
    [SerializeField] private Image coverAbility;
    
    [SerializeField] private TMP_Text tTotalDamage;
    [SerializeField] private TMP_Text tLastDamage;
    [SerializeField] private TMP_Text tAbilityLevel;
    
    private void Start()
    {
        //add observer to game
        GameController.Instance.changeAbility += ChangeIconAbility;
        GameController.Instance.changeCooldown += ChangeCoverCooldown;
        GameController.Instance.changeLastDamage += ChangeLastDamage;
        GameController.Instance.changetotalDamage += ChangeDamage;
        GameController.Instance.changeLevel += DoChangeLevel;

        ChangeIconAbility();
        coverAbility.fillAmount = 0;
    }

    private void DoChangeLevel(int levelnumber)
    {
        tAbilityLevel.text = levelnumber.ToString();
    }

    private void ChangeCoverCooldown(float cooldown)
    {
        coverAbility.fillAmount = cooldown;
    }

    private void ChangeIconAbility()
    {
        iconAbility.sprite = GameController.Instance.skillConfig.iconAbility;
    }

    private void ChangeDamage(double damage)
    {
        tTotalDamage.text = Math.Round(damage).ToString();
    }

    private void ChangeLastDamage(double damage)
    {
        tLastDamage.text = Math.Round(damage).ToString();
    }
    
    //button cast function
    public void CastAbility()
    {
        GameController.Instance.DoCastAbililty();
    }

    public void UpgradeAbility()
    {
        GameController.Instance.DoUpgradeAbility();
    }
    
    
}
    