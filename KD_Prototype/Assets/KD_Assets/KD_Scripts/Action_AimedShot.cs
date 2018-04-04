using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_AimedShot : TimeScaleAction
{
    public Action_AimedShot()
    {
        actionName = "Aimed Shot";
        timeScaleOffSet = 2;
    }

    public override void ActionEffect()
    {
        ActingUnit.shooting.TestShooting();
    }
}
