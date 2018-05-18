using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TimeScaleAction 
{
    public string actionName;
    public int timeScaleOffSet;
    public int timeScalePosition;
    public int timeScalePriority;

    public Unit_Master ActingUnit;
    public Unit_Master TargetUnit;

    public bool wasActivated;

    public virtual void ActionEffect()
    {
        // Does something eventually
    }

    public virtual bool ActionConditional()
    {
        if (ActingUnit.isDead == false)
            return true;
        else
            return false;
    }

    public virtual void SetUp(Unit_Master actingUnit, Unit_Master targetUnit)
    {
        ActingUnit = actingUnit;
        TargetUnit = targetUnit;
    }
}