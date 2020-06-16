using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit_Human : Unit_Master
{
    //[Header("Human Fields")]

    #region Unit Stats
    //public int UnitStat_Level;
    //public int UnitStat_ExperiencePoints;

    //public enum PilotClass
    //{
    //    Passion,
    //    Skillful,
    //    Tactical
    //}

    //public PilotClass SelectedPilotClass;

    //This is where knowledges will go at a later time >>>>
    #endregion

    #region Combat Methods
    public override void Die(string Attacker)
    {
        ChangeTeamNerve(-15);

        isDead = true;

        this.transform.localScale = new Vector3(1f, 0.25f, 1f);

        manager_HUD.AddNotificationToFeed(Attacker + " killed " + characterSheet.UnitStat_Name);
    }
    #endregion



    public override void CalculateWeaponStats()
    {
        Calculated_WeaponAccuracy = (characterSheet.UnitStat_Accuracy + equippedWeapon.Accuracy) / 2;

        if (characterSheet.isPanicked)
        {
            Calculated_WeaponAccuracy = Calculated_WeaponAccuracy * PanicAccMod;
        }
    }

    public override void ChangeNerve(int change)
    {
        if (change > 0)
        {
            characterSheet.UnitStat_Nerve = characterSheet.UnitStat_Nerve + change;
        }

        if (change < 0 && RollStatCheck(characterSheet.UnitStat_Willpower, 1f) == false)
        {
            characterSheet.UnitStat_Nerve = characterSheet.UnitStat_Nerve + change;
        }

        #region Results from change
        if (characterSheet.UnitStat_Nerve < 25 && !characterSheet.isPanicked)
        {
            characterSheet.isPanicked = true;
            manager_HUD.AddNotificationToFeed(characterSheet.UnitStat_Name + " has Panicked!");
        }

        if (characterSheet.UnitStat_Nerve > 25 && characterSheet.isPanicked)
        {
            characterSheet.isPanicked = false;
            manager_HUD.AddNotificationToFeed(characterSheet.UnitStat_Name + " has Recovered!");
        }

        if (characterSheet.UnitStat_Nerve > characterSheet.UnitStat_StartingNerve)
        {
            characterSheet.UnitStat_Nerve = characterSheet.UnitStat_StartingNerve;
        }

        if (characterSheet.UnitStat_Nerve < 0)
        {
            characterSheet.UnitStat_Nerve = 0;
        }
        #endregion
    }
}