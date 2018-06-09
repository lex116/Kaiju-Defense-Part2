using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Master : ScriptableObject
{
    internal string Action_Name;
    internal int Action_AP_Cost;
    internal Sprite Action_Icon;

    void Awake()
    {
        SetUp();
    }

    public virtual void SetUp()
    {
        //
    }

    public virtual void Action_Effect(Unit_Master Action_Owner)
    {
        //
    }

    public virtual bool CheckRequirements(Unit_Master Action_Owner)
    {
        return false;
    }

    public virtual void Selection_Effect(Unit_Master Action_Owner)
    {
        //
    }

    public virtual void Deselection_Effect(Unit_Master Action_Owner)
    {
        //
    }
}
