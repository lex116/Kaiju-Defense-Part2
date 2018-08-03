using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Basic_Interaction : Action_Master
{
    Action_Basic_Interaction()
    {
        Action_Name = "Interaction";
        Action_AP_Cost = 1;
    }

    public override void SetUp()
    {
        Action_Icon = (Resources.Load<Sprite>("KD_Sprites/KD_ActionIcon_Interaction"));
    }

    public override void Action_Effect(Unit_Master Action_Owner)
    {
        Action_Owner.Interaction();
    }

    public override bool CheckRequirements(Unit_Master Action_Owner)
    {
        return true;
    }

    public override void Selection_Effect(Unit_Master Action_Owner)
    {
        //Action_Owner.roundManager.AddNotificationToFeed("Selected Interaction!");
    }

    public override void Deselection_Effect(Unit_Master Action_Owner)
    {
        //Action_Owner.roundManager.AddNotificationToFeed("Deselected Interaction");
    }
}
