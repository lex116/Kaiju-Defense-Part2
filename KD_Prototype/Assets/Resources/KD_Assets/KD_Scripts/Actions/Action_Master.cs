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

    //currently all this does is load the correct icon for each action
    public virtual void SetUp()
    {
        //
    }

    //effect the action has on the Action_Owner 
    public virtual void Action_Effect(Unit_Master Action_Owner)
    {
        //
    }

    //checks to see if the Action_Owner can use the action
    public virtual bool CheckRequirements(Unit_Master Action_Owner)
    {
        return false;
    }

    //effects the hud and camera when the action is selected (zooming, hud elements)
    public virtual void Selection_Effect(Unit_Master Action_Owner)
    {
        //
    }

    //undoes the Selection_Effect effects on the hud on deselection 
    public virtual void Deselection_Effect(Unit_Master Action_Owner)
    {
        //
    }
}
