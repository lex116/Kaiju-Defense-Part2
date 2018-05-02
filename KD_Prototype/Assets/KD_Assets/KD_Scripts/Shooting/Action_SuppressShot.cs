using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_SuppressShot : TimeScaleAction
{
    public Action_SuppressShot()
    {
        actionName = "Suppress Shot";
        //timeScaleOffSet = 1;
        timeScaleOffSet = 0;
    }

    public override void ActionEffect()
    {
        //Cinematic that shows the suppressor is targetting the suppress target?

        //Debug.Log("Suppression Begginning");

        ActingUnit.ShootingStateMachine.SetBool("isSPR", true);
    }
}
