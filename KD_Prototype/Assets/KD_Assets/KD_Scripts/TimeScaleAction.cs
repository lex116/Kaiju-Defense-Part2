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

    public Unit ActingUnit;
    public Unit TargetUnit;

    public bool wasActivated;

    public virtual void ActionEffect()
    {
        // Does something eventually
    }

    public virtual void SetUp(Unit actingUnit, Unit targetUnit)
    {
        ActingUnit = actingUnit;
        TargetUnit = targetUnit;
    }
}