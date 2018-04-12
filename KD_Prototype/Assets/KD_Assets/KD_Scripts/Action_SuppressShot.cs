using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_SuppressShot : TimeScaleAction
{
    public Action_SuppressShot()
    {
        actionName = "Suppress Shot";
        timeScaleOffSet = 1;
    }

    public override void ActionEffect()
    {
        ActingUnit.shooting.TestShooting();
    }
}
