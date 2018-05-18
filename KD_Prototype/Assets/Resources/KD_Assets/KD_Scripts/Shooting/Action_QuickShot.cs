using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_QuickShot : TimeScaleAction
{
    public Action_QuickShot()
    {
        actionName = "Quick Shot";
        timeScaleOffSet = 0;
    }

    public override void ActionEffect()
    {
        ActingUnit.shooting.TestShooting(ActingUnit.QuickShotAccMod);
    }
}